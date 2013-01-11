using UnityEngine;
using System.Collections;

// this class requires a MoveMotor to actual move
[RequireComponent(typeof(MoveMotor))]
public class Enemy : MonoBehaviour 
{	
	// each enemy wanats a reference to the player position so it can seek and destroy
	public Transform player;
	
	// used to distinguish within the OnCollisionEnter event when the enemy is hit by a bullet
	public LayerMask bulletLayer;
	
	// required component on this GameObject that makes us move based on this script's direction
	MoveMotor characterController;
	
	void Start() {
		// grab a reference to the motor component once
		characterController = GetComponent<MoveMotor>();
	}
	
	// the behavior of the enemy depends on the player position, so its done
	// in LateUpdate wich is done after all other objects moved
	void LateUpdate () 
	{
		// direction from this enemy to the player, 
		var toPlayer = (player.position - transform.position).normalized;
		toPlayer.y = 0; // move only in the XZ plane
		toPlayer.Normalize(); // needs to be unit length, the motor will scale it based on speed
		
		// set the motor's movement and facing direction to make it go
		characterController.movementDirection = toPlayer;
		characterController.facingDirection = toPlayer;
	}
	
	// called when this collider component collides with something else
	void OnCollisionEnter(Collision collision) 
	{
		// if a bullet was hit, the enemy is destroyed,
		// good place to play sounds and destruction effects
		if (Utility.IsInLayerMask(collision.gameObject, bulletLayer)) {
			Destroy(gameObject);
		}
	}
}
