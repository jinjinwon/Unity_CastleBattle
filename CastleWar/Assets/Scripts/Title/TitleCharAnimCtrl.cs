using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCharAnimCtrl : MonoBehaviour
{
    Animator[] m_Anims = null;
    SpriteRenderer[] m_Trfms = null;
    public Transform[] m_Particle = null;

    bool m_IsCheck = false;
    static bool m_IsFlag = false;

    void Start()
    {
        m_Anims = GetComponentsInChildren<Animator>();
        m_Trfms = GetComponentsInChildren<SpriteRenderer>();
        m_IsFlag = false;

        for (int ii = 0; ii < m_Anims.Length; ii++)
        {
            m_Anims[ii].SetBool("doMove",true);
        }
    }

    void Update()
    {
        if (transform.position.x <= -10)
            Destroy(gameObject);

        if (gameObject.name == "P_CharAnim")
        {
            // 움직임 처리
            if (transform.position.x <= 28 && m_IsCheck == false)
                transform.Translate(0.01f, 0.0f, 0.0f);
            else
            {
                if (m_IsCheck == false)
                {
                    for (int ii = 0; ii < m_Trfms.Length; ii++)
                    {
                        m_Trfms[ii].flipX = true;
                        m_Particle[ii].localPosition = new Vector2(0.5f, 0.1f);
                    }
                }

                m_IsCheck = true;
                m_IsFlag = true;
                transform.Translate(-0.02f, 0.0f, 0.0f);
            }
        }
        else if(gameObject.name == "E_CharAnim")
        {
            if(m_IsFlag == true)
                transform.Translate(-0.02f, 0.0f, 0.0f);
        }
    }
}
