using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour {
	SpriteRenderer sprend;
	SpriteGrabber spgrabber;
	public bool energy, strength, defense;
	
	// Update is called once per frame
	void Update () {
		if (!sprend)
			sprend = GetComponent<SpriteRenderer> ();
		if (!spgrabber)
			spgrabber = GetComponent<SpriteGrabber> ();
		else {
			if(energy && (int)GameData.instance.energy_level > -1 && (int)GameData.instance.energy_level < spgrabber.sprites.Length) sprend.sprite = spgrabber.sprites [GameData.instance.energy_level];
			if(strength && (int)GameData.instance.strength_level > -1 && (int)GameData.instance.strength_level < spgrabber.sprites.Length) sprend.sprite = spgrabber.sprites [(int)GameData.instance.strength_level];
			if(defense && (int)GameData.instance.defense_level > -1 && (int)GameData.instance.defense_level < spgrabber.sprites.Length) sprend.sprite = spgrabber.sprites [(int)GameData.instance.defense_level];
		}
	}
}
