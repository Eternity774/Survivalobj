using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireController : MonoBehaviour {

	public static GameObject player;
 
    public Creator Creatorscript;

    private PlayerHealth ph;
	private float powerRegenTimer = 0f;
	void Start(){
		player= GameObject.FindGameObjectWithTag ("Player");
		ph=player.GetComponent<PlayerHealth>();
        StartCoroutine(timeofcamp());
        Creatorscript = GameObject.Find("MainController").GetComponent<Creator>();
        

    }

	public void OnTriggerStay(Collider col){
		if (col.tag == "Player") {
			
			//ph.TakePower (2);
           
			if (ph.currentHealth<ph.startHealth){
				if (powerRegenTimer >= 1.0f) {
					ph.Healing (5);
					powerRegenTimer = 0f;
				} else {
					powerRegenTimer += Time.deltaTime;
				}
			}

			//print ("Player in campfire zone");
		}
      
	}
    public void OnTriggerEnter(Collider col)
    {
        print(col.name);
        if (col.name == "Player")
        {
            col.gameObject.GetComponent<PlayerMove>().underfire = true;
            print("playerunderfire!");
      }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.name == "Player")
        {
            col.gameObject.GetComponent<PlayerMove>().underfire = false;
        }
    }
    IEnumerator timeofcamp()
    {
        yield return new WaitForSeconds(30f);
        Destroy(gameObject);
        player.gameObject.GetComponent<PlayerMove>().underfire = false;
        Instantiate(Creatorscript.wood, Creatorscript.FindPoint(), Quaternion.identity);
        Instantiate(Creatorscript.rock, Creatorscript.FindPoint(), Quaternion.identity);
    }
}



