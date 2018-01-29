using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior : MonoBehaviour {

    //Random Random4ik = new Random();
    public State state;
    

    public enum State
    {
        wait,
        walk
    }
	// Use this for initialization
	void Start () {
        state = State.wait;
        StartCoroutine(Wait());
	}
	
	// Update is called once per frame
	void Update ()
    {
        
        if(state == State.walk)
        {
            
            Vector3 v = transform.position;
            v.z+=0.05f;
            transform.position = v;
        }
	}
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Rabbit in trigger");
    }
   IEnumerator Wait()
    {
        Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(5f);
        state = State.walk;
    }
}
