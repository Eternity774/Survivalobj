using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clan{

    
   public GameObject Leader;
    List<GameObject> members;
    public string name;
   
    public void AddToClan(GameObject newmember)
    {
        bool addnewmember = true;
        foreach(GameObject a in members)
        {
            if(a.name == newmember.name)
            {
                addnewmember = false;
                break;
            }
        }
        if (addnewmember)
        {
            members.Add(newmember);
            Debug.Log("В клан " + name + "добавилcя " + newmember.name);
            Creator.ChangeInClans();
        }
    }
   public Clan(GameObject newleader)
    {
        members = new List<GameObject>();
        members.Add(newleader);
        Leader = newleader;
        name = "Clan of " + Leader;
        Debug.Log("Creating "+name);        
        Creator.ListofClans.Add(this);
        //Creator.ChangeInClans();

    }
   public void DeleteFromClan(GameObject oldmember)
    {
        Debug.Log("в клане " + name + " убили " + oldmember.name);
        if (oldmember == Leader)
        {
           
            members.Remove(oldmember);
            Debug.Log("в клане " + name + "умер лидер клана: " + oldmember.name);
            bool destinyofthisclan = false;
            foreach(GameObject a in members)
            {
                if (a.GetComponent<Behavior>().clan==this && a!=Leader)
                {
                    Leader = a;
                    destinyofthisclan = true;
                    Debug.Log("в клане " + name + "новый лидер: " + oldmember.name);
                    Leader.GetComponent<Behavior>().state = Behavior.State.wait;
                    break;
                }
            }
            if (!destinyofthisclan)
            {
                Debug.Log(name + "IS DEAD");
                Creator.ListofClans.Remove(this);
            }       
                
               
            
        }
        else members.Remove(oldmember);
        Debug.Log("в клане " + name + members.Count);
        Creator.ChangeInClans();

    }
   
}
