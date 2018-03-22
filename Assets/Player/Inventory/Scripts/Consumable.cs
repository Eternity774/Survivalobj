using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="New consumable item", menuName="Inventory/Consumable")]
public class Consumable : Item {

	public int healthRestoring;
	public int foodRestoring;
	public int powerRestoring;

	public override void UseItem(){
		base.UseItem ();
		PlayerHealth.instance.TakeFood (foodRestoring);
		PlayerHealth.instance.Healing (healthRestoring);
		PlayerHealth.instance.TakePower (powerRestoring);
		RemoveFromInventory ();
	}


}
