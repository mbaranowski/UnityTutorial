using UnityEngine;
using System.Collections.Generic;

public class Utility  {

	static public bool IsInLayerMask(GameObject go, LayerMask mask) {
		int b = (1 << go.layer) & mask;
		return b > 0;
	}
	
	static public Vector3 RandomXZPosition(Bounds bounds) {
		return new Vector3(bounds.extents.x * (Random.value*2.0f - 1.0f), 0,
						   bounds.extents.y * (Random.value*2.0f - 1.0f));
	}
	
	static public void RemoveAllChildrenWithComponent<T>(Transform transform) where T : Component
	{
		var children = new List<GameObject>();
		foreach (Transform child in transform) {
			if (child.GetComponent<T>()) {
				children.Add(child.gameObject);
			}
		}
		
		foreach (GameObject child in children) {
			GameObject.Destroy(child);
		}
	}
	
}
