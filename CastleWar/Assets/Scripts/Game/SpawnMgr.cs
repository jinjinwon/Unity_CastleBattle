using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMgr : MonoBehaviour
{
    public GameObject[] m_RedUnits = null;

    float m_SpawnTime = 0.0f;
    float m_Timer = 0.0f;

    GameObject m_BaseObj = null;
    Transform m_CharGroup = null;

    void Start()
    {
        m_BaseObj = this.gameObject;
        m_CharGroup = GameObject.Find("Character_Group").transform;
        
        SpawnTimeSetting();
    }

    void Update()
    {
        if (GameMgr.Inst.m_GameOver == true)
            return;

        if (GameMgr.Inst.m_DlgActive == true)
            return;

        m_Timer += Time.deltaTime;

        if(m_SpawnTime < m_Timer)
        {
            m_Timer = 0.0f;
            SpawnPos();
        }
    }

    void SpawnPos()
    {
        SpawnTimeSetting();
        int a_Dice = Random.Range(0, m_RedUnits.Length);
        int a_RdLevel = GlobalValue.g_Rank;

        GameObject a_Go = Instantiate(m_RedUnits[a_Dice]) as GameObject;

        // 초기 위치
        a_Go.transform.position = new Vector3(m_BaseObj.transform.position.x - 3.0f, -2.0f, 0.0f);

        // 캐릭 능력치 할당
        a_Go.GetComponent<E_CharCtrl>().LevelSetting(a_RdLevel);

        // 생성 오브젝트 위치 정리
        a_Go.transform.SetParent(m_CharGroup);
    }

    void SpawnTimeSetting()
    {
        if (GlobalValue.g_Rank == 1)
            m_SpawnTime = 10.0f;
        else if (GlobalValue.g_Rank == 2)
            m_SpawnTime = 8.0f;
        else if (GlobalValue.g_Rank == 3)
            m_SpawnTime = 6.0f;
        else if (GlobalValue.g_Rank == 4)
            m_SpawnTime = 4.0f;
    }
}
