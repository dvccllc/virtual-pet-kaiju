using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq; 
using System.Text.RegularExpressions;
using SimpleJSON;
public class MultiplayerCSh : MonoBehaviour {
    public GameObject creature;
    public GameObject away_sign;
    public static MultiplayerCSh instance;
    public string url = "http://vpkapi.dcj8tjfhw6.us-east-2.elasticbeanstalk.com/api/v1/";
    WWW www;
	// Use this for initialization
	void Start () {
        instance = this;
	}
    public void SendKaiju() {
        string send_url = url + "kaiju/";
        WWWForm form = new WWWForm();
        form.AddField( "method", "send");
        form.AddField( "id", GameData.instance.multiplayer_id);
        form.AddField( "kaiju_name", GameData.instance.info_name);
        form.AddField( "energy", GameData.instance.energy_level);
        form.AddField( "strength", "" + GameData.instance.strength_level);
        form.AddField( "defense", "" + GameData.instance.defense_level);
        www = new WWW(send_url, form);
        GameController.instance.api_loading = true;
        StartCoroutine(WaitForSendKaiju());
    }

    public IEnumerator WaitForSendKaiju() {
        yield return www; 

        if((!string.IsNullOrEmpty(www.error))) {
            print( "Error sending kaiju: " + www.error );
            GameController.instance.api_loading = false;
        }
        else {
            GameData.instance.away = true;
            GameController.instance.away_sprend.enabled = true;
            GameController.instance.api_loading = false;
            UpdateAwayStatusSprites();
            MultiplayerChoices.instance.UpdateBattleStatusText();
            GameController.instance.SendToBattle();
        }
    }
    public void FetchKaiju() {
        string send_url = url + "kaiju/";
        WWWForm form = new WWWForm();
        form.AddField( "method", "fetch");
        form.AddField( "id", GameData.instance.multiplayer_id);
        www = new WWW(send_url, form);
        GameController.instance.api_loading = true;
        StartCoroutine(WaitForFetchKaiju());
    }
    public IEnumerator WaitForFetchKaiju() {
        yield return www; 

        if((!string.IsNullOrEmpty(www.error))) {
            print( "Error fetching kaiju: " + www.error );
            GameController.instance.api_loading = false;
            if(GameController.instance.reset_fetch) {
                GameController.instance.reset_fetch = false;
                GameController.instance.creature_sprend.enabled = true;
            }
        }
        else {
            JSONNode responseJSON = JSONNode.Parse (www.text);
            if(GameController.instance.reset_fetch) {
                GameController.instance.reset_fetch = false;
                GameController.instance.api_loading = false;
                GameController.instance.creature_sprend.enabled = true;
            } else {
                var kaiju = responseJSON["kaiju"];
                var battles = responseJSON["kaiju"]["battles"];
                int wins = 0;
                int losses = 0;
                for(int i = 0; i < battles.Count; i++) {
                    bool found = false;
                    for(int j = 0; j < GameData.instance.battles.Count; j++) {
                        if(Int32.Parse(GameData.instance.battles[j].id) == Int32.Parse(battles[i]["id"])) found = true;
                    }
                    if(!found) {
                        Battle b = new Battle();
                        BATTLE_RESULT result = BATTLE_RESULT.WIN;
                        if(battles[i]["winnerID"] != GameData.instance.multiplayer_id) result = BATTLE_RESULT.LOSS;
                        b.result = result;
                        b.id = battles[i]["id"];
                        GameData.instance.battles.Add(b);
                        if(result == BATTLE_RESULT.LOSS) losses++;
                        else wins++;
                    }
                }
                GameData.instance.away = false;
                GameController.instance.api_loading = false;
                GameData.instance.energy_level = Int32.Parse(kaiju["energy"]);
                GameController.instance.creature_sprend.enabled = true;
                UpdateAwayStatusSprites();
                MultiplayerChoices.instance.UpdateBattleStatusText();
                int exp_points = ((wins * 3) + losses) * 2;
                int coins = GetCoinsFound(wins + losses);
                GameController.instance.FetchFromBattle(wins, losses, coins, exp_points);
                OutToBattle.instance.UpdateWins(0);
                OutToBattle.instance.UpdateLosses(0);
            }
        }
    }
    public void RefreshKaiju() {
        string send_url = url + "kaiju/" + GameData.instance.multiplayer_id + "/";
        www = new WWW(send_url);
        StartCoroutine(WaitForRefreshKaiju());
    }
    public IEnumerator WaitForRefreshKaiju() {
        yield return www; 

        if((!string.IsNullOrEmpty(www.error))) {
            print( "Error fetching kaiju: " + www.error );
        }
        else {
            JSONNode responseJSON = JSONNode.Parse (www.text);
            print(responseJSON);
            var kaiju = responseJSON["kaiju"];
            var battles = responseJSON["kaiju"]["battles"];
            int wins = 0;
            int losses = 0;
            for(int i = 0; i < battles.Count; i++) {
                bool found = false;
                for(int j = 0; j < GameData.instance.battles.Count; j++) {
                    if(Int32.Parse(GameData.instance.battles[j].id) == Int32.Parse(battles[i]["id"])) found = true;
                }
                if(!found) {
                    if(battles[i]["winnerID"] != GameData.instance.multiplayer_id) losses++;
                    else wins++;
                }
            }
            GameData.instance.energy_level = Int32.Parse(kaiju["energy"]);
            if(GameData.instance.energy_level < GameController.instance.battle_energy_cost) {
                GameController.instance.StartFlashFetchLowEnergy();
            }
            OutToBattle.instance.UpdateWins(wins);
            OutToBattle.instance.UpdateLosses(losses);
        }
    }
    public void UpdateAwayStatusSprites()
    {
        creature.GetComponent<SpriteRenderer>().enabled = !GameData.instance.away;
        away_sign.SetActive(GameData.instance.away);
    }
    public int GetCoinsFound(int battles) {
        int coins = 0;
        for(int i = 0; i < battles; ++i) coins += (int)UnityEngine.Random.Range(2, 7);
        return coins;
    }

}
