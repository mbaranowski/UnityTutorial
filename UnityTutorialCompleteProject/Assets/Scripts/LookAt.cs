using UnityEngine;
using System.Collections;

// every frame point this GameObject toward the target
// the target is set in the editor
[ExecuteInEditMode]
public class LookAt : MonoBehaviour 
{	
	public Transform target;
	void LateUpdate() {
		transform.LookAt(target);
	}
}
