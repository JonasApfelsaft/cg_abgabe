using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 

public class PauseMenu : MonoBehaviour {

    public GameObject lostMenuUI; 
	public GameObject singleOrMultiplayer; 
	private SingleOrMultiplayer singleOrMultiplayerScript; 
	private GameObject minimap; 
	private GameObject playingField; 

	public string ip; 
	public int port; 

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject player; 
	// Use this for initialization
	void Start () {
		singleOrMultiplayerScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SingleOrMultiplayer>(); 
		GameObject[] go = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject g in go){
            if(g.tag == "minimap"){
                minimap = g; 
            } else if(g.tag == "playingField"){
				playingField = g;
			}
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume(); 
            } else if (!GameIsPaused)
            {
                Pause(); 
            }
        }

	}

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused=false; 
    }

    public void BackToMenu() {
        //end connections 
        if(Network.isServer){
			NetworkManager.singleton.StopClient(); 
			NetworkManager.singleton.StopHost(); 
			NetworkManager.singleton.StopMatchMaker(); 
			Network.Disconnect(); 
		} else {
			NetworkManager.singleton.StopClient(); 
		}
		pauseMenuUI.SetActive(false);
        destroyAll(); 
        minimap.SetActive(false);
		Time.timeScale = 1f;
		singleOrMultiplayer.SetActive(true);
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true; 
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); 
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
