using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour 
{	
	public Transform target;
	void LateUpdate() {
		transform.LookAt(target);
	}
}
