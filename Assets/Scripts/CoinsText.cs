using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsText : MonoBehaviour {
    public static CoinsText instance;
	TextMesh text;
	// Use this for initialization
	void Awake () {
        instance = this; 
	}

	void Start () {
        instance = this; 
	}

    public void UpdateCoinsText()
    {
        if (!text)
            text = GetComponent<TextMesh>();
        text.text = "";
        text.text = ((int)GameData.instance.num_coins).ToString();
    }
}
