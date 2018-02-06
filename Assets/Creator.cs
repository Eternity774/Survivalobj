using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour {
    
    public GameObject rabbitpref;
    public GameObject wolfpref;
    public GameObject stagpref;
    int[,] ResponceMatrix;

    void Start () {
        
        ResponceMatrix = new int[6, 8] { { 0, 0, 0, 0, 0, 0, 0, 0 },
                                         { 0, 0, 5, 1, 0, 0, 2, 2 },
                                         { 5, 5, 1, 2, 1, 0, 4, 4 },
                                         { 10, 10, 8, 3, 4, 2, 6, 6 },
                                         { 10, 10, 6, 5, 3, 3, 8, 8 },
                                         { 10, 10, 9, 8, 6, 2, 0, 0 } };
        //  for(int i =0;i<5;i++) Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        Instantiate(wolfpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        Instantiate(stagpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
    }
	
    public int Priority(GameObject ai)//функция для определения приоритета каждого ai
    {
        if (ai.tag == "Rabbit") return 1;
        else if (ai.tag == "Stag") return 2;
        else if (ai.tag == "Wolf") return 5;
        else return 0;
    }
    public void SomebodyDead(GameObject somebody)
    {
        if (somebody.tag == "Rabbit") Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        else if (somebody.tag == "Stag") Instantiate(stagpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        else if (somebody.tag == "Wolf") Instantiate(wolfpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
    }
    public bool Response(int who, int whom)
    {
        Debug.Log("response from " + who + " for" + whom);
        int r = Random.Range(0, 10);
        int resp = (ResponceMatrix[who - 1, whom - 1]);
        Debug.Log("Random4ik:"+r+" Resp:"+resp);

        if (ResponceMatrix[who - 1, whom - 1] > r)
        {
            Debug.Log("answer true");
            return true;
        }
        else
        {
            Debug.Log("answer false");
            return false;
        }

    }
}
