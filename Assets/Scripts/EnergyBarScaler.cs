using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarScaler : MonoBehaviour {
	public static EnergyBarScaler instance;
	bool grow, shrink;
	Vector3 original_target;
	public Vector3 shrink_target;
	public float magnitude;
	// Use this for initialization
	void Start () {
		instance = this;
		original_target = this.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(grow) {
			this.transform.localScale = Vector3.Lerp(this.transform.localScale, original_target, 0.1f);
			if(this.transform.localScale.x >= original_target.x - magnitude) {
				this.transform.localScale = original_target;
				grow = false;
			}
		}
		if(shrink) {
			this.transform.localScale = Vector3.Lerp(this.transform.localScale, shrink_target, 0.1f);
			if(this.transform.localScale.x <= shrink_target.x + magnitude) {
				this.transform.localScale = shrink_target;
				shrink = false;
			}
		}
	}
	public void Shrink() {
		shrink = true;
		grow = false;
	}
	public void Grow() {
		shrink = false;
		grow = true;
	}
}
