using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsToggle : MonoBehaviour {

    public Text txt;
    public static GraphicsToggle instance;
	// Use this for initialization
	void Start () {
        instance = this;
        SetText();
	}
	public void ToggleGraphics()
    {
        GameData.instance.low_res = !GameData.instance.low_res;
        SetText();
    }
    public void SetText() {
        if (GameData.instance.low_res) txt.text = "LOW";
        else txt.text = "HIGH";
    }
}
