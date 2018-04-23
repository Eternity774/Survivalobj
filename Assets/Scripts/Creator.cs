using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Creator : MonoBehaviour {
    
    public GameObject rabbitpref;
    public GameObject boarpref;
    public GameObject wolfpref;
    public GameObject stagpref;
    public GameObject bearpref;
    public GameObject manpref;

    public static Text countofclans;
    public static Text withoutclans;
    
    public static List<Clan> ListofClans = new List<Clan>();
    public int countai = 20;

    int[,] ResponceMatrix;
    int id = 0;

    void Start () {
        countofclans = GameObject.Find("Clans").GetComponent<Text>();
        withoutclans = GameObject.Find("Free").GetComponent<Text>();


        ResponceMatrix = new int[6, 7] { { 0, 0, 0, 0, 0, 0, 0 },//кто-строка, на кого-столбец
                                         { 0, 5, 0, 1, 0, 2, 0 },
                                         { 3, 2, 5, 4, 0, 3, 0 },
                                         { 9, 8, 10, 7, 5, 6, 1 },
                                         { 8, 8, 10, 7, 5, 5, 3 },
                                         { 9, 9, 10, 8, 5, 5, 5} };
        
        for (int i = 0; i < 15; i++) { CreateSomebody(rabbitpref); }
        for (int i = 0; i < 12; i++) { CreateSomebody(stagpref); }
        for (int i = 0; i < 9; i++) { CreateSomebody(boarpref); }
        for (int i = 0; i < 6; i++) { CreateSomebody(wolfpref); }
        for (int i = 0; i < 3; i++) { CreateSomebody(bearpref); }
        for (int i = 0; i < 20; i++) { CreateSomebody(manpref); }
        ChangeInClans();

    }

    public int[] StartInformation(GameObject ai)
    {
        int[] info = new int[3];
        switch (ai.tag)
        {
            case "Rabbit":
            {
                info[0] = 1;
                info[1] = 100;
                info[2] = 0;
                        break;
            }
                case "Stag":
            {
                info[0] = 2;
                info[1] = 300;
                info[2] = 20;
                        break;
            }
                case "Boar":
            {
                info[0] = 4;
                info[1] = 500;
                info[2] = 40;
                        break;
            }
                case "Wolf":
        
            {
                info[0] = 5;
                info[1] = 600;
                info[2] = 60;
                        break;
            }
                case "Bear":
            {
                info[0] = 7;
                info[1] = 1000;
                info[2] = 100;
                        break;
            }
            case "AIMan":
                {
                    info[0] = 6;
                    info[1] = 700;
                    info[2] = 60;
                    break;
                }
                
    }
        return info;
    }


    public void SomebodyDead(GameObject somebody)
    {
        
        switch (somebody.tag)
        {
            case "Rabbit": CreateSomebody(rabbitpref); break;
            case "Stag": CreateSomebody(stagpref); break;
            case "Boar": CreateSomebody(boarpref); break;
            case "Wolf": CreateSomebody(wolfpref); break;
            case "Bear": CreateSomebody(bearpref); break;
            //case "AIMAn": CreateSomebody(manpref); break;
            case "AIMAn": ChangeInClans(); break;
                        

        }
        
    }
    void CreateSomebody(GameObject prefub)
    {
        GameObject a = Instantiate(prefub, FindPoint(), Quaternion.identity); a.name += id; id++;
        a.GetComponent<NavMeshAgent>().avoidancePriority = Random.Range(0, 100);
    }
    public bool Response(int who, int whom)
    {
        if (who > 3) who--;           
        if (ResponceMatrix[who - 1, whom - 1] > Random.Range(0, 11)) return true;
        else return false;
        
    }
    public Vector3 FindPoint()//поиск точки на меше, куда возможно дойти
    {
        int radius = 10;
       
        while(true)
        {
           
            Vector3 startpoint = new Vector3(Random.Range(-250, 750), 0, Random.Range(-250, 750));
            Vector3 pointwithR = startpoint + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            for (int i = 0; i < 50; i++)
            {
                if (NavMesh.SamplePosition(pointwithR, out hit, 1f, NavMesh.AllAreas))
                {
                    
                    return hit.position;
                }
            }
            
        }
        
    }
    public static void ChangeInClans()
    {
        if (countofclans != null)//текст уже загрузился
        {
            Debug.Log("Действующих кланов: " + ListofClans.Count);
            countofclans.text = "Clans:" + ListofClans.Count;
            GameObject[] temparray = GameObject.FindGameObjectsWithTag("AIMan");
            Debug.Log("АИ игроков: " + temparray.Length);
            int countwithoutclan = 0;
            foreach (GameObject i in temparray)
            {
                if (i.GetComponent<Behavior>().clan == null&&i.GetComponent<Behavior>().state!=Behavior.State.dead) countwithoutclan++;
                
            }
            Debug.Log("АИ игроков без клана: " + countwithoutclan);
            withoutclans.text = "Free:" + countwithoutclan;
            if (countwithoutclan == 0 && ListofClans.Count == 1)
            {
                Debug.Log("You win");
                countofclans.text = "YOU";
                withoutclans.text = "WIN:";
            }
        }
            
    }



}
