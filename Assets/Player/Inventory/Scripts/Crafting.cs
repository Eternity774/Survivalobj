using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour {

	#region Singleton
	public static Crafting instance;

	void Awake(){
		if (instance != null) {
			print ("More than one instance found");
		}
		instance = this;
	}
	#endregion

//	public delegate void OnItemChanged ();
	//public OnItemChanged onItemChangedCallback;

	public int space = 3;

	public List<Item> crafts = new List<Item>();

	public bool Add (Item item){
		if (!item.isDefaultItem) {
			if (crafts.Count > 3) {
				print ("Not enough space!");
				return false;
			}
			crafts.Add (item);
		}
		return true;

	}
	public void Remove (Item item){
		crafts.Remove (item);
	}
}
