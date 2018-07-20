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


    void Start () {
        rb = GetComponent<Rigidbody>();
    }
    
    
    void Update()
    {
        enemySpawnerScript = enemySpawner.GetComponent<EnemySpawner>(); 
        littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawner>(); 
        // isLocalPlayer returns true if this GameObject is the one that represents 
        // the player on the local client.
        if (!isLocalPlayer)
        {
            return;
        }

        float translation = Input.GetAxis("Vertical");
        float rot = Input.GetAxis("Horizontal");

        if (translation > 0) //move forwards
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
            translateClones(0, 0, speed * Time.deltaTime);
        }
        else if (translation < 0) //move backwards
        {
            transform.Translate(0, 0, -speed * Time.deltaTime);
            translateClones(0, 0, -speed * Time.deltaTime);
        }

        if (rot > 0) //turn right
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            rotateClones(rotationSpeed);
        }
        else if (rot < 0) //turn left   
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            rotateClones(-rotationSpeed);
        }

        if(Input.GetKey(KeyCode.Z)) //move upwards
        {
            transform.Translate(0, speed / 2 * Time.deltaTime, 0);
            translateClones(0, speed / 2 * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.H)) //move downwards
        {
            transform.Translate(0, -speed / 2  * Time.deltaTime, 0);
            translateClones(0, -speed / 2 * Time.deltaTime, 0);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            split(); 
        }

        checkIfMerge(); 
        
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

    // TODO make split for clients work for networking
    void split()
    {
        // Scale player
        transform.localScale = transform.localScale / 2;

        // Scale current splits
        for (int i = 0; i < playerSplits.Count; i++) {
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
            // therefor: assign size of split to current size of player
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
    // [Command]
    void checkIfMerge() 
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
        MinimapScript minimapCam = GameObject.FindGameObjectWithTag("minimapCam").GetComponent<MinimapScript>(); 
        minimapCam.player = gameObject; 
    }

    private void scaleUp(float size)
    {
        // TODO: wenn split was isst, wird aber player groesser

        // scale up player
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
        
        adaptCameraOffset(1 + size); 
        calculateSpeed();
    }
 
    private void adaptCameraOffset(float adaption)
    {
        Vector3 adaptedDistance = new Vector3(followPlayer.distance.x * adaption, followPlayer.distance.y * adaption, followPlayer.distance.z * adaption);
        followPlayer.distance = Vector3.Slerp(followPlayer.distance, adaptedDistance, slerpTime);
     }
}