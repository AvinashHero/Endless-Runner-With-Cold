using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance { set; get; }

    public bool IsDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMotor motor;

    //UI and the UI fields
    public Animator canvasGroup;
    public Text scoreText, coinText, modifierText;
    private float score, coinScore, modifierScore;
    private int lastScore;

    //Death menu
    public Animator deathAnimator;
    public Text deathScoreText, deathCoin;


    //Pause Menu
    public Animator pauseAnimation;
    private static bool isGamePause = false;
    public GameObject pauseMenuUI;

    private void Awake()
    {
        Instance = this;
        modifierScore = 1;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        scoreText.text = scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");
        //highScore.text = PlayerPrefs.GetInt("Hiscore").ToString();

    }

    private void Update()
    {
        if(!isGameStarted)
        {
            isGameStarted = true;
            motor.StartGame();
            FindObjectOfType<GleciarSpawn>().IsScrolling = true;
           
            canvasGroup.SetTrigger("Show");
            //menuTitleAnim.SetTrigger("Hide");
        }

        if(isGameStarted && !IsDead)
        {
            
            score += (Time.deltaTime * modifierScore);
            if(lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsDead)
                OnPauseButton();
            
            if (IsDead)
                UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
            
        }
            
    }

    public void GetCoin()
    {
        coinScore++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT;
        scoreText.text = scoreText.text = score.ToString("0");
    }
  

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void OnPlayButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnHomeButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        
    }

    public void OnPauseButton()
    {
        if (isGamePause)
            OnResume();
        else
            OnPause();
        

    }

    public void OnResume()
    {
        isGamePause = false;
        //pauseAnimation.SetTrigger("onPause");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnPause()
    {
        isGamePause = true;
        //pauseAnimation.SetTrigger("onPause");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

    }

    
    public void OnDeath()
    {
        IsDead = true;
        FindObjectOfType<GleciarSpawn>().IsScrolling = false;
        deathScoreText.text = score.ToString("0");
        deathCoin.text = coinScore.ToString("0");
        deathAnimator.SetTrigger("Death");
        canvasGroup.SetTrigger("Hide");

        //Check if this is a highscore
        if (score > PlayerPrefs.GetInt("Hiscore"))
        {
            float s = score;
            if (s % 1 == 0)
                s += 1;

            PlayerPrefs.SetInt("Hiscore", (int)s);
        }

        


    }
}

