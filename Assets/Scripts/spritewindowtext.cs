using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spritewindowtext : MonoBehaviour {
    public bool has_outline;
    public GameObject obj;
	public SpriteRenderer outline_sprend, padlock_sprend;

    void OnTriggerEnter(Collider coll){
		if (coll.gameObject.tag == "screen") {
            GetComponent<MeshRenderer>().enabled = true;
            if(has_outline)
            {
                outline_sprend.enabled = true;
            }
			if (padlock_sprend)
				padlock_sprend.enabled = true;
            if (obj)
				obj.SetActive(true);
        }
    }
	void OnTriggerExit(Collider coll){
		if (coll.gameObject.tag == "screen") {
			GetComponent<MeshRenderer> ().enabled = false;
            if (has_outline)
            {
               outline_sprend.enabled = false;
            }
			if (padlock_sprend)
				padlock_sprend.enabled = false;
            if (obj)
				obj.SetActive(false);
        }
	}
}
