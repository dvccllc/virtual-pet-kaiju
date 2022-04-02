using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellColorController : MonoBehaviour {
    public static ShellColorController instance;
    public GameObject[] high_colors;
    public GameObject[] low_colors;
    public GameObject active_color;
    public GameObject high_res;
    public GameObject low_res;
    // Use this for initialization
    void Start () {
        instance = this;
        ReactivateAll();
    }

    public void cycleColorUp()
    {
        SoundsController.instance.PlaySound("button 1");
        GameData.instance.chosen_skin++;
        if (GameData.instance.chosen_skin >= low_colors.Length) GameData.instance.chosen_skin = 0;
        if (!GameData.instance.skins[GameData.instance.chosen_skin]) cycleColorUp();
        ReactivateAll();
    }
    public void DestroyShell() {
        Destroy(active_color);
    }
    public void CreateShell() {
        GameObject color;
        GameObject parent;
        if(GameData.instance.low_res) {
            color = low_colors[GameData.instance.chosen_skin];
            parent = low_res;
        }
        else {
            color = high_colors[GameData.instance.chosen_skin];
            parent = high_res;
        }
        active_color = MonoBehaviour.Instantiate(color, color.transform.position, color.transform.rotation);
        active_color.transform.parent = parent.transform;
        active_color.transform.localPosition = color.transform.position;
        active_color.transform.localRotation = color.transform.rotation;
        active_color.transform.localScale = color.transform.localScale;
    }
    public void ReactivateAll()
    {
        DestroyShell();
        CreateShell();
        // if (GameData.instance.low_res)
        // {
        //     for (int i = 0; i < high_colors.Length; i++)
        //     {
        //         DestroyShell(high_colors[i]);
        //     }
        //     for (int i = 0; i < low_colors.Length; i++)
        //     {
        //         CreateShell(low_colors[i]);
        //         if (i != GameData.instance.chosen_skin) DestroyShell(low_colors[i]);
        //     }
        // }
        // else
        // {
        //     for (int i = 0; i < low_colors.Length; i++)
        //     {   
        //         DestroyShell(low_colors[i]);
        //     }
        //     for (int i = 0; i < high_colors.Length; i++)
        //     {
        //         CreateShell(high_colors[i]);
        //         if (i != GameData.instance.chosen_skin) DestroyShell(high_colors[i]);
        //     }
        // }
    }
    public void ToggleGraphics()
    {
        GraphicsToggle.instance.ToggleGraphics();
        ReactivateAll();
    }
    public void UnlockColor(int n, int coin_goal)
    {
        if (coin_goal != -1 && GameData.instance.num_coins >= coin_goal && !GameData.instance.skins[n])
        {
            GameData.instance.num_coins -= coin_goal;
            //CoinsText.instance.UpdateCoinsText();
            GameData.instance.skins[n] = true;
            GameData.instance.chosen_skin = n;
            ReactivateAll();
        }

    }
}
