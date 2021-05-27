using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public TextMeshProUGUI PlayerMana, EnemyMana;
    public TextMeshProUGUI PlayerHP, EnemyHP;

    public GameObject MenuGO, StartMenuGO;
    public TextMeshProUGUI ResultTxt;

    public Image ResultBG;

    public TextMeshProUGUI TurnTime;
    public Button EndTurnBtn, PauseBtn, ResumeBtn, SettingsBtn;

    public SoundControl SoundControl;

    
    bool isPaused = false;

    private void Start()
    {
        StartMenuGO.SetActive(true);
        MenuGO.SetActive(false);
    }
    
    public void Awake()
    {
        if (!Instance)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    public void StartGame()
    {
        StartMenuGO.SetActive(false);
        MenuGO.SetActive(false);
        isPaused = false;
        EndTurnBtn.interactable = true;
        MenuGO.SetActive(false);
        UpdateHPAndMana();
        SoundControl.ShowSettings(true);
    }

    public void UpdateHPAndMana()
    {
        PlayerMana.text = GameManagerScr.Instance.CurrentGame.Player.Mana.ToString();
        EnemyMana.text = GameManagerScr.Instance.CurrentGame.Enemy.Mana.ToString();
        PlayerHP.text = GameManagerScr.Instance.CurrentGame.Player.HP.ToString();
        EnemyHP.text = GameManagerScr.Instance.CurrentGame.Enemy.HP.ToString();
    }

    public void ShowResult()
    {
        MenuGO.SetActive(true);
        ResumeBtn.gameObject.SetActive(false);
        SettingsBtn.gameObject.SetActive(false);
        ResultBG.gameObject.SetActive(true);
        ResultTxt.gameObject.SetActive(true);

        if (GameManagerScr.Instance.CurrentGame.Enemy.HP == 0)
        {
            ResultTxt.text = "You won! Good job";
        }
        else
        {
            ResultTxt.text = "-15 rating";
        }
    }

    public void UpdateTurnTime(int time)
    {
        TurnTime.text = time.ToString();
    }

    public void DisableTurnBtn()
    {
        EndTurnBtn.interactable = GameManagerScr.Instance.IsPlayerTurn;
    }

    void Update()
    {
        if(isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

    }

    public void Resume()
    {
        StartMenuGO.gameObject.SetActive(false);
        isPaused = !isPaused;
        if (!isPaused)
        {
            SoundControl.ShowSettings(true);
            MenuGO.SetActive(false); 
        }
        else
        {
            MenuGO.SetActive(true);
            ResultTxt.gameObject.SetActive(false);
            ResultBG.gameObject.SetActive(false);
            SettingsBtn.gameObject.SetActive(true);
            ResumeBtn.gameObject.SetActive(true);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
