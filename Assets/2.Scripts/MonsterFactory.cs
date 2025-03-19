using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MonsterFactory : MonoBehaviour
{
    [SerializeField] private Transform[] _monsterPos = new Transform[3];
    [SerializeField] private GameObject[] _monsterPrefab;  // ���� ������
    [SerializeField] private List<GameObject> _monsterPool = new List<GameObject>();  // ���� ����Ʈ
    public int poolSize = 10;  // Ǯ���� ���� ����
    private float _genTime = 0f; // ������ �� Ÿ��

    void Init()
    {
        _genTime = 0f;
    }

    void Start()
    {
        // �ʱ� Ǯ ����
        //for (int i = 0; i < poolSize; i++)
        //{
        //    int index = i > 3 ? i % 4 : i;
        //    int posIndex = i > 2 ? i % 3 : i;
        //    CreateMonster(index, posIndex);
        //}
    }



    void Update()
    {
        if(GameManager.instance.gameState == GameState.Run)
        {
            if (GameManager.instance.customTime - _genTime > DEF.MonsterGenTime)
            {
                _genTime = GameManager.instance.customTime;
                GetMonster();
            }
        }
    }

    public void GetMonster()
    {
        int posIndex = Random.Range(0, _monsterPos.Length);
        GameObject targetMonster = null;
        // ��Ȱ��ȭ�� ���� ã��
        foreach (GameObject monster in _monsterPool)
        {
            if (!monster.activeInHierarchy) // ��Ȱ��ȭ�� ���� �߰�
            {
                monster.SetActive(true);
                targetMonster = monster;
            }
        }
        if(targetMonster == null)
        {
            int monsterIndex = Random.Range(0, _monsterPrefab.Length);
            targetMonster = CreateMonster(monsterIndex, posIndex);
        }

       MonsterInit(targetMonster, posIndex);

    }

    private GameObject CreateMonster(int monsterIndex, int posIndex)
    {
        // ��� ���Ͱ� Ȱ��ȭ ���¶�� ���� ����
        GameObject newMonster = Instantiate(_monsterPrefab[monsterIndex], _monsterPos[posIndex].position, Quaternion.identity);
        _monsterPool.Add(newMonster);
        newMonster.tag = DEF.Tag_Monster;
        DEF.Log("Create Monster");  
        return newMonster;
    }

    void MonsterInit(GameObject monster, int posIndex)
    {
        if(monster == null)
        {
            DEF.LogError("monster is null");
            return;
        }
        // ���� ��ġ �� �θ� ����
        monster.transform.parent = _monsterPos[posIndex];
        monster.transform.position = _monsterPos[posIndex].position;

        // ���� ������Ʈ, ��������Ʈ ���̾� ����
        monster.layer = DEF.MonsterLayer[posIndex];  
        // ���̾� ���� ����
        SpriteRenderer[] spriteRenderers = monster.GetComponentsInChildren<SpriteRenderer>();
        string currentLayerName = LayerMask.LayerToName(gameObject.layer); // ���� ������Ʈ�� Layer �̸� ��������
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.sortingLayerName = currentLayerName;  // Sorting Layer ����
        }
    }
}
