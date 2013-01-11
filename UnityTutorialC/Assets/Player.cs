using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public Transform aimTarget;
	public Transform head;
	
	MoveMotor characterController;
	
	void Start () {
		characterController = GetComponent<MoveMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = Vector3.zero;
		if (Input.GetKey (KeyCode.A)) { 
			velocity.z += 1.0f;
		}
		if (Input.GetKey (KeyCode.D)) { 
			velocity.z -= 1.0f;
		}
		if (Input.GetKey (KeyCode.W)) { 
			velocity.x += 1.0f;
		}
		if (Input.GetKey (KeyCode.S)) { 
			velocity.x -= 1.0f;
		}
		
		characterController.movementDirection = velocity.normalized;
	}
	
	void LateUpdate() {
		head.LookAt ( aimTarget );
		head.transform.rotation *= Quaternion.AngleAxis(-90, Vector3.right);
		
		Vector3 toAim = (aimTarget.position - head.position);
		toAim.y = 0;
		characterController.facingDirection = toAim.normalized;
	}
}
