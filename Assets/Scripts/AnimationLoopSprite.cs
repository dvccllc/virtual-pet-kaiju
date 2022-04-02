using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationLoopSprite : MonoBehaviour {
    int idx = 0;
    public float speed;
    float o_speed;
    SpriteRenderer sprend;
    public Sprite[] sprites;
    public bool oscillate;
	// Use this for initialization
	void Start () {
        sprend = GetComponent<SpriteRenderer>();
        Invoke("Animate", speed);
        o_speed = speed;
    }

    void Animate()
    {
        if(oscillate)
        {
            if (idx % 2 == 0)
            {
                speed = o_speed;
            }
            else
            {
                speed = o_speed * 3f;
            }
        }
        idx++;
        if (idx >= sprites.Length) idx = 0;
        sprend.sprite = sprites[idx];
        Invoke("Animate", speed);
    }
}
