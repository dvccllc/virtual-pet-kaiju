using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthMiniGame2 : MonoBehaviour {
    public static StrengthMiniGame2 instance;

	public int current_wave = 0;
    public int win_wave = 0;
	public int player_choice;
	int counter = 0;
	int step = 0;
	public float round_delay, input_delay, go_delay, arrow_delay;
	public int total_score = 0;
	float strength_reward = 0.25f;
    float coin_multiplier = 2f;
	public bool awaiting_input = false;
	public bool incorrect = false;
	public GameObject[] arrow_sprites;
	public TextMesh wave_text;
	public TextMesh xxx;
	int round_num = 1;
	public List<int> moves = new List<int>();
	Vector3 original_position;
	public Vector3 center_position;
	public float kaiju_scale;
	public float end_switch_delay;
	public float animation_delay;
	public int lives = 3; //three lives
    // Use this for initialization
    void Start() {
        instance = this;


    }

    // Update is called once per frame
    void Update() {

    }
    public void StartGame()
    {
		xxx.text = "";
		GameController.instance.creature_sprend.enabled = false;
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer> ().enabled = true;
		original_position = GameController.instance.mini_game_kaiju.transform.localPosition;
		GameController.instance.mini_game_kaiju.transform.localPosition = new Vector3 (center_position.x, center_position.y, center_position.z);
		GameController.instance.mini_game_kaiju.transform.localScale = Vector3.one * kaiju_scale;
		OriginalSprite ();
		moves.Clear ();
		lives = 3;
		counter = 0;
		step = 0;
		round_num = 1;
		wave_text.text = round_num.ToString();
		//Disable the sprites. 
		for (int i = 0; i < arrow_sprites.Length; i++) {
			arrow_sprites [i].SetActive (false);
		}
		Invoke ("Round", go_delay);
    }

	void Round(){
		//Add on another move to the pattern
		AddMove ();
		for (int i = 0; i < moves.Count; i++) {
			print (moves [i]);
		}
		PlayRound ();
	}

	void AddMove() {
		int rand = (int)UnityEngine.Random.Range (0,3);
		moves.Add(rand);
	}

	void PlayRound() {
		DenyInput ();
		//Make the arrows/kaiju dance in the given sequence
		StartSequence();

	}

	public void StartSequence(){
		step = 0;
		DenyInput ();
		Invoke ("PlayMove", arrow_delay);
	}

	public void EndSequence(){
		Invoke("OriginalSprite", 0);
		counter = 0;
		AcceptInput();
	}

	public void PlayMove(){
		//Do Move
		MoveMonster(moves[step]);
		step++;
		if (step == moves.Count) {
			CancelInvoke ("PlayMove");
			step = 0;
			Invoke ("EndSequence", input_delay);
		} else {
			Invoke ("PlayMove", .5f);
		}
	}



	public void Check_move(){
		if (counter >= moves.Count) {
			return;
		}
		if(moves[counter] != player_choice)
		{
			incorrect = true; 
			SoundsController.instance.PlaySound ("crush");
		}
		counter++;

		if (incorrect) {//TODO: declare incorrect
			lives = lives - 1;
			counter = 0;
			incorrect = false;
			xxx.text = "";
			if (lives == 0) {
				xxx.text = "XXX";
				EndGame (); //TODO: Change EndGame function
			} else if (lives == 1) {
				//If lives = 1 then display 2 X's
				xxx.text = "XX"; 
				PlayRound ();//Repeat at level
			} else if (lives == 2) {
				//If lives = 1 then display 1 X
				xxx.text = "X"; 
				PlayRound ();//Repeat at level
			}

		} 
	}

	void PlayerChoice(int choice) {
		player_choice = choice;
		MoveMonster(player_choice);
	}


	public void PressLeftButton() {
		if (!awaiting_input)
			return;
		PlayerChoice(0);
		Check_move ();
		NextRound();
	}
	public void PressMiddleButton() {
		if (!awaiting_input)
			return;
		PlayerChoice(1);
		Check_move ();
		NextRound();
	}
	public void PressRightButton() {
		if (!awaiting_input)
			return;
		PlayerChoice(2);
		Check_move ();
		NextRound();
	}
	void NextRoundBeep() {
			SoundsController.instance.PlaySound ("positive beep");
	}
	void NextRound() {
		if (lives > 0 && counter == moves.Count) {
			CancelInvoke("NextRoundBeep");
			Invoke("NextRoundBeep", 0.5f);
			round_num++;
			wave_text.text = round_num.ToString();
			Invoke ("Round", round_delay);
		} 
	}
	void MoveMonster(int c) { //moves them
		arrow_sprites[0].SetActive(false);
		arrow_sprites [1].SetActive (false);
		arrow_sprites[2].SetActive(false);
		if(c == 0) {
			arrow_sprites[0].SetActive(true);
			ChangeSpriteLeft ();
		}
		else if(c == 1) {
			arrow_sprites[1].SetActive(true);
			ChangeSpriteMiddle ();
		}
		else if(c == 2) {
			arrow_sprites[2].SetActive(true);
			ChangeSpriteRight ();
		}
		else if(c == -1) {
			//DO Nothing
		}
			

	}
	public void ChangeSpriteLeft() {

		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer>().sprite = GameController.instance.character.GetComponent<Character> ().monster.mini_game_sprites[0];
		SoundsController.instance.PlaySound ("mm beep 1");
	}
	public void ChangeSpriteMiddle() {
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer>().sprite = GameController.instance.character.GetComponent<Character> ().monster.mini_game_sprites[1];
		SoundsController.instance.PlaySound ("mm beep 2");
	}
	public void ChangeSpriteRight() {
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer>().sprite = GameController.instance.character.GetComponent<Character> ().monster.mini_game_sprites[2];
		SoundsController.instance.PlaySound ("mm beep 3");
	}

	public void OriginalSprite() { //TODO: Change sprite array to have an original sprite
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer>().sprite = GameController.instance.character.GetComponent<Character> ().monster.mini_game_sprites[2];
	}


    public void EndGame()
    {
		ResetPlayerChoice ();
		CancelInvoke ("Round");
		CancelInvoke ("PlayMove");
		CancelInvoke ("OriginalSprite");
		CancelInvoke ("EndSequence");
		//ResetPlayerChoice ();
		SwitchToPlay();
    }
	public void SwitchToPlay() {
		GameController.instance.creature_sprend.enabled = true;
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer> ().enabled = false;
		GameController.instance.mini_game_kaiju.transform.localPosition = original_position;
		GameController.instance.mini_game_kaiju.transform.localScale = Vector3.one;
		GameController.instance.SwitchStates(GAME_STATE.PLAY);
		if(round_num > 1) Invoke("WinAnimation", animation_delay);
		else {
			SoundsController.instance.PlaySound ("bad");
		}
	}
    public void EndGameEarly()
	{
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer> ().enabled = false;
		EndGame ();
	}

	void DeactivateArrow(){
		for (int i = 0; i < arrow_sprites.Length; i++) {
			arrow_sprites [i].SetActive (false);
		}
	}


	public void AcceptInput() {
		awaiting_input = true;
	}
	public void DenyInput() {
		awaiting_input = false;
	}
	void ResetPlayerChoice() {
		player_choice = -1;
	}	
	void WinAnimation(){
		xxx.text = "";
		SoundsController.instance.PlaySound ("positive beep");
        GameController.instance.icon_reward.UpdateValue(round_num, "strength");
        GameController.instance.coin_reward.UpdateValue(round_num, "coin");
		float reward = strength_reward * round_num;
		if(reward > 2.5f) reward = 2.5f;
        GameData.instance.strength_level += reward;
		if(round_num > GameData.instance.score_strength_2) GameData.instance.score_strength_2 = round_num;
        GameData.instance.num_coins += (int)Math.Round((round_num * strength_reward) * coin_multiplier, 0);
        //CoinsText.instance.UpdateCoinsText();
    }
}
