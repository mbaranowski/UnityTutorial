using UnityEngine;
using System.Collections;

public class AimTarget : MonoBehaviour {
	
	
	// set in the editor so that the aim target only test world-layer objects
	public LayerMask aimLayerMask;
	
	void Update () 
	{
		// get a ray in world space, going through the
		// camera screen position where the mouse cursor is
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		
		// use the physics engine to test what the ray intersects
		// aimLayerMask is specified in the editor so the test ignores the player or enemies
		RaycastHit hitInfo;
		if (Physics.Raycast (ray.origin, ray.direction, out hitInfo,
			Mathf.Infinity, aimLayerMask.value)) 
		{
			// hitInfo is filled in with distance along the ray
			// which needs to be converted to world-space point along the ray.
			// then set this GameObject position to that point.
			transform.position = ray.GetPoint(hitInfo.distance);
		}
	}
}
