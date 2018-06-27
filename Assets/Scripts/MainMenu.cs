using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	public GameObject mainMenuUI; 

	// Use this for initialization
	void Start () {
		mainMenuUI.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startMainMenu(){
		mainMenuUI.SetActive(true);
	}

	public void hostGame(){
		mainMenuUI.SetActive(false); 
	}

	public void joinGame(){

	}

	 public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); 
    }
}
