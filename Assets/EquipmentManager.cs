using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

	#region Singleton 
	public static EquipmentManager instance;

	void Awake(){
		instance = this;
	}

	#endregion

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

		Equipment oldItem = null;

		if (currentEquipment [slotIndex] != null) {
			oldItem = currentEquipment [slotIndex];
			inventory.Add (oldItem);
		}

		if (onEquipmentChanged != null) {
			onEquipmentChanged.Invoke (newItem, oldItem);
		}

		currentEquipment [slotIndex] = newItem;
		MeshRenderer newMesh = Instantiate<MeshRenderer> (newItem.mesh);
		newMesh.transform.parent = targetMesh.transform;
		currentMeshes [slotIndex] = newMesh;
	}



	public void Unequip(int slotIndex){

		if (currentEquipment [slotIndex] != null) {
			if (currentMeshes [slotIndex] != null) {
				Destroy (currentMeshes [slotIndex].gameObject);
			}
			Equipment oldItem = currentEquipment [slotIndex];
			inventory.Add (oldItem);
			currentEquipment [slotIndex] = null;

			if (onEquipmentChanged != null) {
				onEquipmentChanged.Invoke (null, oldItem);
			}
		}
	
	}
}
