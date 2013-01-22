using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public Enemy enemy;
	public Transform player;
	public float rate;
	public bool enableSpawn = true;
	public Transform floor;
	
	void Start () {
		StartCoroutine( SpawnEnemy() );
	}

	Vector3 GetSpawnPosition() 
	{
		while (true) {
			var pos = Utility.RandomXZPosition( floor.collider.bounds );
			if (Vector3.Distance(pos, player.position) > 5.0f) {
				return pos;
			}
		}
	}
	
	IEnumerator SpawnEnemy()
	{	
		while (true) {
			if (enableSpawn)  
			{
				var pos = GetSpawnPosition();
				Enemy newEnemy = Instantiate(enemy, pos, 
					Quaternion.identity) as Enemy;
				newEnemy.transform.parent = transform;
				newEnemy.player = player.transform;
				rate *= 0.975f;
			}
			
			yield return new WaitForSeconds(rate);
		}
	}
	
	public void PlayerDied()
	{	
		enableSpawn = false;
		Utility.RemoveAllChildrenWithComponent<Enemy>( transform );
	}
	
	void PlayerStart() { 
		enableSpawn = true;
	}
}
