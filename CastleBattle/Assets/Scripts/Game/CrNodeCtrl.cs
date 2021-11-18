using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrNodeCtrl : MonoBehaviour
{
    [HideInInspector] public CharType m_CrType;

    public Text m_Lv_Txt = null;
    public Image m_CrIcon_Img = null;
    public Text m_Gold_Txt = null;
    public GameObject m_CrSpawnObj = null;
    public Image m_CrSpawn_Img = null;

    public GameObject[] m_CharPrefabs = null;
    int m_Price = 0;

    GameObject m_BaseObj = null;
    GameObject m_DlgPrefab = null;
    Transform m_CharGroup = null;
    Button m_BtnCom = null;

    string a_Mess = "골드가 모자라요..ㅠㅠ";
    float m_SpawnTime = 5.0f;
    float m_CurTime = 0.0f;
    bool m_SpawnWaitTime = false;

    void Start()
    {
        m_BaseObj = GameObject.FindWithTag("P_Base");
        m_CharGroup = GameObject.Find("Character_Group").transform;
        m_DlgPrefab = (GameObject)Resources.Load("DiaLogBox_1");

        m_BtnCom = GetComponent<Button>();
        if (m_BtnCom != null)
            m_BtnCom.onClick.AddListener(() =>
            {
                if (GlobalValue.g_UserGold <= m_Price)
                {
                    GameObject a_DlgBoxObj = Instantiate(m_DlgPrefab);
                    GameObject a_Canvas = GameObject.Find("Canvas");
                    a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);
                    DialogCtrl a_DlgBox = a_DlgBoxObj.GetComponent<DialogCtrl>();
                    if (a_DlgBox != null)
                        a_DlgBox.SetMessage(a_Mess);

                    return;
                }

                GlobalValue.g_UserGold -= m_Price;
                m_SpawnWaitTime = true;
                m_CrSpawnObj.SetActive(true);
                m_BtnCom.interactable = false;
            });
    }

    void Update()
    {
        if (m_SpawnWaitTime == false)
            return;

        m_CurTime += Time.deltaTime;
        m_CrSpawn_Img.fillAmount = m_CurTime / m_SpawnTime;

        if (m_SpawnTime < m_CurTime)
        {
            m_SpawnWaitTime = false;
            m_CurTime = 0.0f;
            m_CrSpawn_Img.fillAmount = 0.0f;
            m_CrSpawnObj.SetActive(false);
            m_BtnCom.interactable = true;
            Create_Char(m_CrType);
        }
    }

    public void InitState(CharInfo a_CharInfo)
    {
        m_CrType = a_CharInfo.m_CrType;
        m_Price = a_CharInfo.m_Price;
        m_Gold_Txt.text = a_CharInfo.m_Price.ToString();
        m_CrIcon_Img.sprite = a_CharInfo.m_IconImg;
        m_CrIcon_Img.GetComponent<RectTransform>().sizeDelta = new Vector2(60.0f,60.0f);
        m_Lv_Txt.text = "Lv " + a_CharInfo.m_Level.ToString();
    }

    void Create_Char(CharType a_CrType)
    {
        if (m_CharPrefabs.Length < (int)a_CrType)
            return;

        GameObject a_Go = Instantiate(m_CharPrefabs[(int)a_CrType]) as GameObject;

        // 초기 위치
        a_Go.transform.position = new Vector3(m_BaseObj.transform.position.x + 3.0f, -2.0f, 0.0f);

        // 캐릭 능력치 할당
        a_Go.GetComponent<CharCtrl>().CharInit(GlobalValue.m_CrDataList[(int)a_CrType]);

        // 생성 오브젝트 위치 정리
        a_Go.transform.SetParent(m_CharGroup);
    }
}
