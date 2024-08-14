using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool GameOver{get; set;}
    public bool GameStarted{get; set;}
    public bool GamePaused{get; set;}

    private void Awake() {
        GameStarted = false;
        GamePaused = false;
        GameOver = false;
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)){
            if (Application.platform == RuntimePlatform.Android){
                GamePaused = true;
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }else{
                Application.Quit();
            }
        }
        if (GamePaused){
            Time.timeScale = 0.0f;
        }else{
            Time.timeScale = 1.0f;
        }
    }

    private void OnApplicationFocus(bool focusStatus) {
        if (!GamePaused && GameStarted)
        GamePaused = !focusStatus;
    }
    private void OnApplicationPause(bool pauseStatus) {
        if (!GamePaused && GameStarted)
        GamePaused = !pauseStatus;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PauseOrResumeGame()
    {
        GamePaused = !GamePaused;
    }
    public void StartGame()
    {
        GameStarted = true;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
