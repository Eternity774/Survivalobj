using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventorySlot : MonoBehaviour {

	public Image icon;
	public Button removeButton;
    public GameObject player;
    public GameObject foodpanel;
    public GameObject steak;

	Item item;
    void Start()
    {
        foodpanel.SetActive(false);
    }
	public void AddItem(Item newItem){
		item = newItem;
		icon.sprite = item.icon;
        //prefub = newItem.prefub;
        //print(prefub);
        icon.enabled = true;
		removeButton.interactable = true;
	}
	public void ClearSlot(){
		item = null;
		icon.sprite = null;
		icon.enabled = false;
		removeButton.interactable = false;
	}
	public void OnRemoveButton(){
        // print(item.prefub);
        if (item.name == "Meat" && player.GetComponent<PlayerMove>().underfire)
        {
            //print("условие прошло");
            Instantiate(steak, player.transform.TransformPoint(Vector3.forward * 2 + Vector3.up*0.5f), Quaternion.identity);
        }
        else
        {
           // print("условие не прошло");
            //print("имя :" + item.name);
            //print("условие: " + player.GetComponent<PlayerMove>().underfire);
            Instantiate(item.prefub, player.transform.TransformPoint(Vector3.forward * 2 + Vector3.up), Quaternion.identity);
        }
        Inventory.instance.Remove (item);
	}
	public void UseItem(){
		if (item != null) {
            if (item.name == "Meat" && player.GetComponent<PlayerHealth>().currentFood > 0)
            {
                foodpanel.SetActive(true);
                StartCoroutine(timeoftext());
            }
            else
            {
                item.UseItem();
            }
		}
	}
    IEnumerator timeoftext()
    {
        yield return new WaitForSeconds(3f);
        foodpanel.SetActive(false);
      
    }

}
