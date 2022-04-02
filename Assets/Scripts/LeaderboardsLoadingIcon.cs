using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardsLoadingIcon : MonoBehaviour {

	int idx = 1;
	float animate_speed = 0.2f;
	TextMesh text;
	// Use this for initialization
	void OnEnable()
	{	
		idx = 0;
		text = GetComponent<TextMesh>();
		text.text = "";
		CancelInvoke("Animate");
		Animate();
	}
	void Animate() {
		text.text += ".";
		idx++;
		if(idx > 2) {
			idx = 0;
			text.text = ".";
		}
		Invoke("Animate", animate_speed);
	}
}
