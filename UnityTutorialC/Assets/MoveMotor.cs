using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MoveMotor : MonoBehaviour {

	public float moveSpeed = 5.0f;
	public float moveSnap = 50;
	public float turnSmoothing = 0.3f;
	
	[HideInInspector]
	public Vector3 movementDirection;
	
	[HideInInspector]
	public Vector3 facingDirection;
	
	void FixedUpdate() 
	{
		if (rigidbody.isKinematic) 
			return;
		
		var targetVelocity = movementDirection * moveSpeed;
		var deltaVelocity = targetVelocity - rigidbody.velocity;
		
		if (rigidbody.useGravity) {
			deltaVelocity.y = 0;
		}
		
		rigidbody.AddForce(deltaVelocity * moveSnap, ForceMode.Acceleration);
		var faceDir = facingDirection;
		if (faceDir == Vector3.zero) {
			faceDir = movementDirection;
			rigidbody.angularVelocity = Vector3.zero;
		} else {
			var rotationAngle = AngleAroundAxis(transform.forward, faceDir, Vector3.up);
			rigidbody.angularVelocity = Vector3.up * rotationAngle * turnSmoothing;
		}
	}
	
	float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis) {
		dirA = dirA - Vector3.Project (dirA, axis);
		dirB = dirB - Vector3.Project (dirB, axis);
		float angle = Vector3.Angle (dirA, dirB);
		return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 :  1);
	}
}
