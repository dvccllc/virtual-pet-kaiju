using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DWave : MonoBehaviour {
	public Sprite[] waves;

	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer> ().sprite = waves [DefenseMiniGame1.instance.wave];
	}
}
