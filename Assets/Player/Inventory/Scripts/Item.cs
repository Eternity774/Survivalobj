using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName="New Item", menuName="Inventory/Item")]
public class Item : ScriptableObject {


	new public string name = "New Item";
	public Sprite icon = null;
	public bool isDefaultItem=false;

	public virtual void UseItem()
	{
		Debug.Log ("Using " + name);
	}

	public void RemoveFromInventory(){
		Inventory.instance.Remove (this);
	}
	public void DisuseItem(){
		Inventory.instance.Add (this);
		Crafting.instance.Remove (this);
	}
}
