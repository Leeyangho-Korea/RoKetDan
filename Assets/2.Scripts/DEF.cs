using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Run,
    Pause,
    GameOver
}

public static class DEF
{
    #region ========== Hero ==========
    // 공격력
    public const int Attack = 10;
    // 체력
    public const int HP = 100;
    #endregion

    #region ========== Monster ==========
    // 몬스터 태그
    public const string Tag_Monster = "Monster";
    // 공격력
    public const int MonsterAttack = 5;
    // 체력
    public const int MonsterHP = 50;
    // 몬스터 젠 시간
    public const float MonsterGenTime = 1f;

    public static Dictionary<int, int> MonsterLayer = new Dictionary<int, int>
    {
        {0, 8},
        {1, 9},
        {2, 10},
    };
    #endregion

    #region ========== Game ==========  
    // 기본 진행 속도
    public const int GameSpeed = 2;
    #endregion

    #region ========== Log ==========
    public static void Log(string str)
    {
        #if UNITY_EDITOR
        Debug.Log(str);
        #endif
    }
    public static void LogWarning(string str)
    {
        #if UNITY_EDITOR
        Debug.LogWarning(str);
        #endif
    }
    public static void LogError(string str)
    {
        #if UNITY_EDITOR
        Debug.LogError(str);
        #endif
    }
    #endregion
}
