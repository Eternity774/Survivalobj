﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friendship : MonoBehaviour {
    public GameObject FriendlyPanel;
    
    // Use this for initialization
    public GameObject Friend;
    void Start () {
        FriendlyPanel = GameObject.Find("FriendlyPanel");        
        FriendlyPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void answeryes()
    {
        print(gameObject.GetComponent<Friendship>().Friend.name);
        if (Friend != null)
        {
            print("дружба принята");
            Friend.GetComponent<CleverAI>().clan = gameObject.GetComponent<PlayerMove>().ClanOfPlayer;
            gameObject.GetComponent<PlayerMove>().ClanOfPlayer.AddToClan(Friend);
            Friend.GetComponent<CleverAI>().AddTask(new CleverAI.Task(CleverAI.Action.Friend, gameObject, 6));
            Creator.ChangeInClans();
        }

        FriendlyPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void request(GameObject newfriend)//заявка на дружбу
    {
        print("заявку прислал" + newfriend.name);
        FriendlyPanel.SetActive(true);
        Friend = newfriend;
        print(Friend.name);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void answerno()
    {
        FriendlyPanel.SetActive(false);
        Friend = null;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
