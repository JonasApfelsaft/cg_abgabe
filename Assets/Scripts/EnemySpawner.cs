using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {
	public GameObject enemyPrefab;
	public int numberOfEnemies;

	public override void OnStartServer() {
        createEnemy(numberOfEnemies);
	}

    public void createEnemy(int amount) {
        for (int i = 0; i < amount; i++) {

            var spawnPosition = new Vector3(
                Random.Range(-8.0f, 8.0f),
                Random.Range(-8.0f, 8.0f),
                Random.Range(-8.0f, 8.0f));

            var spawnRotation = Quaternion.Euler( 
                0.0f, 
                Random.Range(0,180),
                0.0f);

            var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(enemy);
        }
    }
}
