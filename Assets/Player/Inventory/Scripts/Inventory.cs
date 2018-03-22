using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	#region Singleton

	public static Inventory instance;

	void Awake(){
		if (instance != null) {
			print ("More than one instance found");
		}
		instance = this;
	}
	#endregion

	public delegate void OnItemChanged ();
	public OnItemChanged onItemChangedCallback;

	public int space = 12;

	public List<Item> items = new List<Item>();

	public bool Add (Item item){
		if (!item.isDefaultItem) {
			if (items.Count > 12) {
				print ("Not enough space!");
				return false;
			}
			items.Add (item);
		}
		return true;

	}
	public void Remove (Item item){
		items.Remove (item);
	}
}
