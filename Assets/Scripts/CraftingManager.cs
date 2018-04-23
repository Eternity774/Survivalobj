using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour {


	#region Singleton 
	public static CraftingManager instance;

	void Awake(){
		instance = this;
	}
	#endregion

	CraftItem[] currentCrafts;
	Inventory inventory;
	//CraftingSlot[] c_slots;

	public delegate void OnItemChanged ();
	public OnItemChanged onItemChangedCallback;

	public int space = 3;

	public List<Item> items = new List<Item>();

	public bool Add (Item item){
		if (!item.isDefaultItem) {
			if (items.Count > 3) {
				print ("Not enough space!");
				return false;
			}
			items.Add (item);
		}
		return true;

	}



	public CraftItem Unequip(int slotIndex){
		CraftItem oldItem = null;

			oldItem = currentCrafts [slotIndex];
			inventory.Add (oldItem);
			currentCrafts [slotIndex] = null;

			//e_slots [slotIndex].icon.sprite = e_slots [slotIndex].defaultImage;

//			if (onEquipmentChanged != null) {
//				onEquipmentChanged.Invoke (null, oldItem);
//			}

		return oldItem;
	}
		
	}
	
