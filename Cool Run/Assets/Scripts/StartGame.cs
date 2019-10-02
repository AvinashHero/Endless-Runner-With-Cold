using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{

    
    public Text highScore;
    // Start is called before the first frame update
    void Start()
    {
       
        highScore.text = PlayerPrefs.GetInt("Hiscore").ToString();
       
    }

    void Update()
    {
        
            
        if(MobileInput.Instance.Tap)
        {
            SceneManager.LoadScene("Game");
            
        }

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

    }
}
