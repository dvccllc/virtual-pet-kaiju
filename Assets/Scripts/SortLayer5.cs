using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortLayer5 : MonoBehaviour {
	MeshRenderer target;
	// Use this for initialization
	void Start () {
		if (!target)
			target = GetComponent<MeshRenderer> ();
			target.sortingOrder = 5;
	}
	
}
