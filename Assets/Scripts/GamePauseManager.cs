using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseManager : MonoBehaviour {

    static GamePauseManager instance;
    public static GamePauseManager Instance
    {
        get { return instance; }
    }
    
    [SerializeField] GameObject pauseScreen;
    [SerializeField] List<GameObject> panels = new List<GameObject>();

    List<GameObject> enabledPanels = new List<GameObject>();
    bool hasCursorVisible;


    bool isPause = false;
    public bool IsPause
    {

        get { return isPause; }
        set {
            isPause = value;
            if (isPause == true)
            {
                Time.timeScale = 0;
                SetPause();
            }
            else
            {
                Time.timeScale = 1;
                ExitPause();
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPause = !IsPause;
        }
	}

    void SetPause()
    {
        foreach(GameObject obj in panels)
        {
            if (obj.activeInHierarchy == true)
            {
                enabledPanels.Add(obj);
                obj.SetActive(false);
            }
        }
        pauseScreen.SetActive(true);
        if (Cursor.visible == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            hasCursorVisible = false;
        }
        else
        {
            hasCursorVisible = true;
        }
    }

    void ExitPause()
    {
        foreach(GameObject obj in enabledPanels)
        {
            obj.SetActive(true);
        }
        enabledPanels.Clear();
        pauseScreen.SetActive(false);
        if (hasCursorVisible == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
