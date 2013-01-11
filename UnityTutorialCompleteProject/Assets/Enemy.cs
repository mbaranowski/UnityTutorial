using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{	
	public Transform player;
	public LayerMask bulletLayer;
	
	MoveMotor characterController;
	
	void Start() {
		characterController = GetComponent<MoveMotor>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		var toPlayer = (player.position - transform.position);
		toPlayer.y = 0;
		characterController.movementDirection = toPlayer.normalized;
		characterController.facingDirection = toPlayer;
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		if (Utility.IsInLayerMask(collision.gameObject, bulletLayer)) {
			Destroy(gameObject);
		}
	}
}
