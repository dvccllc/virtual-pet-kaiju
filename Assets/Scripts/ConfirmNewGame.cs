using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmNewGame : MonoBehaviour {
    public static ConfirmNewGame instance;
    public bool activated = false;
    float speed = 0.1f;
	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(activated)
        {
            if(this.transform.localScale.x < 4.95f) {
                this.transform.localScale = Vector3.Lerp(this.transform.localScale, new Vector3(5f,2f,1f), speed);
            } 
            else {
                this.transform.localScale = new Vector3(5f,2f,1f);
            }
        }
        else
        {
            if(this.transform.localScale.x > 0.05f) {
                this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, speed);
            }
            else {
                this.transform.localScale = Vector3.zero;
            }
        }
    }
    public void Yes()
    {
        SaveLoad.instance.NewGame();
        activated = false;
    }
    public void No()
    {
        activated = false;
    }
}
