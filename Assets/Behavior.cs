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
        Creator = GameObject.Find("MainController");//находим контроллер на сцене
        CreatorRef = Creator.GetComponent<Creator>();//берем компонент со скриптом
        nav = GetComponent<NavMeshAgent>();//берем компоненты с того же объекта где и скрипт (навмеш)
        anim = GetComponent<Animator>();//берем компонент аниматора
        int[] infomas = CreatorRef.StartInformation(gameObject);
        priority = infomas[0];//определяем приоритет ai по тэгу через создателя
        hp = infomas[1];//определяем кол-во здоровья
        damage = infomas[2];//опрделеяем наносимый урон 
        StartCoroutine(Wait());//запускаем корутину(процесс) ожидания в 10 сек        
    }

    void FixedUpdate()
    {
       switch (state)
        {
            case State.walk:
                {
                    if(Vector3.Distance(nav.destination, gameObject.transform.position) < 1)
                    {
                        hp += 50;
                        StartCoroutine(Wait());//запускаем корутину ожидания
                    }
                    break;
                }
            case State.runfrom:
                {
                    if (enemy != null)//если враг не отсоединился
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);//проверяем дистанцию до врага

                        if (distance > 40) enemy = null;//если дистанция больше 40 отсоединяем врага
                        else //продолжаем убегать
                        {
                            Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 3);//переводим в глобальные координаты направление вперед
                            nav.SetDestination(forwardPosition);        //назначаем агенту новое направление  
                        }
                    }
                    else//враг был отсоединен
                    {
                        if (enemyinmemory != null)
                        {
                            EnemyInMemory(enemyinmemory);//есть ли враг в памяти

                        }
                        else StartCoroutine(Wait());//отдыхаем
                    }
                    break;
                }
            case State.runfor:
                {
                    if (enemy != null)//если враг не отсоединился
                    {
                        nav.SetDestination(enemy.transform.position);//направляем агента на врага
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);//мереем дистанцию

                        if (distance < 2)//когда дистанция сократилась
                        {
                            state = State.attack;//атакуем
                            anim.SetBool("Attack", true);
                            anim.SetBool("Run", false);
                        }
                        else if (distance > 30) enemy = null;
                    }

                    else if (enemyinmemory != null)
                    {
                        state = State.wait;
                        EnemyInMemory(enemyinmemory);//есть ли враг в памяти
                    }
                    else StartCoroutine(Wait());
                    break;
                }
            case State.attack:
                {

                    if (Vector3.Distance(transform.position, enemy.transform.position) > 3)//если при атаке оказались далеко от объекта
                    {
                        anim.SetBool("Attack", false);
                        anim.SetBool("Run", true);
                        nav.SetDestination(enemy.transform.position);
                        state = State.runfor;
                        //попробовать возможность изменения поведения
                    }
                    else
                    {


                        Vector3 forwardenemyPosition = enemy.transform.TransformPoint(Vector3.forward * 3);//переводим в глобальные координаты направление вперед
                        nav.SetDestination(forwardenemyPosition);
                        transform.LookAt(enemy.transform);

                        int Random4ik = Random.Range(0, 100);
                        if (Random4ik > 95)
                        {
                            bool enemyiskilled = false;
                            if (enemy.tag == "Player")//если сражаемся с игроком
                            {
                                enemy.GetComponent<PlayerHealth>().TakeDamage(damage);//наносим игроку урон
                                if (enemy.GetComponent<PlayerHealth>().currentHealth <= 0) enemyiskilled = true;
                            }
                            else
                            {
                                enemy.GetComponent<Behavior>().TakeDamage(gameObject, damage);//наносим урон
                                if (enemy.GetComponent<Behavior>().priority==3) enemyiskilled = true;//если враг стал мертвым
                            }
                            if (enemyiskilled)
                            {
                                if (enemyinmemory != null)
                                {
                                    if (Vector3.Distance(transform.position, enemyinmemory.transform.position) < 30 && enemyinmemory.GetComponent<Behavior>().priority>4)
                                        EnemyInMemory(enemyinmemory);
                                }                                
                                    if (priority > 3) StartCoroutine(Eating());//начинаем есть                                 
                                    else
                                    {
                                        enemy = null;
                                        anim.SetBool("Attack", false);//выключаем анимацию атаки
                                        StartCoroutine(Wait());
                                    }
                                


                            }
                        }
                    }
                    break;
                }
            case State.eat:
                {
                    if (enemy != null)
                    {
                        Vector3 forwardenemyPosition = enemy.transform.TransformPoint(Vector3.forward * 2);//переводим в глобальные координаты направление вперед
                        nav.SetDestination(forwardenemyPosition);
                        transform.LookAt(enemy.transform);
                    }
                    else
                    {
                        anim.SetBool("Attack", false);
                        StopAllCoroutines();
                        StartCoroutine(Wait());
                    }
                    if (enemyinmemory != null) EnemyInMemory(enemyinmemory);
                    break;
                }
        }
    
    }
    void EnemyInMemory(GameObject inmemory)
    {
        if (Vector3.Distance(transform.position, enemyinmemory.transform.position) < 30)//проверяем рядом еще враг из памяти
        {
            GetEnemy(enemyinmemory);//т.к. враг еще актуален, выполняем запрос на присоединение еще раз
            enemyinmemory = null;
        }
        else
        {
            enemyinmemory = null;
            StartCoroutine(Wait());//если враг из памяти уже далеко - отдыхаем
        }
    }
        public void TakeDamage(GameObject killer, int enemydamage)
    {
        Debug.Log(gameObject.name + " take damage from " + killer.gameObject.name);
        if (state != State.dead)
        {
            if (priority != 1 && priority != 3)
            {
                state = State.attack;
                anim.SetBool("Run", true);
                anim.SetBool("Attack", true);
            }
            if (enemy!=killer) enemy = killer;
            
           // Debug.Log("hp before " + hp);
            hp -= Random.Range(enemydamage, enemydamage + 10);
          //  Debug.Log("hp after " + hp);
            if (hp <= 0)
            {
                Debug.Log(gameObject.name + " is dead");
               nav.ResetPath();
                nav.enabled = false;
                enemy = null;
                state = State.dead;
                anim.SetTrigger("Death");
                priority = 3;               
                CreatorRef.SomebodyDead(gameObject);
                StopAllCoroutines();
            }            
        }
    }
       public void GetEnemy(GameObject newenemy)//рядом враг
    {
        Debug.Log(gameObject.name + "see the" + newenemy.name);
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
                if (newenemy.gameObject.name != enemy.gameObject.name)
                {
                    Debug.Log("write enemy in memory for:"+gameObject.name+" oldenemy: " + enemy.gameObject.name + "newenemy: " + newenemy.gameObject.name);
                    if (changenemy) enemyinmemory = enemy;//если враг будет менятся запомним, что он тоже рядом
                    else enemyinmemory = newenemy;//если враг был проигнорирован из-за приоритета, но запомним, что он тоже рядом
                }
            }

            if (enemy == null || changenemy)
            {
                bool addenemy = false;        //добавить ли врага после проверки условий
                if (CreatorRef.Response(priority, newenemypriority))//запрос к создателю о нападении, если true - нападаем
                {
                    anim.SetBool("Attack", false);//перестаем атаковать
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

                if (addenemy)//если добавляем врага
                {
                    enemy = newenemy.gameObject;//записываем врага
                    if (enemy.tag == "Player") enemy.GetComponent<PlayerMove>().associated = gameObject;
                    // else enemy.GetComponent<Behavior>().GetEnemy(gameObject);//оповещаем о присоединении !!!попробуем НЕ ОПОВЕЩАТЬ!!!
                    StopAllCoroutines();//останавливаем корутины (т.к. есть возможность входа в триггер во время ожидания)
                    nav.speed = Random.Range(5, 8);//включаем высокую скорость
                    anim.SetBool("Walk", false);//выключаем ходьбу
                    anim.SetBool("Run", true);//переключаем анимацию в бег
                }
               // else StartCoroutine(Wait());
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
        
        nav.SetDestination(gameObject.transform.position);//задаем новой точкой текущие координаты          
        anim.SetBool("Run", false);//выключаем бег
        anim.SetBool("Walk", false);//выключаем анимацию ходьбы
        state = State.wait;
        yield return new WaitForSeconds(10f);//ждем 10 секунд               
        nav.SetDestination(CreatorRef.FindPoint());//задаем новую точку для движения в пределах плоскости
        nav.speed = 1;       //включаем низкую скорость для ходьбы
        anim.SetBool("Walk", true);//включаем анимацию ходьбы  
        state = State.walk;//включаем состояние ходьбы       
              
    }
    IEnumerator Eating()
    {
        state = State.eat;//если приоритет больше 3 - съедаем игрока
        yield return new WaitForSeconds(10f);//поедаем 10 секунд
        hp += 200;
        if (enemy != null)
        {
            if (enemy.gameObject.tag != "Player")
            {
                if (enemy.GetComponent<Behavior>().state == State.dead)
                {
                    Destroy(enemy);
                    enemy = null;                 
                    state = State.wait;
                    anim.SetBool("Attack", false);
                    StartCoroutine(Wait());
                    
                }
            }
            else
            {
                Debug.Log("Game Over");
                enemy = null;
                StartCoroutine(Wait());
            }
        }
       
    }
   
}
