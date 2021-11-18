using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum CrState
{
    Lock,
    BeforeBuy,
    Active
}

public class S_CharNodeCtrl : MonoBehaviour
{
    [HideInInspector] public CharType m_CrType = CharType.Char_SW;
    [HideInInspector] public int m_Level = 0;
    [HideInInspector] public CrState m_CrState = CrState.Lock;

    public Text m_Lv_Txt = null;
    public Text m_Help_Txt = null;
    public Text m_Buy_Txt = null;
    public Image m_CrIcon_Img = null;
    public Image m_CrLock_Img = null;

    void Start()
    {
        Button m_BtnCom = this.GetComponentInChildren<Button>();
        if (m_BtnCom != null)
        {
            m_BtnCom.onClick.AddListener(() =>
            {
                ShopMgr a_ShopMgr = null;
                GameObject a_ShopObj = GameObject.Find("ShopMgr");

                if (a_ShopObj != null)
                    a_ShopMgr = a_ShopObj.GetComponent<ShopMgr>();

                if (a_ShopMgr != null)
                    a_ShopMgr.BuyCrItem(m_CrType);
            });
        }
    }

    public void InitData(CharType a_CrType)
    {
        // 캐릭터 타입이 Enum 목록에 없는 경우
        if (a_CrType < CharType.Char_SW || CharType.CrCount <= a_CrType)
            return;

        // 이미지 세팅
        m_CrType = a_CrType;
        m_CrIcon_Img.sprite = GlobalValue.m_CrDataList[(int)a_CrType].m_IconImg;

        // 위치 세팅
        m_CrIcon_Img.GetComponent<RectTransform>().sizeDelta = new Vector2(135.0f, 135.0f);

        // 캐릭터 효과 설명 셋팅
        m_Help_Txt.text = string.Format("<{0}>\n[스킬]\n\n{1}\n{2}", 
                                        GlobalValue.m_CrDataList[(int)a_CrType].m_Name, 
                                        GlobalValue.m_CrDataList[(int)a_CrType].m_SkillExp_1,
                                        GlobalValue.m_CrDataList[(int)a_CrType].m_SkillExp_2);
    }

    public void SetState(CrState a_CrState, int a_Price, int a_Lv = 0)
    {
        m_CrState = a_CrState;
        if (a_CrState == CrState.Lock) //잠긴 상태
        {
            m_Lv_Txt.color = new Color32(255, 255, 255, 255);
            m_Lv_Txt.text = "Lv 1";
            m_CrIcon_Img.color = new Color32(110, 110, 110, 230);
            m_Help_Txt.gameObject.SetActive(true);
            m_Help_Txt.color = new Color32(255, 255, 255, 255);
            m_Buy_Txt.text = a_Price.ToString() + " 골드";
            m_CrLock_Img.gameObject.SetActive(true);
        }
        else if (a_CrState == CrState.BeforeBuy) //구매 가능 상태
        {
            m_Lv_Txt.color = new Color32(255, 255, 255, 255);
            m_Lv_Txt.text = "Lv 1";
            m_CrIcon_Img.color = new Color32(220, 220, 220, 220);
            m_Help_Txt.gameObject.SetActive(true);
            m_Help_Txt.color = new Color32(255, 255, 255, 255);
            m_Buy_Txt.text = a_Price.ToString() + " 골드";
            m_CrLock_Img.gameObject.SetActive(false);
        }
        else if (a_CrState == CrState.Active) //활성화 상태
        {
            m_Lv_Txt.color = new Color32(255, 255, 255, 255);
            m_Lv_Txt.text = "Lv " + a_Lv.ToString();
            m_CrIcon_Img.color = new Color32(255, 255, 255, 255);
            m_Help_Txt.gameObject.SetActive(true);
            m_Help_Txt.color = new Color32(255, 255, 255, 255);
            int a_CacPrice = a_Price + (a_Price * (a_Lv - 1));
            m_Buy_Txt.text = "Up " + a_CacPrice.ToString() + " 골드";
            m_CrLock_Img.gameObject.SetActive(false);
        }
    }
}
