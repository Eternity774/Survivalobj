using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CleverAI : MonoBehaviour {
    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    Creator CreatorRef;//ссылка на создателя    
    public int priority;
    Task currenttask;
   
    struct Task
    {
        public Action action;
        public Vector3 target;
        int enemypriority;

        public Task(Action action, Vector3 target, int enemypriority)
        {
            this.action = action;
            this.target = target;
            this.enemypriority = enemypriority;
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
    List<Task> Tasks;//очередь для выполнения
   // float ControlDistance = null;
    void Start () {
        CreatorRef = GameObject.Find("MainController").GetComponent<Creator>();//находим контроллер на сцене
        nav = GetComponent<NavMeshAgent>();//берем компоненты с того же объекта где и скрипт (навмеш)
        anim = GetComponent<Animator>();//берем компонент аниматора
        currenttask = new Task(Action.Default, gameObject.transform.position, 0);
        Tasks.Add(currenttask);
	}
	
	
	void Update () {
		switch (currenttask.action)
        {
            case 0:
                {
                    if (Vector3.Distance(currenttask.target, gameObject.transform.position) < 2)
                    {
                        StartCoroutine(Wait());//запускаем корутину ожидания
                    }
                    break;
                }
        }
	}
    void AddTask(Task newtask)
    {
        if(newtask.action<currenttask.action)
        {
            Tasks.Add(newtask);
        }
        else
        {
            nav.ResetPath();
            Tasks.Add(newtask);
            Tasks.Sort(Action);
        }
        /*
        if(newtask==Task.Default)
        {
           Debug.Log("Default task is Add");
           currenttask = Task.Default;
           StopAllCoroutines();
           StartCoroutine(Wait());//запускаем корутину ожидания
        }
        */
    }
    IEnumerator Wait()//корутина ожидания
    {
        Debug.Log("Courutine is start");
        nav.ResetPath();
        anim.SetBool("Run", false);//выключаем бег
        anim.SetBool("Walk", false);//выключаем анимацию ходьбы
        
        currenttask.target = CreatorRef.FindPoint();
        yield return new WaitForSeconds(10f);//ждем 10 секунд               
        nav.SetDestination(currenttask.target);//задаем новую точку для движения в пределах плоскости
        nav.speed = 1;       //включаем низкую скорость для ходьбы
        anim.SetBool("Walk", true);//включаем анимацию ходьбы
                
    }
    private void OnTriggerEnter(Collider other)//для обработки взаимодействий (рядом игрок)
    {
        if (currenttask.action != Action.Dead && anim != null)
        {
            if (other.GetComponent<Behavior>() != null || other.GetComponent<PlayerMove>() != null)
            {
                GetEnemy(other.gameObject);
            }
        }
    }
    public void GetEnemy(GameObject newenemy)//рядом враг
    {
       int newenemypriority;//приоритет нового врага

       if (newenemy.tag == "Player") newenemypriority = 6;//у игрока приоритет всегда 6
       else newenemypriority = newenemy.GetComponent<CleverAI>().priority;//узнаем приоритет нового врага
       int reaction = CreatorRef.Response(priority, newenemypriority);
        if(reaction==1)//т.е. нужно убегать
        {
            AddTask(new Task(Action.RunFrom, gameObject.transform.position, newenemypriority));
        }
        else if(reaction==2)
        {
            AddTask(new Task(Action.RunFor, newenemy.transform.position, newenemypriority));
        }
        
        }
    }
