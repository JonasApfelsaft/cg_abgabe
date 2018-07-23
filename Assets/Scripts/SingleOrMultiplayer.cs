using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleOrMultiplayer : MonoBehaviour {

	public GameObject mainMenu; 
	public GameObject singleOrMultiplayer; 
	public GameObject enemySpawner; 
	private EnemySpawner enemySpawnerScript; 
	public GameObject littleBlobSpawner; 
	private LittleBlobSpawnerSingleplayer littleBlobSpawnerScript; 
	private FollowPlayer followPlayer;
	public GameObject playerSingleplayerPrefab;
	private GameObject player; 
	public int numberOfEnemies; 
	public int numberOfLittleBlobs; 
	private GameObject minimap; 

	// Use this for initialization
	void Start () {	
		enemySpawnerScript = enemySpawner.GetComponent<EnemySpawner>(); 
		littleBlobSpawnerScript = littleBlobSpawner.GetComponent<LittleBlobSpawnerSingleplayer>(); 
		mainMenu.SetActive(false);
		GameObject[] go = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach(GameObject g in go){
            if(g.tag == "minimap"){
                minimap = g; 
            }
        }
		minimap.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void singleplayer() {
		singleOrMultiplayer.gameObject.SetActive(false); 
		startSingleplayer(); 
	}

	public void multiplayer(){
		singleOrMultiplayer.gameObject.SetActive(false); 
		mainMenu.SetActive(true); 
	}

	 public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); 
    }

	public void startSingleplayer(){
		//spawn enemies and littleblobs
		enemySpawnerScript.createEnemy(numberOfEnemies);
		littleBlobSpawnerScript.createLittleBlob(numberOfLittleBlobs);
		//create Player
		var player = (GameObject)Instantiate(playerSingleplayerPrefab, new Vector3(0,0,0), Quaternion.identity);
		//Set camera to follow player
		followPlayer = Camera.main.GetComponent<FollowPlayer>();
        followPlayer.player = player.transform;
		//activate minimap and set its player
		minimap.SetActive(true);		
		GameObject.FindGameObjectWithTag("minimapCam").GetComponent<Minimap>().player = player; 
	}
}
