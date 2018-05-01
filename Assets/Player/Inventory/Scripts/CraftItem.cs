using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="New Ingridient item", menuName="Inventory/Ingridient")]
public class CraftItem : Item {


	public MeshRenderer mesh;


	public override void UseItem(){
		base.UseItem ();
		//Crafting.instance.Add (this);
		//RemoveFromInventory ();
	}
	//public void DisUseItem(){
	//	Inventory.instance.Add (this);
	//	Crafting.instance.Remove (this);
	//}
		
}
	