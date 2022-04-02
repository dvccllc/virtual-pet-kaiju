using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeText : MonoBehaviour {
	TextMesh text;

	// Update is called once per frame
	void Update () {
        if(GameController.instance.game_state == GAME_STATE.PAUSE)
        {
            if (!text)
                text = GetComponent<TextMesh>();
            text.text = "";
            text.text = "Age: " + ((int)GameData.instance.age).ToString();
            text.text = text.text.ToUpper();
        }
	}
}
