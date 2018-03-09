using System.Collections;
using UnityEngine;

public enum itemType
{
	consumable,
	weapon,
	armor
}

public interface IStrategy
{
	void Algorithm ();
}

public class iConsumable: IStrategy
{
	public void Algorithm()
	{
		Debug.Log ("Using consumable!");
	}
}

public class iWeapon: IStrategy
{
	public void Algorithm()
	{
		Debug.Log ("Using weapon!");
	}
}

public class iArmor: IStrategy
{
	public void Algorithm()
	{
		Debug.Log ("Using armor!");
	}
}

public class Use{
	
	private IStrategy _strategy;

	public Use (IStrategy strategy){
		_strategy = strategy;
	}

	public void SetStrategy(IStrategy strategy){
		_strategy = strategy;
	}

	public void ExecuteOperation(){
		_strategy.Algorithm ();
	}
}

[CreateAssetMenu(fileName="New Item", menuName="Inventory/Item")]
public class Item : ScriptableObject {


	new public string name = "New Item";
	public Sprite icon = null;
	public bool isDefaultItem=false;
	public itemType current; 

	public virtual void UseItem()
	{
		Use use = new Use (new iArmor ());
				switch(current)
				{
					case itemType.armor:
						use.SetStrategy (new iArmor ());
						use.ExecuteOperation ();
						break;
					case itemType.weapon:
						use.SetStrategy (new iWeapon ());
						use.ExecuteOperation ();
						break;
					case itemType.consumable:
						use.SetStrategy (new iConsumable ());
						use.ExecuteOperation ();
						break;
		
				}
	}

}
