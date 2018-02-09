using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	public int startHealth=100;
	public int currentHealth;
	public int startFood = 75;
	public int currentFood;
	public float startPower = 100;
	public float currentPower;
	public Slider healthbar;
	public Slider foodbar;
	public Slider powerbar;

	void Start () {
		currentHealth = startHealth;
		healthbar.value = startHealth;
		currentFood = startFood;
		foodbar.value = startFood;
		currentPower = startPower;
		powerbar.value = startPower;
		StartCoroutine (FoodBar ());
	}

	void Update () {

	}

	public void TakeDamage (int amount){
		currentHealth -= amount;
		healthbar.value = currentHealth;

		if (currentHealth <= 0) {
			Debug.Log ("You died");
		}
	}
		
	public void Hunger(){
		currentFood--;
		foodbar.value = currentFood;
	}

	public IEnumerator FoodBar (){
		while (true) {			
			if (currentFood > 0) {
				Hunger ();
			} else {				
				TakeDamage (2);
			}
			yield return new WaitForSeconds (3f);
		}
	}
}
