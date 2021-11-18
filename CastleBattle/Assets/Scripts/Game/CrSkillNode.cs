using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrSkillNode : MonoBehaviour
{
    [HideInInspector] public CharType m_CrType;

    public Text m_Lv_Txt = null;
    int m_Level = 0;
    GetResource m_GetResource;

    void Start()
    {
        m_GetResource = FindObjectOfType<GetResource>();

        if (m_GetResource == null)
            return;

        InitState();
    }

    void InitState()
    {
        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            if ((int)GlobalValue.m_CrDataList[ii].m_CrType < 8)
                continue;

            if(gameObject.name.Contains("_1") == true)
            {
                if (GlobalValue.m_CrDataList[ii].m_CrType == CharType.Atk_Upgrade)
                {
                    m_Level = GlobalValue.m_CrDataList[ii].m_Level;
                    GameMgr.Inst.m_AtkGemLevel = m_Level;
                    m_Lv_Txt.text = "Lv " + m_Level;
                    return;
                }
            }
            else if (gameObject.name.Contains("_2") == true)
            {
                if (GlobalValue.m_CrDataList[ii].m_CrType == CharType.HP_Upgrade)
                {
                    m_Level = GlobalValue.m_CrDataList[ii].m_Level;
                    GameMgr.Inst.m_HpGemLevel = m_Level;
                    m_Lv_Txt.text = "Lv " + m_Level;
                    return;
                }
            }
            else if (gameObject.name.Contains("_3") == true)
            {
                if (GlobalValue.m_CrDataList[ii].m_CrType == CharType.Gold_Upgrade)
                {
                    m_GetResource.m_Level = GlobalValue.m_CrDataList[ii].m_Level;
                    m_Lv_Txt.text = "Lv " + m_GetResource.m_Level;
                    return;
                }
            }
        }
    }
}
