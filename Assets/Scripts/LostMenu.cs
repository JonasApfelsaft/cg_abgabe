using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostMenu : MonoBehaviour {

	public GameObject lostMenuUI; 
	public GameObject singleOrMultiplayer; 
	private SingleOrMultiplayer singleOrMultiplayerScript; 

	public void Start() {
		singleOrMultiplayerScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SingleOrMultiplayer>(); 
	}

	public void openLostMenu(){
		lostMenuUI.SetActive(true); 
	}

	public void respawn(){

		//TODO:
		//respawn 
		//Destroy all current Enemies and LittleBlobs
		destroyAll(); 
		singleOrMultiplayerScript.startSingleplayer(); 
		Time.timeScale = 1f;
		lostMenuUI.SetActive(false); 
	}

	public void backToMainMenu(){
		lostMenuUI.SetActive(false); 

		//TODO:
		//spiel beenden !!!! 
		//Destroy all current Enemies and LittleBlobs
		destroyAll(); 
		Time.timeScale = 1f;
		singleOrMultiplayer.SetActive(true);
	}

	public void destroyAll(){
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Collectable"); 
		foreach (GameObject enemy in enemies)
		{
			Destroy(enemy);
		}
		GameObject[] littleBlobs = GameObject.FindGameObjectsWithTag("LittleBlob"); 
		foreach (GameObject littleBlob in littleBlobs)
		{
			Destroy(littleBlob);
		}
	}
}
