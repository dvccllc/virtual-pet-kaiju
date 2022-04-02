using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningFlash : MonoBehaviour {
	bool visible = true;
	public bool energy;
	float rate;
	public GameObject arrow;
	// Use this for initialization
	void Start () {
		if (!energy)
			Flash ();
		else {
			arrow.GetComponent<SpriteRenderer> ().enabled = false;
			GetComponent<SpriteRenderer> ().enabled = false;
		}
		if (energy)
			rate = 0.25f;
		else
			rate = 0.1625f;
	}
	public void StartFlash(bool end_flash){
		if (GetComponent<SpriteRenderer> ().enabled)
			return;
		visible = false;
		CancelInvoke("Flash");
		GetComponent<SpriteRenderer> ().enabled = true;
		arrow.GetComponent<SpriteRenderer> ().enabled = true;
		Flash ();
		if(end_flash) Invoke ("EndFlash", 2f);
	}
	public void EndFlash(){
		CancelInvoke ("Flash");
		GetComponent<SpriteRenderer> ().enabled = false;
		arrow.GetComponent<SpriteRenderer> ().enabled = false;
	}
	void Flash(){
		visible = !visible;
		GetComponent<SpriteRenderer> ().color = visible ? Color.white : new Color (1, 1, 1, 0);
		CancelInvoke("Flash");
		Invoke ("Flash", rate);
	}
}
