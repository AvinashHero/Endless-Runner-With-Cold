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
    public Animator canvasGroup,menuTitleAnim;
    public Text scoreText, coinText, modifierText, highScore;
    private float score, coinScore, modifierScore;
    private int lastScore;

    //Death menu
    public Animator deathAnimator;
    public Text deathScoreText, deathCoin;

    private void Awake()
    {
        Instance = this;
        modifierScore = 1;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        scoreText.text = scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");
        highScore.text = PlayerPrefs.GetInt("Hiscore").ToString();

    }

    private void Update()
    {
        if(MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartGame();
            FindObjectOfType<GleciarSpawn>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;
            canvasGroup.SetTrigger("Show");
            menuTitleAnim.SetTrigger("Hide");
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
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

