using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {


	const int INVENTORY_WINDOW_ID = 0; //id окна инвентаря 
	const int INVENTORY_TEXTURE_ID = 1; //id окна с иконкой 
	public float ButtonWidth = 40; //высота ячейки 
	public float ButtonHeight = 40; //ширина ячейки 

	int invRows = 6; //количество колонок 
	int invColumns = 4; //количество столбцов 
	Rect inventoryWindowRect = new Rect(10, 10, 
		170, 265); //область окна 
	Rect inventoryBoxRect = new Rect(); //область окна с изображением иконки 
	bool isDraggable; //перемещение предмета 
	Item selectItem; //вспомогательная переменная куда заносим предмет инвентаря 
	Texture2D dragTexture; //текстура которая отображается при перетягивании предмета в инвентаре 

	Dictionary<int, Item> InventoryPlayer = new Dictionary<int, Item>(); //словарь содержащий предметы инвентаря 

	void Start () { 
		//добавляем предметы в инвентарь 
		InventoryPlayer.Add(0, ItemData._ItemData.ItemGen(0)); 
		InventoryPlayer.Add(1, ItemData._ItemData.ItemGen(1)); 
		InventoryPlayer.Add(2, ItemData._ItemData.ItemGen(2)); 
		//InventoryPlayer.Add(3, ItemData._ItemData.ItemGen(3)); 
	} 

	// Update is called once per frame 
	void Update () { 

	} 

	void OnGUI() 
	{ 
		inventoryWindowRect = GUI.Window(INVENTORY_WINDOW_ID, inventoryWindowRect, firstInventory, "INVENTORY"); //создаем окно 
		if (isDraggable) 
		{ 
			inventoryBoxRect = GUI.Window(INVENTORY_TEXTURE_ID, new Rect(Event.current.mousePosition.x + 1, Event.current.mousePosition.y + 1, 40, 40), insert, "", "box"); 

		} 
	} 

	//окно с изображением иконки 
	void insert(int id) 
	{ 
		GUI.BringWindowToFront(INVENTORY_TEXTURE_ID);//выводим на передний план окно с иконкой 
		GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 40, 40), dragTexture);//рисуем текстуру иконки 
	} 

	//окно с инвентарем 
	void firstInventory(int id) 
	{ 
		for (int y = 0; y < invRows; y++) 
		{ 
			for (int x = 0; x < invColumns; x++) 
			{ 
				if (InventoryPlayer.ContainsKey(x + y * invColumns))//проверяем содеоржится ли ключ с данным значением 
				{ 
					if (GUI.Button(new Rect(5 + (x * ButtonHeight), 20 + (y * ButtonHeight), ButtonWidth, ButtonHeight), new GUIContent(InventoryPlayer[x + y * invColumns].Textura), "button")) 
					{ 
						if (!isDraggable) 
						{ 
							dragTexture = InventoryPlayer[x + y * invColumns].Textura;//присваиваем нашой текстуре которая должна отображаться при перетаскивании, текстуру предмета 
							isDraggable = true;//возможность перемещать предмет 
							selectItem = InventoryPlayer[x + y * invColumns];//присваиваем вспомогательной переменной наш предмет 
							InventoryPlayer.Remove(x + y * invColumns);//удаляем из словаря предмет 
						} 
					} 
				} 
				else 
				{ 
					if (isDraggable) 
					{ 
						if (GUI.Button(new Rect(5 + (x * ButtonHeight), 20 + (y * ButtonHeight), ButtonWidth, ButtonHeight), "", "button")) 
						{ 
							InventoryPlayer.Add(x + y * invColumns, selectItem);//добавляем предмет который перетаскиваем в словарь 
							//обнуляем переменные 
							isDraggable = false; 
							selectItem = null; 
						} 
					} 
					else 
					{ 
						//делаем ячейки не выделяемыми 
						GUI.Label(new Rect(5 + (x * ButtonHeight), 20 + (y * ButtonHeight), ButtonWidth, ButtonHeight), "", "button"); 
					} 
				} 
			} 
		} 
		GUI.DragWindow();   
	}   
}
