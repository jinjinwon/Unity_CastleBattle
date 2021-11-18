using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class LobbyMgr : MonoBehaviour
{
    [Header("UserInfo UI")]
    public Text m_Nick_Txt = null;
    public Text m_CurGold_Txt = null;

    [Header("Button UI")]
    public Button m_Ranking_Btn = null;
    public Button m_NickChange_Btn = null;
    public Button m_Shop_Btn = null;
    public Button m_BattleStart_Btn = null;
    public Button m_Logout_Btn = null;

    [Header("Rank UI")]
    public GameObject m_RankObj = null;
    public Button m_RankBack_Btn = null;

    [Header("NickChange UI")]
    public GameObject m_NickChangeObj = null;
    public InputField m_NickID_InputField = null;
    public Button m_NickChangeOK_Btn = null;
    public Button m_NickCancel_Btn = null;
    public Text m_Message_Txt = null;
    float m_ShowMsTimer = 0.0f;

    [Header("Fade UI")]
    //------ Fade In 관련 변수들...
    public Image m_FadeImg = null;
    float AniDuring = 0.8f;  //페이드아웃 연출을 시간 설정
    bool m_StartFade = false;
    float m_CacTime = 0.0f;
    float m_AddTimer = 0.0f;
    Color m_Color;
    float m_StVal = 1.0f;
    float m_EndVal = 0.0f;
    string SceneName = "";

    static bool m_Flag = false;

    void Start()
    {
        Time.timeScale = 1.0f;
        GlobalValue.InitData();

        if (m_BattleStart_Btn != null)
            m_BattleStart_Btn.onClick.AddListener(() =>
            {
                AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
                m_RankObj.SetActive(true);
            });

        if (m_Shop_Btn != null)
            m_Shop_Btn.onClick.AddListener(() =>
            {
                AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
                SceneName = "ShopScene";
                SceneOut();
            });

        if (m_Logout_Btn != null)
            m_Logout_Btn.onClick.AddListener(() =>
            {
                AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
                SceneName = "TitleScene";
                OnClickLogOut();
                SceneOut();
            });

        if (m_NickChange_Btn != null)
            m_NickChange_Btn.onClick.AddListener(() =>
            {
                AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
                m_NickChangeObj.SetActive(true);
            });

        if (m_NickCancel_Btn != null)
            m_NickCancel_Btn.onClick.AddListener(()=>
            {
                AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
                m_NickChangeObj.SetActive(false);
            });

        if (m_NickChangeOK_Btn != null)
            m_NickChangeOK_Btn.onClick.AddListener(NickChangeOK);
        


        // 내 정보 갱신
        LoadCharInfo();
        RefreshMyInfo();
    }

    void Update()
    {
        // Message 표시
        if (0.0f < m_ShowMsTimer)
        {
            m_ShowMsTimer -= Time.deltaTime;
            if (m_ShowMsTimer <= 0.0f)
                MessageOnOff(false);
        }

        //-----m_FadeInOut
        if (m_StartFade == true)
        {
            if (m_CacTime < 1.0f)
            {
                m_AddTimer = m_AddTimer + Time.deltaTime;
                m_CacTime = m_AddTimer / AniDuring;
                m_Color = m_FadeImg.color;
                m_Color.a = Mathf.Lerp(m_StVal, m_EndVal, m_CacTime);
                m_FadeImg.color = m_Color;
                if (1.0f <= m_CacTime)
                {
                    if (m_StVal == 1.0f && m_EndVal == 0.0f)// 들어올 때 
                    {
                        m_Color.a = 0.0f;
                        m_FadeImg.color = m_Color;
                        m_FadeImg.gameObject.SetActive(false);
                        m_StartFade = false;
                    }
                    else if (m_StVal == 0.0f && m_EndVal == 1.0f) //나갈 때
                    {
                        SceneManager.LoadScene(SceneName);
                    }
                }
            }
        }
    }

    // 내 정보 갱신
    void RefreshMyInfo()
    {
        m_Nick_Txt.text = "닉네임 : " + GlobalValue.g_NickName;
        m_CurGold_Txt.text = "보유골드 : " + GlobalValue.g_UserGold.ToString();
    }

    // 캐릭터 능력치 불러오기
    void LoadCharInfo()
    {
        if (m_Flag == true)
            return;

        if (TitleMgr.m_FirstCreate == true)
        {
            GlobalValue.g_UserGold = 50000;
            for (int i = 0; i < GlobalValue.m_CrDataList.Count; i++)
                GlobalValue.m_CrDataList[i].m_Level = 1;
            TitleMgr.m_FirstCreate = false;
        }

        for (int i = 0; i < GlobalValue.m_CrDataList.Count; i++)
        {
            if ((int)GlobalValue.m_CrDataList[i].m_CrType >= 8)
                return;

            for (int ii = 0; ii < GlobalValue.m_CrDataList[(int)i].m_Level; ii++)
            {
                GlobalValue.m_CrDataList[(int)i].m_Attack += GlobalValue.m_CrDataList[(int)i].m_UpAttack * 0.05f;
                GlobalValue.m_CrDataList[(int)i].m_AtkDistance += GlobalValue.m_CrDataList[(int)i].m_UpAttDistanc * 0.05f;
                GlobalValue.m_CrDataList[(int)i].m_AtkSpeed -= GlobalValue.m_CrDataList[(int)i].m_UpAtkSpeed * 0.005f;
                GlobalValue.m_CrDataList[(int)i].m_Hp += GlobalValue.m_CrDataList[(int)i].m_UpHp * 0.05f;
                GlobalValue.m_CrDataList[(int)i].m_SkillCoolTime -= GlobalValue.m_CrDataList[(int)i].m_UpSkillCoolTime * 0.005f;
                GlobalValue.m_CrDataList[(int)i].m_MvPower += GlobalValue.m_CrDataList[(int)i].m_UpMvPower * 0.05f;
            }
        }

        m_Flag = true;
    }

    void NickChangeOK()
    {
        // 이름 변경
        string a_NickStr = m_NickID_InputField.text.Trim();
        if (a_NickStr == "")
        {
            MessageOnOff(true, "별명을 빈 칸없이 입력해주세요.");
            return;
        }

        if (!(2 <= m_NickID_InputField.text.Length && m_NickID_InputField.text.Length <= 20))
        {
            MessageOnOff(true, "별명은 2글자 이상 20글자 이하로 작성해주세요.");
            return;
        }

        UpdateNickNameCo(a_NickStr);

        m_NickChangeObj.SetActive(false);
    }

    // 닉네임 갱신
    void UpdateNickNameCo(string a_Nickname)
    {
        if (GlobalValue.g_UniqueID == "")
            return;

        if (a_Nickname == "")
            return;

        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = a_Nickname
            },
            (result) =>
            {
                GlobalValue.g_NickName = result.DisplayName;
                RefreshMyInfo();
            },
            (error) =>
            {

            });
    }

    void MessageOnOff(bool a_IsOn = true, string a_Mess = "")
    {
        if (a_IsOn)
        {
            m_Message_Txt.text = a_Mess;
            m_Message_Txt.gameObject.SetActive(a_IsOn);
            m_ShowMsTimer = 5.0f;
        }
        else
        {
            m_Message_Txt.text = a_Mess;
            m_Message_Txt.gameObject.SetActive(a_IsOn);
        }
    }

    // 로그아웃 처리
    void OnClickLogOut()
    {
        GlobalValue.g_UniqueID = "";
        GlobalValue.g_NickName = "";
        GlobalValue.g_UserGold = 0;
        GlobalValue.g_Rank = 0;
        GlobalValue.g_Exp = 0;
        PlayFabClientAPI.ForgetAllCredentials();
    }

    void SceneOut()
    {
        m_CacTime = 0.0f;
        m_AddTimer = 0.0f;
        m_StVal = 0.0f;
        m_EndVal = 1.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
    }

    public void SetRank(int a_Rank)
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
        GlobalValue.g_Rank = a_Rank;
        SceneName = "InGameScene";
        m_RankObj.SetActive(false);
        SceneOut();
    }
}
