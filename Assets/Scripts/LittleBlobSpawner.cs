using UnityEngine;
using UnityEngine.Networking;

public class LittleBlobSpawner : NetworkBehaviour {
	public GameObject littleBlobPrefab;
	public int numberOfLittleBlobs;

	public override void OnStartServer() {
        for (int i = 0; i < numberOfLittleBlobs; i++) {

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
            
            // client can not spawn
            // source: https://forum.unity.com/threads/cannot-spawn-object-without-an-active-server.366702/
            if(!isServer) {
                return;
            }
            NetworkServer.Spawn(littleBlob);
        }
    }
}