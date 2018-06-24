using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform player;
    public Vector3 distance;
    private float angle;
    
	// Use this for initialization
	void Start () {
        distance = new Vector3(0.0f, 1.0f, -2.0f);
    }
	
	// Update is called once per frame
	void Update () {
        if(player != null) {

            transform.position = player.position + distance;

            angle = transform.rotation.eulerAngles.y; 
        
            transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 0.1f);

            angle = transform.rotation.eulerAngles.y - angle;

            distance = Quaternion.AngleAxis(angle, Vector3.up) * distance;
        }
    }

    // source: https://answers.unity.com/questions/1157437/making-my-camera-follow-player-in-multiplayer.html
    public void setTarget(Transform target)
     {
         player = target;
     }
    


}
