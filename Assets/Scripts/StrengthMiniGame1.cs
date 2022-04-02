using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthMiniGame1 : MonoBehaviour {
	public static StrengthMiniGame1 instance;
	public TextMesh wave_text, mountain_hp, attempts_left;
	public int wave, fireballs, winWave, mountain_size;
	public float waveDelay;
	public GameObject mountain,energy;
	public Sprite [] mountain_sprites;
    float animation_delay = 1f;
    float strength_reward = 0.5f;
    int max_health = 13;
    float mountain_delay = 0.5f;
    float tick_scale_factor = 0.8f;
    int grow_waves = 3;
    int end_wave = 99;
    float coin_multiplier = 5f;
    void Start(){
		instance = this;
		wave = 0;
	}
	public void StartGame(){
		wave = 0;
		wave_text.text = "" + (wave + 1);
		attempts_left.text = "ATTEMPTS LEFT: " + (wave + 1);
        mountain_size = 0;
        mountain.gameObject.SetActive(true);
		mountain.GetComponent<SpriteRenderer> ().sprite = mountain_sprites[0];
		mountain.GetComponent<Mountain> ().wave = 0;
		Invoke ("EnterUI", 0f);
	}
	public void EndGame(){
		if (wave == 0 && fireballs == 0) {
			GameData.instance.energy_level += GameController.instance.mini_game_energy_cost;
		}
		if(wave > 0) Invoke ("WinAnimation", animation_delay);
		winWave = wave;
		mountain.transform.localScale = Vector3.one;
		fireballs = 0;
		StrengthBar.instance.tickRate = StrengthBar.instance.originalTickRate;
		wave = 0;
		CancelInvoke ("EnterUI");
		CancelInvoke ("EnterMountain");
		CancelInvoke ("ExitMountain");
		CancelInvoke ("NextMountain");
		CancelInvoke ("Exit");
		mountain.GetComponent<FadeIn> ().StartExit ();
		energy.GetComponent<FadeIn> ().StartExit ();
	}

	void WinAnimation(){
		SoundsController.instance.PlaySound ("positive beep");
        GameController.instance.icon_reward.UpdateValue(winWave, "strength");
        GameController.instance.coin_reward.UpdateValue(winWave, "coin");
		float reward = strength_reward * winWave;
		if(reward > 2.5f) reward = 2.5f;
        GameData.instance.strength_level += reward;
		if(winWave + 1 > GameData.instance.score_strength_1) GameData.instance.score_strength_1 = winWave + 1;
        GameData.instance.num_coins += (int)Math.Round((winWave * strength_reward) * coin_multiplier, 0);
        //CoinsText.instance.UpdateCoinsText();
		wave_text.text = "" + (wave + 1);
    }
    void EnterUI(){
		fireballs = 0;
		mountain.transform.localScale = Vector3.one;
		mountain.GetComponent<Mountain> ().health = max_health;
		mountain_hp.text = "MOUNTAIN HP: " + mountain.GetComponent<Mountain> ().health;
		attempts_left.text = "ATTEMPTS LEFT: " + (wave + 1);
		StrengthBar.instance.tickRate = StrengthBar.instance.originalTickRate;
		mountain.GetComponent<FadeIn> ().StartEntrance ();
		energy.GetComponent<FadeIn> ().StartEntrance ();
		StartTick ();
	}
	void Exit(){
		GameData.instance.strength_times_played++;
		GameController.instance.PressButton (0);
		SoundsController.instance.PlaySound ("bad");
	}
	public void NextWave(){
		if (mountain.GetComponent<Mountain>().health <= 0) {
			DeathAnimation ();
			SoundsController.instance.PlaySound ("mountain dead");
			Invoke ("ExitMountain", mountain_delay);
			Invoke ("NextMountain", waveDelay);
		} else {
			Invoke ("Exit", waveDelay / 1.75f); 
		}
	}
	void NextMountain(){
		wave++;
		if (wave == end_wave) {
			Invoke ("Exit", waveDelay); 
		}
		wave_text.text = "" + (wave + 1);
		fireballs = 0;
		mountain.GetComponent<Mountain> ().wave = wave;
		//if(wave <= grow_waves) mountain.transform.localScale *= mountain_scale_factor;
		mountain.GetComponent<Mountain> ().health = max_health * (wave + 1);
		mountain_hp.text = "MOUNTAIN HP: " + mountain.GetComponent<Mountain> ().health;
		attempts_left.text = "ATTEMPTS LEFT: " + (wave + 1);
		StrengthBar.instance.tickRate *= tick_scale_factor;
        if(wave < grow_waves) mountain_size++;
        mountain.GetComponent<SpriteRenderer>().sprite = mountain_sprites[mountain_size * 2];  //living mountains lie in even indeces
        Invoke ("EnterMountain", mountain_delay);
		Invoke ("StartTick", mountain_delay);
	}
	void EnterMountain(){
		mountain.GetComponent<FadeIn> ().StartEntrance ();
	}
	void ExitMountain(){
		mountain.GetComponent<FadeIn> ().StartExit ();
	}
	void DeathAnimation(){
		mountain.GetComponent<SpriteRenderer> ().sprite = mountain_sprites[mountain_size * 2 + 1];
		mountain.GetComponent<Mountain> ().Explode ();
	}
	void StartTick(){
		StrengthBar.instance.StartTick ();
	}
}
