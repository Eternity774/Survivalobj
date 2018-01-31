using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour {
    public GameObject rabbitpref;
	// Use this for initialization
	void Start () {
        Instantiate(rabbitpref, new Vector3(Random.Range(-45, 45), 0, Random.Range(-45, 45)), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public static int Priority(GameObject ai)//функция для определения приоритета каждого ai
    {
        if (ai.tag == "Rabit") return 1;
        else return 0;
    }
}
