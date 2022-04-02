using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortLayer : MonoBehaviour {
	MeshRenderer target;
	// Use this for initialization
	void Start () {
		if (!target)
			target = GetComponent<MeshRenderer> ();
			target.sortingOrder = 3;
	}
	
}
