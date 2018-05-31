using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuButtons : MonoBehaviour {

    [SerializeField] RectTransform buttonsPlane;
    [SerializeField] RectTransform settingsPlane;
    [SerializeField] GameObject LoadingScreen;
    Slider slider;
    Text loadText;

    private void Start()
    {
        slider = LoadingScreen.GetComponentInChildren<Slider>();
        loadText = LoadingScreen.GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        Debug.Log(Cursor.visible);
        if (Cursor.visible == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (LoadingScreen.activeSelf == true)
        {
            LoadingScreen.SetActive(false);
        }
    }
    
    public void StartButton()
    {
        StartCoroutine(AsyncLoad());
    }

    public void SettingsButton()
    {
        buttonsPlane.anchoredPosition = new Vector3(300f, buttonsPlane.anchoredPosition.y, 0);
        settingsPlane.anchoredPosition = new Vector3(-260f, buttonsPlane.anchoredPosition.y, 0); 
    }

    public void BackButton()
    {
        buttonsPlane.anchoredPosition = new Vector3(-260f, buttonsPlane.anchoredPosition.y, 0);
        settingsPlane.anchoredPosition = new Vector3(300f, buttonsPlane.anchoredPosition.y, 0);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
    
    IEnumerator AsyncLoad()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Main");
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
