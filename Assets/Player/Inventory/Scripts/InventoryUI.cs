using UnityEngine;

public class InventoryUI : MonoBehaviour {
	
	public Transform itemsParent;
	//public Transform equipParent;


	Inventory inventory;

	InventorySlot[] slots;
	//CharacterSlot[] e_slots;
	// Use this for initialization
	void Start () {
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;
	
		slots = itemsParent.GetComponentsInChildren<InventorySlot> ();
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
