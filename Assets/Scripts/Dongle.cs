using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public int level;
    public bool isDrag;
    Rigidbody2D rigid;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void OnEnable() // 오브젝트가 새로 생성되거나 활성화 될 때 실행됨
    {
        anim.SetInteger("Level", level); // Level 파라미터에 level 변수 넘겨줌
    }

    void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float leftBorder = -4.2f + transform.localScale.x / 2f;
            float rightBorder = 4.2f - transform.localScale.x / 2f;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }
            mousePos.y = 8;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f); // 부드럽게 이동
        }
    
    }

    public void Drag()
    {
        isDrag = true; // Drag 상태이다
    }
    
    public void Drop()
    {
        isDrag = false; // Drag 끝
        rigid.simulated = true; // 물리효과 On
    }
}
