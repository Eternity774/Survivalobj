using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName="New equipment item", menuName="Inventory/Equipment")]
public class Equipment : Item {

	public EquipmentSlot equipSlot;

	public int armorModifier;
	public int damageModifier;

	public MeshRenderer mesh;


	public override void UseItem(){
		base.UseItem ();
		EquipmentManager.instance.Equip (this);
		RemoveFromInventory ();
	}
}

public enum EquipmentSlot{Head, Chest, Boots, rHand, lHand}
