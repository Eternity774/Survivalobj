using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Behavior : MonoBehaviour {
    
    GameObject Creator;//ссылка на создателя
    Creator CreatorRef;//ссылка на компонент скрипта создателя
    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    public GameObject enemy;//храним, кто преследует кролика
    public GameObject enemyinmemory;//для случая, когда основной враг нейтрализуется, но еще один, который был ранее проигнорирован, станет основным
    public int priority;//приоритет для разрешения столкновения нескольких объектов
    public int hp;//здоровье
    public int damage;//наносимый урон

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
        Creator = GameObject.Find("MainController");
        CreatorRef = Creator.GetComponent<Creator>();
        nav = GetComponent<NavMeshAgent>();//берем компоненты с того же объекта где и скрипт
        anim = GetComponent<Animator>();
        int[] infomas = CreatorRef.StartInformation(gameObject);
        priority = infomas[0];//определяем приоритет ai по тэгу через создателя
        hp = infomas[1];//определяем кол-во здоровья
        damage = infomas[2];//опрделеяем наносимый урон 
        StartCoroutine(Wait());//запускаем корутину(процесс) ожидания в 10 сек        
    }
    
    void Update()
    {
        if (state != State.dead)//если не умер
        {
            if (state == State.walk && Vector3.Distance(nav.destination, gameObject.transform.position)<1)//если объект дошел до нужной точки и находится в состоянии ходьбы
            {
                StartCoroutine(Wait());//запускаем корутину ожидания
            }

            else if (state == State.runfrom)//если убегаем
            {
                if (enemy != null)//если враг не отсоединился
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);//проверяем дистанцию до врага
                    if (distance > 40)//если дистанция больше 40
                    {
                        enemy = null;//отсоединяем врага
                       // if (enemyinmemory != null) EnemyInMemory(enemyinmemory);//есть ли враг в памяти
                       // else StartCoroutine(Wait());//запускаем корутину ожидания
                    }
                    else //продолжаем убегать
                    {
                        Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 3);//переводим в глобальные координаты направление вперед
                        nav.SetDestination(forwardPosition);        //назначаем агенту новое направление  
                    }
                }
                else//враг был отсоединен
                {
                    if (enemyinmemory != null) EnemyInMemory(enemyinmemory);//есть ли враг в памяти
                    else StartCoroutine(Wait());//отдыхаем
                }
            }
            else if(state == State.runfor)//гонимся за кем-то
            {
                if (enemy != null)//если враг не отсоединился
                {
                    nav.SetDestination(enemy.transform.position);
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    
                    if (distance < 2)
                    {
                        state = State.attack;
                        anim.SetBool("Attack", true);
                        //anim.SetBool("Run", false);
                       /* if (priority==7&&enemy.tag=="Player")
                        {
                            Debug.Log("try to change legs!");
                            anim.SetBool("Attack", false);
                            anim.SetTrigger("ChangeLegs");
                            anim.SetBool("Attack", true);
                        }
                        */
                        
                    }
                    else if(distance>30)
                    {
                        enemy = null;
                    }

                }
                else if (enemyinmemory != null) EnemyInMemory(enemyinmemory);//есть ли враг в памяти
                else StartCoroutine(Wait());
                                               
            }
            else if(state == State.attack)//сражаемся
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) > 3)//если при атаке оказались далеко от объекта
                {
                   // if (priority == 7) anim.SetBool("2Legs", false);
                    anim.SetBool("Attack", false);
                    anim.SetBool("Run", true);
                    nav.SetDestination(enemy.transform.position);
                    state = State.runfor;
                }
                else
                {/*
                    if (priority == 7)
                    {
                        anim.SetBool("2Legs", false);
                        anim.SetTrigger("ChangeLegs");
                    }
                    */
                    anim.SetBool("Attack", true);
                    anim.SetBool("Run", false);

                    Vector3 forwardenemyPosition = enemy.transform.TransformPoint(Vector3.forward * 2);//переводим в глобальные координаты направление вперед
                    nav.SetDestination(forwardenemyPosition);
                    transform.LookAt(enemy.transform);

                    int Random4ik = Random.Range(0, 100);
                    //Debug.Log("Random4ik: " + Random4ik);
                    if (enemy.tag == "Player")//если сражаемся с игроком
                    {
                        if (Random4ik > 95)
                        {
                            //Debug.Log("You die");
                            enemy.GetComponent<PlayerMove>().TakeDamage(damage);//наносим игроку урон
                            if (enemy.GetComponent<PlayerMove>().hp <= 0)//если здоровье игрока упало за ноль
                            {
                                if (enemyinmemory != null)
                                {
                                    if (enemyinmemory.tag == "Player") EnemyInMemory(enemyinmemory);
                                    else if(enemyinmemory.GetComponent<Behavior>().priority > 4) EnemyInMemory(enemyinmemory);
                                }
                                else if (priority > 3)
                                {
                                    state = State.eat;//если приоритет больше 3 - съедаем игрока
                                    StartCoroutine(Eating());//начинаем есть
                                }
                            }
                        }
                    }
                    else //если враг не игрок
                    {                        
                        if (Random4ik > 95) enemy.GetComponent<Behavior>().TakeDamage(gameObject, damage);//наносим урон
                                                   
                            if (enemy.GetComponent<Behavior>().state == State.dead)//если враг стал мертвым
                            {
                            if (enemyinmemory != null && enemyinmemory.GetComponent<Behavior>().priority > 4) EnemyInMemory(enemyinmemory);
                            else if (priority > 3)
                                {
                                    state = State.eat;
                                    StartCoroutine(Eating());
                                }
                                else
                                {
                                    anim.SetBool("Attack", false);
                                    StartCoroutine(Wait());
                                }
                            
                        }
                    }
                    
                    
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
    void EnemyInMemory(GameObject inmemory)
    {
        if (Vector3.Distance(transform.position, enemyinmemory.transform.position) < 30)//проверяем рядом еще враг из памяти
        {
            GetEnemy(enemyinmemory);//т.к. враг еще актуален, выполняем запрос на присоединение еще раз
        }
        else
        {
            enemyinmemory = null;
            StartCoroutine(Wait());//если враг из памяти уже далеко - отдыхаем
        }
    }
        public void TakeDamage(GameObject killer, int enemydamage)
    {
        Debug.Log(priority + " take damage from damage " + enemydamage);
        if (state != State.dead)
        {
            if (priority != 1 && priority != 3)
            {
                state = State.attack;
                anim.SetBool("Run", true);
                anim.SetBool("Attack", true);
            }
            if (enemy!=killer) enemy = killer;
            
            Debug.Log("hp before " + hp);
            hp -= Random.Range(enemydamage, enemydamage + 10);
            Debug.Log("hp after " + hp);
            if (hp <= 0)
            {

                nav.ResetPath();
                nav.enabled = false;
                //if (priority == 7 && anim.GetBool("2Legs")) anim.SetTrigger("Death2");
                //else anim.SetTrigger("Death");
                enemy = null;
                state = State.dead;
                anim.SetTrigger("Death");
                priority = 3;
               
                CreatorRef.SomebodyDead(gameObject);
                StopAllCoroutines();
            }
            //StartCoroutine(Death());
        }
    }
       public void GetEnemy(GameObject newenemy)//рядом враг
    {
        if (state != State.dead && anim!=null)//если не мертвый и подключился аниматор
        {
            int newenemypriority;//приоритет нового врага
            if (newenemy.tag == "Player") newenemypriority = 6;//у игрока приоритет всегда 6
            else newenemypriority = newenemy.GetComponent<Behavior>().priority;

            bool changenemy = false;
            if (enemy != null)//определяем, будем ли менять врага, если уже есть взаимодействие
            {
                int oldpriority;
                if (enemy.tag == "Player") oldpriority = 6;
                else oldpriority = enemy.GetComponent<Behavior>().priority;
                if (newenemypriority > oldpriority) changenemy = true;
                if(newenemy!=enemy)enemyinmemory = enemy;//враг был проигнорирован из-за приоритета, но запомним, что он тоже рядом
            }

            if (enemy == null || changenemy)
            {
                bool addenemy = false;        //добавить ли врага после проверки условий
                if (CreatorRef.Response(priority, newenemypriority))//запрос к создателю о нападении, если true - нападаем
                {
                    state = State.runfor;//указываем состояние бега
                    transform.LookAt(newenemy.transform.position);//разворачиваем сначала к игроку                                                                 
                    addenemy = true;
                }
                else if(newenemypriority>4) //если не нападаем и его приоритет выше 4 - убегаем
                {
                    if (state == State.eat||state==State.attack)//если в это время ели или сражались с кем-то
                    {
                        StopAllCoroutines();//останавливаем все процессы
                        anim.SetBool("Attack", false);//перестаем атаковать
                    }
                    state = State.runfrom;//указываем состояние бега  

                    transform.rotation = Quaternion.Euler(newenemy.transform.rotation.eulerAngles.x, newenemy.transform.rotation.eulerAngles.y, newenemy.transform.rotation.eulerAngles.z);//бежим туда, куда направлен и враг
                    addenemy = true;//указываем, что враг будет добавлен
                }
                if (enemy==null && gameObject.tag == "Player") addenemy = true;//если запрос пришел на игрока и у него небыло сопряженных врагов - добавляем
                
                if(addenemy)//если добавляем врага
                {
                    enemy = newenemy.gameObject;//записываем врага
                    if (enemy.tag == "Player") enemy.GetComponent<PlayerMove>().associated = gameObject;
                    else enemy.GetComponent<Behavior>().GetEnemy(gameObject);//оповещаем о присоединении
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
       /* if (priority == 7 && Random.Range(0, 1) == 1)//особые анимации для медведей
        {
            Debug.Log("try to change legs!");
            anim.SetTrigger("ChangeLegs");
            anim.SetBool("2Legs", true);
        }
        */
        state = State.wait;
        yield return new WaitForSeconds(10f);//ждем 10 секунд                 
      
       nav.SetDestination(CreatorRef.FindPoint());//задаем новую точку для движения в пределах плоскости
        nav.speed = 1;       //включаем низкую скорость для ходьбы
                            

        anim.SetBool("Walk", true);//включаем анимацию ходьбы  
      //  if (priority == 7) anim.SetBool("2Legs", false);      
        state = State.walk;//включаем состояние ходьбы
        //Debug.Log("wait for " + gameObject+gameObject.GetComponent<Behavior>().state);
              
    }
    IEnumerator Eating()
    {
        yield return new WaitForSeconds(10f);//поедаем 10 секунд
        if (enemy != null)
        {
            if (enemy.gameObject.tag != "Player")
            {
                if (enemy.GetComponent<Behavior>().state == State.dead)
                {
                    Destroy(enemy);
                    enemy = null;
                    if (enemyinmemory != null && enemyinmemory.GetComponent<Behavior>().priority > 4) EnemyInMemory(enemyinmemory);
                    else
                    {
                        state = State.wait;
                        anim.SetBool("Attack", false);
                        StartCoroutine(Wait());
                    }
                }
            }
            else
            {
                Debug.Log("Game Over");
                enemy = null;
                state = State.wait;
            }
        }
       
    }
   
}
