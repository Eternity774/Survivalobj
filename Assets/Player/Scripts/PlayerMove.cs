using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    
	public Transform rotate;
    public GameObject associated;
	public int priority = 6;
	float speedx = 4f;//скорость поворота 
	float speedz = 0.1f;//скорость ходьбы
	Animator animator;//анимаотор
	CharacterController controller; //контроллер для ходьбы

	// Use this for initialization
	void Start () {
    animator = GetComponentInChildren<Animator>();
    controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        

        float x = Input.GetAxis("Horizontal");//перемещение курсора по горизонтали
        float z = Input.GetAxis("Vertical");//перемещение курсора по вертикали




        if (x != 0)
        {
            transform.Rotate(0f, x * speedx, 0f);//вращаем персонажа
        }

        if (z != 0)
        {
            Vector3 dir;
            if (Input.GetKey(KeyCode.W))
            {
				speedz = 0.1f;
                dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedz));
                animator.SetBool("Walk", true);//включаем анимацию ходьбы

            }
			else if (Input.GetKey(KeyCode.S))
            {
				speedz = 0.05f;
                dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedz ));
                animator.SetBool("WalkBack", true);//включаем анимацию ходьбы
			
            }
            dir = transform.TransformDirection(new Vector3(0f, -3f, z * speedz));//высчитываем смещение вперед
            controller.Move(dir);//двигаем контроллер
            
        }
        else//если не нажата клавиша ходьбы
        {
           animator.SetBool("Walk", false);//выключаем ходьбу
           animator.SetBool("WalkBack", false);//выключаем ходьбу
        }

		if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            speedx = 3;
			speedz = 0.4f;
            animator.SetBool("Run", true);		

        }

		if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedx = 4;
			speedz = 0.1f;
            animator.SetBool("Run", false);

		}
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("Attack");  
            if(associated!=null)
            {
                if (Vector3.Distance(transform.position, associated.transform.position) < 2) associated.GetComponent<Behavior>().die();

            }          
        }
		Debug.Log(speedz);
    }
}
