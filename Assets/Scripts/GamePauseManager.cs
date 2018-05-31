using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour {

    static GamePauseManager instance;
    public static GamePauseManager Instance
    {
        get { return instance; }
    }
    
    [SerializeField] GameObject pauseScreen;
    [SerializeField] List<GameObject> panels = new List<GameObject>();
    [SerializeField] GameObject LoadingScreen;
    Slider slider;
    Text loadText;

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

    private void Start()
    {
        slider = LoadingScreen.GetComponentInChildren<Slider>();
        loadText = LoadingScreen.GetComponentInChildren<Text>();
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

    public void ExitingGame()
    {
        StartCoroutine(AsyncLoad());
    }

    IEnumerator AsyncLoad()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Menu");
        LoadingScreen.SetActive(true);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slider.value = progress;
            loadText.text = (int)(progress * 100) + "%";
            yield return null;
        }
    }
}
