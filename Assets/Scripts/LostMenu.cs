using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; 

public class LostMenu : MonoBehaviour {

	public GameObject lostMenuUI; 
	public GameObject singleOrMultiplayer; 
	private SingleOrMultiplayer singleOrMultiplayerScript; 
	private GameObject minimap; 
	private GameObject playingField; 
	private bool multiplayer; 
	public GameObject player; 
	private GameObject respawnBtn; 
	public string ip; 
	public int port; 

	public void Start() {
		port = 3000; 
		multiplayer = false; 
		singleOrMultiplayerScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SingleOrMultiplayer>(); 
		GameObject[] go = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject g in go){
            if(g.tag == "minimap"){
                minimap = g; 
            } else if(g.tag == "playingField"){
				playingField = g;
			} else if (g.tag == "respawnBtn"){
				respawnBtn = g; 
			}
        }
	}

	public void openLostMenu(){
		multiplayer = false; 
		lostMenuUI.SetActive(true); 
		minimap.SetActive(false);
		Time.timeScale = 0f;
		respawnBtn.SetActive(true);
		GameObject.FindGameObjectWithTag("lostMenuText").GetComponent<Text>().text = "GAME OVER";
	}

	public void openLostMenuMultiplayerWithRespawn() {
		multiplayer = true; 
		lostMenuUI.SetActive(true); 
		minimap.SetActive(false);
		respawnBtn.SetActive(true);
		GameObject.FindGameObjectWithTag("lostMenuText").GetComponent<Text>().text = "RETRY?";
	}

	public void openLostMenuMultiplayerWithoutRespawn() {
		multiplayer = true; 
		lostMenuUI.SetActive(true); 
		minimap.SetActive(false);
		respawnBtn.SetActive(false);
		GameObject.FindGameObjectWithTag("lostMenuText").GetComponent<Text>().text = "GAME OVER";
	}

	public void openWonMenuMultiplayerWithoutRespawn(){
		multiplayer = true; 
		lostMenuUI.SetActive(true);
		minimap.SetActive(false);
		Time.timeScale = 0f;
		respawnBtn.SetActive(false);
		GameObject.FindGameObjectWithTag("lostMenuText").GetComponent<Text>().text = "YOU WON";
	}

	public void openWonMenu(){
		multiplayer = false; 
		lostMenuUI.SetActive(true);
		minimap.SetActive(false);
		Time.timeScale = 0f;
		respawnBtn.SetActive(true);
		GameObject.FindGameObjectWithTag("lostMenuText").GetComponent<Text>().text = "YOU WON";
	}

	public void respawn(){
		if(multiplayer){
			ip = NetworkManager.singleton.networkAddress; 
			NetworkManager.singleton.StopClient(); 
			NetworkManager.singleton.networkPort = port; 
			NetworkManager.singleton.networkAddress = ip; 


			var success = NetworkManager.singleton.StartClient();
       		if (success == null)
        	{
				lostMenuUI.SetActive(false);
				singleOrMultiplayer.SetActive(true);

        	}else {
				lostMenuUI.SetActive(false); 
				minimap.SetActive(true);
			}

			
		} else {
			//Destroy all current Enemies and LittleBlobs
			destroyAll(); 
			//restart game
			singleOrMultiplayerScript.startSingleplayer(); 
			Time.timeScale = 1f;
			lostMenuUI.SetActive(false); 
			minimap.SetActive(true); 
		}
		
	}

	public void backToMainMenu(){
		if (multiplayer){
			if(Network.isServer){
				NetworkManager.singleton.StopClient(); 
				NetworkManager.singleton.StopHost(); 
				NetworkManager.singleton.StopMatchMaker(); 
				Network.Disconnect(); 
			} else {
				NetworkManager.singleton.StopClient(); 
			}
			lostMenuUI.SetActive(false);
			destroyAll(); 
			Time.timeScale = 1f;
			singleOrMultiplayer.SetActive(true);
		} else {
			lostMenuUI.SetActive(false); 		
			destroyAll(); 
			Time.timeScale = 1f;
			singleOrMultiplayer.SetActive(true);
		}
		
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
		playingField.SetActive(false);
	}
}
