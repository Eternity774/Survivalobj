using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creator : MonoBehaviour {
    
    public GameObject rabbitpref;
    public GameObject boarpref;
    public GameObject wolfpref;
    public GameObject stagpref;
    public GameObject bearpref;

    int[,] ResponceMatrix;

    void Start () {
        
        ResponceMatrix = new int[6, 7] { { 0, 0, 0, 0, 0, 0, 0 },//кто-строка, на кого-столбец
                                         { 0, 5, 0, 1, 0, 0, 2 },
                                         { 3, 2, 5, 4, 0, 0, 3 },
                                         { 9, 8, 10, 7, 5, 1, 6 },
                                         { 8, 8, 10, 7, 4, 5, 8 },
                                         { 9, 9, 10, 8, 5, 2, 5} };
        //  for(int i =0;i<5;i++) Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        for (int i = 0; i < 6; i++) { Instantiate(rabbitpref, FindPoint(), Quaternion.identity); }
        for (int i = 0; i < 5; i++) { Instantiate(stagpref, FindPoint(), Quaternion.identity); }
        for (int i = 0; i < 4; i++) { Instantiate(boarpref, FindPoint(), Quaternion.identity); }

        for (int i = 0; i < 3; i++) { Instantiate(wolfpref, FindPoint(), Quaternion.identity); }

        Instantiate(bearpref, FindPoint(), Quaternion.identity);
        Instantiate(bearpref, FindPoint(), Quaternion.identity);
    }
	
    public int Priority(GameObject ai)//функция для определения приоритета каждого ai
    {
        if (ai.tag == "Rabbit") return 1;
        else if (ai.tag == "Stag") return 2;
        else if (ai.tag == "Boar") return 4;
        else if (ai.tag == "Wolf") return 5;
        else if (ai.tag == "Bear") return 7;
        else return 0;
    }
    public void SomebodyDead(GameObject somebody)
    {
        if (somebody.tag == "Rabbit") Instantiate(rabbitpref, FindPoint(), Quaternion.identity);
        else if (somebody.tag == "Stag") Instantiate(stagpref, FindPoint(), Quaternion.identity);
        else if (somebody.tag == "Boar") Instantiate(boarpref, FindPoint(), Quaternion.identity);
        else if (somebody.tag == "Wolf") Instantiate(wolfpref, FindPoint(), Quaternion.identity);
        else if (somebody.tag == "Bear") Instantiate(bearpref, FindPoint(), Quaternion.identity);
    }
    public bool Response(int who, int whom)
    {
        if (who > 3) who--;
        Debug.Log("response from " + who + " for" + whom);
        int r = Random.Range(0, 10);
        int resp = (ResponceMatrix[who - 1, whom - 1]);
        Debug.Log("Random4ik:"+r+" Resp:"+resp);
        
        if (ResponceMatrix[who - 1, whom - 1] > r)
        {
            Debug.Log("yes");
            return true;
            
        }
        else
        {
            Debug.Log("no");
            return false;
        }

    }
    public Vector3 FindPoint()
    {
        int radius = 10;
        //Vector3 rezultpoint;
        while(true)
        {
            Vector3 startpoint = new Vector3(Random.Range(-240, 240), 0, Random.Range(-240, 240));
            Vector3 pointwithR = startpoint + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            for (int i = 0; i < 10; i++)
            {
                if (NavMesh.SamplePosition(pointwithR, out hit, 1f, NavMesh.AllAreas))
                {
                    //rezultpoint = ;
                    return hit.position;
                }
            }
        }
        
    }
}
