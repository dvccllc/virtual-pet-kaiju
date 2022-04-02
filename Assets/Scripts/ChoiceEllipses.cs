using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceEllipses : MonoBehaviour {
	int idx = 0;
	public TextMesh text;
	public float speed;

	void Animate() {
		text.text = "";
		for(int i = 0; i < idx; i++) {
			text.text = text.text += ".";
		}
		idx++;
		if(idx > 3) idx = 0;
		Invoke("Animate", speed);
	}
	public void StartAnimation() {
		idx = 0;
		Animate();
	}
	public void StopAnimation() {
		idx = 0;
		CancelInvoke("Animate");
		text.text = "";
	}
}
