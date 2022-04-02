using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteWindow : MonoBehaviour {
	public GameObject obj;
	public bool custom = false;
	public bool character = false;
	public SpriteRenderer custom_spr;
	void OnTriggerEnter(Collider coll){
		if (gameObject.tag == "Mountain") {
			if (coll.gameObject.tag == "mscreen") {
				if(custom) custom_spr.enabled = true;
				else GetComponent<SpriteRenderer> ().enabled = true;
			}
			return;
		}
		if (coll.gameObject.tag == "screen") {
			if(character) {
				obj.SetActive(true);
				return;
			}
			if(obj) obj.SetActive(true);
			if(custom) custom_spr.enabled = true;
			else GetComponent<SpriteRenderer> ().enabled = true;
		}
	}
	void OnTriggerExit(Collider coll){
		if (gameObject.tag == "Mountain") {
			if (coll.gameObject.tag == "mscreen") {
				if(custom) custom_spr.enabled = false;
				else GetComponent<SpriteRenderer> ().enabled = false;
			}
			return;
		}
		if (coll.gameObject.tag == "screen") {
			if(character) {
				obj.SetActive(false);
				return;
			}
			if(obj) obj.SetActive(false);
			if(custom) custom_spr.enabled = false;
			else GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
