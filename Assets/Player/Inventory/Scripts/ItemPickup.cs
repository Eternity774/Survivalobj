using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

	public Item item;

	public void OnTriggerEnter(Collider col){
		if (col.tag == "Player") {
			
			bool wasPickedUp=Inventory.instance.Add(item);
			if (wasPickedUp) {
				Destroy (gameObject);
				//Inventory.instance.Add (item);
			}
				Debug.Log("You picked up "+ gameObject.name);
		}
	}
//	void PickUp(){
//	//Adding item to inventory
//		bool wasPickedUp=Inventory.instance.Add(item);
//		if (wasPickedUp) 
//			Destroy (gameObject);
//	}
}
