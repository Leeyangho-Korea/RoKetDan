using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
/// <summary>
/// 게임 내 몬스터를 생성하고 관리하는 역할을 담당하는 클래스입니다.
/// </summary>
public class MonsterFactory : MonoBehaviour
{
    [SerializeField] private Transform[] _monsterPos = new Transform[3];
    [SerializeField] private GameObject[] _monsterPrefab;  // 몬스터 프리팹
    [SerializeField] private List<GameObject> _monsterPool = new List<GameObject>();  // 몬스터 리스트
    public int poolSize = 10;  // 풀링할 몬스터 개수
    private float _genTime = 0f; // 마지막 젠 타임

    void Init()
    {
        _genTime = 0f;
    }

    void Start()
    {
        // 초기 풀 생성
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
        // 비활성화된 몬스터 찾기
        foreach (GameObject monster in _monsterPool)
        {
            if (!monster.activeInHierarchy) // 비활성화된 몬스터 발견
            {
                monster.SetActive(true);
                targetMonster = monster;
                break;
            }
        }
        if(targetMonster == null)
        {
            int monsterIndex = Random.Range(0, _monsterPrefab.Length);
            targetMonster = CreateMonster(monsterIndex, posIndex);
        }

       MonsterInit(targetMonster, posIndex);

    }

    int i = 0;
    private GameObject CreateMonster(int monsterIndex, int posIndex)
    {
        // 모든 몬스터가 활성화 상태라면 새로 생성
        GameObject newMonster = Instantiate(_monsterPrefab[monsterIndex], _monsterPos[posIndex].position, Quaternion.identity);
        _monsterPool.Add(newMonster);
        newMonster.tag = DEF.Tag_Monster;
        newMonster.name = $"{i}";
        i++;
        return newMonster;
    }

    void MonsterInit(GameObject monster, int posIndex)
    {
        if(monster == null)
        {
            DEF.LogError("monster is null");
            return;
        }
        // 몬스터 위치 및 부모 설정
        monster.transform.parent = _monsterPos[posIndex];
        monster.transform.position = _monsterPos[posIndex].position;

        // 몬스터 오브젝트, 스프라이트 레이어 설정
        monster.layer = DEF.MonsterLayer[posIndex];  
        // 레이어 순서 변경
        SpriteRenderer[] spriteRenderers = monster.GetComponentsInChildren<SpriteRenderer>();
        string currentLayerName = LayerMask.LayerToName(monster.gameObject.layer); // 현재 오브젝트의 Layer 이름 가져오기
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.sortingLayerName = currentLayerName;  // Sorting Layer 변경
        }
        monster.GetComponent<Monster>().HpInit();

    }
}
