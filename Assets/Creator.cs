using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour {
    
    public GameObject rabbitpref;
    public GameObject wolfpref;
    
    void Start () {

        for(int i =0;i<10;i++) Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);

//        Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        Instantiate(wolfpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
    }
	
    public int Priority(GameObject ai)//функция для определения приоритета каждого ai
    {
        if (ai.tag == "Rabbit") return 1;
        if (ai.tag == "Wolf") return 5;
        else return 0;
    }
    public void SomebodyDead(GameObject somebody)
    {
        if (somebody.tag == "Rabbit") Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
        if (somebody.tag == "Wolf") Instantiate(wolfpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
    }
}
