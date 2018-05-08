using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowway : MonoBehaviour {
    public GameObject way;
    public GameObject arrow;
    public GameObject player;

    public void Where()
    {
        Creator.ChangeInClans();

        GameObject otherman = null;

        GameObject[] othermans = GameObject.FindGameObjectsWithTag("AIMan");
        Debug.Log("найдено " + othermans.Length);
        foreach (GameObject man in othermans)
        {
            Debug.Log("в цикле " + man.GetComponent<CleverAI>().clan);
            if (man.GetComponent<CleverAI>().clan != player.GetComponent<PlayerMove>().ClanOfPlayer)
            {
                Debug.Log("условие выполнилось " + othermans.Length);
                otherman = man;
                break;
            }
        }

        if (otherman != null)
        {
            Debug.Log("найден " + otherman.name);
            //arrow = GameObject.Find("arrow");
            // way = GameObject.Find("Arrowrotator");
            arrow = Instantiate(way, player.transform.position + Vector3.forward * 2, Quaternion.identity);
            Debug.Log("создан" + arrow.name);
            arrow.transform.LookAt(otherman.transform.position);
            //arrow.AddComponent<Rigidbody>();
            StartCoroutine(fordelete());
            //arrow.SetActive(true);

        }
    }
        public IEnumerator fordelete()
        {
            yield return new WaitForSeconds(2f);
            Destroy(arrow);

        }
    }



