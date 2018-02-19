using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    
	//public Transform rotate;
    public GameObject associated;
	public int priority = 6;
    public int hp = 800;
	float speedx = 3f;//скорость поворота 
	float speedz = 0.05f;//скорость ходьбы
	Animator animator;//анимаотор
	CharacterController controller; //контроллер для ходьбы


	// Use this for initialization
	void Start () {
    animator = GetComponentInChildren<Animator>();
    controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (hp > 0) {
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
                    dir = transform.TransformDirection(new Vector3(0f, -3f, z * speedz));
                    animator.SetBool("Walk", true);//включаем анимацию ходьбы
                }
                else
                {
                    dir = transform.TransformDirection(new Vector3(0f, -3f, z * speedz / 2));
                    animator.SetBool("WalkBack", true);//включаем анимацию ходьбы
                }
                // dir = transform.TransformDirection(new Vector3(0f, -3f, z * speedz));//высчитываем смещение вперед
                controller.Move(dir);//двигаем контроллер

            }
            else//если не нажата клавиша ходьбы
            {
                animator.SetBool("Walk", false);//выключаем ходьбу
                animator.SetBool("WalkBack", false);//выключаем ходьбу
                animator.SetBool("Run", false);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speedz = 0.2f;
                animator.SetBool("Run", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                
                speedz = 0.1f;
                animator.SetBool("Run", false);

            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                animator.SetTrigger("Attack");
                if (associated != null)
                {
                    if (Vector3.Distance(transform.position, associated.transform.position) < 3) associated.GetComponent<Behavior>().TakeDamage(gameObject, Random.Range(50, 100));

                }
            }
            if (associated != null)
            {
                if (Vector3.Distance(gameObject.transform.position, associated.transform.position) > 30)
                {
                    associated.GetComponent<Behavior>().enemy = null;
                    associated = null;
                }
            }
        }
    }
    public void TakeDamage(int enemydamage)
    {
        if (hp > 0)
        {
            hp -= enemydamage;
            Debug.Log("You take damage and have " + hp + " hp");
            if (hp <= 0) StartCoroutine(Dead());
        }
    }
    IEnumerator Dead()
    {
        Debug.Log("start death");
        animator.SetTrigger("Death");
        associated = null;
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }
   
}
