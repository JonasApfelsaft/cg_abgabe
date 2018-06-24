using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    NavMeshAgent navMeshAgent; 

    public Transform player;
    int MoveSpeed = 2;
    int MaxDist = 10;
    int MinDist = 1;
    int roamRadius = 50; 
    System.Random random = new System.Random();



    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); 
        navMeshAgent.baseOffset = random.Next(0, 40); 
    }

    void Update()
    {
        /*transform.LookAt(player);
        transform.rotation = Quaternion.AngleAxis(90, Vector3.up); 

        if (Vector3.Distance(transform.position, player.position) >= MinDist)
        {

            transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        }*/
        
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
        Vector3 finalPosition = hit.position;
        navMeshAgent.SetDestination(finalPosition);

        /*if (Vector3.Distance(transform.position, player.position) <= MaxDist)
        {
            //Here Call any function U want Like Shoot at here or something
        }*/
    }

    
}