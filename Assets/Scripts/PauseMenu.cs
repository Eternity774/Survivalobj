using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu: MonoBehaviour{


    public void Continue()
    {
        GamePauseManager.Instance.IsPause = false;
    }

    public void Exit()
    {
        GamePauseManager.Instance.IsPause = false;
        GamePauseManager.Instance.ExitingGame();
    }

}
