using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseChoices : MonoBehaviour
{
    public static DefenseChoices instance;

    public int choice = 0;
    public TextMesh choice_2;
    public GameObject[] choices;
	public GameObject locked1, locked2;
    void Awake()
    {
        instance = this;
    }
    public void CycleChoice()
    {
        choice++;
        if (choice > 2) choice = 0;
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }
        choices[choice].SetActive(true);
    }
    public void UpdateChoice2() {
        string txt = "";
		if (GameData.instance.evolved) {
			txt = "STAR CATCHER";
			locked1.SetActive (false);
		}
		else {
			txt = "";
			locked1.SetActive (true);
		}
        choice_2.text = txt;
    }
}
