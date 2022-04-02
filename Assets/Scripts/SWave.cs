using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWave : MonoBehaviour {
	public Sprite[] waves;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer> ().sprite = waves [StrengthMiniGame1.instance.wave];
	}
}
