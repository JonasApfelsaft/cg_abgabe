using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform player;
    public Vector3 distance;
    private float angle;
    private bool lookBack; 
    private bool turned; 
    private bool lookFront; 
    
	// Use this for initialization
	void Start () {
        distance = new Vector3(0.0f, 1.0f, -2.0f);
        turned = false; 
        lookFront = true; 
    }
	
	// Update is called once per frame
	void Update () {
        if(player != null) {

            transform.position = player.position + distance;

            angle = transform.rotation.eulerAngles.y; 
        
            transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 0.1f);

            angle = transform.rotation.eulerAngles.y - angle;

            distance = Quaternion.AngleAxis(angle, Vector3.up) * distance;

            if (Input.GetKey(KeyCode.C))
            {
                lookBack = true;
                lookFront = false; 
            }
            else if(!lookFront)
            {
                lookBack = false;
                turned = false; 
                turn180(); 
                lookFront = true; 
            }

            if (lookBack&&!turned)
            {
                turn180(); 
                turned = true; 
            }
        }
    }
    private void turn180(){
        float newY = transform.rotation.eulerAngles.y + 180f; 
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, newY, transform.rotation.eulerAngles.z); 
    }
}
