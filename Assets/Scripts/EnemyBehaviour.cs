    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    float slerpTime = 0.5f;
    GameObject closestEntity; 
    GameObject closestBlob; 
    GameObject[] players; 
    GameObject[] enemies; 
    GameObject[] blobs; 
    float distOfClosest; 
    Rigidbody rb;
    float speed; 
    public GameObject enemySpawner; 
    public GameObject littleBlobSpawner; 
    public GameObject lostMenuUI; 

    EnemySpawner enemySpawnerScript; 
    LittleBlobSpawner littleBlobSpawnerScript; 

    

    void Start()
    {
        enemySpawnerScript = enemySpawner.GetComponent<EnemySpawner>(); 
        littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawner>(); 
        distOfClosest = 1000; 
        players = GameObject.FindGameObjectsWithTag("Player"); 
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
        if(transform.localScale.x<9){
            calculateSpeed(); 
        players = GameObject.FindGameObjectsWithTag("Player"); 
        enemies = GameObject.FindGameObjectsWithTag("Collectable");

        foreach (GameObject player in players)
        {
            if(closerButBigger(player)){
                closestEntity = player; 
                distOfClosest = Vector3.Distance(transform.position, player.transform.position);
            }
        }
        foreach (GameObject enemy in enemies)
        {
            if(!GameObject.ReferenceEquals(transform.gameObject, enemy)){
                 if(closerButBigger(enemy)){
                    closestEntity = enemy; 
                    distOfClosest = Vector3.Distance(transform.position, enemy.transform.position);
                    Debug.Log("clos"); 
                }
            }

        }
        if(closestEntity==null){
            findLittleBlob(); 
        } else {
            followClosest();
        }
        }else {
            //DAS SOLL BEI ALLEN PLAYERN ANGEZEIGT WERDEN 
            lostMenuUI.SetActive(true); 
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
            Debug.Log("LittleBlob"); 
            scaleUp(other.gameObject.transform.localScale.x*0.7f);
            Destroy(other.gameObject);
            littleBlobSpawnerScript.createLittleBlob(1);
            calculateSpeed();
        }
        else if(other.gameObject.CompareTag("Collectable")){
            if(transform.localScale.x>other.gameObject.transform.localScale.x){
                Debug.Log("Collectable"); 
                scaleUp(other.gameObject.transform.localScale.x);
                Destroy(other.gameObject); 
                enemySpawnerScript.createEnemy(1);
                calculateSpeed(); 
            }
           
        }
        else if(other.gameObject.CompareTag("Player")){
            if(transform.localScale.x>other.gameObject.transform.localScale.x){
                Debug.Log("Player lost"); 

                //TODO: Notify Player that he lost
                //IST WAHRSCHEINLICH FALSCH WEGEN NETZWERK!!! UNBEDINGT BEACHTEN RICHTIGEN PLAYER ZU BENACHRICHTIGEN
                //lostMenuUI = GameObject.FindGameObjectWithTag("lostMenu"); 
               
                lostMenuUI.SetActive(true); 

                scaleUp(other.gameObject.transform.localScale.x);
                Destroy(other.gameObject); 
                calculateSpeed(); 
            }
           
        }
    }

    private void scaleUp(float size)
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