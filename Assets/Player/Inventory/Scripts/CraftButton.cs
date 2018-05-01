using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour {

    Inventory inventory;
    public string _name;
    public string[] ingr;
    public Text craftName;
    public Text[] tIngr;
    public GameObject dPanel;

    private void Start()
    {
        inventory = Inventory.instance;
        
    }

    public void CraftInfo()
    {
        for (int i=0; i < ingr.Length; i++)
        {
            for (int j=0; j < inventory.items.Count; j++)
            {
                if (ingr[i] == inventory.items[j].name) { 
                    tIngr[i].name = ingr[i];
                    tIngr[i].color = Color.green;
                    Debug.Log("Green " + tIngr[i].name + " " + ingr[i]);
                }
                else
                {
                    tIngr[i].name = ingr[i];
                    tIngr[i].color = Color.red;
                    Debug.Log("Red " + tIngr[i].name + " " + ingr[i]);
                }
            }
        }

        craftName.text = _name;
    
        //ingr1.text = _ingr1;
        //ingr2.text = _ingr2;
        //ingr3.text = _ingr3;
        //ingr4.text = _ingr4;
        //ingr5.text = _ingr5;
        dPanel.SetActive(true);
    }
    public void HidePanel()
    {
        dPanel.SetActive(false);
    }
}
