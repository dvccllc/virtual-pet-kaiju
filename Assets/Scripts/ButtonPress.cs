using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour {
	public Button button;
	SpriteRenderer sprend;
	public Sprite sprite;
	Sprite original_sprite;

	// Update is called once per frame
	void Update () {
		if (!sprend) {
			sprend = GetComponent<SpriteRenderer> ();
			original_sprite = sprend.sprite;
		} else if (button && button.pressed)
			sprend.sprite = sprite;
		else {
			sprend.sprite = original_sprite;
		}
	}
}
