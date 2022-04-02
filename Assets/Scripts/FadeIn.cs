using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour {
    public bool character;
    public Vector3 centered;
    public GameObject away;
	public bool enter_x, enter_y, exit_x, exit_y;
	public bool[] moves_x;
	public bool[] moves_y;
	public Vector2[] moves_dest;
	public float speed;
	public float destination_x, destination_y;
	float origin_x, origin_y;
    float magnitude = 0.15f;
    public bool[] moving;
	public bool ui;
	public bool monster;
	public bool exit_sub;
	public float sub_exit_x;
	public float sub_exit_y;
	float ui_speed;
	public bool fast_ui;
	bool exiting = false;
	float monster_speed;

	// Use this for initialization
	void Start () {
		if(fast_ui) ui_speed = 2f;
		else ui_speed = 1f;
		if(monster) monster_speed = 1.3f;
		else monster_speed = 1f;
		origin_x = transform.localPosition.x;
		origin_y = transform.localPosition.y;
        moving = new bool[moves_dest.Length];
	}
	void MoveTo (ref bool x_bool, ref bool y_bool, float x_to, float y_to, int id){
		if (x_bool) {
			float x_dif = transform.localPosition.x - x_to;
			//coming in from the right
			if (x_dif > magnitude) transform.localPosition = new Vector3 (transform.localPosition.x - Time.deltaTime * speed * ui_speed * monster_speed, transform.localPosition.y, transform.localPosition.z);
			//coming in from the left
			if (x_dif < magnitude * -1f) transform.localPosition = new Vector3 (transform.localPosition.x + Time.deltaTime * speed * ui_speed * monster_speed, transform.localPosition.y, transform.localPosition.z);

			if (x_dif > magnitude * -1f && x_dif < magnitude) x_bool = false;
			if(!y_bool && !x_bool && id == -1 && exiting) {
				Destroy(gameObject);
			}
		}
		if (y_bool) {
			float y_dif = transform.localPosition.y - y_to;
			//coming in from the top
			if (y_dif > magnitude) transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y - Time.deltaTime * speed * ui_speed * monster_speed, transform.localPosition.z);

			//coming in from the bottom
			if (y_dif < magnitude * -1f) transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y + Time.deltaTime * speed * ui_speed * monster_speed, transform.localPosition.z);

			if (y_dif > magnitude * -1f && y_dif < magnitude) y_bool = false;
			if(!y_bool && !x_bool && id == -1 && exiting) {
				Destroy(gameObject);
			}
		}
        if(!y_bool && !x_bool && id != -1 && moving[id]) moving[id] = false;
	}
	void ExitSubMenus(){
		if(exit_sub) {

			bool move_y_ = transform.localPosition.y > sub_exit_y;
			bool move_x_ = transform.localPosition.x < sub_exit_x;

			if (move_y_ && transform.localPosition.y < sub_exit_y + Time.deltaTime * speed * ui_speed * monster_speed * 1.5f) {
				transform.localPosition = new Vector3 (transform.localPosition.x, sub_exit_y, transform.localPosition.z);
				move_y_ = false;
			}
			else if (move_x_ && transform.localPosition.x > sub_exit_x - Time.deltaTime * speed * ui_speed * monster_speed * 1.5f) {
				transform.localPosition = new Vector3 (sub_exit_x, transform.localPosition.y, transform.localPosition.z);
				move_x_ = false;
			}

			if (move_y_) transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y - Time.deltaTime * speed * ui_speed * monster_speed, transform.localPosition.z);
			else if (move_x_) transform.localPosition = new Vector3 (transform.localPosition.x + Time.deltaTime * speed * ui_speed * monster_speed, transform.localPosition.y, transform.localPosition.z);
			else {
				exit_sub = false;
				transform.localPosition = new Vector3 (origin_x, origin_y, transform.localPosition.z);
			}
		}
	}
	// Update is called once per frame
	void Update () {
		MoveTo (ref enter_x, ref enter_y, destination_x, destination_y, -1);
		MoveTo (ref exit_x, ref exit_y, origin_x, origin_y, -1);
		for (int i = 0; i < moves_x.Length; i++) {
			MoveTo (ref moves_x[i], ref moves_y[i], moves_dest[i].x, moves_dest[i].y, i);
		}

	}
	public void StartEntrance(){
		enter_x = true;
		enter_y = true;
		for (int i = 0; i < moves_x.Length; i++) {
			moves_x[i] = false;
			moves_y [i] = false;
		}
		exit_x = false;
		exit_y = false;
	}
	public void StartExit(){
		enter_x = false;
		enter_y = false;
		for (int i = 0; i < moves_x.Length; i++) {
			moves_x[i] = false;
			moves_y [i] = false;
		}
		exit_x = true;
		exit_y = true;
		if(ui) exit_x = false;
	}
	public void StartExitInterface(){
		enter_x = false;
		enter_y = false;
		for (int i = 0; i < moves_x.Length; i++) {
			moves_x[i] = false;
			moves_y [i] = false;
		}
		exit_x = true;
		exit_y = true;
		exiting = true;
		if(ui) exit_x = false;
	}
	public void StartMove(int num){
		enter_x = false;
		enter_y = false;
		exit_x = false;
		exit_y = false;
        moving[num] = true;
		for (int i = 0; i < moves_x.Length; i++) {
			if (i != num) {
				moves_x [i] = false;
				moves_y [i] = false;
			}
		}
		moves_x[num] = true;
		moves_y [num] = true;
	}
	void Enter(){

	}

}
