using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour {

	public Image icon;
	public Sprite defaultImage;
	public int slot_id;
	Item item;

	public void AddItem(Item newItem){
		item = newItem;
		icon.sprite = item.icon;
		icon.enabled = true;
	}

	public void ClearSlot(){
		item = null;
		icon.sprite = defaultImage;
		icon.enabled = false;
	}
		
	public void UnequipChar(){
		//if (item != null) {
			EquipmentManager.instance.Unequip (slot_id);
			Debug.Log (slot_id);
		//}
	}
		
//	public void UseItem(){
//		if (item != null) {
//			item.UseItem ();
//		}
//	}

}


