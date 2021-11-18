using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCtrl : MonoBehaviour
{
    BaseCtrl m_TempObj = null;

    public float m_MaxHp = 1000.0f;
    public float m_CurHp = 0.0f;

    public float m_MaxMp = 100.0f;
    public float m_CurMp = 0.0f;

    // 0 = 기본   1 = 파괴     2 = 히트
    public Sprite[] m_EBaseSpt = null;
    public Sprite[] m_PBaseSpt = null;

    [HideInInspector] public SpriteRenderer m_SprRender = null;

    void Start()
    {
        m_TempObj = GetComponent<BaseCtrl>();
        m_SprRender = GetComponent<SpriteRenderer>();

        // 정보 전달
        m_CurHp = m_MaxHp;
        m_CurMp = m_MaxMp;

        if (m_TempObj.tag == "P_Base")
        {
            GameMgr.Inst.m_PHp_Txt.text = (int)m_CurHp + " / " + (int)m_MaxHp;
            m_SprRender.sprite = m_PBaseSpt[0];
        }
        else if (m_TempObj.tag == "E_Base")
        {
            GameMgr.Inst.m_EHp_Txt.text = (int)m_CurHp + " / " + (int)m_MaxHp;
            m_SprRender.sprite = m_EBaseSpt[0];
        }

    }

    // 데미지 함수
    public void TakeDamage(float a_Damage)
    {
        if (m_CurHp <= 0.0f)
            return;

        GameMgr.DamageTxt((int)a_Damage, this.transform , 0.3f, 3.5f);
        m_CurHp -= a_Damage;

        if (m_CurHp <= 0.0f)
            m_CurHp = 0.0f;

        GameMgr.Inst.UpdateBaseUI(m_TempObj,InfoType.HP);

        Invoke("DefaultSpr", 0.3f);

        if (m_CurHp <= 0.0f)
            GameMgr.Inst.Die(m_TempObj, true);
    }

    // 원래 스프라이트로 돌아오는 함수
    public void DefaultSpr()
    {
        if (m_CurHp <= 0.0f)
            return;

       if(m_TempObj.tag == "P_Base")
            m_SprRender.sprite = m_PBaseSpt[0];
       else if(m_TempObj.tag == "E_Base")
            m_SprRender.sprite = m_EBaseSpt[0];
    }
}
