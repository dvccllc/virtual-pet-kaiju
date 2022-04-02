using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureFlash : MonoBehaviour {
	public static CreatureFlash instance;
	public bool visible = true;
	public bool flashing = false;
    float blink_cooldown = 0.1625f;

    int blinks = 0;
	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	public void Flash(){
		blinks = 10;
		flashing = true;
		Blink ();
	}
	public void Blink(){

		GetComponentInChildren<SpriteRenderer> ().color = visible ? Color.white : new Color (1, 1, 1, 0);
		visible = !visible;
		blinks--;
		if (blinks == 0) {
			GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			flashing = false;
			return;
		}
		Invoke ("Blink", blink_cooldown);
	}
}
