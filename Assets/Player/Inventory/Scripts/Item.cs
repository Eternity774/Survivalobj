using System.Collections;
using UnityEngine;

public enum itemType
{
	consumable,
	weapon,
	armor
}


[CreateAssetMenu(fileName="New Item", menuName="Inventory/Item")]
public class Item : ScriptableObject {

	new public string name = "New Item";
	public Sprite icon = null;
	public bool isDefaultItem=false;
	public itemType current; 
	

		public virtual void Use(){

		switch(current)
		{
		case itemType.armor:
			Debug.Log ("using armor");
			break;
		case itemType.weapon:
			Debug.Log ("using weapon");
			break;
		case itemType.consumable:
			Debug.Log ("using consumable");
			break;

		}
	}
}
