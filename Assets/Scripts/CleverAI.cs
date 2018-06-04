using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CleverAI : MonoBehaviour {
    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    Creator CreatorRef;//ссылка на создателя    

    public GameObject player;

    public Task currenttask;
    public Clan clan;
    int sociability;
    int reactiononplayer=5;
    public int priority;//приоритет для разрешения столкновения нескольких объектов
    public int hp;//здоровье
    public int damage;//наносимый урон


    public struct Task
    {
        
        public Action action;
        public GameObject target;
        public int targetpriority;

        public Task(Action action, GameObject target, int targetpriority)
        {
            this.action = action;
            this.target = target;
            this.targetpriority = targetpriority;
        }
    }

    public enum Action
    {
        Default,
        Friend,
        RunFor,
        Eat,
        Attack,
        RunFrom,        
        Dead
    }

    
    public Stack<Task> Tasks = new Stack<Task>();//очередь для выполнения
   
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        CreatorRef = GameObject.Find("MainController").GetComponent<Creator>();//находим контроллер на сцене
        nav = GetComponent<NavMeshAgent>();//берем компоненты с того же объекта где и скрипт (навмеш)
        anim = GetComponent<Animator>();//берем компонент аниматора
        sociability = Random.Range(1, 10);//характер
        int[] infomas = CreatorRef.StartInformation(gameObject);
        priority = infomas[0];//определяем приоритет ai по тэгу через создателя
        hp = infomas[1];//определяем кол-во здоровья
        damage = infomas[2];//опрделеяем наносимый урон 
        currenttask = new Task(Action.Default, gameObject, 0);         
        runuptask();
        GetComponent<SphereCollider>().enabled = true;
       
    }
	
	
	void FixedUpdate () {
       // Debug.Log(currenttask.action);
		switch (currenttask.action)
        {
            case Action.Default:
                {
                    if (Vector3.Distance(nav.destination, gameObject.transform.position) < 4)
                    {
                        runuptask();
                    }
                    break;
                }
            case Action.RunFrom:
                {
                    if (currenttask.target != null)
                    {
                        //Debug.Log("таргет не пустой");
                        if (Vector3.Distance(gameObject.transform.position, currenttask.target.transform.position) > 40)
                        {
                            //Debug.Log("враг далеко, завершаю"+gameObject);
                            CompleteTask();
                        }
                        else if (Vector3.Distance(nav.destination, gameObject.transform.position) < 2)//продолжаем убегать
                        {
                            //Debug.Log("добежал до точки, беру следующую");
                            Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 8);//переводим в глобальные координаты направление вперед
                            nav.SetDestination(forwardPosition);        //назначаем агенту новое направление  
                        }

                    }
                    else CompleteTask();
                    break;
                }
            case Action.RunFor:
                {
                    if (currenttask.target != null)
                    {
                        float distance = Vector3.Distance(transform.position, currenttask.target.transform.position);//мереем дистанцию до врага

                        if (distance >= 2 && distance <= 40 && Vector3.Distance(nav.destination, gameObject.transform.position) < 2)
                        {
                            nav.SetDestination(currenttask.target.transform.position);//направляем агента на врага
                        }
                        
                        else if (distance < 2)//когда дистанция сократилась
                        {
                            AddTask(new Task(Action.Attack, currenttask.target, currenttask.targetpriority));
                        }
                        else if (distance > 40) CompleteTask();
                    }
                    else CompleteTask();
                    break;
                }
            case Action.Attack:
               {
                    if (currenttask.target != null)
                    {

                        transform.LookAt(currenttask.target.transform.position);
                        if (Vector3.Distance(transform.position, currenttask.target.transform.position) > 3)//если при атаке оказались далеко от объекта
                        {
                            CompleteTask();//при завершении в стеке найдется предшествующее действие (runfor или runfrom)
                        }
                        int Random4ik = Random.Range(0, 100);
                        if (Random4ik > 95)
                        {
                            bool enemyiskilled = false;
                            if (currenttask.target.tag == "Player")//если сражаемся с игроком
                            {
                                currenttask.target.GetComponent<PlayerHealth>().TakeDamage(damage);//наносим игроку урон
                                if (currenttask.target.GetComponent<PlayerHealth>().currentHealth <= 0) enemyiskilled = true;
                            }
                            else
                            {
                                currenttask.target.GetComponent<CleverAI>().TakeDamage(gameObject, damage);//наносим урон
                                 if (currenttask.target.GetComponent<CleverAI>().priority == 3) enemyiskilled = true;//если враг стал мертвым
                            }
                            if (enemyiskilled)
                            {

                                if (priority > 3)
                                {
                                    currenttask = new Task(Action.Eat, currenttask.target, currenttask.targetpriority);
                                    runuptask();
                                }
                                else
                                {
                                  //  print(gameObject.name + " ЗАВЕРШАЕТ АТАКУ");
                                  //  print("ЗАПИСЕЙ В ЕГО СТЭКЕ "+Tasks.Count);
                                    CompleteTask();
                                }
                            }
                        }
                        
                        
                    }
                    else CompleteTask();
                    break;
                }
            case Action.Eat:
                {
                    if (currenttask.target != null)//никто другой не съел
                    {
                        transform.LookAt(currenttask.target.transform);
                        //Debug.Log("таргет не пустой");
                        if (Vector3.Distance(currenttask.target.transform.position, gameObject.transform.position) < 5 && !anim.GetBool("Eat"))//должны подойти к туше
                        {
                            //print(gameObject + "буду хавать");
                            anim.SetBool("Run", false);
                            anim.SetBool("Eat", true);
                            nav.ResetPath();
                                                      
                           // anim.SetBool("Run", false);                            
                            StartCoroutine(Wait());
                        }

                    }
                    else CompleteTask();
                    break;
                }
            case Action.Friend:
                {
                    if (clan != null && gameObject != clan.Leader)
                    {
                        float distance = Vector3.Distance(transform.position, clan.Leader.transform.position);//мереем дистанцию до лидера
                        if (distance >= 2 && Vector3.Distance(nav.destination, gameObject.transform.position) < 2)
                        {
                            nav.speed = 1;
                            anim.SetBool("Run", false);
                            anim.SetBool("Walk", true);
                            nav.SetDestination(clan.Leader.transform.position);//направляем агента за лидером                              
                        }

                        if (distance > 20 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                        {
                            anim.SetBool("Walk", false);
                            anim.SetBool("Run", true);
                            nav.speed = 8;
                            nav.SetDestination(clan.Leader.transform.position);//направляем агента за лидером                              

                        }

                        if (distance < 8 && distance >= 2 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))  //когда дистанция сократилась
                        {
                            anim.SetBool("Run", false);
                            anim.SetBool("Walk", true);                            
                            nav.speed = 1;

                        }
                        if (distance < 2)
                        {
                            anim.SetBool("Walk", false);
                            nav.ResetPath();
                        }
                    }
                    else CompleteTask();
                    break;
                }
        }
	}
    public void AddTask(Task newtask)
    {
        //Debug.Log("добавляем задание" + newtask.action + gameObject);
        if (Compare(newtask, currenttask))
        {
            // Debug.Log("это задание выше");
            Tasks.Push(currenttask);
            currenttask = newtask;
            runuptask();
        }
        else//новое задание ниже по приоритету
        {
            Task nexttask = Tasks.Peek();
            if (newtask.target == nexttask.target && newtask.action == nexttask.action) { }
            else {
                if (Compare(newtask, nexttask))//проверяем след. в стэке
                {
                    Tasks.Push(newtask);//просто ставим в верх стэка                    
                }
                else//если и след. не то
                {
                    Stack<Task> temp = new Stack<Task>();//создаем временный стек
                    while (true)
                    {
                        temp.Push(Tasks.Pop());//закидываем верхний в стэке во временный стек
                        if (Compare(newtask, Tasks.Peek()))//если круче след. эелемента
                        {
                            Tasks.Push(currenttask);//закидываем таки на него
                            while (temp.Count > 0)//пока не опустеет временный стек
                            {
                                Tasks.Push(temp.Pop());//перекидываем из временного обратно
                            }
                            break;//завершаем перебор
                        }

                    }
                }

            }

        }
    }
    void CompleteTask()
    {
       //if(priority==6) print(gameObject + " завершает задание " + currenttask.action);
        while(Tasks.Count>0)
        {
            Task temptask = Tasks.Pop();
           
            if (temptask.action == Action.Default || temptask.action == Action.Friend)//если состояние дефолтное или состоит в клане, то оно всегда актуально
            {
                currenttask = temptask;
                currenttask.target = gameObject;
                break;
            }
            else if (temptask.target != null)//след. в стэке состояние не дефолтное
            {
                // print("цель не ноль");
                if (Vector3.Distance(gameObject.transform.position, temptask.target.transform.position) < 40)
                {

                    if (priority == 2)
                    {
                        if (temptask.target.tag != "PLayer")
                        {
                            if (temptask.target.GetComponent<CleverAI>().priority == 3)                           //print("ловушка сработала");

                                continue;
                        }
                        else if (!temptask.target.GetComponent<PlayerMove>().live)
                        {
                            continue;
                        }
                        // print("цель еще актуальна");
                        currenttask = temptask;
                        break;
                    }
                }
                //else print("цель уже не актуальна");
            }
            
        }
        
        runuptask();
    }

    bool Compare(Task newtask, Task oldtask)//возвращет true если новый таск выше
    {
        bool switchtask = false;
        if (newtask.action > oldtask.action) switchtask = true;//смена задания
        else if (newtask.action == oldtask.action)
        {
        if (newtask.targetpriority > oldtask.targetpriority) switchtask = true;
        }
        return switchtask;
    }

    void runuptask()
    {
        StopAllCoroutines();
        nav.ResetPath();
        
        anim.SetBool("Walk", false);
        anim.SetBool("Run", false);
        anim.SetBool("Eat", false);
        anim.SetBool("Attack", false);
        anim.SetBool("RunAttack", false);
        anim.SetBool("Action", false);

        //if (priority == 6)
        //{
        //    print("новое задание у " + gameObject + currenttask.action);
        //    if (currenttask.target != null) print(" с " + currenttask.target.name);
        //}
        switch (currenttask.action)
        {
            case Action.Default:
                {                    
                    nav.speed = 0;
                    nav.SetDestination(CreatorRef.FindPoint());//задаем новую точку для движения в пределах плоскости
                    if (Random.Range(0, 2) == 1)
                    {
                        anim.SetBool("Action", true);
                        if (priority == 5&&Vector3.Distance(gameObject.transform.position,player.transform.position)<500) AudioManager.instance.Play("WolfIdle");
                    }                   
                    StartCoroutine(Wait());
                    if(priority==4 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("BoarIdle");
                    if(priority==7 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("BearIdle");
                    break;
                }
            case Action.Friend:
                {
                    anim.SetTrigger("Hello");
                    break;                    
                }
            case Action.RunFrom:
                {
                   // Debug.Log("начинаем убегать");
                    //M(x;y) - мы, K(a;b)-враг, цель (2a - x;2b - y).
                    transform.LookAt(new Vector3(2*transform.position.x - currenttask.target.transform.position.x, 2 * transform.position.y - currenttask.target.transform.position.y, 2 * transform.position.z - currenttask.target.transform.position.z));
                    Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 20);
                    //currenttask.target.transform.TransformPoint(Vector3.forward * 20);//переводим в глобальные координаты направление вперед
                    nav.SetDestination(forwardPosition);
                    nav.speed = Random.Range(5, 8);
                    anim.SetBool("Run", true);
                    break;
                }
            case Action.RunFor:
                {
                   // Debug.Log("начинаем догонять");
                    nav.SetDestination(currenttask.target.transform.position);
                    nav.speed = Random.Range(5, 8);
                    if (priority > 5) anim.SetBool("Run", true);
                    else anim.SetBool("RunAttack", true);
                    if (priority == 4 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("BoarAttack");
                    if (priority == 5 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("WolfAttack");
                    if (priority == 7 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("BearAttack");
                    break;
                } 
            case Action.Attack:
                {
                   // Debug.Log("начинаем атаковать");
                    nav.SetDestination(currenttask.target.transform.TransformPoint(Vector3.forward * 2));
                    transform.LookAt(currenttask.target.transform.position);
                    anim.SetBool("Attack", true);
                    if (priority == 4 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("BoarAttack");
                    if (priority == 5 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("WolfAttack");
                    if (priority == 7 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Play("BearAttack");
                    break;
                }
            case Action.Eat:
                {
                    //Debug.Log("идем к еде");
                    nav.SetDestination(currenttask.target.transform.position);
                    nav.speed = 5;
                    anim.SetBool("Run", true);               
                    //anim.SetBool("Eat", true);
                    //StartCoroutine(Wait());
                    //transform.LookAt(currenttask.target.transform.position);
                    break;
                }
            case Action.Dead:
                {
                    //Debug.Log("умираем");
                    anim.SetTrigger("Death");
                    if (priority == 4 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Stop("BoarAttack");
                    if (priority == 5 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Stop("WolfAttack");
                    if (priority == 7 && Vector3.Distance(gameObject.transform.position, player.transform.position) < 50) AudioManager.instance.Stop("BearAttack");
                    Tasks.Clear();
                    break;
                }
        }

    }



    IEnumerator Wait()//корутина ожидания
    {

        yield return new WaitForSeconds(15f);//ждем 10 секунд       
        if (currenttask.action == Action.Default)
        {
            nav.speed = 1;       //включаем низкую скорость для ходьбы
            
            anim.SetBool("Action", false);
            anim.SetBool("Walk", true);//включаем анимацию ходьбы
        }
        else if(currenttask.action == Action.Eat)
        {
            Destroy(currenttask.target);
            hp += 200;
        }
    }
                
    
    private void OnTriggerEnter(Collider other)//для обработки взаимодействий (рядом игрок)
    {
        if (currenttask.action != Action.Dead && anim != null && currenttask.target!=other.gameObject)
        {
            //Debug.Log(other.name + currenttask.target.name);
            if (other.GetComponent<CleverAI>() != null || other.GetComponent<PlayerMove>() != null)
            {
                int newenemypriority;//приоритет нового врага
                if (other.gameObject.tag == "Player") newenemypriority = 6;//у игрока приоритет всегда 6
                else newenemypriority = other.gameObject.GetComponent<CleverAI>().priority;//узнаем приоритет нового врага

                if (priority == 6 && newenemypriority == 6) People(other.gameObject);//встретились 2 человека
                else if (priority > 2 && newenemypriority == 3) AddTask(new Task(Action.Eat, other.gameObject, newenemypriority));
                else if (newenemypriority!=0)
                {
                   // print("запуск из триггера" + other.gameObject.name + newenemypriority);
                    GetEnemy(other.gameObject, newenemypriority);
                }
            }
        }
    }

    public void People(GameObject otherman)//втретились 2 человека
    {
        print(gameObject + "встретил" + otherman.name);
        int reaction = 5;//по умолчанию реакция, которой нет 1-убежать, 2-атака, 0- ничего не делать 
        bool friendly = false;//будем ли сотрудничать
        if(clan==null)//если аи не в клане
        {
            if (otherman.tag == "Player" && reactiononplayer != 5)
            {
                reaction = reactiononplayer;
                print("реакция из памяти");
            }
            else
            {
                //print("AI не в клани и ");
                if (Random.Range(0, 10) < sociability)
                {
                    friendly = true;//предлагаем дружбу  
                                    //print("предлагает дружбу");
                }
            }
        }
        else//мы в клане
        {
            if(otherman.tag != "Player")//втретили не игрока
            {
                if (otherman.GetComponent<CleverAI>().clan == null)//другой не в клане
                {
                    if (Random.Range(0, 10) < sociability) friendly = true;//предлагаем дружбу
                }
                else //мы и встречный в кланах
                {
                    //print("оба ии в клане");
                    if (clan == otherman.GetComponent<CleverAI>().clan)//мы в одном клане
                    {
                       // print("ии в одном клане");
                        reaction = 0;//будем игнорировать
                    } 
                    else//мы в разных кланах
                    {
                        print("ии в разных кланах");
                        if (Random.Range(0, 10) < sociability) reaction = 2;//убегаем
                        else reaction = 3;//атакуем
                    }
                }
            }
            else//втретили игрока
            {
                if (clan == otherman.GetComponent<PlayerMove>().ClanOfPlayer)//мы уже в клане игрока
                {
                    reaction = 0;//игнорируем
                }
                else//мы не в клане игрока
                {
                    if (Random.Range(0, 10) < sociability) reaction = 2;//убегаем
                    else reaction = 3;//атакуем
                }
            }
              
            
        }
        if(friendly||(reactiononplayer==4&&otherman.tag=="Player"))//в процессе решили дружить
        {
            if(otherman.tag=="Player")
            {
                if (clan == null)
                {
                    reactiononplayer = 4;
                    otherman.GetComponent<Friendship>().request(gameObject);
                }
            }
            else if (friendly)
            {
                otherman.GetComponent<CleverAI>().request(gameObject);
            }

        }
        else if(reaction==5)//не подружились и реакция не однозначна
        {
            GetEnemy(otherman, 6);
        }
        else//определилась другая конкретная реакция
        {
            if (reaction == 1)//т.е. нужно убегать
            {
                AddTask(new Task(Action.RunFrom, otherman, 6));
            }
            else if (reaction == 2)
            {
                AddTask(new Task(Action.RunFor, otherman, 6));
            }
            
        }      

    }
    public void request(GameObject friend)//заявка на дружбу
    {
        print(gameObject + "получил заявку от " + friend.name);
        if (Random.Range(0, 10) < sociability)//принимаем
        {
            if (clan == null)//если пришла заявка, а мы не в клане
            {
                print(gameObject + "не в клане, обнуляем его и добавляем");
                if (friend.GetComponent<CleverAI>().clan == null)//приславший заявку не состоит в клане, но хочет создать свой
                {
                    clan = new Clan(friend);//в этом случае создаем для него клан 
                    friend.GetComponent<CleverAI>().clan = clan;//записываем для него его же клан
                }
                clan = friend.GetComponent<CleverAI>().clan;//и только теперь добавляемся в его клан
                ResetAllTasks(gameObject);
                clan.AddToClan(gameObject);
                Creator.ChangeInClans();
                
            }
            else//мы уже в клане и пришла заявка от игрока без клана
            {
                print(gameObject + "добавляем в свой клан"+friend.name+"и обнуляет");
                CleverAI cleverfriend = GetComponent<CleverAI>();
                cleverfriend.clan = clan;//и только теперь добавляем его в наш клан
                ResetAllTasks(friend);
                clan.AddToClan(friend);
                Creator.ChangeInClans();
            }
        }
    }
    public void GetEnemy(GameObject newenemy, int newenemypriority)//рядом враг
    {
        int reaction = CreatorRef.Response(priority, newenemypriority);
        //print("Responce" + priority + newenemypriority);
        if (priority==6&&clan == null && newenemy.tag == "Player"&&reactiononplayer!=5)
        {
            print("реакция из памяти");
            reaction = reactiononplayer;
            
        }
             
        
        if(reaction==1)//т.е. нужно убегать
        {
            AddTask(new Task(Action.RunFrom, newenemy, newenemypriority));
        }
        else if(reaction==2)//нападать
        {
            AddTask(new Task(Action.RunFor, newenemy, newenemypriority));
        }

        if (priority == 6&&clan == null && newenemy.tag == "Player" && reactiononplayer == 5)
        {
            reactiononplayer = reaction;
        }
        // if(priority==6) print(gameObject+"работает с " + newenemy + " ответ " + reaction);

    }
    public void TakeDamage(GameObject killer, int enemydamage)
    {
        
        if (currenttask.action != Action.Dead)//если не мертвый       
        {
            if (priority != 1 && currenttask.action != Action.Attack)//не заяц
            {
                Tasks.Push(currenttask);
                if (killer.tag == "Player")
                {
                    currenttask = new Task(Action.RunFor, killer, killer.GetComponent<PlayerMove>().priority);
                    runuptask();
                    reactiononplayer = 2;
                }
                else
                {
                    currenttask = new Task(Action.RunFor, killer, killer.GetComponent<CleverAI>().priority);
                    runuptask();
                }
            
            }                    
            hp -= Random.Range(enemydamage, enemydamage + 10);
            if (clan != null)
            {
                if(killer==clan.Leader)
                {
                    clan.DeleteFromClan(gameObject);
                    clan = null;
                    Creator.ChangeInClans();
                }
                
            }
            if (hp <= 0)
            {
                AddTask(new Task(Action.Dead, null, 0));
                priority = 3;
                CreatorRef.SomebodyDead(gameObject);

                if (clan != null)
                {
                    clan.DeleteFromClan(gameObject);
                    clan = null;
                    Creator.ChangeInClans();
                }                             
                
            }            
        }
    }
    public void ResetAllTasks(GameObject newlifer)
    {
        CleverAI mind = newlifer.GetComponent<CleverAI>();
        mind.CompleteTask();
        mind.currenttask = new Task(Action.Default, gameObject, priority);
        mind.Tasks.Clear();
        mind.AddTask(new Task(Action.Friend, clan.Leader, 6));
    }
}

