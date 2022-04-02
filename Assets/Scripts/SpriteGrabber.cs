using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGrabber : MonoBehaviour {



	public Texture2D texture;
	public Sprite[] sprites;
    bool has_sprites = false;

	// Update is called once per frame
	void Update () {
		if(!has_sprites) GetSprites ();
	}

	public void GetSprites(){
		sprites = Resources.LoadAll<Sprite>(texture.name);
        has_sprites = true;
	}
}
