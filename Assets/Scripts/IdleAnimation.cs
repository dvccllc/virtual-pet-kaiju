using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteGrabber))]
[RequireComponent(typeof(SpriteRenderer))]
public class IdleAnimation : MonoBehaviour {
    public static IdleAnimation instance;
	public float animation_speed;
	bool animating = false;
	bool start_animation = false;
	SpriteRenderer sprend;
	SpriteGrabber grabber;
	int idx = 0;
	// Use this for initialization
	void Start () {
		StartAnimation ();
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (!sprend) {
			sprend = GetComponent<SpriteRenderer> ();
		}
		if (!grabber) {
			grabber = GetComponent<SpriteGrabber> ();
		}
		if (start_animation) {
			start_animation = false;
			animating = true;
			Animate ();
		}
	}
	public void StartAnimation(){
        if (animating) return;
		start_animation = true;
	}
	public void StopAnimation(){
		animating = false;
	}
	void Animate(){

		if (!animating) {
			CancelInvoke ("Animate");
			return;
		}
		if (!sprend || !grabber)
			return;
		if (idx >= grabber.sprites.Length)
			idx = 0;
		sprend.sprite = grabber.sprites [idx];
		idx++;
		Invoke ("Animate", animation_speed);
	}
}
