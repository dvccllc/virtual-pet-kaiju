using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mountain : MonoBehaviour {
	public static Mountain instance;
	public int health;
	public float animationSpeed, animationDelay;
	public SpriteRenderer sprend;
	public Sprite[] sprites;
	int index = 0;
	public int wave;
    int max_health = 1;
	// Use this for initialization
	void Start () {
		instance = this;
		health = max_health;
	}
	public void ExplosionFrame(){
		sprend.enabled = true;
		if (index == sprites.Length) {
			sprend.enabled = false;
			index = 0;
			sprend.sprite = sprites [index];
			return;
		}

		sprend.sprite = sprites [index];
		index++;
		Invoke ("ExplosionFrame", animationDelay);
	}
	public void Explode(){
		Invoke ("ExplosionFrame", animationDelay);
	}
}
