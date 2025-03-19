using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    private float _speed = 0;
    private Vector3 _startPos;

    [SerializeField] private SpriteRenderer[] backGrounds;
     private float[] widths;
    void Start()
    {
        _startPos = transform.position;
        widths = new float[backGrounds.Length];
        for(int i = 0; i < backGrounds.Length; i++)
        {
            widths[i] = backGrounds[i].bounds.size.x;
        }
    }

    void Update()
    {
        if(_speed >0)
        {
            for (int i = 0; i < backGrounds.Length; i++)
            {
                Move(backGrounds[i], widths[i]);
            }
        }
    }

    void Move(SpriteRenderer bg, float width)
    {
        float move = Mathf.Repeat(Time.time * _speed, width);  // 무한 반복 이동
        transform.position = _startPos + Vector3.left * move;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
