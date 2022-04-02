using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBattle : MonoBehaviour {
    public static OfflineBattle instance;
	//determine whether to accept input from the user
	public bool awaiting_input = false;
	public bool game_over = false;
	//the time inbetween rounds
	public float round_delay, start_delay, go_delay, player_choose_duration;
	public int opponent_choice, player_choice;
	float animation_delay = 1f;
	float value = 0.1f;
	float reward; 
	//float coin_multiplier = 5f;
	public TextMesh go_text, result_text, user_health_text, opponent_health_text, instruction_text;
	public SpriteRenderer player_choice_sprite, opponent_choice_sprite;
	public Sprite[] rock_paper_scissors_sprites;
	public ChoiceEllipses opponent_ellipses, player_ellipses;

	public bool allowed;

	public float user_health, opponent_health; 
	public float user_strength, user_defense, opponent_strength, opponent_defense; 


	//Initialize the script, DO NOT CHANGE THIS
    void Start() {
        instance = this;
    }

    public void StartGame()
    {
		allowed = false; 
		awaiting_input = false;
		game_over = false;
		opponent_choice_sprite.enabled = false;
		GameController.instance.creature_sprend.enabled = false;
		player_ellipses.gameObject.SetActive(true);
		GameController.instance.mini_game_kaiju.SetActive(true);
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer>().enabled = true;
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer>().sprite = GameController.instance.character.GetComponent<Character> ().monster.mini_game_sprites[0];
		ResetOpponentChoice();
		ResetPlayerChoice();
		ResetGoText();
		ResetResultText();
		instruction_text.text = "";
		user_health = 50;
		opponent_health = 50;
		reward = 0;
		user_strength = GameData.instance.strength_level;
		user_defense = GameData.instance.defense_level;

		if ((user_strength + user_defense) >= 20) {
			opponent_strength = (int)UnityEngine.Random.Range (5, 15);
			opponent_defense = (int)UnityEngine.Random.Range (5, 15);
		} else {
			opponent_strength = (int)UnityEngine.Random.Range (1, 7);
			opponent_defense = (int)UnityEngine.Random.Range (1, 7);
		}
		user_health_text.text = "HP: " + user_health.ToString();
		opponent_health_text.text = "HP: " + opponent_health.ToString();
		CancelInvoke("PlayRound");
		CancelInvoke("InvokeRound");
		CancelInvoke("PlayerRanOutOfTime");
		Invoke("InvokeRound", start_delay);
    }
	public void EndGame(bool early)
    {
		
		ResetOpponentChoice ();
		ResetPlayerChoice ();
		ResetGoText ();
		CancelInvoke ("PlayRound");
		CancelInvoke ("InvokeRound");
		CancelInvoke ("PlayerRanOutOfTime");
		GameController.instance.creature_sprend.enabled = true;
		GameController.instance.mini_game_kaiju.GetComponent<SpriteRenderer> ().enabled = false;
		opponent_ellipses.text.text = "";
		player_ellipses.text.text = "";
		opponent_ellipses.StopAnimation ();
		player_ellipses.StopAnimation ();
		game_over = !early;
		if (early) {
			GameController.instance.OfflineBattleEndGameEarly ();
		}
    }

    public void EndGameEarly()
	{
		opponent_health = 1;
		EndGame(true);
	}
	public void PressButton(int button_id) {
		if(button_id == 0) {
			EndGameEarly();
			return;
		}
		if (!awaiting_input)
			return;
		awaiting_input = false;
		if(button_id == 1) {
			PlayerChoice(button_id - 1);
		}
		if(button_id == 2) {
			PlayerChoice(button_id - 1);
		}
		if(button_id == 3) {
			PlayerChoice(button_id - 1);
		}
	}
	void PlayRound() {
		if (!gameIsOver ()) {
			SetGoText ();
			AcceptInput ();
			CancelInvoke ("PlayerRanOutOfTime");
			Invoke ("PlayerRanOutOfTime", player_choose_duration);
		}
	}
	void InvokeRound() {
		SetOpponentChoice(-1);
		SetPlayerChoice(-1);
		if (!gameIsOver ()) {
			ResetResultText ();
		}
		CancelInvoke("PlayRound");
		CancelInvoke("InvokeRound");
		Invoke("PlayRound", go_delay);
	}
	void ResetOpponentChoice() {
		opponent_choice_sprite.sprite = null; 
		opponent_choice_sprite.enabled = false;
		opponent_ellipses.StopAnimation();
	}
	void ResetPlayerChoice() {
		player_choice_sprite.enabled = false;
		player_ellipses.StopAnimation();
	}	
	public bool gameIsOver() {
		if (user_health <= 0 || opponent_health <= 0) {
			instruction_text.text = "Press any button\nto continue.";
			if (user_health <= 0) {
				result_text.text = "YOU\nLOSE!";
				user_health = -100;
			}
			if (opponent_health <= 0) {
				result_text.text = "YOU\nWIN!";
				opponent_health = -100;
			}
			return true;
		} else {
			return false;
		}
	}
	void ResetGoText() {
		go_text.text = "";
	}
	void ResetResultText() {
		result_text.text = "";

	}
	void SetOpponentChoice(int c) {
		opponent_ellipses.text.text = "";
		opponent_ellipses.StopAnimation();
		opponent_choice_sprite.enabled = true;
		if(c == 0) {
			opponent_choice_sprite.sprite = rock_paper_scissors_sprites[0];
		}
		if(c == 1) {
			opponent_choice_sprite.sprite = rock_paper_scissors_sprites[1];
		}
		if(c == 2) {
			opponent_choice_sprite.sprite = rock_paper_scissors_sprites[2];
		}
		if(c == -1) {
			opponent_choice_sprite.enabled = false;
			opponent_ellipses.StartAnimation();
		}
	}
	void SetPlayerChoice(int c) {
		player_ellipses.text.text = "";
		player_ellipses.StopAnimation();
		player_choice_sprite.enabled = true;
		if(c == 0) {
			player_choice_sprite.sprite = rock_paper_scissors_sprites[0];
		}
		if(c == 1) {
			player_choice_sprite.sprite = rock_paper_scissors_sprites[1];
		}
		if(c == 2) {
			player_choice_sprite.sprite = rock_paper_scissors_sprites[2];
		}
		if(c == -1) {
			player_choice_sprite.enabled = false;
			player_ellipses.StartAnimation();
		}
		if(c == -2) {
			player_choice_sprite.enabled = false;
		}
	}
	void SetGoText() {
		go_text.text = "GO!";
	}
	void SetResultText() {
		if(player_choice == -2) {
			result_text.text = "LOSE!";
			user_health = user_health - 10 * opponent_strength/user_defense;
			if (user_health < 0) {
				user_health = 0;
			}
			user_health_text.text = "HP: " + ((int) user_health).ToString();
			opponent_health_text.text = "HP: " + ((int)opponent_health).ToString();
			return;
		}
		// 0 is rock
		// 1 is paper
		// 2 is scissors
		if(player_choice == opponent_choice) {
			result_text.text = "DRAW!";
			user_health_text.text = "HP: " + ((int)user_health).ToString();
			opponent_health_text.text = "HP: " + ((int)opponent_health).ToString(); 
			return;
		}
		// I chose rock
		if(player_choice == 0) {
			if(opponent_choice == 1) {
				// he chose paper i lose
				user_health = user_health - 10 * opponent_strength/user_defense;
				if (user_health < 0) {
					user_health = 0;
				}
				result_text.text = "LOSE!";
			} 
			else {
				// he chose scissors i win
				opponent_health = opponent_health - 10 * user_strength/opponent_defense;
				if (opponent_health < 0) {
					opponent_health = 0;
				}
				result_text.text = "WIN!";
			}
			user_health_text.text = "HP: " + ((int)user_health).ToString();
			opponent_health_text.text = "HP: " + ((int)opponent_health).ToString();
			return;
		}
		// I chose paper
		else if(player_choice == 1) {
			if(opponent_choice == 0) {
				// he chose rock i win
				opponent_health = opponent_health - 10 * user_strength/opponent_defense;
				if (opponent_health < 0) {
					opponent_health = 0;
				}
				result_text.text = "WIN!";
			} 
			else {
				// he chose scissors i lose
				user_health = user_health - 10 * opponent_strength/user_defense;
				if (user_health < 0) {
					user_health = 0;
				}
				result_text.text = "LOSE!";
			}
			user_health_text.text = "HP: " + ((int)user_health).ToString();
			opponent_health_text.text = "HP: " + ((int)opponent_health).ToString(); 
			return;
		}
		// I chose scissors
		else {
			if(opponent_choice == 0) {
				// he chose rock i lose
				user_health = user_health - 10 * opponent_strength/user_defense;
				if (user_health < 0) {
					user_health = 0;
				}
				result_text.text = "LOSE!";
			} 
			else {
				// he chose paper i win
				opponent_health = opponent_health - 10 * user_strength/opponent_defense;
				if (opponent_health < 0) {
					opponent_health = 0;
				}
				result_text.text = "WIN!";
			}
			user_health_text.text = "HP: " + ((int)user_health).ToString();
			opponent_health_text.text = "HP: " + ((int)opponent_health).ToString(); 
			return;
		}

	}
	void OpponentChoice() {
		opponent_choice = (int)UnityEngine.Random.Range(0, 3);
	}
	void PlayerChoice(int choice) {
		CancelInvoke("PlayerRanOutOfTime");
		player_choice = choice;
		OpponentChoice();
		SetOpponentChoice(opponent_choice);
		SetPlayerChoice(player_choice);
		SetResultText();
		ResetGoText();
		if(!gameIsOver()) Invoke("InvokeRound", round_delay);
		else EndGame(false);
	}
	void PlayerRanOutOfTime() {
		DenyInput();
		PlayerChoice(-2);
	}
	public void AcceptInput() {
		awaiting_input = true;
	}
	public void DenyInput() {
		awaiting_input = false;
	}
	public void InvokeWinAnimation() {
		if (opponent_health <= 0) {
			Invoke ("WinAnimation", animation_delay);
		}
	}
    void WinAnimation(){
		reward = 2 + user_health * value;  
		SoundsController.instance.PlaySound ("positive beep");
		GameController.instance.coin_reward.UpdateValue(reward, "coin");
		GameData.instance.num_coins += (int)Math.Round((0.5f * reward) * 5f);
		//CoinsText.instance.UpdateCoinsText();

    }
}
