using UnityEngine;
using UnityEngine.Networking;

public class LittleBlobSpawner : NetworkBehaviour {
	public GameObject littleBlobPrefab;
	public int numberOfLittleBlobs;

	public override void OnStartServer() {
        createLittleBlob(numberOfLittleBlobs);
	}

    public void createLittleBlob(int amount) {
        for (int i = 0; i < amount; i++) {

            var spawnPosition = new Vector3(
                Random.Range(-8.0f, 8.0f),
                Random.Range(-8.0f, 8.0f),
                Random.Range(-8.0f, 8.0f));

            var spawnRotation = Quaternion.Euler( 
                0.0f, 
                0.0f, 
                0.0f);

            var littleBlob = (GameObject)Instantiate(littleBlobPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(littleBlob);
        }
    }
}