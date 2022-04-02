using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMG_Fireball{
	public GameObject fb;
	public bool alive;
	public int wave = -1;
}
public class DefenseMiniGame1 : MonoBehaviour {
    public static DefenseMiniGame1 instance;
	public bool playing = true;
	public TextMesh wave_text;
    public GameObject spawner;
    public GameObject fireball;
    public GameObject warning_arrow;
    public Transform[] spawn_coords;
	public GameObject[] arrow_holders;
    public float spawn_rate, start_delay;
	float original_spawn_rate;
    float goal_spawn_rate = 1f;
    bool spawn;
    int index = 0;
	public int creature_health, creature_score;
	public List<DMG_Fireball> fireballs;
	public int wave = 0, winWave, number_fireballs = 0;
	public int current_wave = 0;
	public float wave_break = 5f;
	float rate_decay = 1.5f;
	int total_score = 0;
    int original_creature_health = 3;
    float defense_reward = 0.5f;
    float coin_multiplier = 5f;
	public float rate_min;
	public TextMesh hits_needed;
    // Use this for initialization
    void Start() {
        instance = this;
		fireballs = new List<DMG_Fireball> ();
		original_spawn_rate = spawn_rate;
    }
	public void UpdateHitsNeeded() {
		int hits = 0;
		int goal = (wave + 1) * 3;

		if (goal - creature_score > 0) {
			hits = (goal - creature_score);
		} 
		hits_needed.text = "HITS NEEDED: " + hits;
	}
	void GainSpeed(){
		spawn_rate = Mathf.Lerp (spawn_rate, goal_spawn_rate, 0.01f);
	}
    // Update is called once per frame
    void Update() {
		KillFireballs ();
    }
    public void StartSpawning()
    {
		spawn_rate = original_spawn_rate;
		creature_health = original_creature_health;
		creature_score = 0;
		total_score = 0;
		wave = 0;
		number_fireballs = 0;
        spawn = true;
		wave_text.text = "1";
		UpdateHitsNeeded();
		Invoke ("SpawnFireball", start_delay);
    }
	public void NextWave(){
		CancelInvoke("SpawnFireball");
		int goal = (wave + 1) * 3;
		if(creature_score >= goal)
		{
			spawn = true;
			number_fireballs = 0;
			total_score += creature_score;
			creature_score = 0;
			wave++;
			wave_text.text = "" + (wave + 1);
			UpdateHitsNeeded();
			SoundsController.instance.PlaySound ("positive beep");
			Invoke("SpawnFireball", wave_break);

			spawn_rate -= rate_decay;
			if (spawn_rate <= rate_min)
				spawn_rate = rate_min;
			rate_decay = (rate_decay / 3f) * 2f;
			return;
		} else {
			GameData.instance.defense_times_played++;
			GameController.instance.PressButton (0);
			return;
		}
	}
	public void EndGame(){
		winWave = wave;
		if (wave == 0 && number_fireballs < 2) {
			GameData.instance.energy_level += GameController.instance.mini_game_energy_cost;
		}
		if(wave > 0) Invoke ("WinAnimation", 1f);
		CancelInvoke("SpawnFireball");
		GameController.instance.last_pressed = 0;
	}
	void WinAnimation(){
		SoundsController.instance.PlaySound ("positive beep");
        GameController.instance.icon_reward.UpdateValue(winWave, "defense");
        GameController.instance.coin_reward.UpdateValue(winWave, "coin");
		float reward = defense_reward * winWave;
		if(reward > 2.5f) reward = 2.5f;
        GameData.instance.defense_level += reward;
		if(winWave + 1 > GameData.instance.score_defense_1) GameData.instance.score_defense_1 = winWave + 1;
        GameData.instance.num_coins += (int)Math.Round((winWave * defense_reward) * coin_multiplier, 0);
        //CoinsText.instance.UpdateCoinsText();
		wave_text.text = "0";
    }
    void SpawnFireball()
    {
        if (!spawn)
        {
            CancelInvoke("SpawnFireball");
            return;
        }
		int fireball_max = (wave + 1) * 4;
		if(number_fireballs == fireball_max) {
			NextWave();
			return;
		}
		if(number_fireballs == 0) playing = true;
        GameObject fb = Instantiate(fireball);
        fb.transform.parent = spawner.gameObject.transform;
        index = (int)UnityEngine.Random.Range(0, 3);
		fb.transform.localPosition = spawn_coords[index].localPosition;
		GameObject arrow = Instantiate(warning_arrow);
		arrow.transform.parent = arrow_holders [index].transform;
		arrow.transform.localPosition = Vector3.zero;
		number_fireballs++;
		DMG_Fireball fb_ = new DMG_Fireball();
		fb_.alive = true;
		fb_.wave = wave;
		fb_.fb = fb;
		fb_.fb.GetComponent<Fireball> ().arrow = arrow;
		fireballs.Add (fb_);
        Invoke("SpawnFireball", spawn_rate);

    }
	public void StopSpawning()
    {
        spawn = false;
        CancelInvoke("SpawnFireball");
		foreach(DMG_Fireball fb in fireballs){
			fb.alive = false;
		}
    }
	void KillOlderFireballs() {

	}
	void KillFireballs(){
		List<DMG_Fireball> dead_fbs = new List<DMG_Fireball> ();
		foreach (DMG_Fireball fb in fireballs) {
			if(fb.wave != wave) {
				fb.fb.GetComponent<Fireball>().Explode();
			}
			if(!fb.alive) fb.fb.transform.localScale = Vector3.Lerp (fb.fb.transform.localScale, Vector3.zero, 0.1f);
			if (fb.fb && fb.fb.transform.localScale.x <= 0.05f)
				dead_fbs.Add (fb);
		}
		foreach (DMG_Fireball fb in dead_fbs) {
			if(fb.fb.GetComponent<Fireball> ().arrow) Destroy (fb.fb.GetComponent<Fireball> ().arrow.gameObject);
			Destroy(fb.fb.gameObject);
			fireballs.Remove (fb);
		}
	}
	public void RemoveFireball(GameObject dead_fb){
		foreach (DMG_Fireball fb in fireballs) {
			if (fb.fb == dead_fb) {
				if(fb.fb.GetComponent<Fireball> ().arrow) Destroy (fb.fb.GetComponent<Fireball> ().arrow.gameObject);
				fireballs.Remove (fb);
				return;
			}
		}
	}
}
