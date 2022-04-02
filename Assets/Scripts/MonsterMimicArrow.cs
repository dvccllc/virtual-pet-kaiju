using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMimicArrow : MonoBehaviour {
	public float lifespan;
	void OnEnable(){
		Invoke ("TurnOff", lifespan);
	}
	void TurnOff(){
		gameObject.SetActive (false);
	}
}
