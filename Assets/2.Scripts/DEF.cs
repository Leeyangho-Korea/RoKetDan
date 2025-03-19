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
    // ���ݷ�
    public const int Attack = 10;
    // ü��
    public const int HP = 100;
    #endregion

    #region ========== Monster ==========
    // ���� �±�
    public const string Tag_Monster = "Monster";
    // ���ݷ�
    public const int MonsterAttack = 5;
    // ü��
    public const int MonsterHP = 50;
    // ���� �� �ð�
    public const float MonsterGenTime = 1f;

    public static Dictionary<int, int> MonsterLayer = new Dictionary<int, int>
    {
        {0, 8},
        {1, 9},
        {2, 10},
    };
    #endregion

    #region ========== Game ==========  
    // �⺻ ���� �ӵ�
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
