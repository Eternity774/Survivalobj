using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour {
	
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

	public void UnequipCraft(){
		item.DisuseItem ();
	}
}
