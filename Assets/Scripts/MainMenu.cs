using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking; 

public class MainMenu : MonoBehaviour {

	public GameObject mainMenuUI; 
	public GameObject minimap; 
	InputField inputField; 
	private Text ipInput;
	private int port; 
	public GameObject singleOrMultiplayer; 

	// Use this for initialization
	void Start () {
		inputField = GameObject.FindGameObjectWithTag("ipInput").GetComponent<InputField>();
		//inputField.contentType = InputField.ContentType.IntegerNumber; 
		ipInput = transform.Find("Text").gameObject.GetComponent<Text>(); 
		//ipInput = inputField.GetComponent<Text>(); 
		port = 3000; 
		NetworkManager.singleton.networkPort = port; 
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
		if(ipInput.text == null){
			NetworkManager.singleton.networkAddress = "127.0.0.1"; 
		} else {
			NetworkManager.singleton.networkAddress = ipInput.text.ToString(); 
		}
		
		NetworkManager.singleton.StartClient(); 
		minimap.SetActive(true);
		mainMenuUI.SetActive(false); 
	}

	 public void Back()
    {
        singleOrMultiplayer.SetActive(true);
		mainMenuUI.SetActive(false); 
    }
}
