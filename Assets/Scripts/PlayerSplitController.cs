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

    EnemySpawner enemySpawnerScript; 
    LittleBlobSpawner littleBlobSpawnerScript;

    public GameObject littleBlobSpawnYellow;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawner>(); 		
	}

	void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("LittleBlob") || other.gameObject.CompareTag("LittleBlobSpawnYellow"))
        {
            scaleUp(0.39f);  
            
            var spawnPosition = (other.gameObject.transform.position * 1.7f);

            // oder eher: destroy
            other.gameObject.SetActive(false);

            //if (!isLocalPlayer)
            //{
            //    return;
            //}
            
            Debug.Log("after isLocalPlayer");

            CmdSpawnLittleBlobYellow(spawnPosition);
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

    [Command]
    void CmdSpawnLittleBlobYellow(Vector3 spawnPositionOfLittleBlob) {
        Debug.Log("in CmdSpawnLittleBlobYellow im Split");
        var spawnPosition = spawnPositionOfLittleBlob;

        var spawnRotation = Quaternion.Euler( 
            0.0f, 
            0.0f, 
            0.0f);
  
        var littleBlobYellow = (GameObject)Instantiate(littleBlobSpawnYellow, spawnPosition, spawnRotation);
                        
        NetworkServer.Spawn(littleBlobYellow);
        Debug.Log("after NetworkServer.Spawn(LittleBlob Yellow)");
    }
}
