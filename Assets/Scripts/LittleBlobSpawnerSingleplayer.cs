using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleBlobSpawnerSingleplayer : MonoBehaviour {
	public GameObject littleBlobPrefab;

    public void createLittleBlob(int amount) {
        for (int i = 0; i < amount; i++) {

            var spawnPosition = new Vector3(
                Random.Range(-50.0f, 50.0f),
                Random.Range(-50.0f, 50.0f),
                Random.Range(-50.0f, 50.0f));

            var spawnRotation = Quaternion.Euler( 
                0.0f, 
                0.0f, 
                0.0f);

            var littleBlob = (GameObject)Instantiate(littleBlobPrefab, spawnPosition, spawnRotation);
		}
	}
}
