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
    int xMin = -40;
    int xMax = 40;
    int yMin = 0; 
    int yMax = 40; 
    int zMin = -34;
    int zMax = 36;

    float mergeTime = -1.0f;

    Rigidbody rb;
    System.Random random = new System.Random();


    void Start () {
        rb = GetComponent<Rigidbody>();
    }
    
	
    void Update()
    {
        
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
            split(); 
        }

        checkIfMerge(); 
        
    }

    void split()
    {
        transform.localScale = transform.localScale / 2; 
        GameObject playerClone = Instantiate(gameObject);        
        playerClone.transform.Translate(transform.localScale.x, 0, 0);
        playerClone.tag="Clone";
        calculateSpeed(); 
        mergeTime=5.0f; 

    }

    void checkIfMerge() 
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
    	GetComponent<MeshRenderer>().material.color = Color.blue;
    	// source: https://answers.unity.com/questions/1157437/making-my-camera-follow-player-in-multiplayer.html
    	Camera.main.GetComponent<FollowPlayer>().setTarget(gameObject.transform);
    }

    private void scaleUp(float size)
    {
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
        // adaptCameraOffset(1 + size); 
        calculateSpeed(); 
    }

}