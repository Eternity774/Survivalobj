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
        //if (newmember == members[0]) leader = newmember;
    }
   public Clan(GameObject newleader)
    {
        members.Add(newleader);
        Leader = newleader;
        name = "Clan of " + Leader;
        Debug.Log("Creating "+name);
    }
   public void DeleteFromClan(GameObject oldmember)
    {
        if(oldmember == Leader)
        {
            if (members.Count == 1) Debug.Log(name + "IS DEAD");
            else Leader = members[1];     
        }
        members.Remove(oldmember);       
        
    }
   
}
