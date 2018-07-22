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
    
    Rigidbody rb;
    System.Random random = new System.Random();

    // public GameObject lostMenuUI;


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

        if (translation > 0) //move forward
        {
            CmdMoveForward();
        }
        else if (translation < 0) //move backward
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

    private void translateSplits(float x, float y, float z) {
        for (int i = 0; i < playerSplits.Count; i++) {
            playerSplits[i].transform.Translate(x, y, z);
        }
    }

    private void rotateSplits(float speed) {
        // TODO muss aber eigener Player sein und nicht alle mit Tag Player
        var player = GameObject.FindWithTag("Player").transform;
        
        for (int i = 0; i < playerSplits.Count; i++) {
            // rotate clone around original player which is the object on the left of the row
            playerSplits[i].transform.RotateAround(transform.position, player.up, speed * Time.deltaTime);
        }
    }

    // This command code is called on the client but executed on the server using data from the client
    [Command]
    void CmdMoveForward() {
        RpcMoveForward();
    }

    // runs function on all clients using data from the server
    // called on Server, executed on the clients
    [ClientRpc]
    void RpcMoveForward() {
        transform.Translate(0, 0, speed * Time.deltaTime);
        translateSplits(0, 0, speed * Time.deltaTime);
    }
    
    [Command]
    void CmdMoveBackward() {
        RpcMoveBackward();
    }

    [ClientRpc]
    void RpcMoveBackward() {
        transform.Translate(0, 0, -speed * Time.deltaTime);
        translateSplits(0, 0, -speed * Time.deltaTime);
    }

    [Command]
    void CmdTurnRight() {
        RpcTurnRight();
    }

    [ClientRpc]
    void RpcTurnRight() {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        rotateSplits(rotationSpeed);        
    }

    [Command]
    void CmdTurnLeft() {
        RpcTurnLeft();        
    }

    [ClientRpc]
    void RpcTurnLeft() {
        transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        rotateSplits(-rotationSpeed);
    }

    [Command]
    void CmdMoveUp() {
        RpcMoveUp();
    }

    [ClientRpc]
    void RpcMoveUp() {
        transform.Translate(0, speed / 2 * Time.deltaTime, 0);
        translateSplits(0, speed / 2 * Time.deltaTime, 0);
    }

    [Command]
    void CmdMoveDown() {
        RpcMoveDown();
    }

    [ClientRpc]
    void RpcMoveDown() {
        transform.Translate(0, -speed / 2  * Time.deltaTime, 0);
        translateSplits(0, -speed / 2 * Time.deltaTime, 0);
    }

    // TODO make split for clients work for networking
    // This command code is called on the client but run on the server using data from the client
    [Command]
    void CmdSplit()
    {
        RpcSplit();
    }

    [ClientRpc]
    // runs function on all clients using data from the server
    // source: https://answers.unity.com/questions/1240384/how-to-sync-scale-in-unet.html
    void RpcSplit()
    {
        // scale player
        transform.localScale = transform.localScale / 2;
        
        // Scale current splits
        for (int i = 0; i < playerSplits.Count; i++) {
            playerSplits[i].transform.localScale = playerSplits[i].transform.localScale / 2;
  
            // position current clones
            playerSplits[i].transform.Translate(-transform.localScale.x * (i + 1), 0, 0);
        }

        // save amount of current splits from list
        var currentPlayerSplits = playerSplits.Count;

        // save current position of player
        Vector3 spawnPosition = transform.position;
        
        // total width
        var xOffsetNewSplits = (currentPlayerSplits + 1) * transform.localScale.x;
        
        // Instantiate new clones (one per current clone + player)
        for (int i = 0; i < currentPlayerSplits + 1; i++) {
            // newSplit has initial scale of prefab
            GameObject newSplit = Instantiate(playerSplitPrefab, spawnPosition, transform.rotation);
            // therefore: assign scale of split to current size of player
            newSplit.transform.localScale = transform.localScale;

            if (isLocalPlayer) {
                newSplit.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            // newSplit.GetComponent<MeshRenderer>().material.color = Color.blue;

            // add new split to list
            playerSplits.Add(newSplit);

            // position new split
            newSplit.transform.Translate(xOffsetNewSplits + transform.localScale.x * i, 0, 0);
            
            // Spawn the newSplit on the Clients
            // NetworkServer.Spawn(newSplit);
        }

        calculateSpeed();
        mergeTime = 5.0f;
    }

    // This command code is called on the client but run on the server using data from the client
    [Command]
    void CmdCheckIfMerge() {
        RpcCheckIfMerge();
    }

    [ClientRpc]
    // runs function on all clients using data from the server
    // source: https://answers.unity.com/questions/1240384/how-to-sync-scale-in-unet.html
    void RpcCheckIfMerge() 
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
                CmdScaleUp(other.gameObject.transform.localScale.y*0.7f); 
                enemySpawnerScript.createEnemy(1);  
            }
            else {
                // player is dead
                // lostMenuUI.SetActive(true);
                CmdRespawn();
            }
        }
        else if (other.gameObject.CompareTag("LittleBlob"))
        {
            other.gameObject.SetActive(false);
            //status.scoreAbsorbedLittleBlob();
            CmdScaleUp(other.gameObject.transform.localScale.y);  
            littleBlobSpawnerScript.createLittleBlob(1);
        } 
        else if (other.gameObject.CompareTag("Player"))
        {
            if(transform.localScale.x>other.transform.localScale.x){
                //TO DO
                //call method to let player know he died --> show menu with options respawn and exit
                //other.getComponent<PlayerController>().died();
                other.gameObject.SetActive(false);
                CmdScaleUp(other.gameObject.transform.localScale.y);
            }
            else {
                // player is dead
                // lostMenuUI.SetActive(true);
                // RpcRespawn();
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
        
        // TODO does not work any more
        minimapCam.player = gameObject;
    }

    [Command]
    void CmdScaleUp(float size) {
        RpcScaleUp(size);
    }

    [ClientRpc]
    void RpcScaleUp(float size)
    {
        // scale up player
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
        
        // TODO does not work any more
        adaptCameraOffset(1 + size/4); 
        calculateSpeed();
    }
 
    [Command]
    void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // move back to zero location
            transform.position = Vector3.zero;
        }
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