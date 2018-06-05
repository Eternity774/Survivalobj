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

    public GameObject rock;
    public GameObject wood;

    public GameObject player;
    public GameObject arrow;
    public GameObject way;

    public static Text countofclans;
    public static Text withoutclans;
    
    public static List<Clan> ListofClans = new List<Clan>();
    public int countai = 20;

    int[,,] ResponceMatrix;
    int id = 0;

    void Start()
    {
        countofclans = GameObject.Find("Clans").GetComponent<Text>();
        withoutclans = GameObject.Find("Free").GetComponent<Text>();

        //кто-строка, на кого-столбец (в ячейках вероятности)
        //{игнора, убегания, нападения}
        ResponceMatrix = new int[6, 6, 3] { { {10,0,0}, {10,0,0}, {5,5,0}, {0,10,0}, {0,10,0}, {0,10,0} },
                                         { {10,0,0}, {4,2,4}, {5,3,2}, {0,0,10}, {0,8,2}, {0,10,0} },
                                         { {7,0,3}, {8,1,1},{5,2,3}, {0,9,1}, {1,6,3}, {0,10,0}},
                                         { {1,0,9}, {2,0,8}, {3,1,6}, {4,3,3},{0,3,7}, {0,9,1} },
                                         { {1,0,9}, {2,0,8}, {3,2,5}, {0,7,3}, {6,2,2}, {0,9,1} },
                                         { {5,0,5}, {5,0,5}, {5,0,5}, {4,2,4}, {0,3,7}, {4,3,3}} };

        /*  
          ResponceMatrix = new int[6, 7] { { 0, 0, 0, -5, -10, -10, -10 },//кто-строка, на кого-столбец
                                           { 0, 5, 0, 1, 0, 2, 0 },
                                           { 3, 2, 5, 4, 0, 3, 0 },
                                           { 9, 8, 10, 7, 5, 6, 1 },
                                           { 8, 8, 10, 7, 5, 5, 3 },
                                           { 9, 9, 10, 8, 5, 5, 5} };
        */
        for (int i = 0; i < 15; i++) { CreateSomebody(manpref); }
        for (int i = 0; i < 10; i++) { CreateSomebody(rabbitpref); }
        for (int i = 0; i < 10; i++) { CreateSomebody(stagpref); }
        for (int i = 0; i < 8; i++) { CreateSomebody(boarpref); }
        for (int i = 0; i < 6; i++) { CreateSomebody(wolfpref); }
        for (int i = 0; i < 3; i++) { CreateSomebody(bearpref); }

        for (int i = 0; i < 50; i++) { Instantiate(rock, FindPoint(), Quaternion.identity); }
        for (int i = 0; i < 50; i++) { Instantiate(wood, FindPoint(), Quaternion.identity); }

        ChangeInClans();
        //print("разбор матрицы:"+ResponceMatrix[1,5,0]);
      
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
            case "AIMan": ChangeInClans(); break;
                        

        }
        
    }
    void CreateSomebody(GameObject prefub)
    {
        GameObject a = Instantiate(prefub, FindPoint(), Quaternion.identity);
        if (prefub.tag == "AIMan") a.name = "ai" + id;
        else a.name += id;
        id++;
        a.GetComponent<NavMeshAgent>().avoidancePriority = Random.Range(0, 100);
    }
    public int Response(int who, int whom)
    {
        //Debug.Log("Поступил запрос" + who + whom);
        //if (ResponceMatrix[who - 1, whom - 1] > Random.Range(0, 11)) return true;
        //  else return false;
       
        if (who > 3) who--;
        if (whom > 3) whom--;
        //print("Responce" + who + whom);
        int ignore = ResponceMatrix[who - 1, whom - 1, 0];
        int random4ik = Random.Range(1, 11);
        if (random4ik <= ignore) return 0;
        int runfrom = ResponceMatrix[who - 1, whom - 1, 1];
        if (random4ik <= runfrom+ignore) return 1;
        else return 2;
    }
    public Vector3 FindPoint()//поиск точки на меше, куда возможно дойти
    {
        int radius = 10;
       
        while(true)
        {

            Vector3 startpoint = new Vector3(Random.Range(-250, 750), 0, Random.Range(-250, 750));
           // Vector3 startpoint = new Vector3(Random.Range(500, 600), 0, Random.Range(-150, 0));
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
            //Debug.Log("Действующих кланов: " + ListofClans.Count);
            countofclans.text = "Clans:" + ListofClans.Count;
            GameObject[] temparray = GameObject.FindGameObjectsWithTag("AIMan");
            //Debug.Log("АИ игроков: " + temparray.Length);
            int countwithoutclan = 0;
            foreach (GameObject i in temparray)
            {
                if (i.GetComponent<CleverAI>().clan == null&&i.GetComponent<CleverAI>().priority!=3) countwithoutclan++;
                
            }
            //Debug.Log("АИ игроков без клана: " + countwithoutclan);
            withoutclans.text = "Free:" + countwithoutclan;
            if (countwithoutclan == 0 && ListofClans.Count == 1)
            {
               // Debug.Log("You win");
                countofclans.text = "YOU";
                withoutclans.text = "WIN!";
            }
        }
            
    }
    public void WhereAI()
    {
        ChangeInClans();

        GameObject otherman = null;

        GameObject[] othermans = GameObject.FindGameObjectsWithTag("AIMan");
        //Debug.Log("найдено " + othermans.Length);
        //countofclans.text = othermans.Length.ToString();
        foreach (GameObject man in othermans)
        {
            //Debug.Log("в цикле " + man.GetComponent<CleverAI>().clan);
            if (man.GetComponent<CleverAI>().clan != player.GetComponent<PlayerMove>().ClanOfPlayer)
            {
                //Debug.Log("условие выполнилось " + othermans.Length);
                otherman = man;
                break;
            }
        }

        if (otherman != null)
        {
            //Debug.Log("найден " + otherman.name);
            //arrow = GameObject.Find("arrow");
            // way = GameObject.Find("Arrowrotator");
            arrow = Instantiate(way, player.transform.position + Vector3.forward * 2, Quaternion.identity);
            //Debug.Log("создан" + arrow.name);
            arrow.transform.LookAt(otherman.transform.position);
            //arrow.AddComponent<Rigidbody>();
            StartCoroutine(fordelete());
            //arrow.SetActive(true);

        }
    }
    public IEnumerator fordelete()
    {
        yield return new WaitForSeconds(2f);
        Destroy(arrow);

    }
}




