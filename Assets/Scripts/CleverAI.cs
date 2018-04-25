using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CleverAI : MonoBehaviour {
    NavMeshAgent nav;//агент, который перемещает ai в указанную точку
    Animator anim;//аниматор, для переключения анимаций
    Creator CreatorRef;//ссылка на создателя
    public Vector3 target;
    Task currenttask;
    public enum Task//перечисление возможных задач
    {
        Default,//патрулирование
        RunFrom,
        RunFor,
        Attack,
        Eat,
        Friend,
        Dead
    }
    Queue<Task> Tasks;//очередь для выполнения
   // float ControlDistance = null;
    void Start () {
        AddTask(Task.Default);
	}
	
	
	void Update () {
		switch (currenttask)
        {
            case Task.Default:
                {
                    if (Vector3.Distance(target, gameObject.transform.position) < 1)
                    {
                        StartCoroutine(Wait());//запускаем корутину ожидания
                    }
                    break;
                }
        }
	}
    void AddTask(Task newtask)
    {
        if(newtask==Task.Default)
        {
           currenttask = Task.Default;
            StartCoroutine(Wait());
        }
    }
    IEnumerator Wait()//корутина ожидания
    {

        nav.SetDestination(gameObject.transform.position);//задаем новой точкой текущие координаты          
        anim.SetBool("Run", false);//выключаем бег
        anim.SetBool("Walk", false);//выключаем анимацию ходьбы
        Vector3 temp = CreatorRef.FindPoint();
        yield return new WaitForSeconds(10f);//ждем 10 секунд               
        nav.SetDestination(temp);//задаем новую точку для движения в пределах плоскости
        nav.speed = 1;       //включаем низкую скорость для ходьбы
        anim.SetBool("Walk", true);//включаем анимацию ходьбы
        target = temp;          
    }
}
