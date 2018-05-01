using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour {

    public Transform player;
    Inventory inventory;
    public string _name;
    public Item[] ingr1;
    public bool[] isChecked;
    public Text craftName;
    public Text[] tIngr;
    public GameObject dPanel;
    private bool checkmas;
    public GameObject _instObj;

    private void Start()
    {        
        inventory = Inventory.instance;
        checkmas = false;
    }

    public bool Craftable(bool[] mas)
    {
        for (int i = 0; i < isChecked.Length; i++)
        {
            if (mas[i] == false)
            {
                return false;
            }
        }
        return true;
    }

    public void Craft()
    {
        if (Craftable(isChecked) == true)
        {
            Instantiate(_instObj, player.transform.position + (player.transform.forward * 2), player.transform.rotation);
            for (int i=0; i < ingr1.Length; i++)
            {
                inventory.Remove(ingr1[i]);
                isChecked[i] = false;
                tIngr[i].color = Color.red;
            }
            checkmas = false;
        }
        
    }

    public void CraftInfo()
    {     
        for (int i = ingr1.Length; i < tIngr.Length; i++)
        {
            tIngr[i].text = "";
        }

        for (int i=0; i < ingr1.Length; i++)
        {
            tIngr[i].text = ingr1[i].name;
            for (int j=0; j < inventory.items.Count; j++)
            {
                if (ingr1[i].name == inventory.items[j].name) {
                    isChecked[i] = true;
                    tIngr[i].text = ingr1[i].name;                    
                    tIngr[i].color = Color.green;
                    break;
                }
                else
                {
                    isChecked[i] = false;
                    tIngr[i].text = ingr1[i].name;
                    tIngr[i].color = Color.red;
                }
            }
        }

        craftName.text = _name;
        dPanel.SetActive(true);
    }
    public void HidePanel()
    {
        dPanel.SetActive(false);
    }
}
