using UnityEngine;
using System.Collections;

// this component applies forces to a GameObject's rigidbody to make it move 
// along the movementDirection and turn towards the facingDirection
// this allows controller scripts to move the game object using the physics engine
//  that also will calculate collisions and friction forces.

// another possibility would be to just set the transform.position directly, but then
// your objects will fly through solid walls, and their movement will be less realistic.
[RequireComponent(typeof(Rigidbody))]
public class MoveMotor : MonoBehaviour {
	
	// top ground speed of an unladen GameObject when it gets going
	public float moveSpeed = 5.0f;
	
	// specifies how quickly the motor will reach its full speed or slow down
	public float moveSnap = 50;
	
	// specifies how quickly the motor will turn to its desired facing direction
	public float turnSmoothing = 0.3f;
	
	// set by controller scripts to specify the direction this motor should move
	// for best results set this to a unit vector or zero to stop.
	[HideInInspector]
	public Vector3 movementDirection;
	
	// set by controller script as above, should be a unit vector or zero
	[HideInInspector]
	public Vector3 facingDirection;
	
	
	// FixedUpdate gets called a fixed amount of times per frame, which is useful for 
	// deterministic and stable physics behavior. (in contrast Update and LateUpdate are 
	// called as often as possible)
	void FixedUpdate() 
	{
		if (rigidbody.isKinematic)  // 
			return;
		
		var targetVelocity = movementDirection * moveSpeed;
		var deltaVelocity = targetVelocity - rigidbody.velocity;
		
		if (rigidbody.useGravity) {
			deltaVelocity.y = 0;
		}
		
		// add as much force as necessary to get the rigidbody velocity up to the desired direction and speed
		rigidbody.AddForce(deltaVelocity * moveSnap, ForceMode.Acceleration);
		
		var faceDir = facingDirection;
		if (faceDir == Vector3.zero) {
			faceDir = movementDirection;
			rigidbody.angularVelocity = Vector3.zero;
		} else {
			// get rotation angle to get us from the current forward direction to the facing direction
			var rotationAngle = AngleAroundAxis(transform.forward, faceDir, Vector3.up);
			rigidbody.angularVelocity = Vector3.up * rotationAngle * turnSmoothing;
		}
	}
	
	// calculate signed angle between dirA and dirB in the plane specified by the axis
	float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis) {
		dirA = dirA - Vector3.Project (dirA, axis);
		dirB = dirB - Vector3.Project (dirB, axis);
		float angle = Vector3.Angle (dirA, dirB);
		return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 :  1);
	}
}
