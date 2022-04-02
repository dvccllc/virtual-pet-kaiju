using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Advertisements;
public enum TEXT_INTERFACE_BUTTON { EXIT, SETTINGS, SETTINGS_RETURN, STORE, STORE_RETURN, SHOW_AD, BUY_ENERGY, BUY_COLOR_1, BUY_COLOR_2, BUY_COLOR_3, BUY_COLOR_4, BUY_COLOR_5, BUY_COLOR_6, BUY_COLOR_7, COMING_SOON, LEADERBOARDS, LEADERBOARDS_RETURN }
public class TextInterface : MonoBehaviour {
    public TEXT_INTERFACE_BUTTON choice;
    public TextInterfaceCameraController camera_controller;
    string placementId = "rewardedVideo";
    string gameId = "1585983";
    int energy_price, black_red_shell_price, dino_shell_price, juicy_shell_price, rainbow_shell_price;
    public int ad_reward = 50;
    // Use this for initialization
    void Start () {
        energy_price = 50;
        black_red_shell_price = 100;
        dino_shell_price = 250;
        juicy_shell_price = 500;
        rainbow_shell_price = 750;
        // if (Advertisement.isSupported)
        // {
        //     Advertisement.Initialize(gameId, true);
        // }
    }
	
    void ShowAd()
    {
                 GameData.instance.num_coins += ad_reward;
                 //CoinsText.instance.UpdateCoinsText();

        // ShowOptions options = new ShowOptions();
        // options.resultCallback = HandleShowResult;

        // Advertisement.Show(placementId, options);
    }

    // void HandleShowResult(ShowResult result)
    // {
    //     if (result == ShowResult.Finished)
    //     {
    //         Debug.Log("Video completed - Offer a reward to the player");
    //         GameData.instance.num_coins += ad_reward;
    //         //CoinsText.instance.UpdateCoinsText();
    //     }
    //     else if (result == ShowResult.Skipped)
    //     {
    //         Debug.LogWarning("Video was skipped - Do NOT reward the player");

    //     }
    //     else if (result == ShowResult.Failed)
    //     {
    //         Debug.LogError("Video failed to show");
    //     }
    // }
    public void ActivateStore()
    {
        GameController.instance.store_bar.SetActive(true);
        GameController.instance.store_coin1.SetActive(true);
        GameController.instance.store_coin2.SetActive(true);
    }
    public void OnMouseDown()
    {
        if (choice == TEXT_INTERFACE_BUTTON.LEADERBOARDS)
        {
            Leaderboards.instance.RefreshLeaderboards();
            camera_controller.panUp();
            GameController.instance.leaderboards = true;
        }
        if (choice == TEXT_INTERFACE_BUTTON.LEADERBOARDS_RETURN)
        {
            camera_controller.panDown();
			if (InitialsPrompt.instance.activated)
				InitialsPrompt.instance.Okay ();
            GameController.instance.leaderboards = false;
        }
        if (choice == TEXT_INTERFACE_BUTTON.SETTINGS)
        {
            camera_controller.panRight();
            GameController.instance.settings = true;
        }
        if (choice == TEXT_INTERFACE_BUTTON.SETTINGS_RETURN)
        {
            ConfirmNewGame.instance.activated = false;
            camera_controller.panCenter();
            GameController.instance.settings = false;
        }
        if (choice == TEXT_INTERFACE_BUTTON.STORE)
        {
            camera_controller.panLeft();
            GameController.instance.store = true;
            ActivateStore();
        }
        if (choice == TEXT_INTERFACE_BUTTON.STORE_RETURN)
        {
            ConfirmEnergyPurchase.instance.activated = false;
            ConfirmColorPurchase.instance.activated = false;
            camera_controller.panCenter();
            GameController.instance.store = false;
        }
        if (choice == TEXT_INTERFACE_BUTTON.EXIT)
        {
            Application.Quit();
        }
        if (choice == TEXT_INTERFACE_BUTTON.COMING_SOON)
        {
            ComingSoonPrompt.instance.activated = true;
        }
        if (choice == TEXT_INTERFACE_BUTTON.SHOW_AD)
        {
            ShowAd();
        }
        if (choice == TEXT_INTERFACE_BUTTON.BUY_ENERGY)
        {
            if(GameData.instance.num_coins >= energy_price) {
                ConfirmEnergyPurchase.instance.activated = true;
            }
        }
        if (choice == TEXT_INTERFACE_BUTTON.BUY_COLOR_1)
        {
            if(GameData.instance.num_coins >= black_red_shell_price)
            {
                ConfirmColorPurchase.instance.activated = true;
                ConfirmColorPurchase.instance.coin_goal = black_red_shell_price;
                ConfirmColorPurchase.instance.color_idx = 1;
            }
        }
        if (choice == TEXT_INTERFACE_BUTTON.BUY_COLOR_2)
        {
            if (GameData.instance.num_coins >= dino_shell_price)
            {
                ConfirmColorPurchase.instance.activated = true;
                ConfirmColorPurchase.instance.coin_goal = dino_shell_price;
                ConfirmColorPurchase.instance.color_idx = 2;
            }
        }
        if (choice == TEXT_INTERFACE_BUTTON.BUY_COLOR_3)
        {
            if (GameData.instance.num_coins >= juicy_shell_price)
            {
                ConfirmColorPurchase.instance.activated = true;
                ConfirmColorPurchase.instance.coin_goal = juicy_shell_price;
                ConfirmColorPurchase.instance.color_idx = 3;
            }
        }
        if (choice == TEXT_INTERFACE_BUTTON.BUY_COLOR_4)
        {
            if (GameData.instance.num_coins >= rainbow_shell_price)
            {
                ConfirmColorPurchase.instance.activated = true;
                ConfirmColorPurchase.instance.coin_goal = rainbow_shell_price;
                ConfirmColorPurchase.instance.color_idx = 4;
            }
        }
        if (choice == TEXT_INTERFACE_BUTTON.BUY_COLOR_5)
        {
            if (GameData.instance.num_coins >= rainbow_shell_price)
            {
                ConfirmColorPurchase.instance.activated = true;
                ConfirmColorPurchase.instance.coin_goal = rainbow_shell_price;
                ConfirmColorPurchase.instance.color_idx = 5;
            }
        }
        if (choice == TEXT_INTERFACE_BUTTON.BUY_COLOR_6)
        {
            if (GameData.instance.num_coins >= rainbow_shell_price)
            {
                ConfirmColorPurchase.instance.activated = true;
                ConfirmColorPurchase.instance.coin_goal = rainbow_shell_price;
                ConfirmColorPurchase.instance.color_idx = 6;
            }
        }
    }
}
