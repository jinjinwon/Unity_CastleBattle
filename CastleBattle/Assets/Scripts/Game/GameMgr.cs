using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public enum InfoType
{
    HP
}

public class GameMgr : MonoBehaviour
{
    public static GameMgr Inst;

    // 게임 UI
    [Header("GameMgr UI")]
    public Button m_Exit_Btn = null;
    public Text m_Timer_Txt = null;
    public Text m_CurGold_Txt = null;
    float m_GameTime = 180.0f;
    float m_CurTime = 0.0f;

    // 유저 정보
    [Header("CharInfo Root")]
    public Text m_PHp_Txt = null;
    public Image m_PHp_Img = null;

    // 적 정보
    [Header("EnemyInfo Root")]
    public Text m_EHp_Txt = null;
    public Image m_EHp_Img = null;

    // 캐릭터 스크롤 뷰
    [Header("CreateChar_Scroll View")]
    public Transform m_CrScContent;
    public GameObject m_CrNodePrefab = null;

    // 데미지 텍스트
    [Header("-------- Damage Text --------")]
    public GameObject m_DamageObj = null;
    public GameObject m_GoldObj = null;
    Canvas m_RefCanavs = null;
    Vector3 a_StCacPos = Vector3.zero;

    // 공용 변수
    [Header("Public Variable")]
    public GameObject m_GetResourceObj = null;
    public GameObject m_DlgPrefab = null;
    BaseCtrl m_BCObj = null;
    [HideInInspector] public int m_GetGold = 0;
    public Text m_RankText = null;
    bool m_IsNetworkLock = false;

    public int m_AtkGemLevel = 0;
    public int m_HpGemLevel = 0;

    // 공용 프리팹
    public GameObject m_BWZ_Particle = null;
    public GameObject m_RWZ_Particle = null;

    // 페이드 인
    [Header("Fade UI")]
    //------ Fade In 관련 변수들...
    public Image m_FadeImg = null;
    float AniDuring = 0.8f;  //페이드아웃 연출을 시간 설정
    [HideInInspector] public bool m_StartFade = false;
    float m_CacTime = 0.0f;
    float m_AddTimer = 0.0f;
    Color m_Color;
    float m_StVal = 1.0f;
    float m_EndVal = 0.0f;
    [HideInInspector] public bool m_DlgActive = false;

    // 적 거리 표시
    [Header("Enemy Distance")]
    public GameObject m_BaseObj = null;
    public GameObject m_EnemyDistObj = null;
    public Text m_EnemyDist_Txt = null;
    GameObject[] m_EnemyList = null;
    Vector2 m_CacTgVec = Vector2.zero;
    Vector2 m_CacWdVec = Vector2.zero;
    float m_MaxDist = 15.0f;
    int m_ICount = 0;

    // 게임 오버 변수
    [Header("Game Over")]
    public GameObject m_GameoverObj = null;
    public Image m_Victory_Img = null;
    public Image m_Defeat_Img = null;
    public Text m_Explanation_Text = null;
    public Button m_Lobby_Btn = null;
    int m_GetRkGold = 0;

    // 머티리얼 교체
    public Material m_ChangeMtrl = null;

    [HideInInspector] public bool m_GameOver = false;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        Time.timeScale = 1;
        GlobalValue.InitData();
        AudioMgr.Inst.PlayBGM("BGM", 0.5f);

        if (m_Exit_Btn != null)
            m_Exit_Btn.onClick.AddListener(OnClickExitBtn);

        if (m_Lobby_Btn != null)
            m_Lobby_Btn.onClick.AddListener(SceneOut);

        if (m_RankText != null)
            m_RankText.text = GlobalValue.g_Rank.ToString() + " 단계";

        RenewMyCharList();
        UpdateGold();
    }

    void Update()
    {
        if (m_DlgActive == true)
            return;

        FindEnemyDist();

        // Timer();
        if(m_GameOver == false)
            m_GameTime -= Time.deltaTime;

        m_Timer_Txt.text = ((int)m_GameTime / 60 % 60).ToString() + " : " + ((int)m_GameTime % 60).ToString();

        if(m_GameTime <= 0.0f)
        {
            Time.timeScale = 0;
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
                        SceneManager.LoadScene("LobbyScene");
                    }
                }
            }
        }
    }

    // 데미지 텍스트 스폰 함수
    public static void DamageTxt(int a_Value, Transform txtTr , float a_PosValue_X = 0.3f , float a_PosValue_Y = 1.5f)
    {
        GameObject a_DamClone = Instantiate(Inst.m_DamageObj);
        if (a_DamClone != null)
        {
            if (txtTr.gameObject.tag == "P_Unit" || txtTr.gameObject.tag == "P_Base")
                a_PosValue_X = -0.3f;

            Vector3 a_StCacPos = new Vector3(txtTr.position.x + a_PosValue_X, txtTr.position.y + a_PosValue_Y, txtTr.position.z);

            Inst.m_RefCanavs = txtTr.gameObject.GetComponentInChildren<Canvas>();
            a_DamClone.transform.SetParent(Inst.m_RefCanavs.transform);
            Damage_Text a_DamageTx = a_DamClone.GetComponent<Damage_Text>();
            a_DamageTx.m_DamageVal = a_Value;
            a_DamClone.transform.position = a_StCacPos;
        }
    }

    // 골드 텍스트 스폰 함수
    public static void GoldTxt(int a_Value, Transform txtTr)
    {
        GameObject a_DamClone = (GameObject)Instantiate(Inst.m_GoldObj);
        if (a_DamClone != null)
        {
            Vector3 a_StCacPos = new Vector3(txtTr.position.x - 0.4f, txtTr.position.y + 3.5f, txtTr.position.z);

            Inst.m_RefCanavs = txtTr.gameObject.GetComponent<Canvas>();
            a_DamClone.transform.SetParent(Inst.m_RefCanavs.transform);
            Damage_Text a_DamageTx = a_DamClone.GetComponent<Damage_Text>();
            a_DamageTx.m_DamageVal = a_Value;
            a_DamClone.transform.position = a_StCacPos;
        }
    }

    // 노드 생성
    void RenewMyCharList()
    {
        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            if (GlobalValue.m_CrDataList[ii].m_Level <= 0)
                break;

            if ((int)GlobalValue.m_CrDataList[ii].m_CrType >= 8)
                return;

            GameObject a_CharClone = Instantiate(m_CrNodePrefab) as GameObject;
            a_CharClone.GetComponent<CrNodeCtrl>().InitState(GlobalValue.m_CrDataList[ii]);

            a_CharClone.transform.SetParent(m_CrScContent);

            a_CharClone.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    public void UpdateGold()
    {
        m_CurGold_Txt.text = "보유 골드 : " + GlobalValue.g_UserGold.ToString();

        UpdateGoldCo();
    }

    public void UpdateBaseUI(BaseCtrl a_Target , InfoType a_Type)
    {
        m_BCObj = a_Target;

        if (a_Type == InfoType.HP)
        {
            if (a_Target.tag == "P_Base")
            {
                m_PHp_Txt.text = (int)m_BCObj.m_CurHp + " / " + (int)m_BCObj.m_MaxHp;
                m_PHp_Img.fillAmount = m_BCObj.m_CurHp / m_BCObj.m_MaxHp;
                m_BCObj.m_SprRender.sprite = m_BCObj.m_PBaseSpt[2];
            }
            else
            {
                m_EHp_Txt.text = (int)m_BCObj.m_CurHp + " / " + (int)m_BCObj.m_MaxHp;
                m_EHp_Img.fillAmount = m_BCObj.m_CurHp / m_BCObj.m_MaxHp;
                m_BCObj.m_SprRender.sprite = m_BCObj.m_EBaseSpt[2];
            }
        }
    }

    string a_Mess = "";
    void OnClickExitBtn()
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
        GameObject a_DlgBoxObj = Instantiate(m_DlgPrefab);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);
        DialogCtrl a_DlgBox = a_DlgBoxObj.GetComponent<DialogCtrl>();

        a_Mess = "현재까지 획득한 골드는 총 " + m_GetGold + "입니다. \n 게임을 종료하시겠습니까?";

        if (a_DlgBox != null)
        {
            m_DlgActive = true;
            a_DlgBox.SetMessage(a_Mess, SceneOut);
        }
    }
    
    void SceneOut()
    {
        AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
        m_DlgActive = false;
        m_GameoverObj.SetActive(false);
        m_CacTime = 0.0f;
        m_AddTimer = 0.0f;
        m_StVal = 0.0f;
        m_EndVal = 1.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
        UpdateGoldCo();
    }

    public void Die(BaseCtrl a_Target, bool a_Die)
    {
        m_BCObj = a_Target;

        // 예외처리
        if (a_Die == false)
            return;

        // 게임 종료 로직
        if (m_BCObj.tag == "P_Base")
        {
            Debug.Log("Enemy 승리");
            m_BCObj.m_SprRender.sprite = m_BCObj.m_PBaseSpt[1];
            m_GameOver = true;
            Defeat();
        }
        else
        {
            Debug.Log("Player 승리");
            m_BCObj.m_SprRender.sprite = m_BCObj.m_EBaseSpt[1];
            m_GameOver = true;
            Victory();
        }
    }

    // 골드 값 갱신
    void UpdateGoldCo()
    {
        // 고유 ID가 없다면
        if (GlobalValue.g_UniqueID == "")
            return;

        // <플레이어 데이터(타이틀)> 값 활용 코드
        var a_Request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"UserGold", GlobalValue.g_UserGold.ToString()}
            }
        };

        // 요청 부분
        m_IsNetworkLock = true;
        PlayFabClientAPI.UpdateUserData(a_Request,
            (result) =>
            {
                m_IsNetworkLock = false;
            },
            (error) =>
            {
                m_IsNetworkLock = false;
            });
    }

    // 거리 찾는 함수
    void FindEnemyDist()
    {
        m_EnemyList = GameObject.FindGameObjectsWithTag("E_Unit");

        if (m_EnemyList.Length <= 0)
        {
            if(m_EnemyDistObj.activeSelf == true)
                m_EnemyDistObj.SetActive(false);

            return;
        }

        float a_MinLen = float.MaxValue;
        GameObject a_Target = null;
        m_ICount = m_EnemyList.Length;


        for(int i = 0; i <m_ICount; i++)
        {
            m_CacTgVec = m_BaseObj.transform.position - m_EnemyList[i].transform.position;
            m_CacTgVec.y = 0.0f;

            if(m_CacTgVec.magnitude < a_MinLen)
            {
                a_MinLen = m_CacTgVec.magnitude;
                a_Target = m_EnemyList[i];
            }
        }

        m_CacWdVec = Camera.main.transform.position - a_Target.transform.position;
        float a_CamLen = m_CacWdVec.magnitude;

        if (a_CamLen <= 9.0f)
        {
            m_EnemyDistObj.SetActive(false);
            return;
        }

        if (a_MinLen < m_MaxDist)
        {
            a_MinLen = float.MaxValue;
            m_EnemyDistObj.SetActive(false);
        }
        else
        {
            if(m_EnemyDistObj.activeSelf == false)
                m_EnemyDistObj.SetActive(true);

            m_EnemyDist_Txt.text = "Enemy " + a_MinLen.ToString("F0") + " m";
        }
    }

    string a_GO_Mess = "";
    int a_RefGold = 0;
    void Victory()
    {
        AudioMgr.Inst.PlayEffSound("Victory", 0.5f);
        a_GO_Mess = "전투에서 승리하셨습니다.\n적의 성을 함락하여 보유 골드가 2배가 되었습니다.";
        a_RefGold = GlobalValue.g_UserGold;
        GlobalValue.g_UserGold = (a_RefGold * 2);
        RankCompensation(GlobalValue.g_Rank, 0);
        m_GameoverObj.SetActive(true);
        m_Defeat_Img.gameObject.SetActive(false);
        m_Victory_Img.gameObject.SetActive(true);
        m_Explanation_Text.text = a_GO_Mess + "\n 단계에 따라 " + m_GetRkGold.ToString() + "골드가 추가로 지급됩니다.";
        m_GameOver = true;
        UpdateGoldCo();
    }

    void Defeat()
    {
        AudioMgr.Inst.PlayEffSound("Defeat", 0.5f);
        a_GO_Mess = "전투에서 패배하셨습니다.\n성이 함락당해 보유 골드가 절반이 되었습니다.";
        a_RefGold = GlobalValue.g_UserGold;
        GlobalValue.g_UserGold = (a_RefGold / 2);
        RankCompensation(GlobalValue.g_Rank, 1);
        m_GameoverObj.SetActive(true);
        m_Victory_Img.gameObject.SetActive(false);
        m_Defeat_Img.gameObject.SetActive(true);
        m_Explanation_Text.text = a_GO_Mess + "\n 단계에 따라 -" + m_GetRkGold.ToString() + "골드가 추가로 차감됩니다.";
        m_GameOver = true;
        UpdateGoldCo();
    }

    void RankCompensation(int a_Rank , int a_WinOrLose)
    {
        // 0이면 승리 1이면 패배

        int a_Gold = 1000 * a_Rank;
        m_GetRkGold = a_Gold;

        if (a_WinOrLose == 0)
            GlobalValue.g_UserGold += a_Gold;
        else
            GlobalValue.g_UserGold -= a_Gold;

        if (GlobalValue.g_UserGold <= 0)
            GlobalValue.g_UserGold = 0;

        Debug.Log(GlobalValue.g_UserGold);
    }
}
