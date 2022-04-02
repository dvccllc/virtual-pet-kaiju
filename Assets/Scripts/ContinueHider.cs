using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ContinueHider : MonoBehaviour {

    public SpriteRenderer sprend;
    void Start()
    {
        sprend = GetComponent<SpriteRenderer>();
        sprend.enabled = File.Exists(Application.persistentDataPath + "/playerInfo.dat");
    }
}
