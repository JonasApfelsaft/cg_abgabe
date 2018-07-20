using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostMenu : MonoBehaviour {

	public GameObject lostMenuUI; 
	public GameObject mainMenuUI; 

	public void openLostMenu(){
		lostMenuUI.SetActive(true); 
	}

	public void respawn(){

		//TODO:
		//respawn 

		lostMenuUI.SetActive(false); 
	}

	public void backToMainMenu(){
		lostMenuUI.SetActive(false); 

		//TODO:
		//spiel beenden !!!! 

		mainMenuUI.SetActive(true);
	}
}
