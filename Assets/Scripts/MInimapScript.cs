using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MInimapScript : MonoBehaviour {

	private GameObject player; 
	private Camera minimapCam; 

	// Use this for initialization
	void Start () {
		minimapCam = GameObject.FindGameObjectWithTag("minimapCam").GetComponent<Camera>(); 
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player"); 
		} else {
			Vector3 newPosition = player.transform.position; 
			newPosition.y = transform.position.y; 
			transform.position = newPosition; 
			transform.rotation = Quaternion.Euler(90f, player.transform.rotation.y, 0f); 
			minimapCam.orthographicSize = player.transform.localScale.x + 4.5f; 
		}
		
		
	}
}
