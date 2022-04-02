using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShell : MonoBehaviour
{
    public static RotateShell instance;
    public int id;
    public bool pressed = false;
    public bool cd = false;
    float button_cooldown = 0.2f;
	public bool rotating = false;
	public bool rotate_out = false;
    public bool rotate_in = false;
    public bool rotate_in_right = false;
    public bool rotate_in_left = false;
    public bool rotate_out_right = false;
    public bool rotate_out_left = false;
    public bool rotated = false;
	public bool zoomed_out = true;
	public bool zoomed_in = false;
    public bool zoom_out = false;
    public bool zoom_in = false;
    public float zoom_speed;
    public float rotate_speed;
    public bool zoom_out_far = true;
    float zoom_out_far_f = 10f;
    float zoom_in_f = 5f;
    public GameObject shell;
    public GameObject[] annoying_texts;
    public GameObject annoying_3dtext;
    public GameObject mountain;

    // Use this for initialization
    void Start()
    {
        instance = this;
    }
    void ActivateTexts(bool activate)
    {
        for(int i = 0; i < annoying_texts.Length; i++)
        {
            annoying_texts[i].SetActive(activate);
        }
    }
    // Update is called once per frame
    void Update()
    {
        instance = this;
        //if (zoom_out_far && GameController.instance.game_state != GAME_STATE.PAUSE && annoying_3dtext.activeSelf) annoying_3dtext.SetActive(false);
        //else if (Camera.main.orthographicSize < 8.5f && !annoying_3dtext.activeSelf) annoying_3dtext.SetActive(true);
        if (zoom_in)
        {
            if (Camera.main.orthographicSize > zoom_in_f) Camera.main.orthographicSize = Camera.main.orthographicSize - zoom_speed * Time.deltaTime;
            if (Camera.main.orthographicSize < (zoom_in_f + (zoom_speed * Time.deltaTime)))
            {
                mountain.SetActive(true);
                ActivateTexts(true);
                Camera.main.orthographicSize = zoom_in_f;
                zoom_in = false;
				zoomed_in = true;
                zoomed_out = false;
                GameController.instance.canvas.SetActive(false);
            }
        }
        if (zoom_out_far)
        {
            if (Camera.main.orthographicSize < zoom_out_far_f) Camera.main.orthographicSize = Camera.main.orthographicSize + zoom_speed * Time.deltaTime;
            if (Camera.main.orthographicSize > (zoom_out_far_f - (zoom_speed * Time.deltaTime)))
            {
                Camera.main.orthographicSize = zoom_out_far_f;
                zoom_out_far = false;
                zoomed_out = true;
            }
        }

        if (rotate_out_right)
        {
            if (shell.transform.eulerAngles.y > (180f - (rotate_speed * Time.deltaTime * 2f)) && shell.transform.eulerAngles.y < (180f + (rotate_speed * Time.deltaTime * 2f)))
            {
                shell.transform.rotation = Quaternion.Euler(0, 180f, 0);
                rotate_out_right = false;
                return;
            }
            shell.transform.rotation = Quaternion.Euler(0, shell.transform.eulerAngles.y - rotate_speed * Time.deltaTime, 0);
        }
        if (rotate_in_right)
        {
            if (shell.transform.eulerAngles.y < (rotate_speed * Time.deltaTime * 2f) && shell.transform.eulerAngles.y > (rotate_speed * Time.deltaTime * -2f))
            {
                shell.transform.rotation = Quaternion.Euler(0, 0, 0);
                rotate_in_right = false;
                return;
            }
            shell.transform.rotation = Quaternion.Euler(0, shell.transform.eulerAngles.y - rotate_speed * Time.deltaTime, 0);
        }
        if (rotate_out_left)
        {
            if (shell.transform.eulerAngles.y > 180f - (rotate_speed * Time.deltaTime * 2f) && shell.transform.eulerAngles.y < 180f + (rotate_speed * Time.deltaTime * 2f))
            {
                shell.transform.rotation = Quaternion.Euler(0, 180f, 0);
                rotate_out_left = false;
                return;
            }
            shell.transform.rotation = Quaternion.Euler(0, shell.transform.eulerAngles.y + rotate_speed * Time.deltaTime, 0);
        }
        if (rotate_in_left)
        {
            if (shell.transform.eulerAngles.y < (rotate_speed * Time.deltaTime * 2f) && shell.transform.eulerAngles.y > (rotate_speed * Time.deltaTime * -2f))
            {
                shell.transform.rotation = Quaternion.Euler(0, 0, 0);
                rotate_in_left = false;
                return;
            }
            shell.transform.rotation = Quaternion.Euler(0, shell.transform.eulerAngles.y + rotate_speed * Time.deltaTime, 0);
        }
    }
    public void zoomFar()
    {
        if (rotated) return;
        if(zoom_out_far)
        {
            zoomFarIn();
        }
        else
        {
            mountain.SetActive(false);
            ActivateTexts(false);
            zoomFarOut();
        }
    }
    public void zoomFarOut()
    {
        zoom_out = false;
        rotate_out = false;
        zoom_in = false;
        rotate_in = true;
        rotated = false;
		zoomed_in = false;
        zoom_out_far = true;
    }
    public void zoomFarIn()
    {
        if(TextInterfaceCameraController.instance.pan_left || TextInterfaceCameraController.instance.pan_right) return;
        zoom_out = false;
        rotate_out = false;
        zoom_in = true;
        rotate_in = true;
        rotated = false;
        zoom_out_far = false;
    }
    void Cooldown()
    {
        cd = false;
    }
    public void ReactRight()
    {
        if (!zoomed_out || rotate_in_left || rotate_in_right || rotate_out_left || rotate_out_right) return;
        SoundsController.instance.PlaySound("button 1");
        if (rotated)
        {
            rotate_in_left = false;
            rotate_in_right = true;
            rotate_out_left = false;
            rotate_out_right = false;
            rotated = false;
        }
        else
        {
            rotate_in_left = false;
            rotate_in_right = false;
            rotate_out_left = false;
            rotate_out_right = true;
            rotated = true;
        }
    }
    public void ReactLeft()
    {
        if (!zoomed_out) return;
        SoundsController.instance.PlaySound("button 1");
        if (rotated)
        {
            rotate_in_left = true;
            rotate_in_right = false;
            rotate_out_left = false;
            rotate_out_right = false;
            rotated = false;
        }
        else
        {
            rotate_in_left = false;
            rotate_in_right = false;
            rotate_out_left = true;
            rotate_out_right = false;
            rotated = true;
        }
    }
}
