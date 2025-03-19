using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] BackGround _script_BackGroud;

    public static GameManager instance;
    public  GameState gameState = GameState.Pause;
    public float customTime = 0f;  // Run 상태에서만 증가하는 시간

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GameStart("Play Start");
    }

    void Init()
    {
       if(_script_BackGroud == null)
        {
            _script_BackGroud = FindObjectOfType<BackGround>();
        }
        customTime = 0f;
    }

    void Update()
    {
        if (GameManager.instance.gameState == GameState.Run)
        {
            customTime += Time.deltaTime;  // Run 상태일 때만 시간 증가
        }
    }
    public void GameStart(string timing = null)
    {
        Init();
        gameState = GameState.Run;
        _script_BackGroud.SetSpeed(DEF.GameSpeed);
        DEF.Log($"{timing} : GameStart"); 
    }

    public void GamePause(string timing = null)
    {
        gameState = GameState.Pause;
        DEF.Log($"{timing} : GamePause");
    }

    public void GameOver(string timing = null)
    {
        gameState = GameState.GameOver;
        DEF.Log($"{timing} : GameOver");
    }
}
