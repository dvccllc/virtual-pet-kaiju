using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PERSONAL_STAT { WINS, LOSSES, STRENGTH_1, STRENGTH_2, DEFENSE_1, DEFENSE_2 }
public enum BATTLE_RESULT { WIN, LOSS }
public enum GAME_STATE { VOID, TITLE, START_MENU, PLAY, PAUSE, PERSONAL_STATS, STRENGTH_GAMES, DEFENSE_GAMES, MULTIPLAYER_MENU, OUT_TO_BATTLE, OFFLINE_BATTLE, MINI_GAME_1, MINI_GAME_2, MINI_GAME_3, MINI_GAME_4 }
[Serializable]
public class SendData
{
    public string id;
    public int action;
    public string kaiju_name;
    public float defense;
    public float strength;
    public int energy;

	public string leaderboard_tag;
	// need scores
}
public class GameController : MonoBehaviour {
	public static GameController instance;
    public int DEATH_NUM = 26;
    public bool reset_fetch = false;
    public GameObject character_sprite_window_obj;
    public MeshRenderer[] interface_mesh_renderers;
    public SpriteRenderer[] interface_sprite_renderers;
    public bool tutorial_prompt_active;
    public int tutorial_prompt_num = -1;
    public TextMesh tutorial_text, tutorial_header;
    public RewardAnimation coin_reward;
    public RewardAnimation icon_reward;
    public GameObject fetch_prompt;
    public bool fetch_prompt_active;
    public PersonalStat[] stats;
    public float set_adult_offset;
	public GameObject mini_game_kaiju;
    public GameObject canvas;
    public bool store = false;
    public bool settings = false;
    public bool leaderboards = false;
    public bool playing_intro = false;
    int intro_ticks = 0;
    public GameObject store_bar, store_coin1, store_coin2;
    public GameObject character;
    public SpriteRenderer creature_sprend;
    public SpriteRenderer away_sprend;
    public SpriteRenderer loading_sprite;
    public bool api_loading = false;
    public Button[] buttons;
    public CoinsText pause_coins;
	public GAME_STATE game_state = GAME_STATE.VOID;
    public GameObject interface_obj;
    public GameObject pause_interface_obj, pause_interface_obj2, pause_interface_obj3, strength_interface_obj, strength_interface_obj2, defense_interface_obj, defense_interface_obj2, multiplayer_interface_obj, multiplayer_interface_obj2, multiplayer_interface_obj3;
	public GameObject title, intro, start_menu, cont, tutorial_prompt_interface, out_to_battle_interface, out_to_battle_interface2, out_to_battle_interface3, offline_battle_interface, offline_battle_interface2, missile_defense_interface1, 
    mm_minigame_interface, mm_minigame_interface2, catcher_minigame_interface, catcher_minigame_interface2, catcher_minigame_interface3, personal_stats_interface, pause_interface, pause_interface2, pause_interface3, play_interface, play_interface_2, exit_interface, general_interface, general_interface2, strength_interface, strength_interface2, defense_interface, defense_interface2, multiplayer_interface, multiplayer_interface2, multiplayer_interface3;
	public float shrink_speed, grow_speed;
	public Vector2 title_scale, start_menu_scale;
	public Creature[] monsters;
	int react_age;
	public bool fireball, shrunk = false;
	bool fireball_cd = true;
    public int fireball_num, roarFactor;
	public float fireball_cooldown_time, roarTimer, fireball_strength_factor, fireball_defense_factor;
	public bool evolve, evolving;
    public GameObject fireball_object;
    public GameObject toy,s_wave,d_wave, energy_warning, out_to_battle_energy_warning;
    public RotateShell rotate_ctr;
    public int action_tier = 0;
    public GameObject[] action_tier_0;
    public GameObject[] action_tier_1;
	public GameObject versus, versus_locked;
    // INFO SAVED DURING A PAUSED SESSION
    GAME_STATE paused_state = GAME_STATE.VOID;
	Vector3 original_scale;
	int presses;
	int press_id;
    float shrink_scale = 0.75f;
    int max_energy = 18;
    float fireball_offset_x = -1.8f;
    float fireball_offset_y = 0f;
    public int mini_game_energy_cost = 2;
    public int battle_energy_cost = 5;
    float interface_enter_delay = 0.2f;
    float character_enter_delay = 0.5f;
    float random_roar_delay = 5f;
    float start_delay = 1f;
    float creature_fireball_z = 16f;

    float energy_enter_delay = 0.7f;

	public int last_pressed = 0; 

    void Start () {

		evolve = false;
		press_id = -1;
		presses = 0;

		instance = this;
        if(SaveLoad.instance.file_exists_on_load)
        {
            SaveLoad.instance.Load();
        }
        else
        {
            SaveLoad.instance.EraseFile();
            SaveLoad.instance.Load();
        }
    }
    bool ActiveRenderers() {
        bool active_renderer = false;
        foreach(SpriteRenderer spr in interface_sprite_renderers) {
            if(spr && spr.enabled) {
                active_renderer = true;
                print("active: " + spr.gameObject.name);
            } 
        }
        foreach(MeshRenderer msh in interface_mesh_renderers) {
            if(msh && msh.enabled) {
                active_renderer = true;
                print("active: " + msh.gameObject.name);
            }
        }
        if (pause_interface) active_renderer = true;
        return active_renderer;
    }
    void PlayTheme(){
		SoundsController.instance.PlayTheme ();
	}
    void PlayIntro(){
		SoundsController.instance.PlayIntro ();
	}
    void StartIntroTick() {
        CancelInvoke("IntroTick");
        intro_ticks = 0;
        IntroTick();
    }
    void IntroTick() {
        if(intro_ticks > 15) {
            CancelInvoke("IntroTick");
            playing_intro = true;
        }
        else {
            Invoke("IntroTick", 1f);
            intro_ticks++;
        }
    }
	public void ActivateCurrentHour() {
		int currentHour = Int32.Parse (System.DateTime.UtcNow.ToString ("HH"));
		GameData.instance.hours [currentHour] = true;
	}
    public void CycleActionTier()
    {
        action_tier++;
        if (action_tier > 1) action_tier = 0;
        HandleActionTierIcons();
    }
    public void HandleActionTierIcons()
    {
        print("action tier handle, " + action_tier);
        for (int i = 0; i < action_tier_0.Length; i++)
        {
            action_tier_0[i].SetActive(false);
        }
        for (int i = 0; i < action_tier_1.Length; i++)
        {
            action_tier_1[i].SetActive(false);
        }
        if(action_tier == 0)
        {
            for (int i = 0; i < action_tier_0.Length; i++)
            {
                action_tier_0[i].SetActive(true);
            }
            HandleVersusLocked ();
            versus_locked.SetActive(false);
            versus.SetActive(false);
        } 
        else if(action_tier == 1)
        {
            for (int i = 0; i < action_tier_1.Length; i++)
            {
                action_tier_1[i].SetActive(true);
            }
            versus_locked.SetActive(true);
            versus.SetActive(true);
			HandleVersusLocked ();
        }

    }
	public void HandleVersusLocked() {
		if (GameData.instance.evolved) {
			versus.SetActive (true);
			versus_locked.SetActive (false);
		}
		else {
			versus.SetActive (false);
			versus_locked.SetActive (true);
		}
	}

	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.C)) {
            if(ActiveRenderers()) print("INTERFACE ON THE SCREEN");
            else print("no interface on the screen :)");
        }
		CheckButtons ();
		ControlScreen ();
		React ();
        if (fireball && !character.GetComponent<FadeIn>().moving[fireball_num] && fireball_cd)
        {
			fireball_cd = false;
            float skill = GameData.instance.defense_level;
            if (GameData.instance.strength_level > skill) skill = GameData.instance.strength_level;
            float fb_cd = fireball_cooldown_time - (skill / fireball_defense_factor);
			Invoke ("FireballCD", fb_cd);
            ShootFireball(fireball_num);
        }
        if (GameData.instance.away) creature_sprend.enabled = false;
        else if (game_state == GAME_STATE.PLAY) creature_sprend.enabled = true;
        if (api_loading)
        {
            if (!GameData.instance.away) creature_sprend.enabled = false;
            else away_sprend.enabled = false;
            loading_sprite.enabled = true;
        }
        else
        {
            loading_sprite.enabled = false;
        }
        ShrinkLargeMonster();
		UnshrinkLargeMonster ();
	}
	void FireballCD(){
		fireball_cd = true;
	}
	void CheckForSave(){
		cont.SetActive (File.Exists (Application.persistentDataPath + "/playerInfo.dat"));
	}
	void ShrinkLargeMonster(){
		if (!shrunk && Evolution.instance.IsLarge() && (game_state == GAME_STATE.MINI_GAME_1 || game_state == GAME_STATE.MINI_GAME_2 || game_state == GAME_STATE.PAUSE)) {
			original_scale = character.transform.localScale;
			character.transform.localScale *= shrink_scale;
			shrunk = true;
		} else if (!shrunk && game_state == GAME_STATE.MINI_GAME_1) {
			original_scale = character.transform.localScale;
			character.transform.localScale *= shrink_scale;
			shrunk = true;
		}
	}
	void UnshrinkLargeMonster(){
		if (shrunk && (game_state != GAME_STATE.MINI_GAME_1 && game_state != GAME_STATE.MINI_GAME_2 && game_state != GAME_STATE.PAUSE)) {
			character.transform.localScale = original_scale;
			shrunk = false;
		}

	}
	void CheckButtons(){
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons [i].react) {

				if (game_state == GAME_STATE.PLAY || game_state == GAME_STATE.PAUSE) {

					if (press_id == i)
						presses++;
					else {
						press_id = i;
						presses = 1;
					}

				} else {
					presses = 0;
					press_id = -1;
				}
				PressButton (i);
				buttons [i].react = false;
			}
		}

	}
	void React(){
		if (react_age != (int)GameData.instance.age && !GameData.instance.away) {
			react_age = (int)GameData.instance.age;
			GameData.instance.energy_level++;
			if (GameData.instance.energy_level > max_energy)
				GameData.instance.energy_level = max_energy;
			
		} else {

		}
	}

	void StartGame(){
		SetMonster ();
        StartIntroTick();
        PlayTheme();
        rotate_ctr.zoomFarIn();
		SwitchStates (GAME_STATE.TITLE);
	}
	public void SetMonster(){
		character_sprite_window_obj.GetComponentInChildren<SpriteGrabber>().texture = monsters [GameData.instance.monster_num].sprite_sheet;
		character_sprite_window_obj.GetComponentInChildren<SpriteGrabber> ().GetSprites ();
		character_sprite_window_obj.GetComponentInChildren<SpriteRenderer> ().sprite = monsters [GameData.instance.monster_num].original_sprite;
        character.GetComponent<Character>().monster_num = GameData.instance.monster_num;
        character.GetComponent<Character>().monster = monsters[GameData.instance.monster_num];
		fireball_object = monsters [GameData.instance.monster_num].fireball;
        GameData.instance.info_name = monsters [GameData.instance.monster_num].info_name;
        
	}
    void StartFireball_st(int n)
    {
        character.GetComponent<FadeIn>().StartMove(n);
        character.GetComponent<FadeIn>().moving[fireball_num] = true;
        fireball = true;
        fireball_num = n;
    }
	void StartFireball_d (int n)
	{
		character.GetComponent<FadeIn> ().moving [fireball_num] = true;
		fireball = true;
		fireball_num = n; 
	}

	void StartMovement (int n)
	{
		character.GetComponent<FadeIn> ().StartMove (n); 
	}

    void ShootFireball(int n)
    {
        fireball = false;
        character_sprite_window_obj.GetComponentInChildren<FireBallAnimation>().StartAnimation();
        GameObject fb = Instantiate(fireball_object);
		SoundsController.instance.PlaySound ("shoot");
        fb.transform.position = character.transform.position;
        fb.transform.parent = toy.transform;
        float adult_offset = 0f;
        if (GameData.instance.evolved) adult_offset = 1f;
        fb.transform.localPosition = new Vector3(fb.transform.localPosition.x + fireball_offset_x + (adult_offset * set_adult_offset), fb.transform.localPosition.y + fireball_offset_y, creature_fireball_z);
		if (shrunk)
			fb.transform.localScale *= shrink_scale;
    }
    void SwitchToOfflineBattle()
    {
        if(GameData.instance.away) return;
        if (GameData.instance.energy_level >= mini_game_energy_cost)
        {
            GameData.instance.energy_level -= mini_game_energy_cost;
        }
        else
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.OFFLINE_BATTLE);
    }
	public void OfflineBattleEndGameEarly() {
		SwitchStates (GAME_STATE.PLAY);
	}
    void SwitchToStrength_1()
    {

        if (GameData.instance.energy_level >= mini_game_energy_cost)
        {
            GameData.instance.energy_level -= mini_game_energy_cost;
        }
        else
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.MINI_GAME_1);
    }
    void SwitchToStrength_2()
    {
        if(!GameData.instance.evolved) return;
        if (GameData.instance.energy_level >= mini_game_energy_cost)
        {
            GameData.instance.energy_level -= mini_game_energy_cost;
        }
        else
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.MINI_GAME_3);
    }
    void SwitchToStrength_3()
    {
        /*
        if (GameData.instance.energy_level >= mini_game_energy_cost)
        {
            GameData.instance.energy_level -= mini_game_energy_cost;
        }
        else
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.MINI_GAME_3);
        */
    }
    void SwitchToMultiplayer()
    {
        if (GameData.instance.monster_num == DEATH_NUM) return;
        if(GameData.instance.evolved) SwitchStates(GAME_STATE.MULTIPLAYER_MENU);
    }
    public void SendToBattle()
    {
        ExitCharacter();
    }
	public void RefreshLeaderboards()
	{
		print("refreshing leaderboards");
	}
    public void FetchFromBattle(int wins, int losses, int coins, int exp_points) 
    {
        EnterCharacter();
        if(wins != 0 || losses != 0) { 
            EnterOutToBattleInterface3();
            fetch_prompt_active = true;
            GameData.instance.num_coins += coins;
            FetchPrompt.instance.UpdatePrompt(wins, losses, coins, exp_points);
        }
        StopFlashFetchLowEnergy();
    }
    void SendAPICall() {
        if (GameData.instance.energy_level < battle_energy_cost)
        {
            FlashOutToBattleLowEnergy();
            return;
        }
        MultiplayerCSh.instance.SendKaiju();
    }
    public void ResetAccountAPICall() {
        reset_fetch = true;
        MultiplayerCSh.instance.FetchKaiju();
    }
    void FetchAPICall() {
        MultiplayerCSh.instance.FetchKaiju();
    }
    public void RefreshBattleInterface() {
        if(!GameData.instance.away) return;
        MultiplayerCSh.instance.RefreshKaiju();
    }
    void SwitchToStrength()
    {
        if (GameData.instance.away || GameData.instance.monster_num == DEATH_NUM) return;
        if (GameData.instance.energy_level < mini_game_energy_cost)
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.STRENGTH_GAMES);
    }
    void SwitchToDefense_1()
    {
        if (GameData.instance.energy_level >= mini_game_energy_cost)
        {
            GameData.instance.energy_level -= mini_game_energy_cost;
        }
        else
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.MINI_GAME_2);
    }
    void SwitchToDefense_2()
    {
        if(!GameData.instance.evolved) return;
        if (GameData.instance.energy_level >= mini_game_energy_cost)
        {
            GameData.instance.energy_level -= mini_game_energy_cost;
        }
        else
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.MINI_GAME_4);
    }
    void SwitchToDefense_3()
    {
        /*
        if (GameData.instance.energy_level >= mini_game_energy_cost)
        {
            GameData.instance.energy_level -= mini_game_energy_cost;
        }
        else
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.MINI_GAME_3);
        */
    }
    void SwitchToDefense()
    {
        if (GameData.instance.away || GameData.instance.monster_num == DEATH_NUM) return;
        if (GameData.instance.energy_level < mini_game_energy_cost)
        {
            FlashLowEnergy();
            return;
        }
        SwitchStates(GAME_STATE.DEFENSE_GAMES);
    }
    public void WarpAwaySign()
    {
        character.transform.localPosition = new Vector3(0.11f, -3.3f, -0.69f);
    }
    public void PressButton(int button_id){
        if (MultiplayerChoices.instance && MultiplayerChoices.instance.txt1 && MultiplayerChoices.instance.txt1.text == "LOADING") return;
        if (game_state == GAME_STATE.VOID)
        {
            StartGame();
            return;
        }
        if (game_state == GAME_STATE.TITLE)
        {
            SoundsController.instance.StopTheme();
            SoundsController.instance.StopIntro();

            CancelInvoke("IntroTick");
            playing_intro = false;
            intro.SetActive(false);
            MultiplayerCSh.instance.UpdateAwayStatusSprites();
            SwitchStates(GAME_STATE.PLAY);
            if(GameData.instance.away)
            {
                Invoke("WarpAwaySign", 0.5f);
            }
            return;
        }
        ActivateCurrentHour();
        if (evolving) return;
        if(tutorial_prompt_active) {
            tutorial_prompt_active = false;
            GameData.instance.played[tutorial_prompt_num] = true;
            ExitTutorialPromptInterface();
            if(tutorial_prompt_num == 0) StrengthMiniGame1.instance.StartGame();
            if(tutorial_prompt_num == 1) DefenseMiniGame1.instance.StartSpawning();
            if(tutorial_prompt_num == 2) StrengthMiniGame2.instance.StartGame();
            if(tutorial_prompt_num == 3) DefenseMiniGame2.instance.StartGame();
            if(tutorial_prompt_num == 4) EnterMultiplayerGamesInterface();
            if(tutorial_prompt_num == 6) OfflineBattle.instance.StartGame();
            return;
        }
        if (game_state == GAME_STATE.PLAY) {
            if (rotate_ctr.zoomed_out 
            && !TextInterfaceCameraController.instance.pan_left 
            && !TextInterfaceCameraController.instance.pan_right 
            && !TextInterfaceCameraController.instance.pan_center)
            {
                rotate_ctr.zoomFarIn();
                return;
            }
			if (!rotate_ctr.zoomed_in || rotate_ctr.zoomed_out)
				return;
            if (button_id == 3)
            {
                CycleActionTier();
                return;
            }
            if (action_tier == 0)
            {
                if (button_id == 0)
                {
                    if(!ActiveRenderers()) {
                        canvas.SetActive(true);
                        rotate_ctr.zoomFar();
                    }
                    return;
                }
                if (button_id == 1)
                {
                    SwitchToStrength();
                    return;
                }
                if (button_id == 2)
                {
                    SwitchToDefense();
                    return;
                }

            } else if(action_tier == 1)
            {
                if (button_id == 0)
                {
                    if(!ActiveRenderers()) {
                        canvas.SetActive(true);
                        rotate_ctr.zoomFar();
                    }
                    return;
                }
                if (button_id == 1)
                {
                    SwitchToMultiplayer();
                    return;
                }
                if (button_id == 2)
                {
                    if (GameData.instance.monster_num == DEATH_NUM) return;
                    SwitchStates(GAME_STATE.PAUSE);
                    return;
                }
            }
		}
        if (game_state == GAME_STATE.STRENGTH_GAMES)
        {
            if (button_id == 0)
            {
                SwitchStates(GAME_STATE.PLAY);
                return;
            }
            if (button_id == 2)
            {
                StrengthChoices.instance.CycleChoice();
            }
            if (button_id == 3)
            {
                if (StrengthChoices.instance.choice == 0)
                {
                    SwitchToStrength_1();
                    return;
                }
                if (StrengthChoices.instance.choice == 1)
                {
                    SwitchToStrength_2();
                    return;
                }
                if (StrengthChoices.instance.choice == 2)
                {
                    SwitchToStrength_3();
                    return;
                }
            }
        }
        if (game_state == GAME_STATE.DEFENSE_GAMES)
        {
            if (button_id == 0)
            {
                SwitchStates(GAME_STATE.PLAY);
                return;
            }
            if (button_id == 2)
            {
                DefenseChoices.instance.CycleChoice();
            }
            if (button_id == 3)
            {
                if (DefenseChoices.instance.choice == 0)
                {
                    SwitchToDefense_1();
                    return;
                }
                if (DefenseChoices.instance.choice == 1)
                {
                    SwitchToDefense_2();
                    return;
                }
                if (DefenseChoices.instance.choice == 2)
                {
                    SwitchToDefense_3();
                    return;
                }
            }
        }
        if (game_state == GAME_STATE.MULTIPLAYER_MENU)
        {
            if (button_id == 0)
            {
                SwitchStates(GAME_STATE.PLAY);
                return;
            }
            if (button_id == 2)
            {
                MultiplayerChoices.instance.CycleChoice();
            }
            if (button_id == 3)
            {
                
                if (MultiplayerChoices.instance.choice == 0)
                {
                    SwitchStates(GAME_STATE.OUT_TO_BATTLE);
                    return;
                }
                if (MultiplayerChoices.instance.choice == 1)
                {
                    return;
                }
                if (MultiplayerChoices.instance.choice == 2)
                {
                    SwitchToOfflineBattle();
                    return;
                }
            }
        }
        if (game_state == GAME_STATE.OUT_TO_BATTLE)
        {
            if(fetch_prompt_active) {
                if (button_id == 0 || button_id == 1 || button_id == 2 || button_id == 3) {
                    fetch_prompt_active = false;
                    ExitOutToBattleInterface3();
                    MultiplayerChoices.instance.UpdateBattleStatusText();
                    return;
                }
            }
            if (button_id == 0)
            {
                SwitchStates(GAME_STATE.MULTIPLAYER_MENU);
                return;
            }
            if (button_id == 1 || button_id == 2)
            {
                if(GameData.instance.away) FetchAPICall();
                else SendAPICall();
                return;
            }
            if (button_id == 3) {
                RefreshBattleInterface();
                return;
            }
        }
        if (game_state == GAME_STATE.OFFLINE_BATTLE)
        {
            //offline-battle-todo:
            if(OfflineBattle.instance.game_over) {
                SwitchStates(GAME_STATE.PLAY);
				OfflineBattle.instance.InvokeWinAnimation ();
                return;
            }
            if (button_id == 0)
            {
                OfflineBattle.instance.PressButton(button_id);
                return;
            }
            if (button_id == 1)
            {
                OfflineBattle.instance.PressButton(button_id);
                return;
            }

            if (button_id == 2)
            {
                OfflineBattle.instance.PressButton(button_id);
                return;
            }

            if (button_id == 3)
            {
                OfflineBattle.instance.PressButton(button_id);
                return;
            }
        }
        if (game_state == GAME_STATE.MINI_GAME_1) {
			if (button_id == 0) {
				SwitchStates (GAME_STATE.PLAY);
				return;
			}
			if (button_id == 1 || button_id == 2 || button_id == 3)
            {
				if(StrengthBar.instance.ticking) {
                	StartFireball_st(2);
					StrengthMiniGame1.instance.fireballs++;
                    StrengthMiniGame1.instance.attempts_left.text = "ATTEMPTS LEFT: " + (1 + StrengthMiniGame1.instance.wave - StrengthMiniGame1.instance.fireballs);
					StrengthBar.instance.StopTick ();
				}
                return;
            }
				
        }
        if (game_state == GAME_STATE.MINI_GAME_2)
        {
            if (button_id == 0)
            {
                SwitchStates(GAME_STATE.PLAY);
                return;
            }
            if (button_id == 1)
            {
				if (last_pressed == 2 || last_pressed == 0) {
					last_pressed = 1; 
					StartMovement (1);
				} else if (last_pressed == 3) {
					last_pressed = 2;
					StartMovement (2); 
				}
				return; 
            }

            if (button_id == 2)
            {
				if (last_pressed == 1 || last_pressed == 0) {
					last_pressed = 2;
					StartMovement (2);
				} //else if (last_pressed == 0) {
					//last_pressed = 2;
					//StartMovement (2);
				//}
				else {
					last_pressed = 3;
					StartMovement (3); 
				}
                return;
            }

            if (button_id == 3)
            {
                StartFireball_d(button_id);
                return;
            }
        }
        if (game_state == GAME_STATE.MINI_GAME_3)
        {
			//ss-TODO:
            if (button_id == 0)
            {
                //first button
                //exiting the mini game early
                SwitchStates(GAME_STATE.PLAY);
                StrengthMiniGame2.instance.EndGameEarly();
                return;
            }

            if (button_id == 1)
            {
                //second button
				StrengthMiniGame2.instance.PressLeftButton();
            }

            if (button_id == 2)
            {
                //third button
				StrengthMiniGame2.instance.PressMiddleButton();
                return;
            }

            if (button_id == 3)
            {
                //fourth button
				StrengthMiniGame2.instance.PressRightButton();
                return;
            }
        }
        if (game_state == GAME_STATE.MINI_GAME_4)
        {
			// catcher-TODO:
            if (button_id == 0)
            {
                //first button
                //exiting the mini game early
                SwitchStates(GAME_STATE.PLAY);
                DefenseMiniGame2.instance.EndGameEarly();
                return;
            }
            if (button_id == 1)
            {
                //second button
                DefenseMiniGame2.instance.MoveLeft();
                return;
            }

            if (button_id == 2)
            {
                //third button
                DefenseMiniGame2.instance.MoveRight();
                return;
            }

            if (button_id == 3)
            {
                //fourth button
                return;
            }
        }
        if (game_state == GAME_STATE.PAUSE) {
			if (button_id == 0) {
				SwitchStates (GAME_STATE.PLAY);
				return;
			}
            if (button_id == 1)
            {
                SwitchStates(GAME_STATE.PERSONAL_STATS);
                return;
            }
        }
        if (game_state == GAME_STATE.PERSONAL_STATS)
        {
            if(button_id == 0)
            {
                SwitchStates(GAME_STATE.PAUSE);
                return;
            }
        }

	}
	public void SwitchStates(GAME_STATE new_state){
        CancelInvoke("EnterPlayInterface");
        CancelInvoke("EnterPlayInterface2");
        CancelInvoke("Enter_ExitInterface");
        CancelInvoke("EnterGeneralInterface");
        CancelInvoke("EnterGeneralInterface2");
        CancelInvoke("EnterCharacter");

		if (game_state == GAME_STATE.TITLE) {
			if (new_state == GAME_STATE.PLAY) {
                Invoke("EnterPlayInterface", interface_enter_delay);
                CancelInvoke("EnterPlayInterface2");
                Invoke("EnterPlayInterface2", interface_enter_delay);
                Invoke("Enter_ExitInterface", interface_enter_delay);
                Invoke("EnterGeneralInterface", interface_enter_delay);
                Invoke("EnterGeneralInterface2", interface_enter_delay);
                Invoke("EnterCharacter", character_enter_delay);
			}
		}

        if (game_state == GAME_STATE.STRENGTH_GAMES)
        {
            ExitStrengthGamesInterface();
            ExitStrengthGamesInterface2();
            if (new_state == GAME_STATE.PLAY)
            {
                EnterPlayInterface();
                CancelInvoke("EnterPlayInterface2");
                Invoke("EnterPlayInterface2", energy_enter_delay);
                EnterCharacter();
                EnterGeneralInterface2();
            }
        }
        if (game_state == GAME_STATE.DEFENSE_GAMES)
        {
            ExitDefenseGamesInterface();
            ExitDefenseGamesInterface2();
            if (new_state == GAME_STATE.PLAY)
            {
                EnterPlayInterface();
                CancelInvoke("EnterPlayInterface2");
                Invoke("EnterPlayInterface2", energy_enter_delay);
                EnterCharacter();
                EnterGeneralInterface2();
            }
        }
        if (game_state == GAME_STATE.MULTIPLAYER_MENU)
        {
            ExitMultiplayerGamesInterface();
            ExitMultiplayerGamesInterface2();
            if (new_state == GAME_STATE.PLAY)
            {
                EnterGeneralInterface();
                EnterGeneralInterface2();
                EnterPlayInterface();
                CancelInvoke("EnterPlayInterface2");
                Invoke("EnterPlayInterface2", energy_enter_delay);
                EnterCharacter();
                EnterGeneralInterface2();
                ExitMultiplayerGamesInterface3();
            }
        }
        if (game_state == GAME_STATE.PLAY)
        {
            if (new_state == GAME_STATE.PAUSE)
            {
                ExitPlayInterface();
                ExitPlayInterface2();
                ExitGeneralInterface();
                ExitGeneralInterface2();
                EnterPauseInterface3();
            }
        }
        if(game_state == GAME_STATE.OUT_TO_BATTLE) {
            ExitOutToBattleInterface();
            ExitOutToBattleInterface2();
        }
        if(game_state == GAME_STATE.OFFLINE_BATTLE) {
            ExitOfflineBattleInterface();
            ExitOfflineBattleInterface2();
            if (new_state == GAME_STATE.PLAY) {
				EnterPlayInterface ();
				EnterPlayInterface2 ();
				EnterCharacter ();
                ExitMultiplayerGamesInterface3();
			}
        }
        if ((game_state == GAME_STATE.MINI_GAME_1 || game_state == GAME_STATE.MINI_GAME_2 || game_state == GAME_STATE.MINI_GAME_3 || game_state == GAME_STATE.MINI_GAME_4)) {
            if (game_state == GAME_STATE.MINI_GAME_1) {
				ExitSWave ();
				StrengthMiniGame1.instance.EndGame ();
			}
            if (game_state == GAME_STATE.MINI_GAME_2)
            {
                ExitDWave();
                ExitMissileDefenseInterface1();
                DefenseMiniGame1.instance.StopSpawning();
                DefenseMiniGame1.instance.EndGame();
            }
            if (game_state == GAME_STATE.MINI_GAME_3)
            {
                ExitMMMiniGameInterface();
                // ss-TODO:
                // add functionality for quitting the mini game here

            }
            if (game_state == GAME_STATE.MINI_GAME_4)
            {
                ExitCatcherMiniGameInterface();
                // catch-TODO:
                // add functionality for quitting the mini game here

            }

            if (new_state == GAME_STATE.PLAY) {
				EnterPlayInterface ();
				EnterPlayInterface2 ();
				EnterCharacter ();
			}
			if (new_state == GAME_STATE.PAUSE) {
			}
		}
		if (game_state == GAME_STATE.PAUSE) {
			ExitPauseInterface ();
			ExitPauseInterface2 ();

			if (new_state == GAME_STATE.PLAY) {
				EnterPlayInterface ();
				EnterPlayInterface2 ();
				EnterCharacter ();
                EnterGeneralInterface();
                EnterGeneralInterface2();
                ExitPauseInterface3();
            }
            if (new_state == GAME_STATE.PERSONAL_STATS)
            {
                ExitCharacter();
                ExitGeneralInterface();
                EnterPersonalStatsInterface();
                UpdatePersonalStatsInterface();
            }
        }
        if (game_state == GAME_STATE.PERSONAL_STATS)
        {
            if (new_state == GAME_STATE.PAUSE)
            {
                EnterPauseInterface();
                EnterCharacter();
                ExitPersonalStatsInterface();
            }
        }
		if (new_state == GAME_STATE.PAUSE) {
			EnterPauseInterface ();
			EnterPauseInterface2 ();
			paused_state = game_state;
			PauseCharacter ();
		}
        if (new_state == GAME_STATE.STRENGTH_GAMES)
        {
            ExitPlayInterface();
            ExitPlayInterface2();
            ExitGeneralInterface2();
            PauseCharacter2();
            EnterStrengthGamesInterface();
            EnterStrengthGamesInterface2();
        }
        if (new_state == GAME_STATE.DEFENSE_GAMES)
        {
            ExitPlayInterface();
            ExitPlayInterface2();
            ExitGeneralInterface2();
            PauseCharacter2();
            EnterDefenseGamesInterface();
            EnterDefenseGamesInterface2();
        }
        if (new_state == GAME_STATE.MULTIPLAYER_MENU)
        {
            ExitPlayInterface();
            ExitPlayInterface2();
            ExitGeneralInterface2();
            PauseCharacter2();
            ExitGeneralInterface();
            EnterMultiplayerGamesInterface2();
            EnterMultiplayerGamesInterface3();
            if(!GameData.instance.played[4] || GameData.instance.tutorial_mode) {
                tutorial_prompt_active = true;
                tutorial_prompt_num = 4;
                EnterTutorialPromptInterface(4);
                //togogogo
            } else {
                EnterMultiplayerGamesInterface();
            }
        }
        if(new_state == GAME_STATE.OUT_TO_BATTLE) {
            ExitGeneralInterface();
            ExitGeneralInterface2();
            ExitPlayInterface();
            ExitPlayInterface2();
            if(GameData.instance.away) {
                ExitCharacter();
                RefreshBattleInterface();
            }
            else EnterCharacter();
            EnterOutToBattleInterface();
            EnterOutToBattleInterface2();
            if(!GameData.instance.played[5] || GameData.instance.tutorial_mode) {
                tutorial_prompt_active = true;
                tutorial_prompt_num = 5;
                EnterTutorialPromptInterface(5);
            } 
        }
        if(new_state == GAME_STATE.OFFLINE_BATTLE) {
            ExitGeneralInterface();
            ExitGeneralInterface2();
            ExitPlayInterface();
            ExitPlayInterface2();
            MiniGameCharacter();
            EnterOfflineBattleInterface();
            EnterOfflineBattleInterface2();
            if(!GameData.instance.played[6] || GameData.instance.tutorial_mode) {
                tutorial_prompt_active = true;
                tutorial_prompt_num = 6;
                EnterTutorialPromptInterface(6);
                OfflineBattle.instance.instruction_text.text = "";
		        OfflineBattle.instance.result_text.text = "";
            }
            else {
                OfflineBattle.instance.StartGame();
            }
        }
        if (new_state == GAME_STATE.MINI_GAME_1 || new_state == GAME_STATE.MINI_GAME_2 || new_state == GAME_STATE.MINI_GAME_3 || new_state == GAME_STATE.MINI_GAME_4) {
            ExitGeneralInterface();
            ExitGeneralInterface2();
            ExitPlayInterface();
            ExitPlayInterface2();
            MiniGameCharacter();

			if (new_state == GAME_STATE.MINI_GAME_1) {
				EnterSWave ();
				MiniGameCharacter ();
                if(!GameData.instance.played[0] || GameData.instance.tutorial_mode) {
                    tutorial_prompt_active = true;
                    tutorial_prompt_num = 0;
                    EnterTutorialPromptInterface(0);
                } else {
				    StrengthMiniGame1.instance.StartGame ();
                }
			}
            if (new_state == GAME_STATE.MINI_GAME_2)
            {
                EnterDWave();
                EnterMissileDefenseInterface1();
                if(!GameData.instance.played[1] || GameData.instance.tutorial_mode) {
                    tutorial_prompt_active = true;
                    tutorial_prompt_num = 1;
                    EnterTutorialPromptInterface(1);
                } else {
                    DefenseMiniGame1.instance.StartSpawning();
                }
            }
            if (new_state == GAME_STATE.MINI_GAME_3)
            {
                EnterMMMiniGameInterface();
                if(!GameData.instance.played[2] || GameData.instance.tutorial_mode) {
                    tutorial_prompt_active = true;
                    tutorial_prompt_num = 2;
                    EnterTutorialPromptInterface(2);
                } else {
                    StrengthMiniGame2.instance.StartGame();
                }
            }
            if (new_state == GAME_STATE.MINI_GAME_4)
            {
                EnterCatcherMiniGameInterface();
                if(!GameData.instance.played[3] || GameData.instance.tutorial_mode) {
                    tutorial_prompt_active = true;
                    tutorial_prompt_num = 3;
                    EnterTutorialPromptInterface(3);
                } else {
                    DefenseMiniGame2.instance.StartGame();
                }
            }
        }
        if (new_state == GAME_STATE.PLAY)
        {
            character_sprite_window_obj.GetComponentInChildren<IdleAnimation>().StartAnimation();
            EnterGeneralInterface();
            EnterGeneralInterface2();
        }
        game_state = new_state;
		if (game_state == GAME_STATE.PLAY || game_state == GAME_STATE.PAUSE) {
			Invoke ("RandomRoar", random_roar_delay);
		}
	}
	void EnterSWave(){
		s_wave.GetComponent<FadeIn> ().StartEntrance ();
	}
	void EnterDWave(){
		d_wave.GetComponent<FadeIn> ().StartEntrance ();
	}
	void ExitSWave(){
		s_wave.GetComponent<FadeIn> ().StartExit ();
	}
	void ExitDWave(){
		d_wave.GetComponent<FadeIn> ().StartExit ();
	}
    void EnterMissileDefenseInterface1()
    {
        missile_defense_interface1.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitMissileDefenseInterface1()
    {
        missile_defense_interface1.GetComponent<FadeIn>().StartExit();
    }
    void EnterPersonalStatsInterface()
    {
        UpdatePersonalStatsInterface();
        personal_stats_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitPersonalStatsInterface()
    {
        personal_stats_interface.GetComponent<FadeIn>().StartExit();
    }
    void UpdatePersonalStatsInterface() {
        for(int i = 0; i < stats.Length; i++) {
            stats[i].UpdateStat();
        }
    }
    void EnterTutorialPromptInterface(int tutorial_num)
    {
        tutorial_text.text = "";
        if(tutorial_num == 0) {
            tutorial_header.text = "MOUNTAIN SMASH";
            tutorial_text.text = "BUILD YOUR STRENGTH BY SMASHING\nSOME MOUNTAINS !\n\nTIME YOUR ATTACKS USING THE METER\nABOVE YOUR KAIJU TO DETERMINE \nTHE POWER OF YOUR ATTACK\n\nAS THE MOUNTAINS GET STRONGER YOU\nGAIN MORE ATTEMPTS TO SMASH !";
        }
        if(tutorial_num == 1) {
            tutorial_header.text = "MISSILE DEFENSE";
            tutorial_text.text = "DEFEND YOUR TERRITORY AS MISSILES\nCOME FLYING IN FROM THE RIGHT!\n\nEACH BUTTON CORRESPONDS TO A\nROW THAT YOU MUST DEFEND\n\nDESTROY ENOUGH MISSILES TO MOVE\nON TO THE NEXT ROUND !";
        }
        if(tutorial_num == 2) {
            tutorial_header.text = "MONSTER MIMIC";
            tutorial_text.text = "BEING CLEVER IS HALF THE BATTLE !\n\nREPEAT THE PATTERNS TO \nMOVE ON TO THE NEXT ROUND !\n\nFOR EACH MISTAKE YOU GET AN X\nTHREE X'S AND YOU'RE OUT !";
        }
        if(tutorial_num == 3) {
            tutorial_header.text = "STAR CATCHER";
            tutorial_text.text = "TONE YOUR REFLEXES \nBY CATCHING FALLING STARS !\n\nTHE STARS ARE GOOD, BUT WATCH OUT\nFOR THE DARK ENERGY !\n\nTRY TO HOLD AS MANY STARS AS POSSIBLE\nTHE MORE YOU CATCH THE HARDER TO DODGE !";
        }
        if(tutorial_num == 4) {
            tutorial_header.text = "VS MODE";
            tutorial_text.text = "WELCOME TO KAIJU VS MODE !\n\nLET YOUR KAIJU SEEK OUT OTHER  MONSTERS \nIN REAL TIME WITH ONLINE BATTLE !\n\nVS FIERCE ENEMIES IN A HEAD TO HEAD\nROCK PAPER SCISSORS COMBAT\nWITH OFFLINE BATTLE";
        }
        if(tutorial_num == 5) {
            tutorial_header.text = "ONLINE BATTLE";
            tutorial_text.text = "CHALLENGE FOES ON THE BATTLFIELD !\n\nSEND YOUR KAIJU TO THE BATTLEFIELD !\nTHEY WILL USE ENERGY TO BATTLE OTHERS\nIN REAL-TIME MATCHMAKING !\nUSE THE RIGHTMOST BUTTON TO REFRESH\nTHEIR STATUS AND SEE HOW THEY ARE DOING\n\nFETCH THEM TO SEE THE RICHES THEY FOUND !";
        }
        if(tutorial_num == 6) {
            tutorial_header.text = "OFFLINE BATTLE";
			tutorial_text.text = "DEFEAT YOUR OPPONENT !\n\nROCK BEATS SCISSORS, PAPER BEATS ROCK,\nSCISSORS BEATS PAPER.\n\nTRAIN YOUR KAIJU TO DO MORE DAMAGE !";
        } 
        tutorial_prompt_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitTutorialPromptInterface()
    {
        tutorial_prompt_interface.GetComponent<FadeIn>().StartExit();
    }
    void EnterOfflineBattleInterface()
    {
        offline_battle_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitOfflineBattleInterface()
    {
        offline_battle_interface.GetComponent<FadeIn>().StartExit();
    }
    void EnterOfflineBattleInterface2()
    {
        offline_battle_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitOfflineBattleInterface2()
    {
        offline_battle_interface2.GetComponent<FadeIn>().StartExit();
    }
    void EnterOutToBattleInterface()
    {
        if (GameData.instance.away)
        {
            GameObject.FindGameObjectWithTag("out action").GetComponent<TextMesh>().text = "FETCH FROM BATTLE";
            OutToBattle.instance.UpdateAwayStatus("STATUS: AWAY");
        }
        else
        {
            GameObject.FindGameObjectWithTag("out action").GetComponent<TextMesh>().text = "SEND TO BATTLE";
            OutToBattle.instance.UpdateAwayStatus("STATUS: HOME");
        }
        out_to_battle_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitOutToBattleInterface()
    {
        out_to_battle_interface.GetComponent<FadeIn>().StartExit();
    }
    void EnterOutToBattleInterface2()
    {
        out_to_battle_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitOutToBattleInterface2()
    {
        out_to_battle_interface2.GetComponent<FadeIn>().StartExit();
    }
    void EnterOutToBattleInterface3()
    {
        out_to_battle_interface3.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitOutToBattleInterface3()
    {
        out_to_battle_interface3.GetComponent<FadeIn>().StartExit();
    }
    void EnterMMMiniGameInterface()
    {
        mm_minigame_interface.GetComponent<FadeIn>().StartEntrance();
        EnterMMMiniGameInterface2();
    }
    void ExitMMMiniGameInterface()
    {
        mm_minigame_interface.GetComponent<FadeIn>().StartExit();
        ExitMMMiniGameInterface2();
    }
    void EnterMMMiniGameInterface2()
    {
        mm_minigame_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitMMMiniGameInterface2()
    {
        mm_minigame_interface2.GetComponent<FadeIn>().StartExit();
    }
    void EnterCatcherMiniGameInterface()
    {
        DefenseMiniGame2.instance.character_sprite.GetComponentInChildren<SpriteRenderer>().sprite = GameController.instance.monsters[GameData.instance.monster_num].mini_game_sprites[0];
        GameController.instance.character.GetComponent<FadeIn>().StartExit();
        catcher_minigame_interface.GetComponent<FadeIn>().StartEntrance();
        EnterCatcherMiniGameInterface2();
        EnterCatcherMiniGameInterface3();
    }
    void ExitCatcherMiniGameInterface()
    {
        catcher_minigame_interface.GetComponent<FadeIn>().StartExit();
        ExitCatcherMiniGameInterface2();
        ExitCatcherMiniGameInterface3();
    }
    void EnterCatcherMiniGameInterface2()
    {
        catcher_minigame_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitCatcherMiniGameInterface2()
    {
        catcher_minigame_interface2.GetComponent<FadeIn>().StartExit();
    }
    void EnterCatcherMiniGameInterface3()
    {
        catcher_minigame_interface3.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitCatcherMiniGameInterface3()
    {
        catcher_minigame_interface3.GetComponent<FadeIn>().StartExit();
    }
    void SpawnObject(GameObject spawn, GameObject spawn_obj) {
        
    }
    void EnterPauseInterface()
    {
        if (!pause_interface) {
            pause_interface = Instantiate(pause_interface_obj);
            pause_coins = pause_interface.GetComponentInChildren<CoinsText>();
            pause_coins.UpdateCoinsText();
            pause_interface.transform.parent = interface_obj.transform;
            pause_interface.transform.localPosition = pause_interface_obj.transform.localPosition;
        }
        //CoinsText.instance.UpdateCoinsText();
        pause_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitPauseInterface()
    {
        pause_interface.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterPauseInterface2()
    {
        if (!pause_interface2) {
            pause_interface2 = Instantiate(pause_interface_obj2);
            pause_interface2.transform.parent = interface_obj.transform;
            pause_interface2.transform.localPosition = pause_interface_obj2.transform.localPosition;
        }
        pause_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitPauseInterface2()
    {
        pause_interface2.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterPauseInterface3()
    {
        if (!pause_interface3) {
            pause_interface3 = Instantiate(pause_interface_obj3);
            pause_interface3.transform.parent = interface_obj.transform;
            pause_interface3.transform.localPosition = pause_interface_obj3.transform.localPosition;
        }
        pause_interface3.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitPauseInterface3()
    {
        pause_interface3.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterPlayInterface()
    {
        play_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitPlayInterface()
    {
        play_interface.GetComponent<FadeIn>().StartExit();
    }
    void EnterPlayInterface2()
    {
        play_interface_2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitPlayInterface2()
    {
        CancelInvoke("EnterPlayInterface2");
        play_interface_2.GetComponent<FadeIn>().StartExit();
    }
    
    void EnterStrengthGamesInterface()
    {
        if (!strength_interface) {
            strength_interface = Instantiate(strength_interface_obj);
            strength_interface.transform.parent = interface_obj.transform;
            strength_interface.transform.localPosition = strength_interface_obj.transform.localPosition;
        }
        StrengthChoices.instance.UpdateChoice2();
        strength_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitStrengthGamesInterface()
    {
        strength_interface.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterStrengthGamesInterface2()
    {
        if (!strength_interface2) {
            strength_interface2 = Instantiate(strength_interface_obj2);
            strength_interface2.transform.parent = interface_obj.transform;
            strength_interface2.transform.localPosition = strength_interface_obj2.transform.localPosition;
        }
        strength_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitStrengthGamesInterface2()
    {
        strength_interface2.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterDefenseGamesInterface()
    {
        if (!defense_interface) {
            defense_interface = Instantiate(defense_interface_obj);
            defense_interface.transform.parent = interface_obj.transform;
            defense_interface.transform.localPosition = defense_interface_obj.transform.localPosition;
        }
        DefenseChoices.instance.UpdateChoice2();
        defense_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitDefenseGamesInterface()
    {
        defense_interface.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterDefenseGamesInterface2()
    {
        if (!defense_interface2) {
            defense_interface2 = Instantiate(defense_interface_obj2);
            defense_interface2.transform.parent = interface_obj.transform;
            defense_interface2.transform.localPosition = defense_interface_obj2.transform.localPosition;
        }
        defense_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitDefenseGamesInterface2()
    {
        defense_interface2.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterMultiplayerGamesInterface()
    {
        if (!multiplayer_interface) {
            multiplayer_interface = Instantiate(multiplayer_interface_obj);
            multiplayer_interface.transform.parent = interface_obj.transform;
            multiplayer_interface.transform.localPosition = multiplayer_interface_obj.transform.localPosition;
            
        }
        multiplayer_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitMultiplayerGamesInterface()
    {
        multiplayer_interface.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterMultiplayerGamesInterface2()
    {
        if (!multiplayer_interface2) {
            multiplayer_interface2 = Instantiate(multiplayer_interface_obj2);
            multiplayer_interface2.transform.parent = interface_obj.transform;
            multiplayer_interface2.transform.localPosition = multiplayer_interface_obj2.transform.localPosition;
        }
        multiplayer_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitMultiplayerGamesInterface2()
    {
        multiplayer_interface2.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterMultiplayerGamesInterface3()
    {
        if (!multiplayer_interface3) {
            multiplayer_interface3 = Instantiate(multiplayer_interface_obj3);
            multiplayer_interface3.transform.parent = interface_obj.transform;
            multiplayer_interface3.transform.localPosition = multiplayer_interface_obj3.transform.localPosition;
        }
        multiplayer_interface3.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitMultiplayerGamesInterface3()
    {
        multiplayer_interface3.GetComponent<FadeIn>().StartExitInterface();
    }
    void EnterGeneralInterface()
    {
        general_interface.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitGeneralInterface()
    {
        general_interface.GetComponent<FadeIn>().StartExit();
    }
    void EnterGeneralInterface2()
    {
        general_interface2.GetComponent<FadeIn>().StartEntrance();
    }
    void ExitGeneralInterface2()
    {
        general_interface2.GetComponent<FadeIn>().StartExit();
    }
    void Enter_ExitInterface(){
		exit_interface.GetComponent<FadeIn> ().StartEntrance ();
	}
	void Exit_ExitInterface(){
		exit_interface.GetComponent<FadeIn> ().StartExit ();
	}
	void EnterCharacter(){
		character.GetComponent<FadeIn> ().StartEntrance ();
	}
	void ExitCharacter(){
		character.GetComponent<FadeIn> ().StartExit ();
	}
    void PauseCharacter()
    {
        character.GetComponent<FadeIn>().StartMove(0);
    }
    void PauseCharacter2()
    {
        character.GetComponent<FadeIn>().StartMove(4);
    }
    void MiniGameCharacter(){
		character.GetComponent<FadeIn> ().StartMove (2);
		character_sprite_window_obj.GetComponentInChildren<IdleAnimation>().StopAnimation();
		creature_sprend.sprite = monsters[GameData.instance.monster_num].mini_game_sprites[0];

	}
	void ControlScreen(){
        if (playing_intro) ShrinkTitle();
		else if (game_state == GAME_STATE.TITLE) GrowTitle ();
        else ShrinkTitle ();

		if (game_state == GAME_STATE.START_MENU) GrowStartMenu ();
		else ShrinkStartMenu ();

        if (playing_intro) intro.SetActive(true);
        else intro.SetActive(false);

		//if (game_state == GAME_STATE.MINI_GAME_1) 
	}
	void GrowTitle(){
        title.SetActive(true);
		if (title.transform.localScale.x > title_scale.x - 0.05f && title.transform.localScale.y > title_scale.y - 0.05f) {
			title.transform.localScale = new Vector3(title_scale.x, title_scale.y, 1f);
			return;
		}
		title.transform.localScale = new Vector3 (title.transform.localScale.x + Time.deltaTime * grow_speed,title.transform.localScale.y + Time.deltaTime * grow_speed, 1f);
	}
	void ShrinkTitle(){
		if (title.transform.localScale.x < 0.01f && title.transform.localScale.y < 0.01f) {
			title.transform.localScale = Vector3.zero;
            title.SetActive(false);
			return;
		}
		title.transform.localScale = new Vector3 (title.transform.localScale.x - Time.deltaTime * shrink_speed,title.transform.localScale.y - Time.deltaTime * shrink_speed, title.transform.localScale.z);
	}
	void GrowStartMenu(){
		if (start_menu.transform.localScale.x > start_menu_scale.x - 0.05f && start_menu.transform.localScale.y > start_menu_scale.y - 0.05f) {
			start_menu.transform.localScale = new Vector3(start_menu_scale.x, start_menu_scale.y, 1f);
			return;
		}
		start_menu.transform.localScale = new Vector3 (start_menu.transform.localScale.x + Time.deltaTime * grow_speed,start_menu.transform.localScale.y + Time.deltaTime * grow_speed, 1f);
	}
	void ShrinkStartMenu(){
		if (start_menu.transform.localScale.x < 0.01f && start_menu.transform.localScale.y < 0.01f) {
			start_menu.transform.localScale = Vector3.zero;
			return;
		}
		start_menu.transform.localScale = new Vector3 (start_menu.transform.localScale.x - Time.deltaTime * shrink_speed,start_menu.transform.localScale.y - Time.deltaTime * shrink_speed, start_menu.transform.localScale.z);
	}
	void FlashLowEnergy()
	{
		energy_warning.GetComponent<WarningFlash> ().StartFlash (true);
	}
    void FlashOutToBattleLowEnergy()
	{
		out_to_battle_energy_warning.GetComponent<WarningFlash> ().StartFlash (true);
	}
    public void StartFlashFetchLowEnergy()
	{
		out_to_battle_energy_warning.GetComponent<WarningFlash> ().StartFlash (false);
	}
    public void StopFlashFetchLowEnergy()
	{
		out_to_battle_energy_warning.GetComponent<WarningFlash> ().EndFlash ();
	}
	void RandomRoar(){
		int rand = UnityEngine.Random.Range (0, roarFactor);
		if (game_state == GAME_STATE.PLAY || game_state == GAME_STATE.PAUSE)Invoke ("RandomRoar", roarTimer);
		if (rand == 0) {
			if(!GameData.instance.away) SoundsController.instance.PlayRoar ();
			print ("RANDOM ROAR!");
		}
	}
	public void EndStarCatcher()
	{
		SwitchStates(GAME_STATE.PLAY);
	}
}
