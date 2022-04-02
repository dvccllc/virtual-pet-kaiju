// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    public TextInterfaceCameraController camera_controller;
    public static MoveCamera instance;
    //
    // VARIABLES
    //

    public float panSpeed = 4.0f;       // Speed of the camera when being panned

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isPanning;     // Is the camera being panned?

    //
    // UPDATE
    //
    public bool cd = false;
    float cd_timer = 0.6f;
    public float swipe_friction = 1f;
    public void Cooldown() 
    {
        cd = false;
    }
    void MoveToStore()
    {
        camera_controller.panLeft();
        GameController.instance.store = true;
        GameController.instance.store_bar.SetActive(true);
        GameController.instance.store_coin1.SetActive(true);
        GameController.instance.store_coin2.SetActive(true);
    }
    void MoveToSettings()
    {
        camera_controller.panRight();
        GameController.instance.settings = true;
    }
    void MoveToCenter()
    {
        camera_controller.panCenter();
    }
    void MoveStoreToCenter()
    {
        ConfirmEnergyPurchase.instance.activated = false;
        ConfirmColorPurchase.instance.activated = false;
        camera_controller.panCenter();
        GameController.instance.store = false;
    }
    void MoveSettingsToCenter()
    {
        ConfirmNewGame.instance.activated = false;
        camera_controller.panCenter();
        GameController.instance.settings = false;
    }
    void Update()
    {
        if (!instance) instance = this;
        // Get the right mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        // Disable movements on button release
        if (!Input.GetMouseButton(0))
        {
            isPanning = false;
            /*
            if(transform.localPosition.x < -10f)
            {
                MoveToStore();
            } 
            else if (transform.localPosition.x > 10f) {
                MoveToSettings();
            }
            else
            {
                if(GameController.instance.store)
                {
                    MoveStoreToCenter();
                }
                else if(GameController.instance.settings)
                {
                    MoveSettingsToCenter();
                }
                else
                {
                    MoveToCenter();
                }
            }
            */
        }

        // Move the camera on it's XY plane
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            /*
            Vector3 move = new Vector3(pos.x * panSpeed * Time.deltaTime, 0, 0);
            if(transform.localPosition.x + pos.x * panSpeed * Time.deltaTime < -20f)
            {
                MoveToStore();
            }
            else if(transform.localPosition.x + pos.x * panSpeed * Time.deltaTime > 20f)
            {
                MoveToSettings();
            }
            else transform.Translate(move, Space.Self);
            */
            if (pos.x < -1f * swipe_friction && RotateShell.instance.zoomed_out)
            {
                if (cd) return;
                cd = true;
                Invoke("Cooldown", cd_timer);
                //hard right
                if (GameController.instance.settings) return;
                if (GameController.instance.store)
                {
                    MoveStoreToCenter();
                }
                else
                {
                    MoveToSettings();
                }
                isPanning = false;
                return;

            }
            else if (pos.x > swipe_friction && RotateShell.instance.zoomed_out)
            {
                if (cd) return;
                cd = true;
                Invoke("Cooldown", cd_timer);
                //hard left
                if (GameController.instance.store) return;
                if (GameController.instance.settings)
                {
                    MoveSettingsToCenter();
                }
                else
                {
                    MoveToStore();
                }
                isPanning = false;
                return;
            }
        }


    }
}