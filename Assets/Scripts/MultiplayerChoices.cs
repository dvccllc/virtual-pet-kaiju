using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerChoices : MonoBehaviour
{
    public static MultiplayerChoices instance;
    public TextMesh txt1;
    public int choice = 0;
    public GameObject[] choices;

    void Start()
    {
        instance = this;
        UpdateBattleStatusText();
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
    public void UpdateBattleStatusText()
    {
        if (GameData.instance.away)
        {
            GameObject.FindGameObjectWithTag("out action").GetComponent<TextMesh>().text = "FETCH FROM BATTLE";
            OutToBattle.instance.UpdateAwayStatus("STATUS: AWAY");
        }
        else
        {
            GameObject.FindGameObjectWithTag("out action").GetComponent<TextMesh>().text = "SEND TO BATTLE";
            OutToBattle.instance.UpdateAwayStatus("STATUS: HOME");
        }
        foreach(MeshRenderer msh in OutToBattle.instance.away_interface.GetComponentsInChildren<MeshRenderer>()) {
            if (!GameData.instance.away) msh.enabled = false;
        }
        OutToBattle.instance.away_interface.SetActive(GameData.instance.away);
    }
}
