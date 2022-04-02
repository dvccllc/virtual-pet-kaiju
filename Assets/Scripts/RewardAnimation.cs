using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RewardAnimation : MonoBehaviour
{
    public static RewardAnimation instance;
    bool fade = false;
    float fade_rate = 0.01f;
    float coin_multiplier = 5f;
    public float fade_augmentation;
    public float fadeDelay;
    public MeshRenderer icon_rend, plus_rend;
    public Sprite trainIcon, protectIcon;

    // Use this for initialization
    void Start()
    {
        instance = this;
        FadeInstant(GetComponent<SpriteRenderer>());
        fade = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fade)
        {
            if(icon_rend.material.color.a <= 0.05f)
            {
                fade = false;
                FadeInstant(GetComponent<SpriteRenderer>());
            }
            Fade(GetComponent<SpriteRenderer>());
        }
    }
    public void UpdateValue(float wave, string type)
    {
        fade = true;
        icon_rend.enabled = true;
        plus_rend.enabled = true;
        float waveValue = (0.5f * wave);
        if(type != "coin" && waveValue > 2.5f) waveValue = 2.5f;
        if (type == "strength")
            GetComponent<SpriteRenderer>().sprite = trainIcon;
        if (type == "defense")
            GetComponent<SpriteRenderer>().sprite = protectIcon;
        if (type == "coin")
        {
            waveValue = (int)Math.Round((waveValue) * coin_multiplier);
        }
        icon_rend.gameObject.GetComponent<TextMesh>().text = "" + waveValue;
        TurnWhite(GetComponent<SpriteRenderer>());

    }
    void TurnWhite(SpriteRenderer rend)
    {
        rend.color = new Color(1, 1, 1, 1);
        icon_rend.material.color = new Color(icon_rend.material.color.r, icon_rend.material.color.g, icon_rend.material.color.b, 1f);
        plus_rend.material.color = new Color(plus_rend.material.color.r, plus_rend.material.color.g, plus_rend.material.color.b, 1f);
    }
    void FadeInstant(SpriteRenderer rend)
    {
        rend.color = new Color(1, 1, 1, 0);
        icon_rend.material.color = new Color(icon_rend.material.color.r, icon_rend.material.color.g, icon_rend.material.color.b, 0f);
        plus_rend.material.color = new Color(plus_rend.material.color.r, plus_rend.material.color.g, plus_rend.material.color.b, 0f);
    }
    void Fade(SpriteRenderer rend)
    {

        icon_rend.material.color = new Color(icon_rend.material.color.r, icon_rend.material.color.g, icon_rend.material.color.b, icon_rend.material.color.a - fade_rate);
        plus_rend.material.color = new Color(plus_rend.material.color.r, plus_rend.material.color.g, plus_rend.material.color.b, plus_rend.material.color.a - fade_rate);
        rend.color = new Color(1, 1, 1, rend.color.a - fade_rate);
    }
}

