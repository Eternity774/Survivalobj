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

    public enum Action//перечисление возможных задач
    {
        Default,//патрулирование   
        Friend,
        RunFor,
        Eat,   
        RunFrom,
        Attack,
        Dead
    }
    Stack<Task> Tasks = new Stack<Task>();//очередь для выполнения
   
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
                            Debug.Log("враг далеко, завершаю"+gameObject);
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

                        if (distance >= 2 && distance <= 30 && Vector3.Distance(nav.destination, gameObject.transform.position) < 4)
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
                                CompleteTask();
                                if (priority > 3) AddTask(new Task(Action.Eat, currenttask.target, currenttask.targetpriority));
                            }
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
                    if (clan != null)
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
                    }
                    else CompleteTask();
                    break;
                }
        }
	}
    public void AddTask(Task newtask)
    {
        Debug.Log("добавляем задание" + newtask.action + gameObject);
       if (Compare(newtask,currenttask))
        {
            Debug.Log("это задание выше");
            Tasks.Push(currenttask);
            currenttask = newtask;
            runuptask();
        }
        else
        {
            Debug.Log("это задание ниже");
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
        print("завершаем задание " + currenttask.action + gameObject);
        while(Tasks.Count>0)
        {
            Task temptask = Tasks.Pop();

            if (temptask.action == Action.Default)//если дефолтное состояние, то оно всегда актуально
            {
                currenttask = temptask;
                currenttask.target = gameObject;
                break;
            }
            else if (temptask.target != null)//след. в стэке состояние не дефолтное
            {
                print("цель не ноль");
                if (Vector3.Distance(gameObject.transform.position, temptask.target.transform.position) < 40)
                {
                    print("цель еще актуальна");
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
        anim.SetBool("Eat", false);
        anim.SetBool("Attack", false);

        print("новое задание у " + gameObject);
        switch (currenttask.action)
        {
            case Action.Default:
                {                    
                    nav.speed = 0;
                    nav.SetDestination(CreatorRef.FindPoint());//задаем новую точку для движения в пределах плоскости
                    StartCoroutine(Wait());
                    break;
                }
            case Action.Friend:
                {
                    anim.SetTrigger("Hello");
                    break;                    
                }
            case Action.RunFrom:
                {
                    Debug.Log("начинаем убегать");
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
                    Debug.Log("начинаем догонять");
                    nav.SetDestination(currenttask.target.transform.position);
                    nav.speed = Random.Range(5, 8);
                    anim.SetBool("Run", true);
                    break;
                } 
            case Action.Attack:
                {
                    Debug.Log("начинаем атаковать");
                    nav.SetDestination(currenttask.target.transform.TransformPoint(Vector3.forward * 2));
                    transform.LookAt(currenttask.target.transform.position);
                    anim.SetBool("Attack", true);
                    break;
                }
            case Action.Eat:
                {
                    Debug.Log("начинаем есть");
                    nav.SetDestination(currenttask.target.transform.TransformPoint(Vector3.forward * 2));                    
                    anim.SetBool("Eat", true);
                    StartCoroutine(Wait());
                    transform.LookAt(currenttask.target.transform.position);
                    break;
                }
            case Action.Dead:
                {
                    Debug.Log("умираем");
                    anim.SetTrigger("Death");
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
        if (currenttask.action != Action.Dead && anim != null && currenttask.target!=other.gameObject)
        {
            //Debug.Log(other.name + currenttask.target.name);
            if (other.GetComponent<CleverAI>() != null || other.GetComponent<PlayerMove>() != null)
            {
                int newenemypriority;//приоритет нового врага
                if (other.gameObject.tag == "Player") newenemypriority = 6;//у игрока приоритет всегда 6
                else newenemypriority = other.gameObject.GetComponent<CleverAI>().priority;//узнаем приоритет нового врага

                if (priority == 6 && newenemypriority == 6) People(other.gameObject);//встретились 2 человека
                else GetEnemy(other.gameObject, newenemypriority);
            }
        }
    }

    public void People(GameObject otherman)//втретились 2 человека
    {
        int reaction = 1;//по умолчанию убегаем
        bool friendly = false;//будем ли сотрудничать
        if(clan==null)//если аи не в клане
        {
            if (Random.Range(0, 10) < sociability) friendly=true;//предлагаем дружбу  
                      
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
                    if (clan == otherman.GetComponent<CleverAI>().clan)//мы в одном клане
                    {
                        reaction = 0;//будем игнорировать
                    }
                    else//мы в разных кланах
                    {
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
        if(friendly)//в процессе решили дружить
        {
            if(otherman.tag=="Player")
            {
                otherman.GetComponent<PlayerMove>().request(gameObject);
            }
            else
            {
                otherman.GetComponent<CleverAI>().request(gameObject);
            }
        }
        else if(reaction==0)//не подружились и реакция не однозначна
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
        if (Random.Range(0, 10) < sociability)//принимаем
        {
            clan = friend.GetComponent<CleverAI>().clan;
            clan.AddToClan(gameObject);
            Creator.ChangeInClans();
            AddTask(new Task(Action.Friend, clan.Leader, 6));
        }
    }
    public void GetEnemy(GameObject newenemy, int newenemypriority)//рядом враг
    {      
        int reaction = CreatorRef.Response(priority, newenemypriority);
        
        if(reaction==1)//т.е. нужно убегать
        {
            AddTask(new Task(Action.RunFrom, newenemy, newenemypriority));
        }
        else if(reaction==2)//нападать
        {
            AddTask(new Task(Action.RunFor, newenemy, newenemypriority));
        }
        print(gameObject+"работает с " + newenemy + " ответ " + reaction);
        
    }
    public void TakeDamage(GameObject killer, int enemydamage)
    {
        if (currenttask.action != Action.Dead)//если не мертвый       
        {
            if (priority != 1 && currenttask.action != Action.Attack)//не заяц
            {
                if (killer.tag == "Player")
                {
                   AddTask(new Task(Action.Attack, killer, killer.GetComponent<PlayerMove>().priority));
                }
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

