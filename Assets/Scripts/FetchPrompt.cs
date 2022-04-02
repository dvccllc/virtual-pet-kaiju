using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchPrompt : MonoBehaviour {

	public static FetchPrompt instance;
	public TextMesh wins, losses, coins, exp_points;
	void Start () {
		instance = this;
	}
	public void UpdatePrompt(int w, int l, int c, int e) {
		wins.text = "WINS: " + w;
		losses.text = "LOSSES: " + l;
		coins.text = "COINS FOUND: " + c;
		exp_points.text = "EXP GAINED: " + e;
	}

}
