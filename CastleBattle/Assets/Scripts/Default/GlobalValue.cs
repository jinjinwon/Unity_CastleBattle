using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using SimpleJSON;

public enum CharType
{   
    Char_SW,        // 견습 전사
    Char_AR,        // 견습 궁수
    Char_GU,        // 견습 기사
    Char_WI,        // 견습 마법사
    Char_SWU,       // 정식 전사
    Char_ARU,       // 정식 궁수
    Char_GUU,       // 정식 기사
    Char_WIU,       // 정식 마법사
    Gold_Upgrade,   // 골드 업그레이드         (스킬)
    Atk_Upgrade,    // 공격력 업그레이드       (스킬)
    HP_Upgrade,     // 공격속도 업그레이드     (스킬)
    CrCount
}

// 각 캐릭터 정보
public class CharInfo
{
    public string m_Name = "";                          // 캐릭터 이름
    public CharType m_CrType = CharType.Char_SW;        // 캐릭터 타입
    public float m_Attack = 10.0f;                      // 공격력
    public float m_AtkDistance = 10.0f;                 // 공격 사정거리
    public float m_AtkSpeed = 10.0f;                    // 공격 속도
    public float m_Hp = 100.0f;                         // 체력
    public float m_MvPower = 5.0f;                      // 이동 속도
    public float m_SkillCoolTime = 10.0f;               // 스킬 쿨타임
    public int m_Price = 500;                           // 캐릭터 가격
    public int m_UpPrice = 250;                         // 업그레이드 가격
    public int m_Level = 0;                             // 캐릭터 레벨
    public string m_SkillExp_1 = "";                    // 스킬 효과 설명
    public string m_SkillExp_2 = "";                    // 스킬 효과 설명
    public Sprite m_IconImg = null;                     // 아이콘 이미지

    public float m_UpAttack;                            // 레벨 업 공격력
    public float m_UpAttDistanc;                        // 레벨 업 공격 사정거리
    public float m_UpAtkSpeed;                          // 레벨 업 공격 속도
    public float m_UpHp;                                // 레벨 업 체력
    public float m_UpMvPower;                           // 레벨 업 이동 속도
    public float m_UpSkillCoolTime;                     // 레벨 업 스킬 쿨타임

    // 캐릭터 설정
    public void SetType(CharType a_CrType)
    {
        m_CrType = a_CrType;

        if (a_CrType == CharType.Char_SW)
        {
            m_Name = "견습 전사";

            // 캐릭터 능력치
            m_Attack = 15.0f;
            m_AtkDistance = 0.5f;
            m_Hp = 100.0f;
            m_AtkSpeed = 3.0f;
            m_MvPower = 0.5f;
            m_SkillCoolTime = 60.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;


            m_Price = 500; 
            m_UpPrice = 250;

            m_SkillExp_1 = "자신의 공격력";
            m_SkillExp_2 = "30% 상승";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait S")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Char_AR)
        {
            m_Name = "견습 궁수";

            m_Attack = 10.0f;
            m_AtkDistance = 2.5f;
            m_Hp = 80.0f;
            m_AtkSpeed = 2.0f;
            m_MvPower = 0.8f;
            m_SkillCoolTime = 60.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;


            m_Price = 500;
            m_UpPrice = 250;

            m_SkillExp_1 = "자신의 공격속도";
            m_SkillExp_2 = "30% 상승";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait A")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Char_GU)
        {
            m_Name = "견습 기사";

            m_Attack = 5.0f;
            m_AtkDistance = 0.3f;
            m_Hp = 200.0f;
            m_AtkSpeed = 3.5f;
            m_MvPower = 0.5f;
            m_SkillCoolTime = 60.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;

            m_Price = 500; 
            m_UpPrice = 250;

            m_SkillExp_1 = "자신의 체력";
            m_SkillExp_2 = "50% 회복";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait G")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Char_WI)
        {
            m_Name = "견습 마법사";

            m_Attack = 10.0f;
            m_AtkDistance = 1.5f;
            m_Hp = 80.0f;
            m_AtkSpeed = 2.5f;
            m_MvPower = 0.7f;
            m_SkillCoolTime = 6.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;

            m_Price = 750;
            m_UpPrice = 500;

            m_SkillExp_1 = "전방에";
            m_SkillExp_2 = "스킬 시전";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait W")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Char_SWU)
        {
            m_Name = "정식 전사";

            // 캐릭터 능력치
            m_Attack = 30.0f;
            m_AtkDistance = 0.5f;
            m_Hp = 200.0f;
            m_AtkSpeed = 2.3f;
            m_MvPower = 0.7f;
            m_SkillCoolTime = 60.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;

            m_Price = 1000;
            m_UpPrice = 500;

            m_SkillExp_1 = "자신의 공격력";
            m_SkillExp_2 = "60% 상승";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait SU")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Char_ARU)
        {
            m_Name = "정식 궁수";

            m_Attack = 25.0f;
            m_AtkDistance = 1.5f;
            m_Hp = 150.0f;
            m_AtkSpeed = 1.5f;
            m_MvPower = 0.9f;
            m_SkillCoolTime = 60.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;

            m_Price = 1000;
            m_UpPrice = 500;

            m_SkillExp_1 = "자신의 공격속도";
            m_SkillExp_2 = "60% 상승";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait AU")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Char_GUU)
        {
            m_Name = "정식 기사";

            m_Attack = 5.0f;
            m_AtkDistance = 0.5f;
            m_Hp = 500.0f;
            m_AtkSpeed = 3.0f;
            m_MvPower = 0.5f;
            m_SkillCoolTime = 60.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;

            m_Price = 1000;
            m_UpPrice = 500;

            m_SkillExp_1 = "자신의 체력";
            m_SkillExp_2 = "100% 회복";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait GU")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Char_WIU)
        {
            m_Name = "정식 마법사";

            m_Attack = 10.0f;
            m_AtkDistance = 1.5f;
            m_Hp = 150.0f;
            m_AtkSpeed = 2.5f;
            m_MvPower = 0.7f;
            m_SkillCoolTime = 60.0f;

            // 레벨 업 능력치
            m_UpAttack = m_Attack;
            m_UpAttDistanc = m_AtkDistance;
            m_UpAtkSpeed = m_AtkSpeed;
            m_UpHp = m_Hp;
            m_UpMvPower = m_MvPower;
            m_UpSkillCoolTime = m_SkillCoolTime;

            m_Price = 1500;
            m_UpPrice = 750;

            m_SkillExp_1 = "전방에";
            m_SkillExp_2 = "스킬 시전";

            for (int ii = 0; ii < GlobalValue._Sprites.Length; ii++)
            {
                if (GlobalValue._Sprites[ii].name == "Portrait WU")
                {
                    m_IconImg = GlobalValue._Sprites[ii];
                    break;
                }
            }
        }
        else if(a_CrType == CharType.Gold_Upgrade)
        {
            m_Name = "Gold Gem";

            m_SkillExp_1 = "획득 골드가";
            m_SkillExp_2 = "증가합니다.";

            m_Price = 200;
            m_UpPrice = 100;

            for (int ii = 0; ii < GlobalValue.Skill_Sprites.Length; ii++)
            {
                if (GlobalValue.Skill_Sprites[ii].name == "Gem_3")
                {
                    m_IconImg = GlobalValue.Skill_Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.Atk_Upgrade)
        {
            m_Name = "Atk Gem";

            m_SkillExp_1 = "공격력이";
            m_SkillExp_2 = "증가합니다.";

            m_Price = 200;
            m_UpPrice = 100;

            for (int ii = 0; ii < GlobalValue.Skill_Sprites.Length; ii++)
            {
                if (GlobalValue.Skill_Sprites[ii].name == "Gem_2")
                {
                    m_IconImg = GlobalValue.Skill_Sprites[ii];
                    break;
                }
            }
        }
        else if (a_CrType == CharType.HP_Upgrade)
        {
            m_Name = "HP Gem";

            m_SkillExp_1 = "체력이";
            m_SkillExp_2 = "증가합니다.";

            m_Price = 200;
            m_UpPrice = 100;

            for (int ii = 0; ii < GlobalValue.Skill_Sprites.Length; ii++)
            {
                if (GlobalValue.Skill_Sprites[ii].name == "Gem_0")
                {
                    m_IconImg = GlobalValue.Skill_Sprites[ii];
                    break;
                }
            }
        }
    }
}


public class GlobalValue
{
    public static string g_UniqueID = "";       // 고유 ID 
    public static string g_NickName = "";       // 별명   
    public static int g_UserGold = 0;
    public static int g_Rank = 0;               // 난이도
    public static int g_Exp = 0;                // 경험치

    public static Sprite[] _Sprites = Resources.LoadAll<Sprite>("UI");
    public static Sprite[] Skill_Sprites = Resources.LoadAll<Sprite>("Gem");

    public static Dictionary<string, Sprite> _Dictionary = new Dictionary<string, Sprite>();

    // 캐릭터 리스트
    public static List<CharInfo> m_CrDataList = new List<CharInfo>();


    // 캐릭터 리스트에 추가
    public static void InitData()
    {
        if (0 < m_CrDataList.Count)
        {
            return;
        }

        CharInfo a_CrNode;
        for(int ii = 0; ii < (int)CharType.CrCount; ii++)
        {
            a_CrNode = new CharInfo();
            a_CrNode.SetType((CharType)ii);
            m_CrDataList.Add(a_CrNode);
        }

        if (_Sprites.Length != 0)
            return;

        for (int i = 0; i < _Sprites.Length; i++)
        {
            _Dictionary[_Sprites[i].name] = _Sprites[i];
        }
    }
}
