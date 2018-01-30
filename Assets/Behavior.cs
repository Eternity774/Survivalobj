using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Behavior : MonoBehaviour {
       
    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    //GameObject enemy;

    public enum State//перечисление состояний
    {
        wait,
        walk,
        run
    }
    public State state;//переменная для состояния
    
    void Start () {
        nav = GetComponent<NavMeshAgent>();//берем компоненты с того же объекта где и скрипт
        anim = GetComponent<Animator>();
        state = State.wait;//ставим в начале в состояние ожидания
        StartCoroutine(Wait());//запускаем корутину(процесс) ожидания в 10 сек
	}
	
	
	void Update ()
    {
        //Debug.Log(nav.remainingDistance);
        if(nav.remainingDistance<1&&state==State.walk)//если объект дошел до нужной точки и находится в состоянии ходьбы
        {
            state = State.wait;//переключаем в состояние ожидания
            anim.SetBool("Walk", false);//выключаем анимацию ходьбы
            StartCoroutine(Wait());//запускаем корутину ожидания
        }
        if(state==State.run)//если убегаем
        {
            Vector3 forwardPosition = transform.TransformPoint(Vector3.forward);//переводим в глобальные координаты направление вперед
            nav.SetDestination(forwardPosition);        //назначаем агенту новое направление    
        }
       
	}
    private void OnTriggerEnter(Collider other)//для обработки взаимодействий (рядом игрок)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Rabbit in trigger");
            anim.SetBool("Run", true);//переключаем анимацию в бег
            anim.SetBool("Walk", false);//выключаем ходьбу
            state = State.run;//указываем состояние бега
            transform.LookAt(other.transform);//разворачиваем сначала к игроку
            //а затем на 180, чтобы развернуть в другую сторону
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 180f, transform.rotation.eulerAngles.z);
            nav.speed = 2;//выключаем скорость навмеша (будем двигать без его помощи)
            
        }
    }
    private void OnTriggerExit(Collider other)//когда игрок далеко
    {
        
        if (other.tag == "Player")
        {
            Debug.Log("Rabbit out of trigger");
            anim.SetBool("Run", false);//выключаем бег
            anim.SetBool("Walk", false);
            nav.speed = 0;//выключаем перемещание
            state = State.wait;//ставим в ожидание
            StartCoroutine(Wait());//запускаем корутину ожидания
        }
        
    }

    IEnumerator Wait()//корутина ожидания
    {
        Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(10f);//ждем 10 секунд
        if (state == State.wait)
        {
            Debug.Log("Go at new point");
            nav.speed = 1;
            state = State.walk;//включаем состояние ходьбы
            nav.SetDestination(new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)));//задаем новую точку для движения в пределах плоскости
            anim.SetBool("Walk", true);//включаем анимацию ходьбы
        }
    }
}
