using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SETTINGS_CHOICE { VOID, NEW_GAME, RESET_ACCOUNT, TOGGLE_GRAPHICS, TOGGLE_DEMO }
public class Settings : MonoBehaviour {
    public SETTINGS_CHOICE choice = SETTINGS_CHOICE.VOID;
    public void PushSimpleButton()
    {
        if (choice == SETTINGS_CHOICE.VOID) return;
        if (choice == SETTINGS_CHOICE.NEW_GAME)
        {
            ConfirmNewGame.instance.activated = true;
        }
        else if (choice == SETTINGS_CHOICE.RESET_ACCOUNT)
        {
            ConfirmResetAccount.instance.activated = true;
        }
        else if (choice == SETTINGS_CHOICE.TOGGLE_GRAPHICS)
        {
            ShellColorController.instance.ToggleGraphics();
        }
        else if (choice == SETTINGS_CHOICE.TOGGLE_DEMO)
        {
            DemoMode.instance.ToggleDemoMode();
        }
    }
}
