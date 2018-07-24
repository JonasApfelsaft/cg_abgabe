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
    
    public GameObject playerSplitPrefab;
    List<GameObject> playerSplits = new List<GameObject>();
    
    Rigidbody rb;
    System.Random random = new System.Random();
    
    public GameObject littleBlobSpawnYellow;

    void Start () {
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<LostMenu>().player = this.gameObject; 
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

        float translation = Input.GetAxis("Vertical");
        float rot = Input.GetAxis("Horizontal");

        if (translation > 0) //move forward
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
            CmdTranslateSplits(0, 0, speed * Time.deltaTime);    
        }
        else if (translation < 0) //move backward
        {
            transform.Translate(0, 0, -speed * Time.deltaTime);
            CmdTranslateSplits(0, 0, -speed * Time.deltaTime);        
        }

        if (rot > 0) //turn right
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            CmdRotateSplits(rotationSpeed);  
        }
        else if (rot < 0) //turn left   
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            CmdRotateSplits(-rotationSpeed);
        }

        if(Input.GetKey(KeyCode.Z)) //move upwards
        {
            transform.Translate(0, speed / 2 * Time.deltaTime, 0);
            CmdTranslateSplits(0, speed / 2 * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.H)) //move downwards
        {
            transform.Translate(0, -speed / 2  * Time.deltaTime, 0);
            CmdTranslateSplits(0, -speed / 2 * Time.deltaTime, 0);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdSplit(); 
        }

        checkIfMerge();      
        checkIfWonOrLost();    
    }

    [Command]
    void CmdTranslateSplits(float x, float y, float z) {
        RpcTranslateSplits(x, y, z);
    }

    [ClientRpc]
    void RpcTranslateSplits(float x, float y, float z) {
        for (int i = 0; i < playerSplits.Count; i++) {
            playerSplits[i].transform.Translate(x, y, z);
        }
    }
    
    [Command]
    void CmdRotateSplits(float speed) {
        RpcRotateSplits(speed);
    }

    [ClientRpc]
    void RpcRotateSplits(float speed) {        
        for (int i = 0; i < playerSplits.Count; i++) {
            // rotate clone around original player which is the object on the left of the row
            playerSplits[i].transform.RotateAround(transform.position, gameObject.transform.up, speed * Time.deltaTime);
        }
    }
 
    // This command code is called on the client but executed on the server using data from the client
    [Command]
    void CmdSplit()
    {
        RpcSplit();
    }

    [ClientRpc]
    // runs function on all clients using data from the server
    // called on Server, executed on the clients    
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

    void checkIfMerge() {
        if(mergeTime<=0.0f && mergeTime>-0.5f){
            CmdMerge(); 
        } else if(mergeTime!=-1.0f){
            mergeTime-=Time.deltaTime; 
        }
    }
    
    [Command]
    void CmdMerge(){
        RpcMerge();
    }

    [ClientRpc]
    void RpcMerge(){
        for(int i = 0; i < playerSplits.Count; i++){
            transform.localScale = transform.localScale + playerSplits[i].transform.localScale; 
            Destroy(playerSplits[i]);
        }

        // remove all splits form the list
        playerSplits.Clear();

        calculateSpeed(); 

        mergeTime=-1.0f; 
    }

    void checkIfWonOrLost() {
        if(this.transform.localScale.x>=6.5f){
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<LostMenu>().openWonMenuMultiplayerWithoutRespawn();
        } else {
            var players = GameObject.FindGameObjectsWithTag("Player"); 
            foreach (GameObject player in players)
            {
                if(player.transform.localScale.x>=6.5f){
                    GameObject.FindGameObjectWithTag("Canvas").GetComponent<LostMenu>().openLostMenuMultiplayerWithoutRespawn();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LittleBlob") || other.gameObject.CompareTag("LittleBlobSpawnYellow"))
        {
            //status.scoreAbsorbedLittleBlob();
            // scaleUp(other.gameObject.transform.localScale.y);  
            scaleUp(0.39f);

            var spawnPosition = (other.gameObject.transform.position * 1.7f);

            other.gameObject.SetActive(false);
            
            // destroy over network
           // CmdDestroy(other.gameObject);

            // if (!isLocalPlayer)
            // {
            //     return;
            // }
            

            // CmdSpawnLittleBlob(spawnPosition);

            CmdSpawnLittleBlobYellow(spawnPosition);
        }else if (other.gameObject.CompareTag("Player"))
        {
            if(isLocalPlayer){
                if(transform.localScale.x>other.transform.localScale.x){
                    //TO DO
                    //call method to let player know he died --> show menu with options respawn and exit
                    //other.getComponent<PlayerController>().died();
                    other.gameObject.SetActive(false);
                    scaleUp(other.gameObject.transform.localScale.y);
                    Debug.Log("won");
                }
                else if (transform.localScale.x<other.transform.localScale.x){
                    this.gameObject.SetActive(false);
                    Debug.Log("lost");
                    
                    if(isServer){
                        GameObject.FindGameObjectWithTag("Canvas").GetComponent<LostMenu>().openLostMenuMultiplayerWithoutRespawn();
                        Debug.Log("Server"); 
                    }else if(isClient){
                        GameObject.FindGameObjectWithTag("Canvas").GetComponent<LostMenu>().openLostMenuMultiplayerWithRespawn();
                        Debug.Log("Client"); 
                    } 
                    
                }  
            }
             
        }
    }

    void calculateSpeed(){
        if(transform.localScale.x<5){
            speed = 7-transform.localScale.x;
        }
    }

    [Command]
    void CmdSpawnLittleBlobYellow(Vector3 spawnPositionOfLittleBlob) {
        Debug.Log("in CmdSpawnLittleBlobYellow");
        var spawnPosition = spawnPositionOfLittleBlob;

        var spawnRotation = Quaternion.Euler( 
            0.0f, 
            0.0f, 
            0.0f);
  
        var littleBlobYellow = (GameObject)Instantiate(littleBlobSpawnYellow, spawnPosition, spawnRotation);
                        
        NetworkServer.Spawn(littleBlobYellow);
        Debug.Log("after NetworkServer.Spawn(LittleBlob Yellow)");
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

    void scaleUp(float size) {
        // scale up player
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
        
        if (isLocalPlayer) {
            adaptCameraOffset(1 + size/4); 
        }
        calculateSpeed();    
    }

 
    public void respawn(){
        if (isLocalPlayer)
        {
            // move back to zero location
            transform.position = Vector3.zero;
            this.gameObject.SetActive(true);
        }
    }

    private void adaptCameraOffset(float adaption)
    {
        // TODO: 
        Vector3 adaptedDistance = new Vector3(followPlayer.distance.x * adaption, followPlayer.distance.y * adaption, followPlayer.distance.z * adaption);
        followPlayer.distance = Vector3.Slerp(followPlayer.distance, adaptedDistance, slerpTime);
     }

    private void won(){
        //TODO: Tell player that he won
    }
}