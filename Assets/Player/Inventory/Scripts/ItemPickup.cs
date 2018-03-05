using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

	public Item item;

	void PickUp(){
	//Adding item to inventory
		bool wasPickedUp=Inventory.instance.Add(item);
		if (wasPickedUp) 
			Destroy (gameObject);
	}
}
