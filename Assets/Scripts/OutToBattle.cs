using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutToBattle : MonoBehaviour {

	public static OutToBattle instance;
	public TextMesh wins, losses, away_status;
	public GameObject away_interface;
	void Start () {
		instance = this;
	}
	public void UpdateWins(int n) {
		wins.text = "" + n;
	}
	public void UpdateLosses(int n) {
		losses.text = "" + n;
	}
	public void UpdateAwayStatus(string s) {
		away_status.text = "" + s;
	}
}
