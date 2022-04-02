using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallAnimation : MonoBehaviour {
    public bool animating;
    float animation_speed = 0.25f;
    int index = 0;
    public void StartAnimation()
    {
        animating = true;
        Animate();
    }
    public void StopAnimation()
    {
        animating = false;
        GetComponent<SpriteRenderer>().sprite = GetComponentInParent<Character>().monster.mini_game_sprites[0];
        CancelInvoke("Animate");
    }
    void Animate()
    {
        if (!animating)
        {
            CancelInvoke("Animate");
            GetComponent<SpriteRenderer>().sprite = GetComponentInParent<Character>().monster.mini_game_sprites[0];
            return;
        }
        index++;
        if(index >= GetComponentInParent<Character>().monster.mini_game_sprites.Length)
        {
            index = 0;
            animating = false;
            CancelInvoke("Animate");
            GetComponent<SpriteRenderer>().sprite = GetComponentInParent<Character>().monster.mini_game_sprites[0];
            return;
        }
        GetComponent<SpriteRenderer>().sprite = GetComponentInParent<Character>().monster.mini_game_sprites[index];
        Invoke("Animate", animation_speed);
    }
}
