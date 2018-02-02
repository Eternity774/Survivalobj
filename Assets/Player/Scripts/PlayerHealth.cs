using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	public int startHealth=100;
	public int currentHealth;
	public Slider healthbar;
	public Slider foodbar;
	public Slider powerbar;


	// Use this for initialization
	void Start () {
		currentHealth = startHealth;
		healthbar.value = startHealth;
	}

	public void TakeDamage (int amount){
		currentHealth -= amount;
		healthbar.value = currentHealth;

	}
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider col){
		if (col.tag == "dmgzone") {
			TakeDamage (5);
		}
	}
}
