using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour {

	public static ItemData _ItemData;
	public List<Item> Items=new List<Item>();


	void Awake (){
		_ItemData = this;
	}

	void Start () {
		
	}

	void Update () {
		
	}

	//Item generation
	public Item ItemGen(int win_id){
		Item item = new Item ();
		item.Name = Items [win_id].Name;
		item.Textura = Items [win_id].Textura;
		return item;
	}
}
