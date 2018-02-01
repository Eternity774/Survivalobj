using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    
	public Transform rotate;
    public GameObject associated;
	public int priority = 6;
	private Vector3 direction;
	float speedx = 3f;//скорость поворота 
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

		direction = new Vector3 (x, 0, z);
		direction = Camera.main.transform.TransformDirection (direction);
		direction = new Vector3 (direction.x, 0, direction.z);

		if (Mathf.Abs (z) > 0 || Mathf.Abs (x) > 0) {
			rotate.rotation = Quaternion.Lerp (rotate.rotation, Quaternion.LookRotation (direction), 10 * Time.deltaTime);
		}

//        if (x != 0)
//        {
//            transform.Rotate(0f, x * speedx, 0f);//вращаем персонажа
//        }

        if (z != 0)
        {
            Vector3 dir;
            if (Input.GetKey(KeyCode.W))
            {
                dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedz));
                animator.SetBool("Walk", true);//включаем анимацию ходьбы
            }
            else
            {
                dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedz / 2));
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
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("Attack");  
            if(associated!=null)
            {
                if (Vector3.Distance(transform.position, associated.transform.position) < 2) associated.GetComponent<Behavior>().die();

            }          
        }
    }
}
