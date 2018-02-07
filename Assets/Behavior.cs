using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Behavior : MonoBehaviour {
    GameObject CreatorRef;   

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
        eat,
        dead        
    }
    public State state;//переменная для состояния
    
    void Start () {
        CreatorRef = GameObject.Find("MainController");
        
        nav = GetComponent<NavMeshAgent>();//берем компоненты с того же объекта где и скрипт
        anim = GetComponent<Animator>();
        priority = CreatorRef.GetComponent<Creator>().Priority(gameObject);//определяем приоритет ai по тэгу через создателя
        
        StartCoroutine(Wait());//запускаем корутину(процесс) ожидания в 10 сек        
    }
    
    void Update()
    {
        if (state != State.dead)
        {
            if (state == State.walk && Vector3.Distance(nav.destination, gameObject.transform.position)<1)//если объект дошел до нужной точки и находится в состоянии ходьбы
            {
                StartCoroutine(Wait());//запускаем корутину ожидания
            }

            else if (state == State.runfrom)//если убегаем
            {
               if (enemy != null)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance > 40)
                    {
                        enemy = null;
                        StartCoroutine(Wait());//запускаем корутину ожидания
                    }
                    else
                    {
                        Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 3);//переводим в глобальные координаты направление вперед
                        nav.SetDestination(forwardPosition);        //назначаем агенту новое направление  
                    }
                }
                else StartCoroutine(Wait());

            }
            else if(state == State.runfor)
            {
                if (enemy != null)
                {
                    nav.SetDestination(enemy.transform.position);
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    //float distance = nav.remainingDistance;
                    if (distance < 2)
                    {
                        state = State.attack;
                        anim.SetBool("Attack", true);
                        anim.SetBool("Run", false);
                    }
                    else if(distance>30)
                    {
                        enemy = null;
                    }

                }
                else
                {
                   // Debug.Log("send detach with courutine");
                    StartCoroutine(Wait());
                }
                               
            }
            else if(state == State.attack)
            {
                // nav.ResetPath();
                // anim.SetBool("Attack", true);
               
                transform.LookAt(enemy.transform);
                if (enemy.tag == "Player")
                {
                    Debug.Log("You die");
                    enemy.GetComponent<PlayerMove>().Die();
                }
                else enemy.GetComponent<Behavior>().die(gameObject);
                state = State.eat;
                //anim.SetBool("Attack", false);
                //Debug.Log("start coroutine after attack");
                // enemy = null;
                // StartCoroutine(Wait());
                if (Vector3.Distance(transform.position, enemy.transform.position) > 2)
                {
                    anim.SetBool("Attack", false);
                    anim.SetBool("Run", true);
                    nav.SetDestination(enemy.transform.position);
                }
                else
                {
                    anim.SetBool("Attack", true);
                    anim.SetBool("Run", false);                
                    StartCoroutine(Eating());
                }
            }
            else if(state == State.eat)
            {
                Vector3 forwardenemyPosition = enemy.transform.TransformPoint(Vector3.forward*2);//переводим в глобальные координаты направление вперед
                nav.SetDestination(forwardenemyPosition);
                transform.LookAt(enemy.transform);
            }
           
        }
        
    }
        public void die(GameObject killer)
    {
        if (state != State.dead)
        {
            nav.ResetPath();
            nav.enabled = false;
            anim.SetTrigger("Death");
            enemy = null;
            state = State.dead;
            priority = 3;
            CreatorRef.GetComponent<Creator>().SomebodyDead(gameObject);
            StopAllCoroutines();
            //StartCoroutine(Death());
        }
    }
       public void GetEnemy(GameObject newenemy)
    {
        if (state != State.dead && anim!=null)
        {
            int newenemypriority;//приоритет нового врага
            ///Debug.Log()
            if (newenemy.tag == "Player") newenemypriority = 6;
            else newenemypriority = newenemy.GetComponent<Behavior>().priority;

            bool changenemy = false;
            if (enemy != null)//определяем, будем ли менять врага, если уже есть взаимодействие
            {
                int oldpriority;
                if (enemy.tag == "Player") oldpriority = 6;
                else oldpriority = enemy.GetComponent<Behavior>().priority;
                if (newenemypriority > oldpriority) changenemy = true;
            }

            if (enemy == null || changenemy)
            {
                bool addenemy = false;        //добавить ли врага после проверки условий

                // Debug.Log(gameObject.name + "with priority:" + priority + " with:" + newenemypriority);
                //вероятность нападения
                if (CreatorRef.GetComponent<Creator>().Response(priority, newenemypriority))
                {
                    state = State.runfor;//указываем состояние бега
                    transform.LookAt(newenemy.transform.position);//разворачиваем сначала к игроку                                                                 
                    addenemy = true;
                }
                else if(newenemypriority>4)
                {
                    if (state == State.eat)
                    {
                        StopAllCoroutines();
                        anim.SetBool("Attack", false);
                    }
                    state = State.runfrom;//указываем состояние бега    

                    transform.rotation = Quaternion.Euler(newenemy.transform.rotation.eulerAngles.x, newenemy.transform.rotation.eulerAngles.y, newenemy.transform.rotation.eulerAngles.z);
                    addenemy = true;
                }
                
                if(addenemy)
                {
                    enemy = newenemy.gameObject;

                    if (enemy.tag == "Player") enemy.GetComponent<PlayerMove>().associated = gameObject;
                    else enemy.GetComponent<Behavior>().GetEnemy(gameObject);

                    StopAllCoroutines();//останавливаем корутины (т.к. есть возможность входа в триггер во время ожидания)
                    nav.speed = Random.Range(5, 8);//включаем высокую скорость
                    anim.SetBool("Walk", false);//выключаем ходьбу
                    anim.SetBool("Run", true);//переключаем анимацию в бег
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)//для обработки взаимодействий (рядом игрок)
    {
        //Debug.Log(gameObject.name+"go into"+other.gameObject.name);
        if (state != State.dead && anim!=null)
        {

            if (other.GetComponent<Behavior>() != null || other.GetComponent<PlayerMove>() != null)
            {
                GetEnemy(other.gameObject);
            }
        }
    }
   

    IEnumerator Wait()//корутина ожидания
    {
        // Debug.Log("Start Coroutine");
        //nav.ResetPath();        
        nav.SetDestination(gameObject.transform.position);//задаем новую точку для движения в пределах плоскости
        anim.SetBool("Run", false);//выключаем бег
        anim.SetBool("Walk", false);//выключаем анимацию ходьбы
        
        state = State.wait;
        yield return new WaitForSeconds(10f);//ждем 10 секунд
       
            nav.speed = 1;       //включаем низкую скорость для ходьбы
      
       nav.SetDestination(CreatorRef.GetComponent<Creator>().FindPoint());//задаем новую точку для движения в пределах плоскости
       //Debug.Log("Exeception!!!: " + gameObject.name + state);
       
        anim.SetBool("Walk", true);//включаем анимацию ходьбы        
        state = State.walk;//включаем состояние ходьбы
        //Debug.Log("wait for " + gameObject+gameObject.GetComponent<Behavior>().state);
              
    }
    IEnumerator Eating()
    {
        yield return new WaitForSeconds(10f);//поедаем 10 секунд
        if (enemy != null)
        {
            if (enemy.GetComponent<Behavior>().state == State.dead)
            {
                Destroy(enemy);
                enemy = null;
            }
        }
        anim.SetBool("Attack", false);
        StartCoroutine(Wait());
    }
    /*
    IEnumerator Death()
    {
        yield return new WaitForSeconds(10f);//ждем 10 секунд
        Destroy(gameObject);
    }
    */
}
