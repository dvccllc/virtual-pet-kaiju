using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SimpleJSON;
public class Leaderboards : MonoBehaviour {
	public static Leaderboards instance;
    public GameObject top10;
    public GameObject loading_icon;
    public bool loading = false;
    public int category = 0;
    public string category_label;
    public TextMesh label;
    public string leaderboard_tag;
    public string[] categories;
    public string[] categories_labels;
	public TextMesh[] lb_ranks;
	public TextMesh[] lb_initials;
	public TextMesh[] lb_scores;
	public Leaderboard[] lb_entries;
    public Leaderboard personal_lb;
    public TextMesh personal_rank, personal_initials, personal_score;
	public InputField initials_text;
	public float initials_delay;
    public string url = "http://vpkapi.dcj8tjfhw6.us-east-2.elasticbeanstalk.com/api/v1/";

    WWW www;
	void Start () {
		instance = this;
		initials_text.text = "";
	}
    public void CycleCategoryRight() {
        if(loading) return;
        category++;
        if(category == categories.Length) category = 0;
        leaderboard_tag = categories[category];
        category_label = categories_labels[category];
        label.text = category_label;
        RefreshLeaderboards();
    }
	public void SubmitInitials() {
		if (initials_text.text == "")
			return;
		GameData.instance.initials = initials_text.text;
		PostLeaderboardsAPICall ();
		InitialsPrompt.instance.Okay ();
	}
	public void CapitalizeInitials() {
		initials_text.text = initials_text.text.ToUpper ();
	}
    public void CycleCategoryLeft() {
        if(loading) return;
        category--;
        if(category < 0) category = categories.Length - 1;
        leaderboard_tag = categories[category];
        category_label = categories_labels[category];
        label.text = category_label;
        RefreshLeaderboards();
    }
    public void PostLeaderboard() {
		if (!GameData.instance.in_leaderboards) { 
			AskForInitials ();
		}
        else UpdateLeaderboardsAPICall();

    }
	public void ActivateInitialsPrompt() {
		InitialsPrompt.instance.activated = true;
	}
	public void AskForInitials() {
		CancelInvoke ("ActivateInitialsPrompt");
		initials_text.text = "";
		Invoke ("ActivateInitialsPrompt", initials_delay);
	}
	public void RefreshLeaderboards(){
        loading = true;
        top10.SetActive(false);
        loading_icon.SetActive(true);
        PostLeaderboard();
	}
	public void PostLeaderboardsAPICall() {
		string send_url = url + "leaderboards/";
        WWWForm form = new WWWForm();
        form.AddField( "method", "add");
        form.AddField( "id", GameData.instance.multiplayer_id);
		form.AddField( "strength1_score", GameData.instance.score_strength_1);
		form.AddField( "strength2_score", GameData.instance.score_strength_2);
		form.AddField( "defense1_score", GameData.instance.score_defense_1);
		form.AddField( "defense2_score", GameData.instance.score_defense_2);
        int sum = 0;
		for(int i = 0; i < GameData.instance.battles.Count; i++) {
			if(GameData.instance.battles[i].result == BATTLE_RESULT.WIN) sum++;
		}
        form.AddField( "battles_won", sum);
        form.AddField( "coins", GameData.instance.num_coins);
		form.AddField( "initials", GameData.instance.initials);
        www = new WWW(send_url, form);
        StartCoroutine(WaitForPostLeaderboards());
    }
    public IEnumerator WaitForPostLeaderboards() {
        yield return www; 

        if((!string.IsNullOrEmpty(www.error))) {
            print( "Error getting leaderboards: " + www.error );
        }
        else {
            GameData.instance.in_leaderboards = true;
            LeaderboardsAPICall ();
        }
    }
	public void UpdateLeaderboardsAPICall() {
		string send_url = url + "leaderboards/";
        WWWForm form = new WWWForm();
        form.AddField( "method", "update");
        form.AddField( "id", GameData.instance.multiplayer_id);
		form.AddField( "strength1_score", GameData.instance.score_strength_1);
		form.AddField( "strength2_score", GameData.instance.score_strength_2);
		form.AddField( "defense1_score", GameData.instance.score_defense_1);
		form.AddField( "defense2_score", GameData.instance.score_defense_2);
        int sum = 0;
		for(int i = 0; i < GameData.instance.battles.Count; i++) {
			if(GameData.instance.battles[i].result == BATTLE_RESULT.WIN) sum++;
		}
        form.AddField( "battles_won", sum);
        form.AddField( "coins", GameData.instance.num_coins);
		form.AddField( "initials", GameData.instance.initials);
        www = new WWW(send_url, form);
        StartCoroutine(WaitForUpdateLeaderboards());
    }
    public IEnumerator WaitForUpdateLeaderboards() {
        yield return www; 

        if((!string.IsNullOrEmpty(www.error))) {
            print( "Error updating leaderboards: " + www.error );
        }
        else {
            GameData.instance.in_leaderboards = true;
            LeaderboardsAPICall ();
        }
    }
	public void LeaderboardsAPICall() {
		string send_url = url + "leaderboards/" + GameData.instance.multiplayer_id + "/" + leaderboard_tag + "/";   
        www = new WWW(send_url);
        StartCoroutine(WaitForLeaderboards(leaderboard_tag));
    }
    public IEnumerator WaitForLeaderboards(string lb_tag) {
        yield return www; 

        if((!string.IsNullOrEmpty(www.error))) {
            print( "Error getting leaderboards: " + www.error );
        }
        else {
            JSONNode responseJSON = JSONNode.Parse (www.text);
            var kaiju = responseJSON["kaiju"];
            Leaderboard p_lb = new Leaderboard();
            p_lb.rank = kaiju["rank"];
            p_lb.score = kaiju["user_score"];
            p_lb.initials = "-ME-";
            personal_lb = p_lb;
            var leaderboards = responseJSON["leaderboards"];
            lb_entries = new Leaderboard[10];
            for(int i = 0; i < 10; i++) {
				Leaderboard lb = new Leaderboard();
                lb.rank = i + 1;
				lb.score = leaderboards[i][lb_tag];
				lb.id = leaderboards[i]["id"];
				lb.initials = leaderboards[i]["initials"];
                lb_entries[i] = lb;
            }
            UpdateLeaderboardsTexts();
        }
    }
    void UpdateLeaderboardsTexts() {
        ResetInitials();
        UpdateInitials();
        ResetScores();
        UpdateScores();
        ResetRanks();
        UpdateRanks();
        top10.SetActive(true);
        loading_icon.SetActive(false);
        loading = false;
    }
    void ResetInitials(){
        personal_initials.text = "";
        for(int i = 0; i < lb_initials.Length; i++) {
            lb_initials[i].text = "";
        }
    }
    void UpdateInitials() {
        personal_initials.text = personal_lb.initials;
        for(int i = 0; i < lb_entries.Length; i++) {
            lb_initials[i].text = lb_entries[i].initials;
        }
    }
    void ResetScores(){
        personal_score.text = "";
        for(int i = 0; i < lb_scores.Length; i++) {
            lb_scores[i].text = "";
        }
    }
    void UpdateScores() {
        personal_score.text = "" + personal_lb.score;
        for(int i = 0; i < lb_entries.Length; i++) {
            lb_scores[i].text = "" + lb_entries[i].score;
        }
    }
    void ResetRanks(){
        personal_rank.text = "";
        for(int i = 0; i < lb_ranks.Length; i++) {
            lb_ranks[i].text = "";
        }
    }
    void UpdateRanks() {
        personal_rank.text = "" + personal_lb.rank;
        for(int i = 0; i < lb_entries.Length; i++) {
            lb_ranks[i].text = "" + lb_entries[i].rank;
        }
    }
}
public class Leaderboard {
	public int score;
	public string id;
	public string initials;
    public int rank;
}
