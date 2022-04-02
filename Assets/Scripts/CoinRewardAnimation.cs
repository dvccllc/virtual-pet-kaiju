using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRewardAnimation : MonoBehaviour {
	public static CoinRewardAnimation instance;
	bool fade = false;
    float fade_rate = 0.005f;
	public float fade_augmentation;
    public float fadeDelay;
    public MeshRenderer coin_rend, plus_rend;
	// Use this for initialization
	void Start () {
		instance = this;
		FadeInstant (GetComponent<SpriteRenderer>());
        fade = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (fade) {
			Fade (GetComponent<SpriteRenderer>());
		}
	}
	public void UpdateValue(float wave, string type){
        fade = true;
		TurnWhite (GetComponent<SpriteRenderer>());
		float waveValue = 0.5f * wave;

	}
	void TurnWhite(SpriteRenderer rend){
        rend.color = new Color (1, 1, 1, 1);
        coin_rend.material.color = new Color(coin_rend.material.color.r, coin_rend.material.color.g, coin_rend.material.color.b, 1f);
        plus_rend.material.color = new Color(plus_rend.material.color.r, plus_rend.material.color.g, plus_rend.material.color.b, 1f);
    }
    void FadeInstant(SpriteRenderer rend){
        rend.color = new Color (1, 1, 1, 0);
        coin_rend.material.color = new Color(coin_rend.material.color.r, coin_rend.material.color.g, coin_rend.material.color.b, 0f);
        plus_rend.material.color = new Color(plus_rend.material.color.r, plus_rend.material.color.g, plus_rend.material.color.b, 0f);
    }
    void Fade(SpriteRenderer rend){

        coin_rend.material.color = new Color(coin_rend.material.color.r, coin_rend.material.color.g, coin_rend.material.color.b, coin_rend.material.color.a - fade_rate);
        plus_rend.material.color = new Color(plus_rend.material.color.r, plus_rend.material.color.g, plus_rend.material.color.b, plus_rend.material.color.a - fade_rate);
        rend.color = new Color (1, 1, 1, rend.color.a - fade_rate);
	}
}
