using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopMgr : MonoBehaviour
{
    public Button m_GoLobby_Btn = null;
    public Text m_UserNick_Txt = null;
    public Text m_UserGold_Txt = null;

    public GameObject m_Char_ScrollContent; 
    public GameObject m_Char_NodeObj = null;
    public GameObject m_DlgPrefab = null;

    S_CharNodeCtrl[] m_CrList;              

    // 구매 시도 변수
    CharType m_BuyCrType;
    int m_SvMyGold = 0;                     

    void Start()
    {
        GlobalValue.InitData();

        if (m_GoLobby_Btn != null)
            m_GoLobby_Btn.onClick.AddListener(OnClcikBack);

        // ---- 아이템 목록 추가
        AddItemNode();

        // ---- UI 갱신
        RefreshUserInfo();
        RefreshCrList();
    }

    // 아이템 목록 추가
    void AddItemNode()
    {
        GameObject a_CharObj = null;
        S_CharNodeCtrl a_CharNode = null;

        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            a_CharObj = (GameObject)Instantiate(m_Char_NodeObj);
            a_CharNode = a_CharObj.GetComponent<S_CharNodeCtrl>();

            a_CharNode.InitData(GlobalValue.m_CrDataList[ii].m_CrType);

            a_CharObj.transform.SetParent(m_Char_ScrollContent.transform, false);
        }
    }

    // UI 갱신
    void RefreshCrList()
    {
        if (m_Char_ScrollContent != null)
        {
            if (m_CrList == null || m_CrList.Length <= 0)
                m_CrList = m_Char_ScrollContent.GetComponentsInChildren<S_CharNodeCtrl>();
        }

        // 찾기 위한 변수
        int a_FindAv = -1;
        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            // 이미 구매를 했다면 넘어간다.
            if (m_CrList[ii].m_CrType != GlobalValue.m_CrDataList[ii].m_CrType)
            {
                continue;
            }

            // 레벨이 0인 경우에만 (구매를 하지 않은 경우)
            if (GlobalValue.m_CrDataList[ii].m_Level <= 0)
            {
                // 첫 아이템만 이미지 활성화 
                if (a_FindAv < 0)
                {
                    //구입가능 표시
                    m_CrList[ii].SetState(CrState.BeforeBuy,
                                     GlobalValue.m_CrDataList[ii].m_Price);

                    a_FindAv = ii;
                }
                // 나머지는 이미지 비활성화
                else
                {
                    //전부 Lock 표시
                    m_CrList[ii].SetState(CrState.Lock,
                                    GlobalValue.m_CrDataList[ii].m_Price);
                }
                continue;
            }

            // 레벨이 0이 아닌 경우 활성화 (구매한 적이 있다면)
            m_CrList[ii].SetState(CrState.Active,
                                    GlobalValue.m_CrDataList[ii].m_UpPrice,
                                    GlobalValue.m_CrDataList[ii].m_Level);
        }
    }

    // UI 갱신
    void RefreshUserInfo()
    {
        m_UserNick_Txt.text = "닉네임 : " + GlobalValue.g_NickName;
        m_UserGold_Txt.text = "보유 골드 : " + GlobalValue.g_UserGold;
    }

    void OnClcikBack()
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
        SceneManager.LoadScene("LobbyScene");
    }

    // 구매 요청
    public void BuyCrItem(CharType a_CrType)
    {
        m_BuyCrType = a_CrType;
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
        BuyBeforeJobCo();
    }

    // 구매 1단계 검사 (서버로부터 골드, 아이템의 상태를 받아와서 클라이언트 동기화)
    bool m_IsDifferent = false;
    public void BuyBeforeJobCo()
    {
        if (GlobalValue.g_UniqueID == "")
            return;

        m_IsDifferent = false;

        var a_Request = new GetUserDataRequest()
        {
            PlayFabId = GlobalValue.g_UniqueID
        };

        PlayFabClientAPI.GetUserData(a_Request,
            (result) =>
            {
                // 유저 정보 받아오기 성공
                int a_GetValue = 0;
                int a_Idx = 0;
                foreach (var a_EachData in result.Data)
                {
                    if (a_EachData.Key == "UserGold")
                    {
                        if (int.TryParse(a_EachData.Value.Value, out a_GetValue) == false)
                            continue;

                        if (a_GetValue != GlobalValue.g_UserGold)
                            m_IsDifferent = true;

                        GlobalValue.g_UserGold = a_GetValue;
                    }
                    else if (a_EachData.Key.Contains("ChrList_") == true)
                    {
                        a_Idx = 0;
                        string[] a_StrArr = a_EachData.Key.Split('_');
                        if (2 <= a_StrArr.Length)
                        {
                            if (int.TryParse(a_StrArr[1], out a_Idx) == false)
                                continue;
                        }

                        if (GlobalValue.m_CrDataList.Count <= a_Idx)
                            continue;

                        if (int.TryParse(a_EachData.Value.Value, out a_GetValue) == false)
                            continue;

                        if (a_GetValue != GlobalValue.m_CrDataList[a_Idx].m_Level)
                            m_IsDifferent = true;

                        GlobalValue.m_CrDataList[a_Idx].m_Level = a_GetValue;
                    }
                }

                string a_Mess = "";
                CrState a_CrState = CrState.Lock;
                bool a_NeedDelegate = false;
                CharInfo a_CrInfo = GlobalValue.m_CrDataList[(int)m_BuyCrType];

                if (m_CrList != null && (int)m_BuyCrType < m_CrList.Length)
                {
                    a_CrState = m_CrList[(int)m_BuyCrType].m_CrState;
                }

                // 잠긴 상태
                if (a_CrState == CrState.Lock)
                {
                    a_Mess = "이 아이템은 Lock 상태로 구입할 수 없습니다.";
                }

                // 구매 가능 상태
                else if (a_CrState == CrState.BeforeBuy)
                {
                    if (GlobalValue.g_UserGold < a_CrInfo.m_Price)
                    {
                        a_Mess = "보유(누적) 골드가 모자랍니다.";
                    }
                    else
                    {
                        a_Mess = "정말 구입하시겠습니까?";
                        a_NeedDelegate = true;
                    }
                }
                // 활성화(업그레이드가능) 상태
                else if (a_CrState == CrState.Active)
                {
                    int a_Cost = a_CrInfo.m_UpPrice + (a_CrInfo.m_UpPrice * (a_CrInfo.m_Level - 1));
                    if (99 <= a_CrInfo.m_Level)
                    {
                        a_Mess = "최고 레벨입니다.";
                    }
                    else if (GlobalValue.g_UserGold < a_Cost)
                    {
                        a_Mess = "레벨업에 필요한 보유(누적) 골드가 모자랍니다.";
                    }
                    else
                    {
                        a_Mess = "정말 업그레이드하시겠습니까?";
                        a_NeedDelegate = true;
                    }
                }

                if (m_IsDifferent == true)
                    a_Mess += "\n(서버와 다른 정보가 있어서 수정되었습니다.)";

                GameObject a_DlgBoxObj = Instantiate(m_DlgPrefab);
                GameObject a_Canvas = GameObject.Find("Canvas");
                a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);
                DialogCtrl a_DlgBox = a_DlgBoxObj.GetComponent<DialogCtrl>();
                if (a_DlgBox != null)
                {
                    if (a_NeedDelegate == true)
                        a_DlgBox.SetMessage(a_Mess, TryBuyCrItem);
                    else
                        a_DlgBox.SetMessage(a_Mess);
                }
            },
            (error) =>
            {
                Debug.Log("유저 정보 받아오기 실패");
            });
    }

    // 서버에 값을 전달하기 위한 배열
    List<int> a_SetLevel = new List<int>();
    // 구매 2단계 검사 (서버에 전달할 데이터 값을 만들어준다.)
    public void TryBuyCrItem()
    {
        Debug.Log("이제 아이템 구매 시도");

        bool a_BuyOK = false;
        CharInfo a_CrInfo = null;
        a_SetLevel.Clear();

        // 아이템의 레벨을 넣어준다.
        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            a_CrInfo = GlobalValue.m_CrDataList[ii];
            a_SetLevel.Add(a_CrInfo.m_Level);

            if (!(ii == (int)m_BuyCrType && a_CrInfo.m_Level <= 99))
                continue;

            int a_Cost = a_CrInfo.m_Price;

            if (0 < a_CrInfo.m_Level)
                a_Cost = a_CrInfo.m_UpPrice + (a_CrInfo.m_UpPrice * (a_CrInfo.m_Level - 1));

            if (GlobalValue.g_UserGold < a_Cost)
                continue;

            // 골드 차감 (서버로부터 응답을 받고 차감해주는 방식)
            m_SvMyGold = GlobalValue.g_UserGold;        // 골드 값 차감 백업
            m_SvMyGold -= a_Cost;                       // 골드 값 차감
            a_SetLevel[ii]++;                           // 레벨 증가 백업

            // 서버에 아이템 구매 요청이 확실히 필요하다는 의미
            a_BuyOK = true;
        }

        // 골드 차감 (서버로부터 응답을 받고 차감해주는 방식)
        if (a_BuyOK == true)
            BuyRequestCo();
    }

    // 서버 구매 요청
    // 구매 3단계 검사 (서버에 데이터 전달하기)
    public void BuyRequestCo()
    {
        Debug.Log("구매 3단계 검사");

        // 로그인한 상태가 아니라면
        if (GlobalValue.g_UniqueID == "")
            return;

        // 아이템 목록이 정상적이지 않다면
        if (a_SetLevel.Count <= 0)
            return;

        Dictionary<string, string> a_CharList = new Dictionary<string, string>();
        string a_MkKey = "";
        a_CharList.Clear();
        a_CharList.Add("UserGold", m_SvMyGold.ToString());

        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            if (GlobalValue.m_CrDataList[ii].m_CrType == m_BuyCrType)
            {
                a_MkKey = "ChrList_" + ii.ToString();
                a_CharList.Add(a_MkKey, a_SetLevel[ii].ToString());
                break;
            }
        }

        Debug.Log(a_CharList.Count);

        var a_Request = new UpdateUserDataRequest()
        {
            Data = a_CharList
        };

        PlayFabClientAPI.UpdateUserData(a_Request,
            (result) =>
            {
                RefreshMyInfoCo();
            },
            (error) =>
            {
                Debug.Log(error);
            });
    }

    // 구매 4단계 검사 (로컬 변수, UI값을 갱신해준다.)
    void RefreshMyInfoCo()
    {
        Debug.Log("구매 4단계 검사");

        // 잘못된 타입을 가지고 있다면
        if (m_BuyCrType < CharType.Char_SW || CharType.CrCount <= m_BuyCrType)
            return;

        GlobalValue.g_UserGold = m_SvMyGold;

        UpgradeChar();

        GlobalValue.m_CrDataList[(int)m_BuyCrType].m_Level = a_SetLevel[(int)m_BuyCrType];

        RefreshCrList();
        RefreshUserInfo();
    }

    // 레벨업 능력치
    void UpgradeChar()
    {
        for (int i = GlobalValue.m_CrDataList[(int)m_BuyCrType].m_Level; i < a_SetLevel[(int)m_BuyCrType]; i++)
        {
            GlobalValue.m_CrDataList[(int)m_BuyCrType].m_Attack += GlobalValue.m_CrDataList[(int)m_BuyCrType].m_UpAttack * 0.05f;
            GlobalValue.m_CrDataList[(int)m_BuyCrType].m_AtkDistance += GlobalValue.m_CrDataList[(int)m_BuyCrType].m_UpAttDistanc * 0.05f;
            GlobalValue.m_CrDataList[(int)m_BuyCrType].m_AtkSpeed -= GlobalValue.m_CrDataList[(int)m_BuyCrType].m_UpAtkSpeed * 0.005f;
            GlobalValue.m_CrDataList[(int)m_BuyCrType].m_Hp += GlobalValue.m_CrDataList[(int)m_BuyCrType].m_UpHp * 0.05f;
            GlobalValue.m_CrDataList[(int)m_BuyCrType].m_SkillCoolTime -= GlobalValue.m_CrDataList[(int)m_BuyCrType].m_UpSkillCoolTime * 0.005f;
            GlobalValue.m_CrDataList[(int)m_BuyCrType].m_MvPower += GlobalValue.m_CrDataList[(int)m_BuyCrType].m_UpMvPower * 0.05f;
        }
    }
}
