using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
	public int id;
	public bool pressed = false;
	public bool react = false;
    public bool cd = false;
    float button_cooldown = 0.2f;
	void OnMouseDown(){
        if (!pressed && !cd)
        {
            cd = true;
            Invoke("Cooldown", button_cooldown);
            React();
        }
		pressed = true;
	}
    void Cooldown()
    {
        cd = false;
    }
	void OnMouseUp(){
		pressed = false;
	}
    void React() {
        SoundsController.instance.PlaySound("button 1");
        if (RotateShell.instance.zoom_out_far) {
            RotateShell.instance.zoomFar();
            if (GameController.instance.game_state == GAME_STATE.VOID) react = true;
        }
        else react = true;

    }
}
