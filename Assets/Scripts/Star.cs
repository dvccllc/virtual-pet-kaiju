using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    Vector3 zero_vector = new Vector3(0, 0, 0);
    public bool on_plate = false;
    public bool is_bomb = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Creature" || coll.gameObject.tag == "star" && on_plate == false)
        {
            gameObject.transform.parent = coll.gameObject.transform;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().velocity = zero_vector;
            on_plate = true;
            ContactPlate(gameObject.transform, !is_bomb);
        }
    }
    public void ContactPlate(Transform pos, bool not_bomb)
    {
        if (gameObject.transform.parent.tag == "star")
        {
            gameObject.transform.parent.GetComponent<Star>().ContactPlate(pos, not_bomb);
        }
        else if (gameObject.transform.parent.tag == "Creature")
        {
            gameObject.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<DefenseMiniGame2>().NewContact(pos, not_bomb);
        }
    }
}
