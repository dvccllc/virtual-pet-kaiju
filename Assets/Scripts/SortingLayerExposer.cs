﻿using UnityEngine;
using System.Collections;

public class SortingLayerExposer : MonoBehaviour
{

	public string SortingLayerName = "Default";
	public int SortingOrder = 50;

	void Awake ()
	{
		gameObject.GetComponent<MeshRenderer> ().sortingLayerName = SortingLayerName;
		gameObject.GetComponent<MeshRenderer> ().sortingOrder = SortingOrder;
	}
}
