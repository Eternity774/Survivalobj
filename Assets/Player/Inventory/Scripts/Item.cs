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

	}
}
