using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInterfaceCameraController : MonoBehaviour {
    public static TextInterfaceCameraController instance;
    public bool pan_center = true;
    public bool pan_left = false;
    public bool pan_right = false;
    public bool pan_up = false;
    public bool pan_down = false;
    public GameObject cam_;
    Vector3 cam_original;
    public float pan_speed;
    float cam_left = -20f;
    float cam_right = 20f;
    float cam_up = 20f;
    // Use this for initialization
    void Start () {
        instance = this;
        cam_original = new Vector3(0, -3, -10);
    }

    // Update is called once per frame
    void Update () {
        if (pan_center)
        {
            if (cam_.transform.localPosition.x >= -1f * pan_speed * Time.deltaTime && cam_.transform.localPosition.x <= pan_speed * Time.deltaTime)
            {
                cam_.transform.localPosition = new Vector3(0f, cam_original.y, cam_original.z);
                GameController.instance.store_bar.SetActive(false);
                GameController.instance.store_coin1.SetActive(false);
                GameController.instance.store_coin2.SetActive(false);
                pan_center = false;
                return;
            }
            if (cam_.transform.localPosition.x > 0f) cam_.transform.localPosition = new Vector3 (cam_.transform.localPosition.x - pan_speed * Time.deltaTime, cam_original.y, cam_original.z);
			else if(cam_.transform.localPosition.x < 0f) cam_.transform.localPosition = new Vector3 (cam_.transform.localPosition.x + pan_speed * Time.deltaTime, cam_original.y, cam_original.z);
            else
            {
                cam_.transform.localPosition = new Vector3(0f, cam_original.y, cam_original.z);
                GameController.instance.store_bar.SetActive(false);
                GameController.instance.store_coin1.SetActive(false);
                GameController.instance.store_coin2.SetActive(false);
                pan_center = false;
                return;
            }
        }
        if (pan_left)
        {
            if (cam_.transform.localPosition.x < (cam_left + (pan_speed * Time.deltaTime)))
            {
                cam_.transform.localPosition = new Vector3(cam_left, cam_original.y, cam_original.z);
                pan_left = false;
                return;
            }
            if (cam_.transform.localPosition.x > cam_left) cam_.transform.localPosition = new Vector3(cam_.transform.localPosition.x - pan_speed * Time.deltaTime, cam_original.y, cam_original.z);
            else
            {
                cam_.transform.localPosition = new Vector3(cam_left, cam_original.y, cam_original.z);
                pan_left = false;
                return;
            }
        }
        if (pan_right)
        {
            if (cam_.transform.localPosition.x > (cam_right - (pan_speed * Time.deltaTime)))
            {
                cam_.transform.localPosition = new Vector3(cam_right, cam_original.y, cam_original.z);
                pan_right = false;
                return;
            }
            if (cam_.transform.localPosition.x < cam_right) cam_.transform.localPosition = new Vector3(cam_.transform.localPosition.x + pan_speed * Time.deltaTime, cam_original.y, cam_original.z);
            else
            {
                cam_.transform.localPosition = new Vector3(cam_right, cam_original.y, cam_original.z);
                pan_right = false;
                return;
            }
        }
        if (pan_up)
        {
            if (cam_.transform.localPosition.y > (cam_up - (pan_speed * Time.deltaTime)))
            {
                cam_.transform.localPosition = new Vector3(cam_original.x, cam_up, cam_original.z);
                pan_up = false;
                return;
            }
            if (cam_.transform.localPosition.x < cam_up) cam_.transform.localPosition = new Vector3(cam_original.x, cam_.transform.localPosition.y + pan_speed * Time.deltaTime, cam_original.z);
            else
            {
                cam_.transform.localPosition = new Vector3(cam_original.x, cam_up, cam_original.z);
                pan_up = false;
                return;
            }
        }
        if (pan_down)
        {
            if (cam_.transform.localPosition.y < (cam_original.y + (pan_speed * Time.deltaTime)))
            {
                cam_.transform.localPosition = new Vector3(cam_original.x, cam_original.y, cam_original.z);
                pan_down = false;
                return;
            }
            if (cam_.transform.localPosition.y > cam_original.y) cam_.transform.localPosition = new Vector3(cam_original.x, cam_.transform.localPosition.y - pan_speed * Time.deltaTime, cam_original.z);
            else
            {
                cam_.transform.localPosition = new Vector3(cam_original.x, cam_original.y, cam_original.z);
                pan_down = false;
                return;
            }
        }
    }

    public void panLeft()
    {
        pan_center = false;
        pan_right = false;
        pan_left = true;
        pan_up = false;
        pan_down = false;
    }
    public void panCenter()
    {
        pan_center = true;
        pan_right = false;
        pan_left = false;
        pan_up = false;
        pan_down = false;
    }
    public void panRight()
    {
        pan_center = false;
        pan_right = true;
        pan_left = false;
        pan_up = false;
        pan_down = false;
    }
    public void panUp()
    {
        pan_center = false;
        pan_right = false;
        pan_left = false;
        pan_up = true;
        pan_down = false;
    }
    public void panDown()
    {
        pan_center = false;
        pan_right = false;
        pan_left = false;
        pan_up = false;
        pan_down = true;
    }
}
