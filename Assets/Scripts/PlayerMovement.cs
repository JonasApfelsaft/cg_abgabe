using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System;

public class PlayerMovement : MonoBehaviour {

    float speed = 5.0F;
    float rotationSpeed = 65.0F;
    float slerpTime = 0.5f;
    float mergeTime = -1.0f;
    public GameObject enemySpawner; 
    public GameObject littleBlobSpawner; 
    public GameObject lostMenuUI; 

    EnemySpawner enemySpawnerScript; 
    LittleBlobSpawnerSingleplayer littleBlobSpawnerScript;

    public FollowPlayer camera;
    Rigidbody rb;
    //public GameStatus status; 


    void Start () {
        enemySpawnerScript = enemySpawner.GetComponent<EnemySpawner>(); 
        littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawnerSingleplayer>(); 
        camera = Camera.main.GetComponent<FollowPlayer>();
        rb = GetComponent<Rigidbody>();
        GameObject[] go= Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject g in go){
            if(g.tag == "lostMenu"){
                lostMenuUI = g; 
            }
        }
    }
    

    void Update()
    {
        

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
        checkIfWon(); 
        
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
            if(transform.localScale.x>other.transform.localScale.x){
                other.gameObject.SetActive(false);
                scaleUp(other.gameObject.transform.localScale.y*0.7f); 
                enemySpawnerScript.createEnemy(1);  
            }
            else {
                //player lost 
                this.gameObject.SetActive(false);   
                GameObject.FindGameObjectWithTag("Canvas").GetComponent<LostMenu>().openLostMenu();
            }
        }
        else if (other.gameObject.CompareTag("LittleBlob"))
        {
            other.gameObject.SetActive(false);
            //status.scoreAbsorbedLittleBlob();
            littleBlobSpawnerScript.createLittleBlob(1);
            scaleUp(other.gameObject.transform.localScale.y);  
        }  
        
    }

    void calculateSpeed(){
        if(transform.localScale.x<5){
            speed = 7-transform.localScale.x;
        }
    }

    private void scaleUp(float size)
    {
        Vector3 newScale = new Vector3(transform.localScale.x + size, transform.localScale.y + size, transform.localScale.z + size);
        transform.localScale = Vector3.Slerp(transform.localScale, newScale, slerpTime); 
        adaptCameraOffset(1 + size/4    ); 
        calculateSpeed(); 
    }

    private void adaptCameraOffset(float adaption)
    {
        Vector3 adaptedDistance = new Vector3(camera.distance.x * adaption, camera.distance.y * adaption, camera.distance.z * adaption);
        camera.distance = Vector3.Slerp(camera.distance, adaptedDistance, slerpTime);
    }

    private void checkIfWon(){
        if(this.transform.localScale.x>=6.5f){
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<LostMenu>().openWonMenu();
            this.gameObject.SetActive(false);  
        }
    }
}
