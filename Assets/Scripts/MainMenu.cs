using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		mainMenuUI.SetActive(false); 
		minimap.SetActive(true);
	}

	public void joinGame(){
		minimap.SetActive(true);
	}

	 public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); 
    }
}
