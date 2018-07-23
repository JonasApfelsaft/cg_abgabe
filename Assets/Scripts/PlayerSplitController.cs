using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System;
using UnityEngine.Networking;

public class PlayerSplitController : NetworkBehaviour {
	float speed = 5.0f;
    float slerpTime = 0.5f;
    float mergeTime = -1.0f;
    public GameObject enemySpawner; 
    public GameObject littleBlobSpawner; 


    // start oder update...    public Vector3 scale = transform.localScale;
    
    EnemySpawner enemySpawnerScript; 
    LittleBlobSpawner littleBlobSpawnerScript; 

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawner>(); 		
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
            else {
                // player is dead
                // lostMenuUI.SetActive(true);
                // CmdRespawn();
            }
        }
        else if (other.gameObject.CompareTag("LittleBlob"))
        {
            scaleUp(other.gameObject.transform.localScale.y);  
            other.gameObject.SetActive(false);
            //status.scoreAbsorbedLittleBlob();
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
            else {
                // player is dead
                // lostMenuUI.SetActive(true);
                // RpcRespawn();
            }   
        }
    }

    void scaleUp(float size)
    {
        // scale up split
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
    }

    private void calculateSpeed(){
        if(transform.localScale.x<5){
            speed = 4-transform.localScale.x;
        }
    }
}
