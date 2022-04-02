using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySlots : MonoBehaviour {
    public static BuySlots instance;
    public GameObject[] slots;
	// Use this for initialization
	void Start () {
        instance = this;
        UpdateSlots();
	}
    public void UpdateSlots()
    {
        for(int i = 1; i < slots.Length; i++)
        {
            slots[i].SetActive(true);
            if (GameData.instance.skins[i]) slots[i].SetActive(false);
        }
    }
}
