using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Behavior : MonoBehaviour {
    GameObject Creator;   

    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    GameObject enemy;//храним, кто преследует кролика
    public int priority;//приоритет для разрешения столкновения нескольких объектов
    //public bool live = true;

    public enum State//перечисление состояний
    {
        wait,
        walk,
        run,
        dead        
    }
    public State state;//переменная для состояния
    
    void Start () {
        Creator = GameObject.Find("MainController");
        nav = GetComponent<NavMeshAgent>();//берем компоненты с того же объекта где и скрипт
        anim = GetComponent<Animator>();
        priority = Creator.GetComponent<Creator>().Priority(gameObject);//определяем приоритет ai по тэгу через создателя
        state = State.wait;//ставим в начале в состояние ожидания           
        StartCoroutine(Wait());//запускаем корутину(процесс) ожидания в 10 сек
        
    }


    void Update()
    {
        if (state != State.dead)
        {

            //Debug.Log(nav.remainingDistance);
            if (state == State.walk && nav.remainingDistance < 1)//если объект дошел до нужной точки и находится в состоянии ходьбы
            {
                state = State.wait;//переключаем в состояние ожидания
                anim.SetBool("Walk", false);//выключаем анимацию ходьбы
                Debug.Log("Rabbit at point");
                StartCoroutine(Wait());//запускаем корутину ожидания

            }
            if (state == State.run)//если убегаем
            {
                Vector3 forwardPosition = transform.TransformPoint(Vector3.forward);//переводим в глобальные координаты направление вперед
                nav.SetDestination(forwardPosition*3);        //назначаем агенту новое направление  
                if (enemy != null)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    //Debug.Log("distance: " + distance);
                    if (distance > 30)
                    {
                        enemy.GetComponent<PlayerMove>().associated = null;
                        enemy = null;
                        anim.SetBool("Run", false);//выключаем бег
                        anim.SetBool("Walk", false);
                        nav.speed = 0;//выключаем перемещание
                        state = State.wait;//ставим в ожидание
                        StartCoroutine(Wait());//запускаем корутину ожидания
                    }
                }
            }
        }
        
    }
        public void die()
    {
        anim.SetTrigger("Death");
        nav.enabled = false;
        enemy.GetComponent<PlayerMove>().associated = null;
        enemy = null;
        state = State.dead;
        priority = 3;
        Creator.GetComponent<Creator>().SomebodyDead(gameObject);
    }
       
	
    private void OnTriggerEnter(Collider other)//для обработки взаимодействий (рядом игрок)
    {
        if (state != State.dead)
        {
            if (other.tag == "Player")//проверяем, что увидели именно игрока (зашли в его триггер)
            {
               // Debug.Log("Rabbit in trigger");
                enemy = other.gameObject;
                enemy.GetComponent<PlayerMove>().associated = gameObject;
                StopAllCoroutines();//останавливаем корутины (т.к. есть возможность входа в триггер во время ожидания)
                anim.SetBool("Run", true);//переключаем анимацию в бег
                anim.SetBool("Walk", false);//выключаем ходьбу
                state = State.run;//указываем состояние бега
                transform.LookAt(other.transform);//разворачиваем сначала к игроку
                                                  //а затем на 180, чтобы развернуть в другую сторону
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 180f, transform.rotation.eulerAngles.z);
                nav.speed = Random.Range(5, 8);//включаем высокую скорость

            }
        }
    }
   

    IEnumerator Wait()//корутина ожидания
    {
       // Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(10f);//ждем 10 секунд
        //Debug.Log("Go at new point");
        nav.speed = 1;//включаем низкую скорость для ходьбы
        state = State.walk;//включаем состояние ходьбы
        nav.SetDestination(new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)));//задаем новую точку для движения в пределах плоскости
        anim.SetBool("Walk", true);//включаем анимацию ходьбы        
        
    }
}
