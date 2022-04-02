using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLayer : MonoBehaviour {
	MeshRenderer text;
	// Use this for initialization
	void Start () {
		if (!text)
			text = GetComponent<MeshRenderer> ();
		text.sortingOrder = 4;	
	}
}
