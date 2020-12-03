using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _centerText;
    //[SerializeField]
    //private Sprite[] _liveSprites;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private GameObject _preGameUI;
    private GameManager _gameManager;
    // Pause
    [SerializeField]
    private GameObject _pauseMenuPanel;
    private bool _pauseMenuShown;
    private Animator _pauseAnim;
    // Game is Running
    [SerializeField]
    private GameObject _gameUI;
    //Game Over
    [SerializeField]
    private GameObject _gameOverPanel;
    private Animator _gameOverAnim;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _finalScoreText;
    [SerializeField]
    private Text _resultsScreenBestScoreText;
    [SerializeField]
    private Text _gameBestScoreText;
    private string _defaultBestScoreText = "Best Score: ";
    private string _defaultScoreText = "Score: ";
    [SerializeField]
    private TMP_Text  _livesText;
    [SerializeField]
    private HUDController _hudController;

    private void Start() {
        _preGameUI.gameObject.SetActive(true);
        _pauseMenuPanel.gameObject.SetActive(false);
        _gameUI.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(false);
        _centerText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _pauseAnim = _pauseMenuPanel.GetComponent<Animator>();
        _gameOverAnim = _gameOverPanel.GetComponent<Animator>();
        if (_gameManager == null){
            Debug.LogError("Game Manager is null");
        }
    }

    private void Update() {
        if (_gameManager.GamePaused && _pauseMenuShown == false && _gameManager.GameStarted && _gameManager.GameOver == false){
            showAndHidePauseUI();
        }else if(_gameManager.GamePaused == false && _pauseMenuShown && _gameManager.GameStarted && _gameManager.GameOver == false){
            showAndHidePauseUI();
        }
    }

    public void updateScoreUI(int newScore)
    {
        _scoreText.text = _defaultScoreText + newScore;
        _finalScoreText.text = "Final Score: " + newScore;
    }

    public void updateBestScoreUI(int bestScore){
        _gameBestScoreText.text = _defaultBestScoreText + bestScore;
        _resultsScreenBestScoreText.text = _defaultBestScoreText + bestScore;
    }

    public void updateLivesUI(int currentLives, int totalLives){
        if (currentLives >= 0){
            //_livesImage.sprite = _liveSprites[currentLives];
            _hudController.SetCurrentHealth(currentLives, totalLives);
            _livesText.text = currentLives.ToString();
        }
    }
    
    public void updateStartUI(){
        _preGameUI.gameObject.SetActive(false);
        StartCoroutine(Countdown(3));
        _gameUI.gameObject.SetActive(true);
    }

    public void showAndHidePauseUI(){
        _pauseMenuShown = !_pauseMenuShown;
        if (_pauseMenuShown){
            _pauseMenuPanel.gameObject.SetActive(true);
            _pauseAnim.SetBool("isActive", true);
        }else{
            _pauseAnim.SetBool("isActive", false);
            _pauseMenuPanel.gameObject.SetActive(false);
        }
    }

    public void onGameOver(){
        StartCoroutine(GameOver(1.5f));
    }
    IEnumerator GameOver(float seconds){
        yield return new WaitForSeconds(seconds);
        _gameUI.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(true);
        _gameOverAnim.SetBool("isActive", true);
        StartCoroutine(TextFlicker(_gameOverText));
        _gameManager.GameStarted = false;
        _gameManager.GameOver = true;
    }
    IEnumerator TextFlicker(Text textToFlicker, int flickerCount = 0){
        Color lowFlicker = new Color(textToFlicker.color.r, textToFlicker.color.g, textToFlicker.color.b, 0.3f);
        Color highFlicker = textToFlicker.color;
        if (flickerCount > 0){
            while (flickerCount > 0){
                yield return new WaitForSeconds(0.5f);
                textToFlicker.color = lowFlicker;
                yield return new WaitForSeconds(0.5f);
                textToFlicker.color = highFlicker;
                flickerCount--;
            }
        }else{
            while(true){
                yield return new WaitForSeconds(0.5f);
                textToFlicker.color = lowFlicker;
                yield return new WaitForSeconds(0.5f);
                textToFlicker.color = highFlicker;  
            }
        }
    }

    IEnumerator Countdown(int startingSeconds){
        _centerText.gameObject.SetActive(true);
        int currentSecond = startingSeconds;
        // Count down 3..2..1 
        for (int i = 0; i < startingSeconds; i++)
        {
            _centerText.text = currentSecond.ToString();
            currentSecond--;
            yield return new WaitForSeconds(1);
        }
        _centerText.text = null;
        _centerText.gameObject.SetActive(false);
    }
    
    public void StartShieldDisplay(float seconds)
    {
        _hudController.SetShieldDelay(seconds);
    }
}
