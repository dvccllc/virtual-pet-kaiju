using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

	public static GameData instance;
    public float version = 0.0f;
	public string info_name = "";
    public string multiplayer_id = "";
    public bool low_res = false;
    public float age, elapsed_time_value;
	public DateTime birth, last_played_time;
    public bool evolved;
    public bool away = false;
    public int energy_level;
    public float strength_level, defense_level;
	public int strength_times_played, defense_times_played;
    public int score_strength_1 = 0;
    public int score_strength_2 = 0;
    public int score_defense_1 = 0;
    public int score_defense_2 = 0;
    public bool in_leaderboards = false;
    public string initials = "";
    public List<Battle> battles;
    public bool tutorial_mode = false;
	public int monster_num, monster_tier, num_coins;
    public int chosen_skin = 0;
    public bool[] skins;
    public bool[] played;
    public bool[] hours;
    // float evolve_time_minutes = 3f;
    public float evolve_time_minutes = 1440f;
    int minutes_per_energy = 2;
    // LOAD / SAVE GAME

    public void Save(GameDataInfo other){
        other.version = version;
		other.info_name = info_name;
        other.multiplayer_id = multiplayer_id;
        other.low_res = low_res;
        other.age = age;
		other.birth = birth;
        other.evolved = evolved;
        other.away = away;
        other.energy_level = energy_level;
        other.strength_level = strength_level;
		other.defense_level = defense_level;
		other.monster_num = monster_num;
		other.monster_tier = monster_tier;
        other.num_coins = num_coins;
        other.chosen_skin = chosen_skin;
        other.skins = skins;
        other.played = played;
        other.hours = hours;
        other.score_strength_1 = score_strength_1;
        other.score_strength_2 = score_strength_2;
        other.score_defense_1 = score_defense_1;
        other.score_defense_2 = score_defense_2;
        other.in_leaderboards = in_leaderboards;
        other.strength_times_played = strength_times_played;
		other.defense_times_played = defense_times_played;
        other.last_played_time = DateTime.UtcNow;
        other.battles = battles;
        other.tutorial_mode = tutorial_mode;
	}
	public void Load(GameDataInfo other){
        version = other.version;
		info_name = other.info_name;
        multiplayer_id = other.multiplayer_id;
        low_res = other.low_res;
        age = other.age;
		birth = other.birth;
        evolved = other.evolved;
        away = other.away;
        energy_level = other.energy_level;
        strength_level = other.strength_level;
		defense_level = other.defense_level;
		monster_num = other.monster_num;
		monster_tier = other.monster_tier;
        num_coins = other.num_coins;
        chosen_skin = other.chosen_skin;
        skins = other.skins;
        played = other.played;
        hours = other.hours;
        score_strength_1 = other.score_strength_1;
        score_strength_2 = other.score_strength_2;
        score_defense_1 = other.score_defense_1;
        score_defense_2 = other.score_defense_2;
        in_leaderboards = other.in_leaderboards;
        strength_times_played = other.strength_times_played;
		defense_times_played = other.defense_times_played;
        battles = other.battles;
        tutorial_mode = other.tutorial_mode;
        DateTime startTime = other.last_played_time;
        DateTime endTime = DateTime.UtcNow;
        TimeSpan span = endTime.Subtract(startTime);
        energy_level += span.Minutes / minutes_per_energy;
		Evolution.instance.Evolve ();
	}
	void Awake(){
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// age the creature only while playing 
		if (GameController.instance.game_state != GAME_STATE.VOID && 
			GameController.instance.game_state != GAME_STATE.TITLE && 
			GameController.instance.game_state != GAME_STATE.START_MENU && 
			GameController.instance.game_state != GAME_STATE.PAUSE 
		) {
			TimeSpan tmElapsed = DateTime.UtcNow - birth;
			age = (int) Math.Round( tmElapsed.TotalMinutes , 0 ,MidpointRounding.ToEven ) ;
            if (age >= evolve_time_minutes)
            {
                GameController.instance.evolve = true;
            }
		}
	}
}

[Serializable]
public class Battle{
    public BATTLE_RESULT result;
    public string id;
}
[Serializable]
public class GameDataInfo{
    public float version = 0.0f;
	public string info_name = "";
    public string multiplayer_id = "";
    public bool low_res = false;
    public float age;
    public DateTime birth;
    public DateTime last_played_time;
    public bool evolved;
    public bool away;
    public int energy_level;
    public float strength_level;
	public float defense_level;
	public int monster_num;
	public int monster_tier;
	public int num_coins;
    public int chosen_skin;
    public bool[] skins;
    public bool[] played;
    public bool[] hours;
    public int score_strength_1;
    public int score_strength_2;
    public int score_defense_1;
    public int score_defense_2;
    public bool in_leaderboards;
    public string initials;
    public int strength_times_played;
	public int defense_times_played;
    public List<Battle> battles;
    public bool tutorial_mode;
}