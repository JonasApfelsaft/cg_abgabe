using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    float speed = 5.0f;
    float rotationSpeed = 65.0f;
    float slerpTime = 0.5f;
    float mergeTime = -1.0f;
    FollowPlayer followPlayer; 
    public GameObject enemySpawner; 
    public GameObject littleBlobSpawner; 
 
    EnemySpawner enemySpawnerScript; 
    LittleBlobSpawner littleBlobSpawnerScript; 
    
    // attempt to make splitting work for networking
    public GameObject playerSplitPrefab;
    List<GameObject> playerSplits = new List<GameObject>();

    // source: https://answers.unity.com/questions/1295543/how-do-i-network-sync-scaling-of-character.html
    [SyncVar(hook = "OnSetScale")]
    private Vector3 scale;

    Rigidbody rb;
    System.Random random = new System.Random();


    void Start () {
        rb = GetComponent<Rigidbody>();
    }
    
    
    void Update()
    {
        // isLocalPlayer returns true if this GameObject is the one that represents 
        // the player on the local client.
        if (!isLocalPlayer)
        {
            return;
        }

        if (transform.localScale.x >= 9) {
            won();
            return;
        }

        enemySpawnerScript = enemySpawner.GetComponent<EnemySpawner>(); 
        littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawner>(); 

        float translation = Input.GetAxis("Vertical");
        float rot = Input.GetAxis("Horizontal");

        // movement: working for host, not working for client
        if (translation > 0) //move forwards
        {
            CmdMoveForward();
        }
        else if (translation < 0) //move backwards
        {
            CmdMoveBackward();
        }

        if (rot > 0) //turn right
        {
            CmdTurnRight();
        }
        else if (rot < 0) //turn left   
        {
            CmdTurnLeft();
        }

        if(Input.GetKey(KeyCode.Z)) //move upwards
        {
            CmdMoveUp();
        }
        else if (Input.GetKey(KeyCode.H)) //move downwards
        {
            CmdMoveDown();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdSplit();
        }

        CmdCheckIfMerge();
    }

    // source: https://answers.unity.com/questions/1295543/how-do-i-network-sync-scaling-of-character.html
    [Command]
    public void CmdSetScale(Vector3 vec) {
        this.scale = vec;
    }

    // source: https://answers.unity.com/questions/1295543/how-do-i-network-sync-scaling-of-character.html
    private void OnSetScale(Vector3 vec) {
     this.scale = vec;
     this.transform.localScale = vec;    
    }

    private void translateClones(float x, float y, float z) {
        for (int i = 0; i < playerSplits.Count; i++) {
            playerSplits[i].transform.Translate(x, y, z);
        }
    }

    private void rotateClones(float speed) {
        // TODO muss aber eigener Player sein und nicht alle mit Tag Player
        var player = GameObject.FindWithTag("Player").transform;
        
        for (int i = 0; i < playerSplits.Count; i++) {
            // rotate clone around original player which is the object on the left of the row
            playerSplits[i].transform.RotateAround(transform.position, player.up, speed * Time.deltaTime);
        }
    }

    [Command]
    void CmdMoveForward() {
        transform.Translate(0, 0, speed * Time.deltaTime);
        translateClones(0, 0, speed * Time.deltaTime);
    }
    
    [Command]
    void CmdMoveBackward() {
        transform.Translate(0, 0, -speed * Time.deltaTime);
        translateClones(0, 0, -speed * Time.deltaTime);
    }

    [Command]
    void CmdTurnRight() {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        rotateClones(rotationSpeed);        
    }

    [Command]
    void CmdTurnLeft() {
        transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        rotateClones(-rotationSpeed);
    }

    [Command]
    void CmdMoveUp() {
        transform.Translate(0, speed / 2 * Time.deltaTime, 0);
        translateClones(0, speed / 2 * Time.deltaTime, 0);
    }

    [Command]
    void CmdMoveDown() {
        transform.Translate(0, -speed / 2  * Time.deltaTime, 0);
        translateClones(0, -speed / 2 * Time.deltaTime, 0);
    }

    // TODO make split for clients work for networking
    [Command]
    void CmdSplit()
    {
        // Scale player
        // old / single player: transform.localScale = transform.localScale / 2;
        // source: https://answers.unity.com/questions/1295543/how-do-i-network-sync-scaling-of-character.html
        CmdSetScale(transform.localScale / 2);

        // Scale current splits
        for (int i = 0; i < playerSplits.Count; i++) {
            // working for host, not working for client
            playerSplits[i].transform.localScale = playerSplits[i].transform.localScale / 2;
  
            // position current clones
            playerSplits[i].transform.Translate(-transform.localScale.x * (i + 1), 0, 0);
        }
        
        // save temporary amount of current splits from list
        var currentPlayerSplits = playerSplits.Count;

        // save current position of player
        Vector3 spawnPosition = transform.position;

        var xOffsetNewSplits = (currentPlayerSplits + 1) * transform.localScale.x;
        
        // Instantiate new splits (one per current split + player)
        for (int i = 0; i < currentPlayerSplits + 1; i++) {
            // newSplit has initial size of prefab
            GameObject newSplit = Instantiate(playerSplitPrefab, spawnPosition, transform.rotation);
            // therefore: assign size of split to current size of player
            newSplit.transform.localScale = transform.localScale;

            newSplit.GetComponent<MeshRenderer>().material.color = Color.blue;

            // add new split to list
            playerSplits.Add(newSplit);

            // position new split
            newSplit.transform.Translate(xOffsetNewSplits + transform.localScale.x * i, 0, 0);
            
            // Spawn the newSplit on the Clients
            NetworkServer.Spawn(newSplit);
        }

        calculateSpeed();
        mergeTime = 5.0f;
    }

    // This command code is called on the client but run on the server
    [Command]
    void CmdCheckIfMerge()
    {
        if(mergeTime<=0.0f && mergeTime>-0.5f){
            merge(); 
        } else if(mergeTime!=-1.0f){
            mergeTime-=Time.deltaTime;
        }
    }

    void merge(){
        for(int i = 0; i < playerSplits.Count; i++){
            transform.localScale = transform.localScale + playerSplits[i].transform.localScale; 
            Destroy(playerSplits[i]);
        }
        // remove all splits form the list
        playerSplits.Clear();

        calculateSpeed();
        mergeTime=-1.0f; 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            if(transform.localScale.x>other.transform.localScale.x){
                other.gameObject.SetActive(false);
                //status.scoreKilledOtherPlayer(5);
                scaleUp(other.gameObject.transform.localScale.y*0.7f); 
                enemySpawnerScript.createEnemy(1);  
            }
        }
        else if (other.gameObject.CompareTag("LittleBlob"))
        {
            other.gameObject.SetActive(false);
            //status.scoreAbsorbedLittleBlob();
            scaleUp(other.gameObject.transform.localScale.y);  
            littleBlobSpawnerScript.createLittleBlob(1);
        } 
        else if (other.gameObject.CompareTag("Player"))
        {
            if(transform.localScale.x>other.transform.localScale.x){
                //TO DO
                //call method to let player know he died --> show menu with options respawn and exit
                //other.getComponent<PlayerController>().died();
                other.gameObject.SetActive(false);
                scaleUp(other.gameObject.transform.localScale.y);
            }
            
        }
        
    }

    void calculateSpeed(){
        if(transform.localScale.x<5){
            speed = 7-transform.localScale.x;
        }
    }

    public override void OnStartLocalPlayer()
    {
        // local player is blue so that client can identify their player object
        GetComponent<MeshRenderer>().material.color = Color.blue;
        
        // Camera
        followPlayer = Camera.main.GetComponent<FollowPlayer>();
        followPlayer.player = gameObject.transform;
        Minimap minimapCam = GameObject.FindGameObjectWithTag("minimapCam").GetComponent<Minimap>(); 
        minimapCam.player = gameObject; 
    }

    private void scaleUp(float size)
    {
        // TODO: wenn split was isst, wird aber player groesser

        // scale up player
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
        
        adaptCameraOffset(1 + (size/4)); 
        calculateSpeed();
    }
 
    private void adaptCameraOffset(float adaption)
    {
        Vector3 adaptedDistance = new Vector3(followPlayer.distance.x * adaption, followPlayer.distance.y * adaption, followPlayer.distance.z * adaption);
        followPlayer.distance = Vector3.Slerp(followPlayer.distance, adaptedDistance, slerpTime);
    }

    private void won(){
        //TODO: Tell player that he won
    }
}