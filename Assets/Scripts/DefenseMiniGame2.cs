using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseMiniGame2 : MonoBehaviour
{
    public static DefenseMiniGame2 instance;
    public bool active = false, screen_in_position;
    public int current_wave = 0;
    public int win_wave = 0;
    float movement = 0;
    static float movement_speed = 1.5f;
    float pile_left = 0;
    float pile_right = 0;
    int total_score = 0;
    int spawn_num = 0;
    float defense_reward = 0.5f;
    float coin_multiplier = 5f;
    public GameObject minigame_character;
    public GameObject star_spawner;
    public GameObject character_sprite;
    public GameObject star;
    public GameObject bomb;
    public GameObject max_height;
    public int starspawndir = 1;
    public float screen_edge_distance = 2;
    public TextMesh score_text;
    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            HandlePlayerMovement();
            StarSpawnerMove();
        }
    }
    public void StartGame()
    {
        score_text.text = "0";
        total_score = 0;
        movement = 0;
		pile_left = 0;
		pile_right = 0;
        CancelInvoke("Activate");
        Invoke("Activate", 2);
        Vector3 start_position = minigame_character.transform.position;
        start_position.x = gameObject.transform.position.x;
        //minigame_character.GetComponent<Transform>().SetPositionAndRotation(start_position, minigame_character.transform.rotation);
        minigame_character.GetComponent<Transform>().position = start_position;
        minigame_character.GetComponent<Transform>().rotation = minigame_character.transform.rotation;
    }
    void Activate()
    {
        active = true;
        CancelInvoke("SpawnStar");
        Invoke("SpawnStar", 2);
    }
    void NextWave()
    {

    }
    void HandlePlayerMovement()
    {
        Vector3 next_position = minigame_character.transform.position;
        next_position.x = next_position.x + movement * Time.deltaTime;
        if (next_position.x + pile_right > screen_edge_distance)
        {
            next_position.x = screen_edge_distance - pile_right;
        }
        else if (next_position.x + pile_left < screen_edge_distance * -1)
        {
            next_position.x = screen_edge_distance * -1 - pile_left;
        }
        // minigame_character.GetComponent<Transform>().SetPositionAndRotation(next_position, minigame_character.transform.rotation);
        minigame_character.GetComponent<Transform>().position = next_position;
        minigame_character.GetComponent<Transform>().rotation = minigame_character.transform.rotation;
    }
    void StarSpawnerMove()
    {
        Vector3 starspawner_next_position = star_spawner.transform.position;
        starspawner_next_position.x = starspawner_next_position.x + UnityEngine.Random.value * 2 * Time.deltaTime * starspawndir;
        if (starspawner_next_position.x > screen_edge_distance)
        {
            starspawner_next_position.x = screen_edge_distance;
            starspawndir *= -1;
        }
        else if (starspawner_next_position.x < screen_edge_distance * -1)
        {
            starspawner_next_position.x = screen_edge_distance * -1;
            starspawndir *= -1;
        }
        // star_spawner.GetComponent<Transform>().SetPositionAndRotation(starspawner_next_position, star_spawner.transform.rotation);
        star_spawner.GetComponent<Transform>().position = starspawner_next_position;
        star_spawner.GetComponent<Transform>().rotation = star_spawner.transform.rotation;
    }
    void SpawnStar()
    {
        GameObject next_spawn = star;
        if (spawn_num % 5 == 3) next_spawn = bomb;
        spawn_num++;
        GameObject s = Instantiate(next_spawn);
        s.transform.position = star_spawner.transform.position;
        SoundsController.instance.PlaySound("falling star");
        Invoke("SpawnStar", 2);
    }
    public void NewContact(Transform pos, bool not_bomb)
    {
        if (not_bomb)
        {
            total_score++;
            score_text.text = "" + total_score;
            SoundsController.instance.PlaySound("star hit");
            if (transform.TransformPoint(max_height.transform.position).y < transform.TransformPoint(pos.position).y) EndGame();
            if (transform.TransformPoint(pos.position).x < transform.TransformPoint(minigame_character.transform.position).x + pile_left) pile_left = transform.TransformPoint(pos.position).x - transform.TransformPoint(minigame_character.transform.position).x;
            else if (transform.TransformPoint(pos.position).x > transform.TransformPoint(minigame_character.transform.position).x + pile_right) pile_right = transform.TransformPoint(pos.position).x - transform.TransformPoint(minigame_character.transform.position).x;
        }
        else
        {
            SoundsController.instance.PlaySound("bomb hit");
            EndGame();
        }
    }
    public void MoveLeft()
    {
        movement = movement_speed * -1;
    }
    public void MoveRight()
    {
        movement = movement_speed;
    }
    public void EndGame()
    {
        CancelInvoke("SpawnStar");
        CancelInvoke("Activate");
        GameObject[] stars = GameObject.FindGameObjectsWithTag("star");
        foreach (GameObject star_object in stars)
        {
            Destroy(star_object.gameObject);
        }
        win_wave = current_wave;
        active = false;
        if (total_score >= 3) Invoke("WinAnimation", 1.5f);
        else {
			SoundsController.instance.PlaySound ("bad");
		}
        //gameObject.GetComponent<FadeIn>().StartExit();
        GameController.instance.EndStarCatcher();
    }
    public void EndGameEarly()
    {
        active = false;
        EndGame();
    }
	void WinAnimation(){
        int score_value = total_score / 2;
		SoundsController.instance.PlaySound ("positive beep");
        GameController.instance.icon_reward.UpdateValue(score_value, "defense");
        GameController.instance.coin_reward.UpdateValue(score_value, "coin");
		float reward = defense_reward * score_value;
		if(reward > 2.5f) reward = 2.5f;
        GameData.instance.defense_level += reward;
		if(total_score > GameData.instance.score_defense_2) GameData.instance.score_defense_2 = total_score;
        GameData.instance.num_coins += (int)Math.Round((score_value * defense_reward) * coin_multiplier, 0);
        //CoinsText.instance.UpdateCoinsText();
    }
}
