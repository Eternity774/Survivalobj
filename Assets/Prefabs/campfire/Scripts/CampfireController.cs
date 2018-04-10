using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireController : MonoBehaviour {

	public static GameObject player;
	private PlayerHealth ph;
	private float powerRegenTimer = 0f;
	void Start(){
		player= GameObject.FindGameObjectWithTag ("Player");
		ph=player.GetComponent<PlayerHealth>();
	}

	public void OnTriggerStay(Collider col){
		if (col.tag == "Player") {
			
			//ph.TakePower (2);

			if (ph.currentHealth<ph.startHealth){
				if (powerRegenTimer >= 1.0f) {
					ph.Healing (50);
					powerRegenTimer = 0f;
				} else {
					powerRegenTimer += Time.deltaTime;
				}
			}

			print ("Player in campfire zone");
		}
	}
}



