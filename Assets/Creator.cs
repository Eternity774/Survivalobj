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
        
        ResponceMatrix = new int[6, 8] { { 0, 0, 0, 0, 0, 0, 0, 0 },
                                         { 0, 0, 5, 1, 0, 0, 2, 2 },
                                         { 5, 0, 1, 2, 1, 0, 4, 4 },
                                         { 10, 5, 8, 3, 4, 2, 6, 6 },
                                         { 10, 10, 6, 5, 3, 3, 8, 8 },
                                         { 10, 10, 9, 8, 6, 2, 0, 0 } };
        //  for(int i =0;i<5;i++) Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        Instantiate(rabbitpref, FindPoint(), Quaternion.identity);
        Instantiate(wolfpref, FindPoint(), Quaternion.identity);
        Instantiate(stagpref, FindPoint(), Quaternion.identity);
        Instantiate(boarpref, FindPoint(), Quaternion.identity);
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
            return true;
        }
        else
        {            
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
