using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskController : MonoBehaviour {

    public static DeskController instance;
    public int default_desk = 2;
    int desk = 2;
    public GameObject[] desks;

    // Use this for initialization
    void Start () {
        instance = this;
        ReactivateDesks();
	}
    public void CycleColor()
    {
        desk++;
        if (desk == desks.Length) desk = 0;
        ReactivateDesks();
    }
    public void ReactivateDesks()
    {
        for(int i = 0; i < desks.Length; i++)
        {
            if (desks[i].activeSelf) desks[i].SetActive(false);
        }
        desks[desk].SetActive(true);
    }
}
