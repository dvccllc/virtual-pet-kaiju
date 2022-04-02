using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteGrabber))]
[RequireComponent(typeof(SpriteRenderer))]
public class IntroAnimation : MonoBehaviour {
	public float animation_speed, title_delay, initial_title_delay;
	bool animating = false;
	bool start_animation = false;
	SpriteRenderer sprend;
	SpriteGrabber grabber;
	int idx = 0, repeat = 99;
	// Use this for initialization
	void Start () {
		Invoke ("StartAnimation", initial_title_delay);
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
		SoundsController.instance.StopIntro();
		if (GameController.instance.playing_intro) {
			SoundsController.instance.PlayIntro();
			start_animation = true;
			idx = 0;
		}
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
		if (idx >= grabber.sprites.Length) {
			animating = false;
			CancelInvoke ("Animate"); 
			if (repeat > 0) {
				repeat--;
				GameController.instance.playing_intro = false;
				Invoke ("StartAnimation", title_delay);
			} else {
				GameController.instance.playing_intro = false;
			}
			return;
		}
		sprend.sprite = grabber.sprites [idx];
		idx++;
		Invoke ("Animate", animation_speed);
	}


}
