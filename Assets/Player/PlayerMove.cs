using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    float speedx = 3f;//скорость поворота 
    float speedz = 0.1f;//скорость ходьбы
    Animator animator;//анимаотор
    CharacterController controller;//контроллер для ходьбы
    
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
            //if (Input.GetKey(KeyCode.W)) dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedmove));
            // else dir = transform.TransformDirection(new Vector3(0f, 0f, z * speedmove / 2));
            dir = transform.TransformDirection(new Vector3(0f, -3f, z * speedz));//высчитываем смещение вперед
            controller.Move(dir);//двигаем контроллер
            animator.SetBool("Walk", true);//включаем анимацию ходьбы
        }
        else//если не нажата клавиша ходьбы
        {
           animator.SetBool("Walk", false);//выключаем ходьбу
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
        
       if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) animator.SetBool("Attack", false); //проверяем какое сейчас состояние в аниматоре
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetBool("Attack", true);            
        }
    }
}
