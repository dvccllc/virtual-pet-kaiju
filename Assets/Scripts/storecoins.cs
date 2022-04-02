using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class storecoins : MonoBehaviour {
    Text txt;
	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        if(GameController.instance.store) txt.text = "" + GameData.instance.num_coins;
    }
}
