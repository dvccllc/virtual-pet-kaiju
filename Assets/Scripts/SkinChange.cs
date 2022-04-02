using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChange : MonoBehaviour {

    public bool change = false;

	// Update is called once per frame
	void Update () {
        if (change)
        {
            Change();
        }
	}
    public void StartChange()
    {
        change = true;
    }
    void Change()
    {

    }
}
