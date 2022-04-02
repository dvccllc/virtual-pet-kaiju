using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoMode : MonoBehaviour {
    public static DemoMode instance;
    public Text txt;
    public bool on = false;
	// Use this for initialization
	void Start () {
        instance = this;
        on = GameData.instance.tutorial_mode;
        SetText();
	}
	public void ToggleDemoMode()
    {
        on = !on;
        GameData.instance.tutorial_mode = !GameData.instance.tutorial_mode;
        SetText();
    }
    public void SetText() {
        if (on) txt.text = "ON";
        else txt.text = "OFF";
    }
}
