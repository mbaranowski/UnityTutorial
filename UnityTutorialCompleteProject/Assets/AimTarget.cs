using UnityEngine;
using System.Collections;

public class AimTarget : MonoBehaviour {

	public LayerMask aimLayerMask;
	
	void Update () 
	{
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		if (Physics.Raycast (ray.origin, ray.direction, out hitInfo,
			Mathf.Infinity, aimLayerMask.value)) {
			transform.position = ray.GetPoint(hitInfo.distance);
		}
	}
}
