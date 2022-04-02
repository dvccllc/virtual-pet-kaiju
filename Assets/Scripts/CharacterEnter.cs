using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEnter : MonoBehaviour {

	bool enter_x, enter_y;
	public float speed;
	public float destination_x, destination_y;
    float magnitude = 0.1f;
	// Update is called once per frame
	void Update () {
		if (enter_x) {
			
			float x_dif = transform.localPosition.x - destination_x;

			if (x_dif > 0.1f) {
				//coming in from the right
				transform.localPosition = new Vector3 (transform.localPosition.x - Time.deltaTime * speed, transform.localPosition.y, transform.localPosition.z);
			}

			if (x_dif < -0.1f) {
				//coming in from the left
				transform.localPosition = new Vector3 (transform.localPosition.x + Time.deltaTime * speed, transform.localPosition.y, transform.localPosition.z);
			}

			if (x_dif > -1 * magnitude && x_dif < magnitude) {
				enter_x = false;
			}


		}
		if (enter_y) {


			float y_dif = transform.localPosition.y - destination_y;

			if (y_dif > magnitude) {
				//coming in from the top
				transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y - Time.deltaTime * speed, transform.localPosition.z);
			}

			if (y_dif < -1 * magnitude) {
				//coming in from the bottom
				transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y + Time.deltaTime * speed, transform.localPosition.z);
			}
            if (y_dif > -1 * magnitude && y_dif < magnitude)
            {
                enter_y = false;
			}

		}
	}
	public void StartEntrance(){
		enter_x = true;
		enter_y = true;
	}
	void Enter(){

	}

}
