using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

//----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
using System.Globalization;
using System.Text.RegularExpressions;
using System;
using SimpleJSON;


public class TitleMgr : MonoBehaviour
{
    [Header("---------------------LoginPanel----------------------")]
    public GameObject m_LoginPanel = null;
    public InputField m_ID_InputField = null;
    public InputField m_PW_InputField = null;
    public Button m_Login_Btn = null;
    public Button m_CreateAccountOpen_Btn = null;

    [Header("---------------------CreateAccountPanel----------------------")]
    public GameObject m_CreateAccountPanel = null;
    public InputField m_NewID_InputField = null;
    public InputField m_NewPW_InputField = null;
    public InputField m_NewNick_InputField = null;
    public Button m_CreateAccount_Btn = null;
    public Button m_Cancel_Btn = null;

    [Header("---------------------NoticePanel----------------------")]
    public GameObject m_NoticePanel = null;
    public Text m_Notice_Txt = null;
    public Button m_No_Cancel_Btn = null;

    private bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    private bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크

    //------ Fade Out 관련 변수들...
    public Image m_FadeImg = null;
    private float AniDuring = 0.8f;  //페이드아웃 연출을 시간 설정
    private bool m_StartFade = false;
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    private Color m_Color;

    float m_IsOnece = 0.02f;
    public static bool m_FirstCreate = false;
    string m_RefID = "";

    void Start()
    {
        GlobalValue.InitData();

        if (m_CreateAccountOpen_Btn != null)
            m_CreateAccountOpen_Btn.onClick.AddListener(OpenCreateAccBtn);

        if (m_Cancel_Btn != null)
            m_Cancel_Btn.onClick.AddListener(CancelCreateAccBtn);

        if (m_CreateAccount_Btn != null)
            m_CreateAccount_Btn.onClick.AddListener(CreateAccoutBtn);

        if (m_Login_Btn != null)
            m_Login_Btn.onClick.AddListener(LoginBtn);

        if (m_No_Cancel_Btn != null)
            m_No_Cancel_Btn.onClick.AddListener(() =>
            {
                if (m_NoticePanel.activeSelf == false)
                    return;

                m_NoticePanel.SetActive(false);
            });
    }

    void Update()
    {
        if(0 < m_IsOnece)
        {
            m_IsOnece -= Time.deltaTime;
            if(m_IsOnece <= 0.0f)
                AudioMgr.Inst.PlayBGM("TITLE", 0.5f);
        }

        if (m_StartFade == true)
        {
            if (m_CacTime < 1.0f)
            {
                m_AddTimer = m_AddTimer + Time.deltaTime;
                m_CacTime = m_AddTimer / AniDuring;
                m_Color = m_FadeImg.color;
                m_Color.a = m_CacTime;
                m_FadeImg.color = m_Color;
                if (1.0f <= m_CacTime)
                {
                    SceneManager.LoadScene("LobbyScene");
                }
            }
        }
    }

    // 계정 생성 버튼 누를 시
    public void OpenCreateAccBtn()
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);

        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(false);

        if (m_CreateAccountPanel != null)
            m_CreateAccountPanel.SetActive(true);
    }

    // 취소 버튼 누를 시
    public void CancelCreateAccBtn()
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);

        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(true);

        if (m_CreateAccountPanel != null)
            m_CreateAccountPanel.SetActive(false);
    }

    // 계정 생성 요청 함수
    public void CreateAccoutBtn()
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);

        string a_IdStr = m_NewID_InputField.text;
        string a_PwStr = m_NewPW_InputField.text;
        string a_NickStr = m_NewNick_InputField.text;

        // 예외처리
        if (a_IdStr.Trim() == "" || a_PwStr.Trim() == "" || a_NickStr.Trim() == "")
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "ID, PW, 별명을 빈칸 없이 입력해주세요.";
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 30))
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "ID는 3글자 이상 20글자 이하로 입력해주세요.";
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length < 20))
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "PW는 6글자 이상 20글자 이하로 입력해주세요.";
            return;
        }

        if (!(1 <= a_NickStr.Length && a_NickStr.Length < 20))
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "별명은 2글자 이상 20글자 이하로 입력해주세요.";
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "Email 형식이 맞지 않습니다.";
            return;
        }

        var a_Request = new RegisterPlayFabUserRequest
        {
            Email = m_NewID_InputField.text,
            Password = m_NewPW_InputField.text,
            DisplayName = m_NewNick_InputField.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(a_Request, RegisterSuccess, RegisterFailure);
    }

    // 계정 생성 요청 성공
    void RegisterSuccess(RegisterPlayFabUserResult a_Result)
    {
        m_NoticePanel.SetActive(true);
        m_CreateAccountPanel.SetActive(false);
        m_LoginPanel.SetActive(true);
        m_Notice_Txt.text = "가입 성공!";
        m_RefID = m_NewID_InputField.text;
    }

    // 계정 생성 요청 실패
    void RegisterFailure(PlayFabError a_Error)
    {
        string a_Str = "";

        if (a_Error.GenerateErrorReport().Contains("Email address not available") == true)
            a_Str = "중복된 아이디입니다.";

        m_NoticePanel.SetActive(true);
        m_Notice_Txt.text = "가입 실패 : " + a_Str;

        m_NewID_InputField.text = "";
        m_NewPW_InputField.text = "";
        m_NewNick_InputField.text = "";
    }

    // 올바른 이메일인지 체크
    bool CheckEmailAddress(string a_EmailStr)
    {
        if (string.IsNullOrEmpty(a_EmailStr)) isValidFormat = false;

        a_EmailStr = Regex.Replace(a_EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType) isValidFormat = false;

        // true 로 반환할 시, 올바른 이메일 포맷임.
        isValidFormat = Regex.IsMatch(a_EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);

        return isValidFormat;
    }

    // 도메인으로 변경해줌
    string DomainMapper(Match match)
    {
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            invalidEmailType = true;
        }
        return match.Groups[1].Value + domainName;
    }

    // 로그인 요청
    public void LoginBtn()
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);

        // ID,PW 받아오기
        string a_IdStr = m_ID_InputField.text;
        string a_PwStr = m_PW_InputField.text;

        // 빈칸에 대한 예외처리
        if (a_IdStr.Trim() == "" || a_PwStr.Trim() == "")
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "ID, PW를 빈칸 없이 입력해주세요.";
            return;
        }

        // 혹시 모르는 상황을 대비한 예외처리
        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 30))
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "ID는 3글자 이상 20글자 이하로 입력해주세요.";
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length < 20))
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "PW는 6글자 이상 20글자 이하로 입력해주세요.";
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            m_NoticePanel.SetActive(true);
            m_Notice_Txt.text = "Email 형식이 맞지 않습니다.";
            return;
        }

        // 이 옵션을 추가해 줘야 로그인하면서 유저의 각종 정보를 가져올 수 있다.
        var a_Option = new GetPlayerCombinedInfoRequestParams()
        {
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            },
            GetUserData = true,
            GetPlayerStatistics = true

        };

        var a_Request = new LoginWithEmailAddressRequest
        {
            Email = m_ID_InputField.text,
            Password = m_PW_InputField.text,

            InfoRequestParameters = a_Option
        };

        PlayFabClientAPI.LoginWithEmailAddress(a_Request, OnLoginSucces, OnLoginFailure);
    }

    // 로그인 요청 성공
    void OnLoginSucces(LoginResult a_Result)
    {
        GlobalValue.g_UniqueID = a_Result.PlayFabId;

        if (m_RefID == m_ID_InputField.text) m_FirstCreate = true;

        // 값 받아오기
        if (a_Result.InfoResultPayload != null)
        {
            GlobalValue.g_NickName = a_Result.InfoResultPayload.PlayerProfile.DisplayName;

            // 골드 값 받아오기
            int a_GetValue = 0;
            int a_Idx = 0;
            foreach (var a_EachData in a_Result.InfoResultPayload.UserData)
            {
                if (a_EachData.Key == "UserGold")
                {
                    if (int.TryParse(a_EachData.Value.Value, out a_GetValue) == true)
                        GlobalValue.g_UserGold = a_GetValue;
                }
                else if (a_EachData.Key.Contains("ChrList_") == true)
                {
                    a_Idx = 0;
                    string[] strArr = a_EachData.Key.Split('_');

                    if (2 <= strArr.Length)
                    {
                        //Idx = int.Parse(strArr[1]);
                        if (int.TryParse(strArr[1], out a_Idx) == false)
                            Debug.Log("string -> int : TryParse 실패");
                    }

                    if (GlobalValue.m_CrDataList.Count <= a_Idx)
                        continue;

                    if (int.TryParse(a_EachData.Value.Value, out a_GetValue) == false)
                        Debug.Log("string -> int : TryParse 실패");

                    GlobalValue.m_CrDataList[a_Idx].m_Level = a_GetValue;
                }
            }
        }
        StBtnClick();
    }

    // 로그인 요청 실패
    void OnLoginFailure(PlayFabError a_Error)
    {
        string a_Str = "";

        if (a_Error.GenerateErrorReport().Contains("User not found") == true)
            a_Str = "계정이 존재하지 않습니다.";

        if (a_Error.GenerateErrorReport().Contains("Invalid email address or password") == true)
            a_Str = "아이디 또는 비밀번호가 틀립니다.";

        m_NoticePanel.SetActive(true);
        m_Notice_Txt.text = "로그인 실패 : " + a_Str;

        m_ID_InputField.text = "";
        m_PW_InputField.text = "";
    }

    public void StBtnClick()
    {
        m_FadeImg.gameObject.SetActive(true);
        if (m_StartFade == false)
        {
            m_StartFade = true;
        }
    }
}
