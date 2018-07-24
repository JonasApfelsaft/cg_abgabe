using UnityEngine;

public class EnemySpawner : MonoBehaviour {
	public GameObject enemyPrefab;

	public void Start() {
        
	}

    public void createEnemy(int amount) {
        for (int i = 0; i < amount; i++) {

            var spawnPosition = new Vector3(
                Random.Range(-50.0f, 50.0f),
                Random.Range(-50.0f, 50.0f),
                Random.Range(-50.0f, 50.0f));

            var spawnRotation = Quaternion.Euler( 
                0.0f, 
                Random.Range(0,180),
                0.0f);

            var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        }
    }
}
