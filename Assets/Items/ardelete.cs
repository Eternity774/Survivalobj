using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ardelete : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(fordelete());
	}
	
    public IEnumerator fordelete()
    {
        print("корутина запустилась");
        yield return new WaitForSeconds(2f);
        print("корутина завершилась");
        Destroy(gameObject);

    }
}
