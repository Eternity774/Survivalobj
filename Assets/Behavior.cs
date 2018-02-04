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
        StartCoroutine(Wait());//запускаем корутину(процесс) ожидания в 10 сек
        
    }


    void FixedUpdate()
    {
        if (state != State.dead)
        {

            //Debug.Log(nav.remainingDistance);
            if (state == State.walk && Vector3.Distance(nav.destination, gameObject.transform.position)<1)//если объект дошел до нужной точки и находится в состоянии ходьбы
            {
                //nav.remainingDistance < 1
                if (gameObject.tag == "Wolf")
                {
                    Debug.Log("walk at point");
                    
                }
                StartCoroutine(Wait());//запускаем корутину ожидания
            }

            if (state == State.runfrom)//если убегаем
            {
                //Debug.Log(gameObject.name + "убегает от" + enemy.gameObject.name);
                if (enemy != null)

                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance > 30)
                    {
                        enemy = null;
                        StartCoroutine(Wait());//запускаем корутину ожидания
                    }
                    else
                    {
                        Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 2);//переводим в глобальные координаты направление вперед
                        nav.SetDestination(forwardPosition);        //назначаем агенту новое направление  
                    }
                }
                else StartCoroutine(Wait());

            }
            if(state == State.runfor)
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
                        enemy.GetComponent<Behavior>().enemy = null;
                        enemy = null;
                    }

                }
                else
                {
                    Debug.Log("send detach with courutine");
                    StartCoroutine(Wait());
                }
                               
            }
            if(state == State.attack)
            {
               // nav.ResetPath();
                transform.LookAt(enemy.transform);
                enemy.GetComponent<Behavior>().die(gameObject);
                anim.SetBool("Attack", false);
                Debug.Log("start coroutine after attack");
                enemy = null;
                StartCoroutine(Wait());
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
            StartCoroutine(Death());
        }
    }
       public void GetEnemy(GameObject newenemy)
    {
        if (state != State.dead && anim!=null)
        {
            int newenemypriority;//приоритет нового врага

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
                bool addenemy = false;        
                            
                // Debug.Log(gameObject.name + "with priority:" + priority + " with:" + newenemypriority);
                if (priority < newenemypriority && newenemypriority > 3)
                {
                                      
                    state = State.runfrom;//указываем состояние бега                                          
                    transform.rotation = Quaternion.Euler(newenemy.transform.rotation.eulerAngles.x, newenemy.transform.rotation.eulerAngles.y, newenemy.transform.rotation.eulerAngles.z);
                    addenemy = true;
                }
                else if(priority>3)
                {
                    state = State.runfor;//указываем состояние бега
                    transform.LookAt(newenemy.transform.position);//разворачиваем сначала к игроку                                                                 
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
       if(gameObject.tag=="Wolf") Debug.Log("Go at new point");
        nav.speed = 1;//включаем низкую скорость для ходьбы
        try
        {
            nav.SetDestination(new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)));//задаем новую точку для движения в пределах плоскости
        }
        catch { Debug.Log("Exeception!!!: " + gameObject.name + state); }
            if (gameObject.tag == "Wolf") Debug.Log("Where go: "+nav.destination+" Where I: "+gameObject.transform.position + "distance: "+nav.remainingDistance);
        anim.SetBool("Walk", true);//включаем анимацию ходьбы        
        state = State.walk;//включаем состояние ходьбы
        //Debug.Log("wait for " + gameObject+gameObject.GetComponent<Behavior>().state);
              
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(10f);//ждем 10 секунд
        Destroy(gameObject);
    }
}
