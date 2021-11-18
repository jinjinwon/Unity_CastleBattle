using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Arrow : MonoBehaviour
{
    public Sprite m_P_Arrow = null;
    public Sprite m_E_Arrow = null;

    Rigidbody2D m_Rig2d = null;

    public float m_AtkDamage = 0.0f;

    void Start()
    {
        m_Rig2d = GetComponent<Rigidbody2D>();

        if (gameObject.tag == "P_Arrow")
            GetComponent<SpriteRenderer>().sprite = m_P_Arrow;
        else if(gameObject.tag == "E_Arrow")
            GetComponent<SpriteRenderer>().sprite = m_E_Arrow;
    }

    void FixedUpdate()
    {
        if (GameMgr.Inst.m_GameOver == true)
            return;

        if (GameMgr.Inst.m_DlgActive == true)
            return;

        if (gameObject.tag == "P_Arrow")
            m_Rig2d.AddForce(Vector2.right * 500.0f * Time.deltaTime);
        else if (gameObject.tag == "E_Arrow")
            m_Rig2d.AddForce(Vector2.left * 500.0f * Time.deltaTime);
    }

    public void ArrowDamage(E_CharCtrl a_EcharCtrl , float a_AtkDamage)
    {
        a_EcharCtrl.TakeDamage(a_AtkDamage);
    }

    public void ArrowDamage(CharCtrl a_CharCtrl, float a_AtkDamage)
    {
        a_CharCtrl.TakeDamage(a_AtkDamage);
    }

    public void ArrowDamage(BaseCtrl a_BaseCtrl, float a_AtkDamage)
    {
        a_BaseCtrl.TakeDamage(a_AtkDamage);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "P_Arrow")
        {
            if (other.tag == "E_Base")
            {
                BaseCtrl a_Base_Ctrl = other.GetComponent<BaseCtrl>();

                if (a_Base_Ctrl == null)
                    return;

                ArrowDamage(a_Base_Ctrl, m_AtkDamage);
                Destroy(gameObject);
            }
            else if (other.tag == "E_Unit")
            {
                E_CharCtrl a_ECr_Ctrl = other.GetComponent<E_CharCtrl>();

                if (a_ECr_Ctrl == null)
                    return;

                ArrowDamage(a_ECr_Ctrl, m_AtkDamage);
                Destroy(gameObject);
            }
        }
        else if (gameObject.tag == "E_Arrow")
        {
            if (other.tag == "P_Base")
            {
                BaseCtrl a_Base_Ctrl = other.GetComponent<BaseCtrl>();

                if (a_Base_Ctrl == null)
                    return;

                ArrowDamage(a_Base_Ctrl, m_AtkDamage);
                Destroy(gameObject);
            }
            else if (other.tag == "P_Unit")
            {
                CharCtrl a_Cr_Ctrl = other.GetComponent<CharCtrl>();

                if (a_Cr_Ctrl == null)
                    return;

                ArrowDamage(a_Cr_Ctrl, m_AtkDamage);
                Destroy(gameObject);
            }
        }
    }
}
