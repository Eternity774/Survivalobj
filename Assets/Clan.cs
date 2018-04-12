using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clan{

    
   public GameObject Leader;
    List<GameObject> members;
    public string name;
   
    public void AddToClan(GameObject newmember)
    {
        members.Add(newmember);
        Debug.Log("В клан " + name + "добавилcя " + newmember.name);
        Creator.ChangeInClans();

    }
   public Clan(GameObject newleader)
    {
        members = new List<GameObject>();
        members.Add(newleader);
        Leader = newleader;
        name = "Clan of " + Leader;
        Debug.Log("Creating "+name);        
        Creator.ListofClans.Add(this);
        Creator.ChangeInClans();

    }
   public void DeleteFromClan(GameObject oldmember)
    {
        Debug.Log("в клане " + name + " убили " + oldmember.name);
        if (oldmember == Leader)
        {
            if (members.Count == 1)
            {
                Debug.Log(name + "IS DEAD");
                Creator.ListofClans.Remove(this);               
               
            }
            else Leader = members[1];     
        }
        else members.Remove(oldmember);
        Creator.ChangeInClans();

    }
   
}
