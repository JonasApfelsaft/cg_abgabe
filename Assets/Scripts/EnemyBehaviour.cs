using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    GameObject closestPlayer; 
    GameObject[] players; 
    float distOfClosest; 
    Rigidbody rb;
    float speed; 



    void Start()
    {
        distOfClosest = 1000; 
        players = GameObject.FindGameObjectsWithTag("Player"); 
        rb = transform.gameObject.GetComponent<Rigidbody>(); 
        speed = 2; 
    }

    void Update()
    {

         players = GameObject.FindGameObjectsWithTag("Player"); 


        foreach ( GameObject player in players)
        {
            if(Vector3.Distance(transform.position, player.transform.position)<distOfClosest){
                closestPlayer = player; 
                distOfClosest = Vector3.Distance(transform.position, player.transform.position);
                Debug.Log(Vector3.Distance(transform.position, player.transform.position)); 
            }
        }
        if(closestPlayer!=null){
            transform.position = Vector3.MoveTowards(transform.position, closestPlayer.transform.position, speed*Time.deltaTime); 
        }

    }

    
}