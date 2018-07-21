using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class MainMenu : MonoBehaviour {

	public GameObject mainMenuUI; 
	public GameObject minimap; 

	// Use this for initialization
	void Start () {
		mainMenuUI.SetActive(true);
		minimap = GameObject.FindGameObjectWithTag("minimap");
		minimap.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void hostGame(){
		NetworkManager.singleton.StartHost(); 
		mainMenuUI.SetActive(false); 
		minimap.SetActive(true);
	}

	public void joinGame(){
		//NetworkManager.singleton.StartClient(); 
		minimap.SetActive(true);
		mainMenuUI.SetActive(false); 
	}

	 public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); 
    }
}
