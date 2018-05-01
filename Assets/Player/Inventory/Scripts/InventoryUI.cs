using UnityEngine;

public class InventoryUI : MonoBehaviour {
	
	public Transform itemsParent;
	public Transform craftingParent;
	//public Transform equipParent;


	Inventory inventory;
	Crafting crafting;
	InventorySlot[] slots;
	//CraftingSlot[] c_slots;
	//CharacterSlot[] e_slots;
	// Use this for initialization
	void Start () {
		inventory = Inventory.instance;
		crafting = Crafting.instance;
		inventory.onItemChangedCallback += UpdateUI;
	
		slots = itemsParent.GetComponentsInChildren<InventorySlot> ();
		//c_slots = craftingParent.GetComponentsInChildren<CraftingSlot> ();
	//	e_slots = equipParent.GetComponentsInChildren<CharacterSlot> ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < slots.Length; i++) {
			if (i < inventory.items.Count) {
				slots [i].AddItem (inventory.items [i]);
			} else {
				slots [i].ClearSlot ();
			}
		}
		//for (int i = 0; i < c_slots.Length; i++) {
		//	if (i < crafting.crafts.Count) {
		//		c_slots [i].AddItem (crafting.crafts [i]);
		//	} else {
		//		c_slots [i].ClearSlot ();
		//	}
		//}
	}

	void UpdateUI(){
//		for (int i = 0; i < slots.Length; i++) {
//			if (i < inventory.items.Count) {
//				slots [i].AddItem (inventory.items [i]);
//			} else {
//				slots [i].ClearSlot ();
//			}
//		}
	}
}
