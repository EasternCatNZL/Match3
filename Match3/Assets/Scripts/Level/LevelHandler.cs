using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    [HideInInspector]
    public static LevelHandler singleton;

    [Header("Level vars")]
    public int jewelsToClear = 30;
    public int jewelsCleared = 0;
    public float levelTotalStartTime = 60.0f;

    [Header("UI things")]
    public Text timerUI;
    public Text tileUI;

    [Header("Messages")]
    public string wonText = "You win!";
    public string timeUpText = "Time up!";

    [Header("Refs")]
    public BoardHandler board;

    //control vars    
    private float timeLeft = 0.0f;
    private float totalTime = 0.0f;
    private float timeTimerStarted = 0.0f;
    private float pauseStartTime = 0.0f;
    private float pauseEndTime = 0.0f;
    //private string timerText = "0:00";
    public bool gameActive = false;
    private bool gamePaused = false;
    
    public static LevelHandler GetLevelHandler()
    {
        if(singleton == null)
        {
            GameObject levelHandle = new GameObject();
            levelHandle.name = "Level Handler";
            levelHandle.AddComponent<LevelHandler>();
            singleton = levelHandle.GetComponent<LevelHandler>();
        }
        return singleton;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            ClockTick();
        }
    }

    //init for clock
    private void InitClock()
    {
        timeLeft = levelTotalStartTime;
        totalTime = levelTotalStartTime;
        timeTimerStarted = Time.time;
        gameActive = true;
    }

    //clock countdown logic
    private void ClockTick()
    {
        timeLeft = totalTime - (Time.time - timeTimerStarted);
        if(timeLeft <= 0)
        {
            gameActive = false;
            timerUI.text = timeUpText;
        }
        else
        {
            PresentTime();
        }
        
    }

    //ends the game if score reached
    public void ScoreReached()
    {
        if (jewelsCleared >= jewelsToClear)
        {
            jewelsCleared = jewelsToClear;
            gameActive = false;
            timerUI.text = wonText;
        }
    }

    //present the time in minute:seconds format
    private void PresentTime()
    {
        int secondsLeft = (int)(timeLeft % 60);
        float timeMinusSeconds = timeLeft - secondsLeft;
        int minutesLeft = (int)(timeMinusSeconds / 60);
        timerUI.text = "Time left: " + minutesLeft + ":" + secondsLeft;
        tileUI.text = "Tiles left: " + (jewelsToClear - jewelsCleared);
    }

    //button logics
    public void StartGameButton()
    {
        board.InitialiseBoard();
        jewelsCleared = 0;
        InitClock();
    }

    public void PauseGameButton()
    {
        //resume if paused
        if (gamePaused)
        {
            gamePaused = false;
            //set game to active
            gameActive = true;
            //reset time timer started
            timeTimerStarted = Time.time;
        }
        //pause
        else
        {
            gamePaused = true;
            //set game active to false
            gameActive = false;
            //prep total time left after resuming
            totalTime = timeLeft;
        }
    }

    public void MuteButton()
    {

    }
}
