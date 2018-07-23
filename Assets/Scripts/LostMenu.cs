using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LostMenu : MonoBehaviour {

	public GameObject lostMenuUI; 
	public GameObject singleOrMultiplayer; 
	private SingleOrMultiplayer singleOrMultiplayerScript; 
	private GameObject minimap; 

	public void Start() {
		singleOrMultiplayerScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SingleOrMultiplayer>(); 
		GameObject[] go = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject g in go){
            if(g.tag == "minimap"){
                minimap = g; 
            }
        }
	}

	public void openLostMenu(){
		lostMenuUI.SetActive(true); 
		minimap.SetActive(false);
		Time.timeScale = 0f;
		GameObject.FindGameObjectWithTag("lostMenuText").GetComponent<Text>().text = "GAME OVER";
	}

	public void openWonMenu(){
		lostMenuUI.SetActive(true);
		minimap.SetActive(false);
		Time.timeScale = 0f;
		GameObject.FindGameObjectWithTag("lostMenuText").GetComponent<Text>().text = "YOU WON";
	}

	public void respawn(){
		//Destroy all current Enemies and LittleBlobs
		destroyAll(); 
		//restart game
		singleOrMultiplayerScript.startSingleplayer(); 
		Time.timeScale = 1f;
		lostMenuUI.SetActive(false); 
		minimap.SetActive(true); 
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
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); 
		foreach (GameObject player in players)
		{
			Destroy(player);
		}
	}
}
