using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Behavior : MonoBehaviour {
    GameObject Creator;   

    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    public GameObject enemy;//храним, кто преследует кролика
    public int priority;//приоритет для разрешения столкновения нескольких объектов
    //public bool live = true;

    public enum State//перечисление состояний
    {
        wait,
        walk,
        runfrom,
        runfor,
        attack,
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
               // Debug.Log("Rabbit at point");
                StartCoroutine(Wait());//запускаем корутину ожидания

            }
            if (state == State.runfrom)//если убегаем
            {
                Debug.Log(gameObject.name + "убегает от" + enemy.gameObject.name);
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance > 30)
                {
                    anim.SetBool("Run", false);//выключаем бег
                    anim.SetBool("Walk", false);
                    nav.speed = 0;//выключаем перемещание
                    state = State.wait;//ставим в ожидание
                    if (enemy.tag == "Player")
                    {
                        enemy.GetComponent<PlayerMove>().associated = null;
                    }
                    else
                    {
                        enemy.GetComponent<Behavior>().enemy = null;
                        enemy.GetComponent<Behavior>().state = State.wait;
                    }
                    enemy = null;
                    StartCoroutine(Wait());//запускаем корутину ожидания
                }
                else
                {
                    Vector3 forwardPosition = transform.TransformPoint(Vector3.forward*2);//переводим в глобальные координаты направление вперед
                    nav.SetDestination(forwardPosition);        //назначаем агенту новое направление  
                }



            }
            if(state == State.runfor)
            {
                if (enemy != null)
                {
                    nav.SetDestination(enemy.transform.position);
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);                    
                    if (distance < 2)
                    {

                        state = State.attack;
                        anim.SetBool("Attack", true);
                        anim.SetBool("Run", false);
                    }                    
                    
                }
                else
                {
                    state = State.wait;
                    anim.SetBool("Run", false);
                    nav.SetDestination(transform.localPosition);
                    StartCoroutine(Wait());
                }
                
            }
            if(state == State.attack)
            {
                transform.LookAt(enemy.transform);
                enemy.GetComponent<Behavior>().die(gameObject);
                anim.SetBool("Attack", false);
                state = State.wait;
                StartCoroutine(Wait());
            }
        }
        
    }
        public void die(GameObject killer)
    {
        anim.SetTrigger("Death");
        nav.enabled = false;
        if (killer.tag == "Player") killer.GetComponent<PlayerMove>().associated = null;
        else killer.GetComponent<Behavior>().enemy = null;
        if (enemy != null)
        {
            if (enemy.tag == "Player") enemy.GetComponent<PlayerMove>().associated = null;//на случай, если гнались двое
            else enemy.GetComponent<Behavior>().enemy = null;
        }
        enemy = null;
        state = State.dead;
        priority = 3;
        Creator.GetComponent<Creator>().SomebodyDead(gameObject);
    }
       public void GetEnemy(GameObject newenemy)
    {
        if (state != State.dead)
        {
            int newenemypriority;
            if (newenemy.tag == "Player") newenemypriority = newenemy.GetComponent<PlayerMove>().priority;
            else newenemypriority = newenemy.GetComponent<Behavior>().priority;
            bool changenemy = false;
            if (enemy != null)//определяем, будем ли менять врага, если уже есть взаимодействие
            {
                int oldpriority;
                if (enemy.tag == "Player") oldpriority = enemy.GetComponent<PlayerMove>().priority;
                else oldpriority = enemy.GetComponent<Behavior>().priority;
                if (newenemypriority > oldpriority) changenemy = true;
            }

            if (enemy == null || changenemy)
            {
                enemy = newenemy.gameObject;
                if (enemy.tag == "Player") enemy.GetComponent<PlayerMove>().associated = gameObject;
                else enemy.GetComponent<Behavior>().GetEnemy(gameObject);
                StopAllCoroutines();//останавливаем корутины (т.к. есть возможность входа в триггер во время ожидания)
                anim.SetBool("Run", true);//переключаем анимацию в бег
                anim.SetBool("Walk", false);//выключаем ходьбу


                Debug.Log(gameObject.name + "with priority:" + priority + " with:" + newenemypriority);
                if (priority < newenemypriority && newenemypriority > 3)
                {
                    state = State.runfrom;//указываем состояние бега
                                          //transform.LookAt(newenemy.transform);//разворачиваем сначала к игроку
                                          //а затем на 180, чтобы развернуть в другую сторону
                    transform.rotation = Quaternion.Euler(enemy.transform.rotation.eulerAngles.x, enemy.transform.rotation.eulerAngles.y, enemy.transform.rotation.eulerAngles.z);
                    nav.speed = Random.Range(5, 8);//включаем высокую скорость
                }
                else if(priority>3)
                {
                    state = State.runfor;//указываем состояние бега
                    transform.LookAt(newenemy.transform.position);//разворачиваем сначала к игроку
                                                                  //а затем на 180, чтобы развернуть в другую сторону
                                                                  // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 180f, transform.rotation.eulerAngles.z);
                    nav.speed = Random.Range(5, 8);//включаем высокую скорость
                    anim.SetBool("Run", true);
                }
            }
        }
    }
	
    private void OnTriggerEnter(Collider other)//для обработки взаимодействий (рядом игрок)
    {
        Debug.Log(gameObject.name+"go into"+other.gameObject.name);
        if (state != State.dead)
        {
            
            if (other.GetComponent<Behavior>() != null|| other.GetComponent<PlayerMove>() != null)
            {
                GetEnemy(other.gameObject);                
            }
        }
    }
   

    IEnumerator Wait()//корутина ожидания
    {
       // Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(10f);//ждем 10 секунд
       // Debug.Log("Go at new point");
        nav.speed = 1;//включаем низкую скорость для ходьбы
        state = State.walk;//включаем состояние ходьбы
        nav.SetDestination(new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)));//задаем новую точку для движения в пределах плоскости
        anim.SetBool("Walk", true);//включаем анимацию ходьбы        
        
    }
}
