using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetResource : MonoBehaviour
{
    float m_Tiemr = 1.0f;
    float m_Delta = 0.0f;

    int a_GetGold = 10;

    public int m_Level = 1;

    void Update()
    {
        if (GameMgr.Inst.m_GameOver == true)
            return;

        if (GameMgr.Inst.m_DlgActive == true)
            return;

        m_Delta += Time.deltaTime;

        if(m_Tiemr < m_Delta)
        {
            m_Delta = 0.0f;
            GameMgr.GoldTxt(a_GetGold * m_Level, this.gameObject.transform);
            GameMgr.Inst.m_GetGold += (a_GetGold * m_Level);
            GlobalValue.g_UserGold =  GlobalValue.g_UserGold + (a_GetGold * m_Level);
            GameMgr.Inst.UpdateGold();
        }
    }
}
