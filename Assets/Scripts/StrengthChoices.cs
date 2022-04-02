using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthChoices : MonoBehaviour {
    public static StrengthChoices instance;
    public TextMesh choice_2;
    public int choice = 0;
    public GameObject[] choices;
	public GameObject locked1, locked2;
	void Awake () {
        instance = this;
	}

    public void CycleChoice()
    {
        choice++;
        if (choice > 2) choice = 0;
        for(int i = 0; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }
        choices[choice].SetActive(true);
    }
    public void UpdateChoice2() {
        string txt = "";
		if (GameData.instance.evolved) {
			
			txt = "MONSTER MIMIC";
			locked1.SetActive (false);
		}
		else {
			locked1.SetActive (true);
			txt = "";
		}
        choice_2.text = txt;
    }
}
