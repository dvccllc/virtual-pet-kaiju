using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour {
	public static SaveLoad instance;
    public bool file_exists_on_load = false;
    public int starting_energy = 12;
    public int newgame_starting_energy = 4;
    public int starting_coins = 200;
    float currentversion = 1.51f;
    int num_skins = 7;
    int num_tutorials = 8;
    public bool delete;
	void OnApplicationQuit(){
		if(GameController.instance && 
			GameController.instance.game_state != GAME_STATE.START_MENU &&
			GameController.instance.game_state != GAME_STATE.TITLE) Save ();
	}
	void OnApplicationPause(){
		if(GameController.instance && 
			GameController.instance.game_state != GAME_STATE.START_MENU &&
			GameController.instance.game_state != GAME_STATE.TITLE) Save ();
	}
	void Awake(){
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
            file_exists_on_load = File.Exists(Application.persistentDataPath + "/playerInfo.dat");
        } else if (instance != this) {
			Destroy (gameObject);
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Invoke ("Quit", 0.4f);
		}
	}
	void Quit(){
		Application.Quit ();
	}
	public void EraseFile() {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")) File.Delete(Application.persistentDataPath + "/playerInfo.dat");
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
		GameDataInfo game = new GameDataInfo ();
		GameData.instance.Save (game);
		bf.Serialize (file, game);
		file.Close ();
	}
    public void NewGame()
    {
        if(GameData.instance.away)
        {
            int action_type = 2;
            SendData args = new SendData();
            args.id = GameData.instance.multiplayer_id;
            args.action = action_type;
            string json = JsonUtility.ToJson(args);
            GameController.instance.SendMessage("ExecuteAction", json);
        }
        GameController.instance.evolve = false;
        GameData.instance.version = currentversion;
        GameData.instance.age = 0;
        GameData.instance.multiplayer_id = GetUniqueID();
        GameData.instance.birth = System.DateTime.UtcNow;
        GameData.instance.energy_level = newgame_starting_energy;
        GameData.instance.evolved = false;
        GameData.instance.away = false;
        GameData.instance.monster_tier = 0;
        GameData.instance.defense_level = 0;
        GameData.instance.score_defense_1 = 0;
        GameData.instance.score_defense_2 = 0;
        GameData.instance.strength_level = 0;
        GameData.instance.score_strength_1 = 0;
        GameData.instance.score_strength_2 = 0;
        GameData.instance.in_leaderboards = false;
        GameData.instance.initials = "NaN";
        GameData.instance.played = new bool [num_tutorials];
        for(int i = 0; i < GameData.instance.played.Length; i++)
        {
            GameData.instance.played[i] = false;
        }
        GameData.instance.hours = new bool [24];
        for(int i = 0; i < GameData.instance.hours.Length; i++)
        {
            GameData.instance.hours[i] = false;
        }
        GameData.instance.tutorial_mode = false;
        GameData.instance.battles.Clear();
        UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
        int rand = UnityEngine.Random.Range(0, 7);
        if (rand == 0)
            GameData.instance.monster_num = 0;
        else if (rand == 1)
            GameData.instance.monster_num = 19;
        else if (rand == 2)
            GameData.instance.monster_num = 20;
        else
            GameData.instance.monster_num = 1;
        Evolution.instance.Evolve();
		if (GameController.instance.action_tier == 1) {
			GameController.instance.HandleVersusLocked ();
		}
    }
    public void ResetAccount()
    {
        if (GameData.instance.away)
        {
            int action_type = 2;
            SendData args = new SendData();
            args.id = GameData.instance.multiplayer_id;
            args.action = action_type;
            string json = JsonUtility.ToJson(args);
            GameController.instance.SendMessage("ExecuteAction", json);
        }
        EraseFile();
        Load();
        GameController.instance.evolve = false;
        GameData.instance.num_coins = starting_coins / 2;
        BuySlots.instance.UpdateSlots();
		if (GameController.instance.action_tier == 1) {
			GameController.instance.HandleVersusLocked ();
		}
    }
    public void Load(){
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat") && !delete) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			GameDataInfo game = (GameDataInfo)bf.Deserialize (file);
            if (game.version != currentversion) {
                print("resetting the boi");
                LoadResetAccount();
            }
            else {
                GameData.instance.Load (game);
                ShellColorController.instance.ReactivateAll();
                file.Close ();
            }
		} else {
            LoadResetAccount();
		}
	}
    void LoadResetAccount() {
        GameData.instance.version = currentversion;
        GameData.instance.age = 0;
        GameData.instance.multiplayer_id = GetUniqueID();
        GameData.instance.birth = System.DateTime.UtcNow;
        GameData.instance.energy_level = starting_energy;
        GameData.instance.evolved = false;
        GameData.instance.away = false;
        GameData.instance.num_coins = starting_coins;
        GameData.instance.chosen_skin = 0;
        GameData.instance.skins = new bool [num_skins];
        for(int i = 0; i < GameData.instance.skins.Length; i++)
        {
            GameData.instance.skins[i] = false;
        }
        GameData.instance.skins[0] = true;
        GameData.instance.monster_tier = 0;
        GameData.instance.defense_level = 0;
        GameData.instance.score_defense_1 = 0;
        GameData.instance.score_defense_2 = 0;
        GameData.instance.strength_level = 0;
        GameData.instance.score_strength_1 = 0;
        GameData.instance.score_strength_2 = 0;
        GameData.instance.in_leaderboards = false;
        GameData.instance.initials = "NaN";
        GameData.instance.played = new bool [num_tutorials];
        for(int i = 0; i < GameData.instance.played.Length; i++)
        {
            GameData.instance.played[i] = false;
        }
        GameData.instance.hours = new bool [24];
        for(int i = 0; i < GameData.instance.hours.Length; i++)
        {
            GameData.instance.hours[i] = false;
        }
        GameData.instance.tutorial_mode = false;
        GameData.instance.battles.Clear();
        UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
        int rand = UnityEngine.Random.Range (0, 7);
        if (rand == 0)
            GameData.instance.monster_num = 0;
        else if (rand == 1)
            GameData.instance.monster_num = 19;
        else if (rand == 2)
            GameData.instance.monster_num = 20;
        else
            GameData.instance.monster_num = 1;
        Evolution.instance.Evolve ();
    }
    public string GetUniqueID()
    {
        string key = "ID";

        var random = new System.Random();
        DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;

        string uniqueID = String.Format("{0:X}", Convert.ToInt32(timestamp))                //Time
                + "-" + String.Format("{0:X}", Convert.ToInt32(Time.time * 1000000))        //Time in game
                + "-" + String.Format("{0:X}", random.Next(1000000000));                //random number

        Debug.Log("Generated Unique ID: " + uniqueID);

        return uniqueID;
    }
}
