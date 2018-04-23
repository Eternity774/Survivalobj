using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Behavior : MonoBehaviour {
    
   // GameObject Creator;//ссылка на создателя
    Creator CreatorRef;//ссылка на компонент скрипта создателя
    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    
    public GameObject enemy;//храним, кто преследует кролика
    public GameObject enemyinmemory;//для случая, когда основной враг нейтрализуется, но еще один, который был ранее проигнорирован, станет основным
    public GameObject friend;
    //public GameObject leader;//лидер клана
    public Clan clan;

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
        friend,
        dead        
    }
    public State state;//переменная для состояния
    
    void Start () {
        CreatorRef = GameObject.Find("MainController").GetComponent<Creator>();//находим контроллер на сцене
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
                        else if(Vector3.Distance(nav.destination, gameObject.transform.position) < 1)//продолжаем убегать
                        {
                            Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 5);//переводим в глобальные координаты направление вперед
                            nav.SetDestination(forwardPosition);        //назначаем агенту новое направление  
                        }
                    }
                    else//враг был отсоединен
                    {
                        if (enemyinmemory != null)
                        {
                            EnemyInMemory(enemyinmemory);//есть ли враг в памяти

                        }
                        else
                        {
                            if (clan != null)
                            {
                                nav.ResetPath();
                                state = State.friend;
                                StopAllCoroutines();
                            }
                            else StartCoroutine(Wait());//отдыхаем
                        }
                    }
                    break;
                }
            case State.runfor:
                {
                    if (enemy != null)//если враг не отсоединился
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);//мереем дистанцию
                        if(distance>=2 && distance <= 30 && Vector3.Distance(nav.destination, gameObject.transform.position)<1)
                        {
                            nav.SetDestination(enemy.transform.position);//направляем агента на врага
                        }              
                        

                        else if (distance < 2)//когда дистанция сократилась
                        {
                            state = State.attack;//атакуем
                            anim.SetBool("Attack", true);
                            anim.SetBool("Run", false);
                        }
                        else if (distance > 30) enemy = null;
                    }

                    else if (enemyinmemory != null)
                    {
                        //state = State.wait;
                        EnemyInMemory(enemyinmemory);//есть ли враг в памяти
                    }
                    else
                    {
                        anim.SetBool("Attack", false);
                        if (clan != null)
                        {
                            nav.ResetPath();
                            state = State.friend;
                            StopAllCoroutines();
                        }
                        else StartCoroutine(Wait());
                    }
                    
                    break;
                }
            case State.attack:
                {
                    if (enemy != null)
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
                                                       
                            Vector3 forwardenemyPosition = enemy.transform.TransformPoint(Vector3.forward * 2);//переводим в глобальные координаты направление вперед
                            if(Vector3.Distance(nav.destination, gameObject.transform.position)>1) nav.SetDestination(forwardenemyPosition);
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
                                    if (enemy.GetComponent<Behavior>().priority == 3) enemyiskilled = true;//если враг стал мертвым
                                }
                                if (enemyiskilled)
                                {
                                    if (enemyinmemory != null)
                                    {
                                        if (Vector3.Distance(transform.position, enemyinmemory.transform.position) < 30)
                                        {
                                            if (enemyinmemory.tag == "Player")
                                            {
                                                EnemyInMemory(enemyinmemory);
                                            }
                                            else if (enemyinmemory.GetComponent<Behavior>().priority > 4) EnemyInMemory(enemyinmemory);
                                        }
                                       
                                           
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
                    }
                    else//после атаки
                    {
                        anim.SetBool("Attack", false);
                        if (clan != null)
                        {
                            nav.ResetPath();
                            state = State.friend;
                            StopAllCoroutines();
                        }
                        else StartCoroutine(Wait());
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
                        if(priority==5) anim.SetBool("Eating", false);
                        if (clan != null)
                        {
                            nav.ResetPath();
                            state = State.friend;
                            StopAllCoroutines();
                        }
                        else StartCoroutine(Wait());
                    }
                    if (enemyinmemory != null) EnemyInMemory(enemyinmemory);
                    break;
                }
            case State.friend://имеется в виду не главный
                {
                    if (clan != null)//если друг не отсоединился
                    {
                        float distance = Vector3.Distance(transform.position, clan.Leader.transform.position);//мереем дистанцию
                        if (distance >= 2&&Vector3.Distance(nav.destination, gameObject.transform.position) < 1) nav.SetDestination(clan.Leader.transform.position);//направляем агента за лидером   

                        if (distance > 20)
                        {                                                           
                                anim.SetBool("Run", true);
                                nav.speed = 8;                           
                        }               
                         
                        if (distance < 8&&distance>=2)//когда дистанция сократилась
                        {                            
                                anim.SetBool("Walk", true);
                                anim.SetBool("Run", false);
                                nav.speed = 1;                         
                            
                        }
                        if (distance < 2)
                        {
                            //nav.SetDestination(transform.position);//направляем агента на врага   
                            anim.SetBool("Walk", false);
                            nav.ResetPath();
                        }
                        
                    }

                    else if (enemyinmemory != null)
                    {
                        //state = State.wait;
                        EnemyInMemory(enemyinmemory);//есть ли враг в памяти
                    }
                    else StartCoroutine(Wait());
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
            if (clan != null)
            {
                nav.ResetPath();
                state = State.friend;
                StopAllCoroutines();
            }
            else StartCoroutine(Wait());//если враг из памяти уже далеко - отдыхаем
        }
    }
        public void TakeDamage(GameObject killer, int enemydamage)
    {
        //Debug.Log(gameObject.name + " take damage from " + killer.gameObject.name);
        if (state != State.dead)
        {
            if (priority != 1 && priority != 3)
            {
                state = State.attack;
                anim.SetBool("Run", true);
                anim.SetBool("Attack", true);
            }
            if (enemy!=killer) enemy = killer;
            if (friend == killer)
            {
                friend = null;
                clan.DeleteFromClan(gameObject);
                clan = null;
            }
           // Debug.Log("hp before " + hp);
            hp -= Random.Range(enemydamage, enemydamage + 10);
          //  Debug.Log("hp after " + hp);
            if (hp <= 0)
            {
                //Debug.Log(gameObject.name + " is dead");
               nav.ResetPath();
                nav.enabled = false;
                enemy = null;
                if (clan != null)
                {
                    Debug.Log("удаляем умершего");
                    clan.DeleteFromClan(gameObject);
                    clan = null;
                }
                if(priority==6) Creator.ChangeInClans();
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
       // Debug.Log(gameObject.name + "see the" + newenemy.name);
       
        if (state != State.dead && anim!=null)//если не мертвый и подключился аниматор
        {
            if (friend!=newenemy)
            { 
            int newenemypriority;//приоритет нового врага
            if (newenemy.tag == "Player") newenemypriority = 6;//у игрока приоритет всегда 6
            else newenemypriority = newenemy.GetComponent<Behavior>().priority;//узнаем приоритет нового врага

            bool changenemy = false;//будем ли менять врага
            if (enemy != null)//определяем, будем ли менять врага, если уже есть взаимодействие
            {
                int oldpriority;
                if (enemy.tag == "Player") oldpriority = 6;
                else oldpriority = enemy.GetComponent<Behavior>().priority;
                if (newenemypriority > oldpriority) changenemy = true;
                if (newenemy.gameObject.name != enemy.gameObject.name)
                {
                   // Debug.Log("write enemy in memory for:"+gameObject.name+" oldenemy: " + enemy.gameObject.name + "newenemy: " + newenemy.gameObject.name);
                    if (changenemy) enemyinmemory = enemy;//если враг будет менятся запомним, что он тоже рядом
                    else enemyinmemory = newenemy;//если враг был проигнорирован из-за приоритета, но запомним, что он тоже рядом
                }
            }

                if (enemy == null || changenemy)
                {

                    bool addenemy = false;        //добавить ли врага после проверки условий
                    bool friendly = false;
                    // Debug.Log("About friendly!" + priority + newenemypriority);
                    if (priority == 6 && newenemypriority==6)
                    {
                        if(clan!=null)//мы в клане
                        {
                            if (newenemy.tag == "Player")//мы втретили игрока
                            {
                                if (clan.Leader == newenemy)
                                {
                                    friendly = true;
                                    Debug.Log(gameObject.name + "я уже был в клане игрока");
                                }
                                else Debug.Log(gameObject.name + "я не в клане игрока");
                            } 
                            else if (newenemy.GetComponent<Behavior>().clan != null)//мы встретили не игрока
                            {
                                if (newenemy.GetComponent<Behavior>().clan == clan)
                                {
                                    friendly = true;
                                    Debug.Log(gameObject.name + "я в том же клане, что и этот аи");
                                }
                                else Debug.Log(gameObject.name + "я в другом клане");

                            }
                            else//т.е. мы в клане, а он - нет
                            {
                                if (Random.Range(0, 3) != 0)
                                {
                                    if(newenemy.GetComponent<Behavior>().enemy!=null)//у нашего объекта есть враг
                                    {
                                        if (newenemy.GetComponent<Behavior>().enemy.tag == "Player")//если враг объекта игрок
                                        {
                                            if (clan.Leader.name == "Player") friendly = false;//и мы в клане игрока
                                            else friendly = true;
                                        }
                                        else if (newenemy.GetComponent<Behavior>().enemy.GetComponent<Behavior>().clan != clan) friendly = true;//если встретили объект с врагом не из нашего клана
                                    }
                                    else
                                    {
                                        friendly = true;
                                    }
                                        //добавить в клан!!!
                                    if(friendly)
                                    {
                                        clan.AddToClan(newenemy);
                                        Debug.Log(gameObject.name + "я возьму его в свой клан");
                                    }
                                    else Debug.Log("он враждует с моим кланом!");
                                }
                                else Debug.Log(gameObject.name + "я не возьму его в свой клан");
                            }
                            
                        }
                        else//мы не в клане
                        {
                            if (newenemy.tag == "Player")//мы встретили игрока
                            {
                                if (Random.Range(0, 3) != 0)
                                {
                                    friendly = true;//присоединть к клану игрока
                                    clan = newenemy.GetComponent<PlayerMove>().ClanOfPlayer;
                                    clan.AddToClan(gameObject);
                                    Debug.Log(gameObject.name + "Я хочу в клан игрока!");
                                }
                                else Debug.Log(gameObject.name + "я не хочу в клан игрока");

                            }
                            else if (newenemy.GetComponent<Behavior>().clan != null)//встретили не игрока уже в клане
                            {
                                if (Random.Range(0, 3) != 0)
                                {
                                    friendly = true;//присоединиться к клану другого аи
                                    clan = newenemy.GetComponent<Behavior>().clan;
                                    clan.AddToClan(gameObject);
                                    Debug.Log(gameObject.name + "я пойду в клан этого аи");
                                    if (enemy != null)
                                    {
                                        if (enemy.tag != "Player")
                                        {
                                            if (enemy.GetComponent<Behavior>().clan != null)
                                            {
                                                if (enemy.GetComponent<Behavior>().clan == newenemy.GetComponent<Behavior>().clan) enemy = null;
                                            }
                                        }
                                        else if (enemy.GetComponent<PlayerMove>().ClanOfPlayer == clan) enemy = null;
                                    }
                                }
                                else Debug.Log(gameObject.name + "я не пойду в клан этого аи");
                            }
                            else //мы оба не в кланах
                            {
                                if (Random.Range(0, 3) != 0)
                                {
                                    friendly = true;
                                    clan = new Clan(gameObject);
                                    Creator.ChangeInClans();
                                    Debug.Log(gameObject.name + "Мы сформируем новый клан!");
                                }
                                else Debug.Log(gameObject.name + "не будем формировать новый клан");

                            }
                        }
                        //Debug.Log("About friendly!" + priority + newenemypriority);
                       
                        if(friendly)//если решили дружить
                        {
                            if (priority == 6)
                            {
                                //Debug.Log("FRIENDLY! " + gameObject.name);
                                friend = newenemy;
                                anim.SetBool("Attack", false);
                                anim.SetTrigger("Hello");
                                if (friend.tag == "Player")//добавляемся в клан игрока
                                {
                                    nav.ResetPath();
                                    state = State.friend;
                                    StopAllCoroutines();
                                }
                                //  else if (friend.GetComponent<Behavior>().state != State.friend)
                                else if(clan.Leader != gameObject)//если мы не лидер клана
                                {
                                    nav.ResetPath();
                                    state = State.friend;
                                    StopAllCoroutines();
                                }

                            }
                        }


                    }
                    if (!friendly)
                    {
                        if (CreatorRef.Response(priority, newenemypriority))//запрос к создателю о нападении, если true - нападаем
                        {
                            anim.SetBool("Attack", false);//перестаем атаковать
                            if (priority == 5) anim.SetBool("Eating", false);//перестаем ксть
                            state = State.runfor;//указываем состояние бега
                            transform.LookAt(newenemy.transform.position);//разворачиваем сначала к игроку                                                                 
                            addenemy = true;
                        }
                        else if (newenemypriority > 4) //если не нападаем и его приоритет выше 4 - убегаем
                        {
                            if (state == State.eat || state == State.attack)//если в это время ели или сражались с кем-то
                            {
                                StopAllCoroutines();//останавливаем все процессы
                                anim.SetBool("Attack", false);//перестаем атаковать
                                if (priority == 5) anim.SetBool("Eating", false);
                            }
                            state = State.runfrom;//указываем состояние бега  

                            transform.rotation = Quaternion.Euler(newenemy.transform.rotation.eulerAngles.x, newenemy.transform.rotation.eulerAngles.y, newenemy.transform.rotation.eulerAngles.z);//бежим туда, куда направлен и враг
                            addenemy = true;//указываем, что враг будет добавлен
                        }
                        if (enemy == null && gameObject.tag == "Player") addenemy = true;//если запрос пришел на игрока и у него небыло сопряженных врагов - добавляем

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
                    }
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
        Vector3 temp = CreatorRef.FindPoint();
        yield return new WaitForSeconds(10f);//ждем 10 секунд               
        nav.SetDestination(temp);//задаем новую точку для движения в пределах плоскости
        nav.speed = 1;       //включаем низкую скорость для ходьбы
        anim.SetBool("Walk", true);//включаем анимацию ходьбы  
        state = State.walk;//включаем состояние ходьбы       
              
    }
    IEnumerator Eating()
    {
        state = State.eat;//если приоритет больше 3 - съедаем игрока
        nav.SetDestination(enemy.transform.position);

        if (priority==5) anim.SetBool("Eating", true);
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
                    //state = State.wait;
                    if (priority == 5) anim.SetBool("Eating", false);
                    else anim.SetBool("Attack", false);
                    if (enemyinmemory != null) EnemyInMemory(enemyinmemory);//test
                   // StartCoroutine(Wait());
                    
                }
            }
            else
            {
               // Debug.Log("Game Over");
                enemy = null;
                StartCoroutine(Wait());
            }
        }
       
    }
   
}
