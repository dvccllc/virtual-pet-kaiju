using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour {
    bool cheat = false;
	bool up = false, down = false;
    public EvolutionAnimation evol_anim;
	public static Evolution instance;
	public SpriteRenderer new_body_sprend;
	public int activity_pts_to_die = 2;
	// Use this for initialization
	void Start () {
		instance = this;

	}
    public void ExecuteEvolution()
	{
		int activity_points = 0;
		for (int i = 0; i < 24; i++) {
			if (GameData.instance.hours [i])
				activity_points++;
		}
		print ("activity points: " + activity_points);

		if (activity_points <= activity_pts_to_die) {
			//death

			SoundsController.instance.PlaySound ("death");

			EvolveIntoMonster ("DEAD");

		return;
	}

		SoundsController.instance.PlaySound ("Evolve");

        if (isMonster("Orikin"))
        {
			if(GameData.instance.strength_level == 18 && GameData.instance.defense_level == 18) {
				EvolveIntoMonster("X-I");
			}
			else {
				EvolveIntoMonster("Yiemniak");
			}
        }
        else if (isMonster("Rarikin"))
        {
			if (GameData.instance.strength_level == 1 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Delta");
			} 
			else if (GameData.instance.strength_level == 2 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Kerberos");
			}
			else if(GameData.instance.strength_level == 3 && GameData.instance.defense_level == 1) {
					EvolveIntoMonster("Ghaspar");
			}
			else if(GameData.instance.strength_level == 0 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster("ColourMapster");
			}
			else if(GameData.instance.strength_level == 2 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster("Gemini");
			}
			else if(GameData.instance.strength_level == 3 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster("Mykitas");
			}
			else if(GameData.instance.strength_level == 4 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster("Asura");
			}
			else if(GameData.instance.strength_level == 5 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster("Kyorah");
			}
			else if(GameData.instance.strength_level == 5 && GameData.instance.defense_level >= 5) {
				EvolveIntoMonster("Fumegog");
			}
			else if(GameData.instance.strength_level == 6 && GameData.instance.defense_level >= 6) {
				EvolveIntoMonster("Axoloc");
			}
			else if(GameData.instance.strength_level == 7 && GameData.instance.defense_level >= 7) {
				EvolveIntoMonster("Bubbles");
			}
			else if(GameData.instance.strength_level == 8 && GameData.instance.defense_level >= 8) {
				EvolveIntoMonster("Thaloch");
			}
			else if(GameData.instance.strength_level == 9 && GameData.instance.defense_level >= 9) {
				EvolveIntoMonster("Chelonra");
			}
			else if(GameData.instance.strength_level == 10 && GameData.instance.defense_level >= 10) {
				EvolveIntoMonster("Gaonaga");
			}
			else if(GameData.instance.strength_level == 11 && GameData.instance.defense_level >= 11) {
				EvolveIntoMonster("Kagiza");
			}
			else if(GameData.instance.strength_level == 17 && GameData.instance.defense_level >= 18) {
				EvolveIntoMonster("DragonLotus");
			}
			else if(GameData.instance.strength_level == 18 && GameData.instance.defense_level >= 17) {
				EvolveIntoMonster("Exterminus");
			}
			else {
				EvolveIntoMonster("Tsuplex");
			}
		}
        else if (isMonster("Ukokin"))
        {
			if (GameData.instance.strength_level == 1 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Delta");
			} else if (GameData.instance.strength_level == 2 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Keberos");
			} else if (GameData.instance.strength_level == 3 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Ghaspar");
			} else if (GameData.instance.strength_level == 0 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("ColourMapster");
			} else if (GameData.instance.strength_level == 2 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Gemini");
			} else if (GameData.instance.strength_level == 3 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Mykitas");
			} else if (GameData.instance.strength_level == 4 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Asura");
			} else if (GameData.instance.strength_level == 5 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Kyorah");
			} else if (GameData.instance.strength_level == 5 && GameData.instance.defense_level >= 5) {
				EvolveIntoMonster ("Fumegog");
			} else if (GameData.instance.strength_level == 6 && GameData.instance.defense_level >= 6) {
				EvolveIntoMonster ("Axoloc");
			} else if (GameData.instance.strength_level == 7 && GameData.instance.defense_level >= 7) {
				EvolveIntoMonster ("Bubbles");
			} else if (GameData.instance.strength_level == 8 && GameData.instance.defense_level >= 8) {
				EvolveIntoMonster ("Thaloch");
			} else if (GameData.instance.strength_level == 9 && GameData.instance.defense_level >= 9) {
				EvolveIntoMonster ("Chelonra");
			} else if (GameData.instance.strength_level == 10 && GameData.instance.defense_level >= 10) {
				EvolveIntoMonster ("Gaonaga");
			} else if (GameData.instance.strength_level == 11 && GameData.instance.defense_level >= 11) {
				EvolveIntoMonster ("Kagiza");
			} else if (GameData.instance.strength_level == 17 && GameData.instance.defense_level >= 18) {
				EvolveIntoMonster ("DragonLotus");
			} else if (GameData.instance.strength_level == 18 && GameData.instance.defense_level >= 17) {
				EvolveIntoMonster ("Exterminus");
			} else {
				EvolveIntoMonster ("Cyning");
			}
        }
        else if (isMonster("Herikin"))
        {
			if (GameData.instance.strength_level == 1 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Delta");
			} else if (GameData.instance.strength_level == 2 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Kerberos");
			} else if (GameData.instance.strength_level == 3 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("Ghaspar");
			} else if (GameData.instance.strength_level == 0 && GameData.instance.defense_level == 1) {
				EvolveIntoMonster ("ColourMapster");
			} else if (GameData.instance.strength_level == 2 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Gemini");
			} else if (GameData.instance.strength_level == 3 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Mykitas");
			} else if (GameData.instance.strength_level == 4 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Asura");
			} else if (GameData.instance.strength_level == 5 && GameData.instance.defense_level == 2) {
				EvolveIntoMonster ("Kyorah");
			} else if (GameData.instance.strength_level == 5 && GameData.instance.defense_level >= 5) {
				EvolveIntoMonster ("Fumegog");
			} else if (GameData.instance.strength_level == 6 && GameData.instance.defense_level >= 6) {
				EvolveIntoMonster ("Axoloc");
			} else if (GameData.instance.strength_level == 7 && GameData.instance.defense_level >= 7) {
				EvolveIntoMonster ("Bubbles");
			} else if (GameData.instance.strength_level == 8 && GameData.instance.defense_level >= 8) {
				EvolveIntoMonster ("Thaloch");
			} else if (GameData.instance.strength_level == 9 && GameData.instance.defense_level >= 9) {
				EvolveIntoMonster ("Chelonra");
			} else if (GameData.instance.strength_level == 10 && GameData.instance.defense_level >= 10) {
				EvolveIntoMonster ("Gaonaga");
			} else if (GameData.instance.strength_level == 11 && GameData.instance.defense_level >= 11) {
				EvolveIntoMonster ("Kagiza");
			} else if (GameData.instance.strength_level == 17 && GameData.instance.defense_level >= 18) {
				EvolveIntoMonster ("DragonLotus");
			} else if (GameData.instance.strength_level == 18 && GameData.instance.defense_level >= 17) {
				EvolveIntoMonster ("Exterminus");
			} else {
				EvolveIntoMonster ("Shezura");
			}
        }
        return;

    }
    void AttemptEvolution() {
        if (GameController.instance.character.transform.localPosition.x < -0.2f || GameController.instance.character.transform.localPosition.x > 0.2f || GameData.instance.away ) return;
        GameData.instance.evolved = true;
		if (GameController.instance.action_tier == 1) {
			GameController.instance.HandleVersusLocked ();
		}
        evol_anim.Evolve();
    }
	// Update is called once per frame
	void Update () {
        if (GameController.instance.game_state != GAME_STATE.PLAY || GameData.instance.evolved) return;

		if (GameController.instance.evolve) {
			AttemptEvolution ();
		} 
			
		if (up) {
			up = false;
			EvolveIntoNextMonster ();
		}
		if (down) {
			down = false;
			EvolveIntoPreviousMonster ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			cheat = true;
			Invoke ("StopCheat", 5f);
		}

		if (cheat) {
			if (!up && Input.GetKeyDown (KeyCode.UpArrow)) {
				up = true;
			}
			if (!down && Input.GetKeyDown (KeyCode.DownArrow)) {
				down = true;
			}
		}
	}
	bool isMonster(string name){
		return GameData.instance.info_name == name;
	}
	public bool IsLarge(){

		int[] large_monster_numbers = { 
			GetMonsterNum("Thaloch"),
			GetMonsterNum("X-I"),

		};





		for (int i = 0; i < large_monster_numbers.Length; i++) {
			if (GameData.instance.monster_num == large_monster_numbers [i]) {
				return true;
			}
		}
		return false;
	}
	bool IsBaby(){

		int[] baby_numbers = { 
			GetMonsterNum("Rarikin"),
			GetMonsterNum("Orikin"),
			GetMonsterNum("Herikin"),
			GetMonsterNum("Ukokin"),

		};





		for (int i = 0; i < baby_numbers.Length; i++) {
			if (GameData.instance.monster_num == baby_numbers [i]) {
				return true;
			}
		}
		return false;
	}
	public void EvolveIntoRandomMonster(){
		EvolveIntoMonster (GetRandomMonster ());
	}
	public void EvolveIntoNextMonster(){
		print ("Cheat code entered: NEXT MONSTER");
		GameData.instance.monster_num++;
		if(GameData.instance.monster_num == 25) GameData.instance.monster_num = 0;
		Evolve();
	}
	public void EvolveIntoPreviousMonster(){
		print ("Cheat code entered: PREVIOUS MONSTER");
		GameData.instance.monster_num -= 1;
		if(GameData.instance.monster_num == -1) GameData.instance.monster_num = 25;
		Evolve();
	}
	public void EvolveIntoMonster(string monster){
		GameData.instance.monster_num = GetMonsterNum(monster);
        print("new monster num for " + monster + ": " + GetMonsterNum(monster));
		Evolve ();
	}
	public string GetRandomMonster(){
		int rand = UnityEngine.Random.Range (0, GameController.instance.monsters.Length);
		return GetMonsterName (rand);
	}
	public string GetMonsterName(int num){
		switch (num) {
		case 0:
			return "Rarikin";
		case 1:
			return "Orikin";
		case 2:
			return "Gemini";
		case 3:
			return "Asura";
		case 4:
			return "Axoloc";
		case 5:
			return "Bubbles";
		case 6:
			return "DragonLotus";
		case 7:
			return "Exterminus";
		case 8:
			return "Fumegog";
		case 9:
			return "Gaonaga";
		case 10:
			return "X-I";
		case 11:
			return "Yiemniak";
		case 12:
			return "Ghaspar";
		case 13:
			return "Chelonra";
		case 14:
			return "Delta";
		case 15:
			return "Mykitas";
		case 16:
			return "Kerberos";
		case 17:
			return "Thaloch";
		case 18:
			return "Kyorah";
		case 19:
			return "Herikin";
		case 20:
			return "Ukokin";
		case 21:
			return "Kagiza";
		case 22:
			return "ColourMapster";
		case 23:
			return "Tsuplex";
		case 24:
			return "Cyning";
		case 25:
			return "Shezura";
		case 26:
			return "DEAD";
		default:
			print ("error in GetMonsterName: INVALID NUM");
			return "error in GetMonsterName: INVALID NUM";

		}

	}
	public int GetMonsterNum(string name){
		switch (name) {
		case "Rarikin":
			return 0;
		case "Orikin":
			return 1;
		case "Gemini":
			return 2;
		case "Asura":
			return 3;
		case "Axoloc":
			return 4;
		case "Bubbles":
			return 5;
		case "DragonLotus":
			return 6;
		case "Exterminus":
			return 7;
		case "Fumegog":
			return 8;
		case "Gaonaga":
			return 9;
		case "X-I":
			return 10;
		case "Yiemniak":
			return 11;
		case "Ghaspar":
			return 12;
		case "Chelonra":
			return 13;
		case "Delta":
			return 14;
		case "Mykitas":
			return 15;
		case "Kerberos":
			return 16;
		case "Thaloch":
			return 17;
		case "Kyorah":
			return 18;
		case "Herikin":
			return 19;
		case "Ukokin":
			return 20;
		case "Kagiza":
			return 21;
		case "ColourMapster":
			return 22;
		case "Tsuplex":
			return 23;
		case "Cyning":
			return 24;
		case "Shezura":
			return 25;
		case "DEAD":
			return 26;
		default:
			print ("error in GetMonsterNum: INVALID NAME");
			return -1;
		}
	}
	void StopCheat(){
		cheat = false;
	}
	public void Evolve(){
		SetMonster (GameData.instance.monster_num);
	}
	void SetMonster(int n){
		MultiplayerCSh.instance.creature.SetActive(true);
		new_body_sprend.sprite = GameController.instance.monsters[n].original_sprite;
        GameController.instance.character_sprite_window_obj.GetComponentInChildren<SpriteGrabber>().texture = GameController.instance.monsters[n].sprite_sheet;
        GameController.instance.character_sprite_window_obj.GetComponentInChildren<SpriteGrabber> ().GetSprites ();
        GameController.instance.character_sprite_window_obj.GetComponentInChildren<SpriteRenderer>().sprite = GameController.instance.monsters[n].original_sprite;
        GameController.instance.toy.GetComponentInChildren<Character>().monster = GameController.instance.monsters[n].GetComponent<Creature>();
        GameData.instance.info_name = GameController.instance.monsters [n].info_name;
		GameData.instance.monster_tier = GameController.instance.monsters [n].GetComponent<Creature> ().evolution_tier;
        GameController.instance.character_sprite_window_obj.GetComponentInChildren<IdleAnimation>().StartAnimation();
		GameController.instance.fireball_object = GameController.instance.monsters [n].fireball;
    }
	bool EvolutionCheck(float sa, float sp, float da, float dp, string monster){
		if (GameData.instance.strength_level <= sa && 
			GameData.instance.strength_times_played >= sp && 
			GameData.instance.defense_level <= da && 
			GameData.instance.defense_times_played >= dp) {
			EvolveIntoMonster (monster);
			return true;
		} 
		return false;
	}
}
