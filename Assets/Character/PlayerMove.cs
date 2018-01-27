using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    float speedx = 1f;
    float speedz = 0.1f;
    Animator animator;
    CharacterController controller;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0)
        {
            transform.Rotate(0f, x * speedx, 0f);
        }

        if (z != 0)
        {
            Vector3 dir;
            //if (Input.GetKey(KeyCode.W)) dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedmove));
            // else dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedmove / 2));
            dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedz));
            controller.Move(dir);
            animator.SetBool("Walk", true);
        }
        else
        {
           animator.SetBool("Walk", false);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speedx = 2;
            animator.SetBool("Run", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedx = 1;
            animator.SetBool("Run", false);
        }
    }
}
