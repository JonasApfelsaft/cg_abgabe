    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    float slerpTime = 0.5f;
    GameObject closestEntity; 
    GameObject closestBlob; 
    GameObject player; 
    GameObject[] enemies; 
    GameObject[] blobs; 
    float distOfClosest; 
    Rigidbody rb;
    float speed; 
    public GameObject enemySpawner; 
    public GameObject littleBlobSpawner; 
    public GameObject lostMenuUI; 
    private bool eatingFirstBlobs; 
    private int numberFirstBlobs; 

    EnemySpawner enemySpawnerScript; 
    LittleBlobSpawnerSingleplayer littleBlobSpawnerScript; 

    

    void Start()
    {
        numberFirstBlobs = Random.Range(3,10);
        eatingFirstBlobs = true; 
        player = GameObject.FindGameObjectWithTag("Player"); 
        enemySpawnerScript = enemySpawner.GetComponent<EnemySpawner>(); 
        littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawnerSingleplayer>(); 
        distOfClosest = 1000; 
        enemies = GameObject.FindGameObjectsWithTag("Collectable");
        rb = transform.gameObject.GetComponent<Rigidbody>(); 
        speed = 2; 

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject[] go= Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject g in go){
            if(g.tag == "lostMenu"){
                lostMenuUI = g; 
            }
        }
    }

    void Update()
    {
        if(eatingFirstBlobs){
            if(numberFirstBlobs>0){
                findLittleBlob();
            }else {
                eatingFirstBlobs = false; 
            }
        }else {
            if(transform.localScale.x<9){
                calculateSpeed(); 
            
                enemies = GameObject.FindGameObjectsWithTag("Collectable");

                if(closerButBigger(player)){
                    closestEntity = player; 
                    distOfClosest = Vector3.Distance(transform.position, player.transform.position);
                }
            
                foreach (GameObject enemy in enemies)
                {
                    if(!GameObject.ReferenceEquals(transform.gameObject, enemy)){
                        if(closerButBigger(enemy)){
                            closestEntity = enemy; 
                            distOfClosest = Vector3.Distance(transform.position, enemy.transform.position);
                        }
                    } else {
                    }

                }
                if(closestEntity==null){
                    findLittleBlob(); 
                } else {
                    followClosest();
                }
            }else {
                // enemy hat gewonnen, alle anderen haben verloren
                //DAS SOLL BEI ALLEN PLAYERN ANGEZEIGT WERDEN 
                //notify player that he lost
            }
        }
        
        

    }

    private bool closerButBigger(GameObject obj){
        return (Vector3.Distance(transform.position, obj.transform.position)<distOfClosest && transform.localScale.x>obj.transform.localScale.x);
    }

    private void followClosest() { 
        transform.position = Vector3.MoveTowards(transform.position, closestEntity.transform.position, speed*Time.deltaTime); 
    }

    private void findLittleBlob() { 
        //Debug.Log("littleBlob");
        blobs = GameObject.FindGameObjectsWithTag("LittleBlob"); 
        float closest=1000; 
        float dist; 
        GameObject closestBlob = blobs[0];
        foreach (GameObject blob in blobs)
        {
            dist = Vector3.Distance(transform.position, blob.transform.position); 
            if(dist<closest){
                closest = dist; 
                closestBlob = blob; 
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, closestBlob.transform.position, speed*Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("LittleBlob")){
            if(numberFirstBlobs>0){
                numberFirstBlobs--;
            }
            RpcScaleUp(other.gameObject.transform.localScale.x*0.7f);
            Destroy(other.gameObject);
            littleBlobSpawnerScript.createLittleBlob(1);
            calculateSpeed();
        }
        else if(other.gameObject.CompareTag("Collectable")){
            if(transform.localScale.x>other.gameObject.transform.localScale.x){
                RpcScaleUp(other.gameObject.transform.localScale.x);
                Destroy(other.gameObject); 
                enemySpawnerScript.createEnemy(1);
                calculateSpeed(); 
            }
           
        }
        else if(other.gameObject.CompareTag("Player")){
            if(transform.localScale.x>other.gameObject.transform.localScale.x){

                //TODO: Notify Player that he lost
               
                // new: lostMenuUI.SetActive(true); 

                RpcScaleUp(other.gameObject.transform.localScale.x);
                
                // new: Destroy(other.gameObject); 
                calculateSpeed(); 
            }
           
        }
    }

    private void RpcScaleUp(float size)
    {
        // scale up player
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
        calculateSpeed();
    }

    private void calculateSpeed(){
        if(transform.localScale.x<5){
            speed = 4-transform.localScale.x;
        }
    }

    
}