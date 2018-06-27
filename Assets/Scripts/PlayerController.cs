using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	float speed = 5.0F;
    float rotationSpeed = 65.0F;
    float slerpTime = 0.5f;
    float mergeTime = -1.0f;
    FollowPlayer followPlayer; 

    // attempt to make splitting work for networking
    // public GameObject splittedPlayerPrefab;  // public field for the split prefab
    // public Transform splittedPlayerSpawn;    // public field for the location of split spawn

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

        float translation = Input.GetAxis("Vertical");
        float rot = Input.GetAxis("Horizontal");

        if (translation > 0) //move forwards
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else if (translation < 0) //move backwards
        {
            transform.Translate(0, 0, -speed * Time.deltaTime);
        }

        if (rot > 0) //turn right
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else if (rot < 0) //turn left   
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }

        if(Input.GetKey(KeyCode.Z)) //move upwards
        {
            transform.Translate(0, speed / 2 * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.H)) //move downwards
        {
            transform.Translate(0, -speed / 2  * Time.deltaTime, 0);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdSplit(); 
        }

        CmdCheckIfMerge(); 
        
    }

    // This command code is called on the client but run on the server
    [Command]
    void CmdSplit()
    {

        transform.localScale = transform.localScale / 2; 
        GameObject playerClone = Instantiate(gameObject);
        // Farbe der playerClone anpassen               
        playerClone.transform.Translate(transform.localScale.x, 0, 0);
        playerClone.tag="Clone";
        calculateSpeed(); 
        mergeTime=5.0f; 

        // Spawn the playerClone on the Clients
        NetworkServer.Spawn(playerClone);
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
        var clones = GameObject.FindGameObjectsWithTag("Clone"); 
        for(int i =0; i<clones.Length; i++){
            transform.localScale = transform.localScale + clones[i].transform.localScale; 
            Destroy(clones[i]); 
        }
        calculateSpeed(); 

        mergeTime=-1.0f; 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            other.gameObject.SetActive(false);
            //status.scoreKilledOtherPlayer(5);
            scaleUp(other.gameObject.transform.localScale.y*0.7f); 
            // createEnemy(); 
        }
        else if (other.gameObject.CompareTag("LittleBlob"))
        {
            other.gameObject.SetActive(false);
            //status.scoreAbsorbedLittleBlob();
            // createLittleBlob();
            scaleUp(other.gameObject.transform.localScale.y);  
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
    }

    private void scaleUp(float size)
    {
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