using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class VolumeController : MonoBehaviour {

    float volume = 1f;
    public Text txt;
    public void VolumeUp()
    {
        volume = volume + 0.1f;
        if (volume > 1f)
        {
            volume = 1f;
        }
        UpdateVolume();
    }
    public void VolumeDown()
    {
        volume = volume - 0.1f;
        if (volume < 0.1f)
        {
            volume = 0.1f;
        }
        UpdateVolume();
    }
    // Use this for initialization
    void Start () {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        AudioListener.volume = volume;
        txt.text = "" + Math.Round(volume * 10f, MidpointRounding.AwayFromZero);
    }
}
