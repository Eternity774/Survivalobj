using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CleverAI : MonoBehaviour {
    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    Creator CreatorRef;//ссылка на создателя    
    
    Task currenttask;
    public Clan clan;
    int sociability;
    public int priority;//приоритет для разрешения столкновения нескольких объектов
    public int hp;//здоровье
    public int damage;//наносимый урон


    struct Task
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

    public enum Action//перечисление возможных задач
    {
        Default,//патрулирование   
        Friend,
        RunFor,
        Eat,        
        Attack,
        RunFrom,
        Dead
    }
    Stack<Task> Tasks;//очередь для выполнения
   
    void Start () {
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
        
        
    }
	
	
	void Update () {
		switch (currenttask.action)
        {
            case Action.Default:
                {
                    if (Vector3.Distance(currenttask.target.transform.position, gameObject.transform.position) < 2)
                    {
                        runuptask();
                    }
                    break;
                }
            case Action.RunFrom:
                {
                    if (currenttask.target != null)
                    {
                        if (Vector3.Distance(gameObject.transform.position,currenttask.target.transform.position) > 40) CompleteTask();
                        else if (Vector3.Distance(nav.destination, gameObject.transform.position) < 2)//продолжаем убегать
                        {
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
                        float distance = Vector3.Distance(transform.position, currenttask.target.transform.position);//мереем дистанцию
                        if (distance >= 2 && distance <= 30 && Vector3.Distance(nav.destination, gameObject.transform.position) < 2)
                        {
                            nav.SetDestination(currenttask.target.transform.position);//направляем агента на врага
                        }
                        
                        else if (distance < 2)//когда дистанция сократилась
                        {
                            AddTask(new Task(Action.Attack, currenttask.target, currenttask.targetpriority));
                        }
                        else if (distance > 30) CompleteTask();
                    }
                    else CompleteTask();
                    break;
                }
            case Action.Attack:
                {
                    if (currenttask.target != null)
                    {
                        if (Vector3.Distance(transform.position, currenttask.target.transform.position) > 3)//если при атаке оказались далеко от объекта
                        {
                            CompleteTask();//при завершении в стеке найдется предшествующее действие (runfor или runfrom)
                        }

                    }
                    else CompleteTask();
                    break;
                }
            case Action.Eat:
                {
                    if (currenttask.target == null) CompleteTask();
                    break;
                }
            case Action.Friend:
                {
                    float distance = Vector3.Distance(transform.position, clan.Leader.transform.position);//мереем дистанцию
                    if (distance >= 2 && Vector3.Distance(nav.destination, gameObject.transform.position) < 2) nav.SetDestination(clan.Leader.transform.position);//направляем агента за лидером   

                    if (distance > 20)
                    {
                        anim.SetBool("Run", true);
                        nav.speed = 8;
                    }

                    if (distance < 8 && distance >= 2)//когда дистанция сократилась
                    {
                        anim.SetBool("Walk", true);
                        anim.SetBool("Run", false);
                        nav.speed = 1;

                    }
                    if (distance < 2)
                    {
                        anim.SetBool("Walk", false);
                        nav.ResetPath();
                    }
                    break;
                }
        }
	}
    void AddTask(Task newtask)
    {
       
       if (Compare(newtask,currenttask))
        {
            Tasks.Push(currenttask);
            currenttask = newtask;
            runuptask();
        }
        else
        {
            Stack<Task> temp = new Stack<Task>();
            temp.Push(Tasks.Pop());
            while (Tasks.Count > 0)
            {
                              
                if (Compare(newtask,Tasks.Peek()))
                {
                    nav.ResetPath();
                    StopAllCoroutines();
                    currenttask = newtask;
                    while (temp.Count >= 0)
                    {
                        Tasks.Push(temp.Pop());
                    }
                    break;

                }
            }
        }
    }
    void CompleteTask()
    {
        
        while(Tasks.Count>0)
        {
            Task temptask = Tasks.Pop();
            if (temptask.target != null)
            {
                if (Vector3.Distance(gameObject.transform.position, temptask.target.transform.position) < 40)
                {
                    currenttask = temptask;
                    break;
                }
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
        anim.SetBool("Attack", false);

        switch (currenttask.action)
        {
            case Action.Default:
                {
                    currenttask.target.transform.position = CreatorRef.FindPoint();
                    nav.speed = 0;
                    nav.SetDestination(currenttask.target.transform.position);//задаем новую точку для движения в пределах плоскости
                    StartCoroutine(Wait());
                    break;
                }
            case Action.RunFrom:
                {
                    
                    Vector3 forwardPosition = transform.TransformPoint(Vector3.forward * 8);//переводим в глобальные координаты направление вперед
                    nav.SetDestination(forwardPosition);
                    anim.SetBool("Run", true);
                    break;
                }
            case Action.RunFor:
                {
                    
                    nav.SetDestination(currenttask.target.transform.position);
                    anim.SetBool("Run", true);
                    break;
                } 
            case Action.Attack:
                {
                    
                    nav.SetDestination(currenttask.target.transform.TransformPoint(Vector3.forward * 2));
                    transform.LookAt(currenttask.target.transform.position);
                    anim.SetBool("Attack", true);
                    break;
                }
            case Action.Eat:
                {
                    
                    nav.SetDestination(currenttask.target.transform.TransformPoint(Vector3.forward * 2));
                    transform.LookAt(currenttask.target.transform.position);
                    anim.SetBool("Eat", true);
                    StartCoroutine(Wait());
                    break;
                }
            case Action.Dead:
                {
                    anim.SetTrigger("Deth");
                    Tasks.Clear();
                    break;
                }
        }

    }



    IEnumerator Wait()//корутина ожидания
    {

        yield return new WaitForSeconds(10f);//ждем 10 секунд       
        if (currenttask.action == Action.Default)
        {
            nav.speed = 1;       //включаем низкую скорость для ходьбы
            anim.SetBool("Walk", true);//включаем анимацию ходьбы
        }
        else if(currenttask.action == Action.Eat)
        {
            Destroy(currenttask.target);
        }
    }
                
    
    private void OnTriggerEnter(Collider other)//для обработки взаимодействий (рядом игрок)
    {
        if (currenttask.action != Action.Dead && anim != null)
        {
            if (other.GetComponent<CleverAI>() != null || other.GetComponent<PlayerMove>() != null)
            {
                int newenemypriority;//приоритет нового врага
                if (other.gameObject.tag == "Player") newenemypriority = 6;//у игрока приоритет всегда 6
                else newenemypriority = other.gameObject.GetComponent<CleverAI>().priority;//узнаем приоритет нового врага

                if (priority == 6 && newenemypriority == 6) people(other.gameObject);//встретились 2 человека
                else GetEnemy(other.gameObject, newenemypriority);
            }
        }
    }

    public void people(GameObject otherman)
    {
        int reaction = 0;
        if(clan==null)
        {
            if (Random.Range(0, 10) < sociability) reaction = 3;
        }


        /*if (priority == 6 && newenemypriority == 6)//встретились 2 человека
        {
            bool friendly = false;
            if (clan != null)//мы в клане
            {
                if (newenemy.tag == "Player")//мы вcтретили игрока
                {
                    if (clan.Leader == newenemy)
                    {
                        friendly = true;
                        Debug.Log(gameObject.name + "я уже был в клане игрока");
                    }
                    else Debug.Log(gameObject.name + "я не в клане игрока");
                }
                else if (newenemy.GetComponent<CleverAI>().clan != null)//мы встретили не игрока
                {
                    if (newenemy.GetComponent<CleverAI>().clan == clan)
                    {
                        friendly = true;
                        Debug.Log(gameObject.name + "я в том же клане, что и этот аи");
                    }
                    else Debug.Log(gameObject.name + "я в другом клане");

                }
                else//т.е. мы в клане, а он - нет
                {
                    if (Random.Range(0, 10) < sociability)//дружелюбный исход
                    {
                        if (newenemy.GetComponent<CleverAI>().currenttask.target != null)//у нашего объекта есть враг
                        {
                            if (newenemy.GetComponent<CleverAI>().currenttask.target.tag == "Player")//если враг объекта игрок
                            {
                                if (clan.Leader.name == "Player") friendly = false;//и мы в клане игрока
                                else friendly = true;
                            }
                        }
                        else
                        {
                            friendly = true;
                        }
                        //добавить в клан!!!
                        if (friendly)
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
                    if (Random.Range(0, 10) < sociability)//дружелюбный исход
                    {
                        friendly = true;//присоединть к клану игрока
                        clan = newenemy.GetComponent<PlayerMove>().ClanOfPlayer;
                        clan.AddToClan(gameObject);
                        Debug.Log(gameObject.name + "Я хочу в клан игрока!");
                    }
                    else Debug.Log(gameObject.name + "я не хочу в клан игрока");

                }
                else if (newenemy.GetComponent<CleverAI>().clan != null)//встретили не игрока уже в клане
                {
                    if (Random.Range(0, 10) < sociability)//дружелюбный исход
                    {
                        friendly = true;//присоединиться к клану другого аи
                        clan = newenemy.GetComponent<CleverAI>().clan;
                        clan.AddToClan(gameObject);
                        Debug.Log(gameObject.name + "я пойду в клан этого аи");
                    /*    if (currenttask.target != null)//случай, когда нужно вступить в клан и сбросить из него врагов?!
                        {
                            if (currenttask.target.tag != "Player")
                            {
                                if (currenttask.target.GetComponent<CleverAI>().clan != null)
                                {
                                    if (currenttask.target.GetComponent<CleverAI>().clan == newenemy.GetComponent<CleverAI>().clan) enemy = null;
                                }
                            }
                            else if (enemy.GetComponent<PlayerMove>().ClanOfPlayer == clan) enemy = null;
                        }
                    }
                    
                   // else Debug.Log(gameObject.name + "я не пойду в клан этого аи");
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

            if (friendly)//если решили дружить
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
                    else if (clan.Leader != gameObject)//если мы не лидер клана
                    {
                        nav.ResetPath();
                        state = State.friend;
                        StopAllCoroutines();
                    }

                }
            }


        }
        */
        if (reaction == 0)//т.е. выпал игнор
        {
            if (Random.Range(0, 10) < sociability) reaction = 3;
        }

    }
    public void GetEnemy(GameObject newenemy, int newenemypriority)//рядом враг
    {      
        int reaction = CreatorRef.Response(priority, newenemypriority);
        
        if(reaction==1)//т.е. нужно убегать
        {
            AddTask(new Task(Action.RunFrom, gameObject, newenemypriority));
        }
        else if(reaction==2)
        {
            AddTask(new Task(Action.RunFor, newenemy, newenemypriority));
        }
        
        
    }
    public void TakeDamage(GameObject killer, int enemydamage)
    {
        if (currenttask.action != Action.Dead)//если не мертвый       
        {
            if (priority != 1)//не заяц
            {
                if (killer.tag == "Player") AddTask(new Task(Action.Attack, killer, killer.GetComponent<PlayerMove>().priority));
                else AddTask(new Task(Action.Attack, killer, killer.GetComponent<CleverAI>().priority));
            
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
}

