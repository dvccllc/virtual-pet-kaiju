using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
	public GameObject arrow;
    public float fireball_speed, animationSpeed, animationDelay;
    public bool reversed;
	public SpriteRenderer sprend;
    public Sprite[] sprites, deathSprites;
    float animation_speed = 0.2f;
    int sprite_index = 0;
	int index = 0;
	bool dead = false;
    float x_bound = 2.7f;
    float creature_x_bound = -12f;
    // Use this for initialization
    void Start () {
		sprend = GetComponent<SpriteRenderer> ();
        Animate();
	}
	void Animate()
    {
		if (dead) {
			CancelInvoke ("Animate");
			return;
		}
        GetComponent<SpriteRenderer>().sprite = sprites[sprite_index];
        sprite_index++;
        if (sprite_index == sprites.Length) sprite_index = 0;
        Invoke("Animate", animation_speed);
    }
	// Update is called once per frame
	void Update () {
		if (dead)
			return;
		if(!sprend) sprend = GetComponent<SpriteRenderer> ();
        if (reversed){
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            transform.localPosition = new Vector3(transform.localPosition.x - Time.deltaTime * fireball_speed, transform.localPosition.y, transform.localPosition.z);
			if (arrow) {
				arrow.transform.localScale =  Vector3.Lerp(arrow.transform.localScale, Vector3.one, 0.05f);
				if (arrow.transform.localScale.x >= 0.95f) {
					Destroy (arrow.gameObject);
				}
			}

			if (transform.position.x < -1 * x_bound){
				DefenseMiniGame1.instance.RemoveFireball (this.gameObject);
				Destroy (this.gameObject);
			}
		}
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x + Time.deltaTime * fireball_speed, transform.localPosition.y, transform.localPosition.z);
            if (transform.localPosition.x < creature_x_bound) Destroy(this.gameObject);
		}

    }
	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.tag == "screen")
			return;
		if (reversed) {
			if (coll.gameObject.tag == "Creature") {
				print ("hit creature");
				if (!CreatureFlash.instance.flashing) {
					CreatureFlash.instance.Flash ();
					DefenseMiniGame1.instance.creature_health--;
					if (DefenseMiniGame1.instance.creature_health == 0) {
						GameData.instance.defense_times_played++;
						//GameController.instance.PressButton (0);
					}
				}
				Explode ();
			}
			if (coll.gameObject.tag == "Fireball" && transform.localPosition.x < x_bound) {
				//enemy entering the player's fireball's trigger
				Destroy (coll.gameObject);
				if(DefenseMiniGame1.instance.playing && !dead) { 
					DefenseMiniGame1.instance.creature_score++;
					int goal = (DefenseMiniGame1.instance.wave + 1) * 3;
					if(DefenseMiniGame1.instance.creature_score == goal)
					{
						DefenseMiniGame1.instance.playing = false;
						DefenseMiniGame1.instance.NextWave ();
					}
				}
				Explode ();
				DefenseMiniGame1.instance.UpdateHitsNeeded();
			}
		} else {
			if (coll.gameObject.tag == "Mountain") {
				//enemy entering the player's fireball's trigger
				print("power: " + StrengthBar.instance.powerLevel);
				coll.gameObject.GetComponent<Mountain>().health -= StrengthBar.instance.powerLock;
				if (coll.gameObject.GetComponent<Mountain> ().health > 0) {
					SoundsController.instance.PlaySound ("mountain hit");
					StrengthMiniGame1.instance.mountain_hp.text = "MOUNTAIN HP: " + coll.gameObject.GetComponent<Mountain> ().health;
				}
				if (coll.gameObject.GetComponent<Mountain> ().health <= 0 || StrengthMiniGame1.instance.fireballs > StrengthMiniGame1.instance.wave) {
					if(coll.gameObject.GetComponent<Mountain> ().health <= 0) StrengthMiniGame1.instance.mountain_hp.text = "MOUNTAIN HP: " + 0;
					else StrengthMiniGame1.instance.mountain_hp.text = "MOUNTAIN HP: " + coll.gameObject.GetComponent<Mountain> ().health;
					if(coll.gameObject.GetComponent<Mountain> ().wave == StrengthMiniGame1.instance.wave) StrengthMiniGame1.instance.NextWave ();
				} else {
					StrengthBar.instance.StartTick ();
				}
				Destroy (this.gameObject);
			}
		}
	}
	public void ExplosionFrame(){
		sprend.enabled = true;
		if (index == deathSprites.Length) {
			DefenseMiniGame1.instance.RemoveFireball (this.gameObject);
			Destroy (this.gameObject);
			return;
		}

		sprend.sprite = deathSprites [index];
		index++;
		Invoke ("ExplosionFrame", animationDelay);
	}
	public void Explode(){
		dead = true;
		SoundsController.instance.PlaySound ("missile explode");
		Invoke ("ExplosionFrame", animationDelay);
	}
}
