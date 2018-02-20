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
    int id = 0;

    void Start () {
        
        ResponceMatrix = new int[6, 7] { { 0, 0, 0, 0, 0, 0, 0 },//кто-строка, на кого-столбец
                                         { 0, 5, 0, 1, 0, 2, 0 },
                                         { 3, 2, 5, 4, 0, 3, 0 },
                                         { 9, 8, 10, 7, 5, 6, 1 },
                                         { 8, 8, 10, 7, 4, 5, 3 },
                                         { 9, 9, 10, 8, 5, 5, 5} };
        
       for (int i = 0; i < 6; i++) { CreateSomebody(rabbitpref); }
        for (int i = 0; i < 5; i++) { CreateSomebody(stagpref); }
        for (int i = 0; i < 4; i++) { CreateSomebody(boarpref); }
        for (int i = 0; i < 3; i++) { CreateSomebody(wolfpref); }
        for (int i = 0; i < 2; i++) { CreateSomebody(bearpref); }
        
    }
	
    public int[] StartInformation(GameObject ai)
    {
        int[] info = new int[3] ;
        if (ai.tag == "Rabbit")
        {
            info[0] = 1;
            info[1] = 100;
            info[2] = 0;
        }
        else if (ai.tag == "Stag")
        {
            info[0] = 2;
            info[1] = 300;
            info[2] = 20;
        }
        else if (ai.tag == "Boar")
        {
            info[0] = 4;
            info[1] = 500;
            info[2] = 40;
        }
        else if (ai.tag == "Wolf")
        {
            info[0] = 5;
            info[1] = 600;
            info[2] = 60;
        }
        else if (ai.tag == "Bear")
        {
            info[0] = 7;
            info[1] = 1000;
            info[2] = 100;
        }
        return info;
    }


    public void SomebodyDead(GameObject somebody)
    {
        if (somebody.tag == "Rabbit") CreateSomebody(rabbitpref);
        else if (somebody.tag == "Stag") CreateSomebody(stagpref); 
        else if (somebody.tag == "Boar") CreateSomebody(boarpref); 
        else if (somebody.tag == "Wolf") CreateSomebody(wolfpref); 
        else if (somebody.tag == "Bear") CreateSomebody(bearpref); 
    }
    void CreateSomebody(GameObject prefub)
    {
        GameObject a = Instantiate(prefub, FindPoint(), Quaternion.identity); a.name += id; id++;
    }
    public bool Response(int who, int whom)
    {
        if (who > 3) who--;
        if (ResponceMatrix[who - 1, whom - 1] > Random.Range(0, 10)) return true;
        else return false;
        
    }
    public Vector3 FindPoint()//поиск точки на меше, куда возможно дойти
    {
        int radius = 10;
        
        while(true)
        {
            Vector3 startpoint = new Vector3(Random.Range(-240, 240), 0, Random.Range(-240, 240));
            Vector3 pointwithR = startpoint + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            for (int i = 0; i < 10; i++)
            {
                if (NavMesh.SamplePosition(pointwithR, out hit, 1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
        }
        
    }
}
