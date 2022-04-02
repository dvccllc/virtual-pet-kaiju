using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundsController : MonoBehaviour {
	public static SoundsController instance;
	public AudioSource[] sounds;
	public AudioSource[] roars;
	public AudioSource theme, intro;
	public bool mute = false;
    public Text txt;
	// Use this for initialization
	void Start () {
		instance = this;
	}
    public void Mute()
    {
        mute = !mute;
        if(!mute) StopTheme();

    }
    // Update is called once per frame
    void Update () {
		HandleMute ();
    }
	public void PlaySound(string sound){
		if (mute)
			return;
		switch (sound) {
		case "button 1":
			sounds [0].Play ();
			break;
		case "crush":
			sounds [1].Play ();
			break;
		case "meter":
			sounds [2].Play ();
			break;
		case "positive beep":
			sounds [3].Play ();
			break;		
		case "bad":
			sounds [4].Play ();
			break;
		case "shoot":
			sounds [5].Play ();
			break;
		case "mountain hit":
			sounds [6].Play ();
			break;
		case "mountain dead":
			sounds [7].Play ();
			break;
		case "missile explode":
			sounds [8].Play ();
			break;
		case "mm beep 1":
			sounds [9].Play ();
			break;
		case "mm beep 2":
			sounds [10].Play ();
			break;
		case "mm beep 3":
			sounds [11].Play ();
			break;
		case "falling star":
			sounds [12].Play ();
			break;
		case "star hit":
			sounds [13].Play ();
			break;
		case "bomb hit":
			sounds [14].Play ();
			break;
		case "death":
			sounds [15].Play();
			break;
		case "Evolve":
			sounds [16].Play();
			break;
		default:
			print ("SOUND ERROR");
			break;
		}
	}
	public void PlayRoar(){
		if (mute)
			return;
		if(GameData.instance.monster_num != GameController.instance.DEATH_NUM && !roars [GameData.instance.monster_num].isPlaying) roars [GameData.instance.monster_num].Play ();
	}
	public void PlayTheme(){
		theme.Play ();
	}
	public void StopTheme(){
		if(theme.isPlaying) theme.Stop ();
	}
	public void PlayIntro(){
		intro.Play ();
	}
	public void StopIntro(){
		if(intro.isPlaying) intro.Stop ();
	}
	void HandleMute(){
		if (Input.GetKeyDown (KeyCode.M)) {
			mute = true;
		}
		if (Input.GetKeyDown (KeyCode.N) && mute) {
			mute = false;
		}
		if (mute)
			StopTheme ();
		if (mute) txt.text = "YES";
        else txt.text = "NO";
	}
}
