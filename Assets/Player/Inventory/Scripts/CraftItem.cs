using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="New Ingridient item", menuName="Inventory/Ingridient")]
public class CraftItem : Item {


	public MeshRenderer mesh;


	public override void UseItem(){
		base.UseItem ();
	}


		
}
	