using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthBar : MonoBehaviour {
	public static StrengthBar instance;
	SpriteRenderer sprend;
	public int powerLevel = 0;
	public int powerLock;
	public float tickRate = 0.25f, hitDelay, originalTickRate;
	public bool ticking = false;
	public TextMesh power_text;
	// Use this for initialization
	void Start () {
		sprend = GetComponent<SpriteRenderer> ();
		instance = this;
        CancelInvoke("TickBar");
        Invoke("TickBar", tickRate);
		originalTickRate = tickRate;
	}


	void TickBar() {
		ticking = true;
		powerLock = powerLevel;
		UpdatePowerText();
		sprend.sprite = GetComponent<SpriteGrabber> ().sprites [powerLevel++];
		if (powerLevel == GetComponent<SpriteGrabber> ().sprites.Length)
			powerLevel = 0;
        CancelInvoke("TickBar");
        Invoke("TickBar", tickRate);
	}
	public void StartTick(){
		powerLevel = 0;
		powerLock = powerLevel;
		UpdatePowerText();
        CancelInvoke("TickBar");
        Invoke("TickBar", hitDelay);
	}
	public void StopTick(){
		ticking = false;
		CancelInvoke ("TickBar");
	}
	public void UpdatePowerText() {
		power_text.text = "POWER: " + powerLock;
	}
}
