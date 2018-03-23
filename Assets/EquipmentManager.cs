﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

	#region Singleton 
	public static EquipmentManager instance;

	void Awake(){
		instance = this;
	}

	#endregion

	public Transform rHand;
	public Transform lHand;
//	public Transform rHandRotation;
	public MeshRenderer targetMesh;
	Equipment[] currentEquipment;
	MeshRenderer[] currentMeshes;
	Inventory inventory;


	public delegate void OnEquipmentChanged (Equipment newItem, Equipment oldItem);
	public OnEquipmentChanged onEquipmentChanged;

	void Start(){

		inventory = Inventory.instance;

		int numSlots = System.Enum.GetNames (typeof(EquipmentSlot)).Length;
		currentEquipment = new Equipment[numSlots];
		currentMeshes = new MeshRenderer[numSlots];
	}

	public void Equip (Equipment newItem){

		int slotIndex = (int)newItem.equipSlot;

		Equipment oldItem = Unequip(slotIndex);

//		if (currentEquipment [slotIndex] != null) {
//			oldItem = currentEquipment [slotIndex];
//			inventory.Add (oldItem);
//		}

		if (onEquipmentChanged != null)
		{
			onEquipmentChanged.Invoke(newItem, oldItem);
		}


		currentEquipment [slotIndex] = newItem;
		//HERE WE NEED TO ATTACH WEAPON TO CHARACTER

		MeshRenderer newMesh = Instantiate<MeshRenderer> (newItem.mesh, new Vector3(0,0,0),Quaternion.identity);
	//	newMesh.transform.parent = targetMesh.transform;

		switch (slotIndex) {  // 3 - Right hand slot, 4 - left hand slot
		case 3: 
			newMesh.transform.parent = rHand.transform;
			break;
		case 4:
			newMesh.transform.parent = lHand.transform;
			break;
		}

		newMesh.transform.localPosition= new Vector3 (0, 0, 0);
		newMesh.transform.localRotation = Quaternion.identity;
		currentMeshes [slotIndex] = newMesh;
	}



	public Equipment Unequip(int slotIndex){
		Equipment oldItem = null;

		if (currentEquipment [slotIndex] != null) {
			
				if (currentMeshes [slotIndex] != null) {
					Destroy (currentMeshes [slotIndex].gameObject);
				}

			oldItem = currentEquipment [slotIndex];
			inventory.Add (oldItem);
			currentEquipment [slotIndex] = null;

			if (onEquipmentChanged != null) {
				onEquipmentChanged.Invoke (null, oldItem);
			}
		}
		return oldItem;
	}
}
