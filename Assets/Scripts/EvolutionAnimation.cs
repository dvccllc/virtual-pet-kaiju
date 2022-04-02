using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionAnimation : MonoBehaviour {

    SpriteRenderer sprend;
    public GameObject creature;
    public Sprite[] sprites;
    public int repeats;
    int idx = 0;
    public float speed;
	// Use this for initialization
	void Start () {
        sprend = GetComponent<SpriteRenderer>();
	}
    public void Evolve()
    {
        creature.SetActive(false);
        GameController.instance.evolving = true;
        sprend.enabled = true;
        Invoke("Animate", speed);
    }
    public void Animate()
    {
        idx++;
        if(idx >= sprites.Length)
        {
            if(repeats > 0)
            {
                repeats--;
                idx = 0;
            } else
            {
                GameController.instance.evolving = false;
                sprend.enabled = false;
                Evolution.instance.ExecuteEvolution();
                return;
            }
        }
        sprend.sprite = sprites[idx];
        Invoke("Animate", speed);
    }
}
