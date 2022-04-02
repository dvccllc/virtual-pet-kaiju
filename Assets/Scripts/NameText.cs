using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameText : MonoBehaviour {
	TextMesh text;
	// Update is called once per frame
	void Update () {
        if(GameController.instance.game_state == GAME_STATE.PAUSE)
        {
            if (!text)
                text = GetComponent<TextMesh>();
            text.text = "";
            text.text = "Name: " + GameData.instance.info_name;
            text.text = text.text.ToUpper();
        }
	}
}
