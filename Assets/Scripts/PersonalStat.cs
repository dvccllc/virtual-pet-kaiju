using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersonalStat : MonoBehaviour {
	public PERSONAL_STAT stat;
	TextMesh text;
	public void UpdateStat() {
		text = GetComponent<TextMesh>();
		text.text = "";
		if(stat == PERSONAL_STAT.STRENGTH_1) {
			text.text = "MOUNTAIN SMASH: " + GameData.instance.score_strength_1;
		}
		if(stat == PERSONAL_STAT.STRENGTH_2) {
			text.text = "MONSTER MIMIC: " + GameData.instance.score_strength_2;
		}
		if(stat == PERSONAL_STAT.DEFENSE_1) {
			text.text = "MISSILE DEFENSE: " + GameData.instance.score_defense_1;
		}
		if(stat == PERSONAL_STAT.DEFENSE_2) {
			text.text = "STAR CATCHER: " + GameData.instance.score_defense_2;
		}
		if(stat == PERSONAL_STAT.WINS) {
			text.text = "WINS: " + GetWins();
		}
		if(stat == PERSONAL_STAT.LOSSES) {
			text.text = "LOSSES: " + GetLosses();
		}
	}
	int GetWins() {
		int sum = 0;
		for(int i = 0; i < GameData.instance.battles.Count; i++) {
			if(GameData.instance.battles[i].result == BATTLE_RESULT.WIN) sum++;
		}
		return sum;
	}
	int GetLosses() {
		int sum = 0;
		for(int i = 0; i < GameData.instance.battles.Count; i++) {
			if(GameData.instance.battles[i].result == BATTLE_RESULT.LOSS) sum++;
		}
		return sum;
	}
}
