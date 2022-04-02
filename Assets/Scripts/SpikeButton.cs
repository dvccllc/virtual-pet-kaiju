using System.Collections;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;


public enum LOCATION { LEFT, MIDDLE, RIGHT };
public class SpikeButton : MonoBehaviour {
    public LOCATION location;
    public ShellColorController shell_color_contr;
    public RotateShell shell_rotate_contr;
    bool m_cooldown = false;

    public void DatabaseButtonCooldown()
    {
        m_cooldown = false;
    }
    public void OnMouseDown()
    {
        if (location == LOCATION.LEFT)
        {
            if(GameController.instance.game_state == GAME_STATE.PLAY && !GameController.instance.rotate_ctr.zoomed_out) {
                GameController.instance.canvas.SetActive(true);
                GameController.instance.rotate_ctr.zoomFar();
            }
            if(GameController.instance.game_state == GAME_STATE.PLAY && GameController.instance.rotate_ctr.zoomed_out) {
                GameController.instance.rotate_ctr.zoomFarIn();
            }
        }
        if (location == LOCATION.MIDDLE)
        {
            if (!m_cooldown)
            {
                m_cooldown = true;
                Invoke("DatabaseButtonCooldown", 0.25f);
                print("OnMouseDown()");
                DeskController.instance.CycleColor();
            }
        }
        if(location == LOCATION.RIGHT)
        {
            shell_color_contr.cycleColorUp();
        }
    }
}
