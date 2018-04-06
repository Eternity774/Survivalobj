using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	#region Singleton 
	public static PlayerHealth instance;

	void Awake(){
		instance = this;
	}

	#endregion

	public int startHealth=700;
	public int currentHealth;
	public int startFood = 75;
	public int currentFood;
	public float startPower = 100;
	public float currentPower;
	public Slider healthbar;
	public Slider foodbar;
	public Slider powerbar;

	void Start () {
		currentHealth = startHealth/2;
		healthbar.value = startHealth;
		currentFood = startFood;
		foodbar.value = startFood;
		currentPower = startPower;
		powerbar.value = startPower;
		StartCoroutine (FoodBar ());
        StartCoroutine(HealthBar());
	}
    

	public void TakeDamage (int amount){
		currentHealth -= amount;
		healthbar.value = currentHealth;

		if (currentHealth <= 0) {
			Debug.Log ("You died");
            gameObject.GetComponent<PlayerMove>().StartCoroutine("Dead");
		}
	}
    public void TakeFood(int amount)
    {
        currentFood += amount;
        if (currentFood > startFood) currentFood = startFood;        
        foodbar.value = currentFood;        
    }

	public void Healing(int amount)
	{
		currentHealth += amount;
		if (currentHealth > startHealth) currentHealth = startHealth;
		healthbar.value = currentHealth;
	}

	public void TakePower(int amount)
	{
		currentPower += amount;
		if (currentPower> startPower) currentPower = startPower;
		powerbar.value = currentPower;
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
			yield return new WaitForSeconds (5f);
		}
	}
    public IEnumerator HealthBar()
    {
        while (true)
        {
            if (currentFood > 50) Healing(10);            
            yield return new WaitForSeconds(3f);
        }
    }


}
