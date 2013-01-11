using UnityEngine;
using System.Collections;

public class AimTarget : MonoBehaviour {
	
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		if (Physics.Raycast(ray.origin, ray.direction, out hitInfo)) {
			transform.position = ray.GetPoint( hitInfo.distance );
		}
	}
}
