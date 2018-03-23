using UnityEngine;

public class CharacterUI : MonoBehaviour {
	
	public Transform itemsParent;


	Inventory inventory;

	CharacterSlot[] c_slots;
	// Use this for initialization
	void Start () {
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;
	
		c_slots = itemsParent.GetComponentsInChildren<CharacterSlot> ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < c_slots.Length; i++) {
			if (i < inventory.items.Count) {
				c_slots [i].AddItem (inventory.items [i]);
			} else {
				c_slots [i].ClearSlot ();
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
