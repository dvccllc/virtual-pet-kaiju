using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationLoopCanvas : MonoBehaviour {
    int idx = 0;
    public float speed;
    Image img;
    public Sprite[] sprites;
	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
        Invoke("Animate", speed);
    }
    void Animate()
    {
        idx++;
        if (idx >= sprites.Length) idx = 0;
        img.sprite = sprites[idx];
        Invoke("Animate", speed);
    }
}
