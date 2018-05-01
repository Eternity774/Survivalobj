using UnityEngine;

public class InventoryUI : MonoBehaviour {
	
	public Transform itemsParent;
	public Transform craftingParent;


	Inventory inventory;
	Crafting crafting;
	InventorySlot[] slots;

	void Start () {
		inventory = Inventory.instance;
		crafting = Crafting.instance;
		inventory.onItemChangedCallback += UpdateUI;
	
		slots = itemsParent.GetComponentsInChildren<InventorySlot> ();

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

	}
}
