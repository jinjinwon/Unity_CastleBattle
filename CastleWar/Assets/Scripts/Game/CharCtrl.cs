using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharAnimType
{
    MOVE = 0,
    STOP,
    ATTACK,
    HIT,
    DIE,
    COUNT
}

public class CharCtrl : MonoBehaviour
{
    CharType m_CrType = CharType.Char_SW;
    float m_Attack = 0.0f;
    float m_AtkDistance = 0.0f;
    float m_AtkSpeed = 0.0f;
    public float m_Hp = 0.0f;
    float m_MvPower = 0.0f;
    float m_SkillCoolTime = 0.0f;
    float m_MaxHp = 0.0f;

    float m_CurMove = 0.0f;
    float m_CurAttack = 0.0f;
    float m_CurAttSpeed = 0.0f;


    public Image m_HpBar = null;
    public Text m_SkTick_Txt = null;

    // 화살 프리팹 변수
    GameObject m_Arrow = null;
    Arrow m_ArrowCtrl = null;

    // 낙뢰 프리팹 변수
    GameObject m_Lightning = null;
    Lightning_Strike m_Lightning_ST = null;

    // 애니메이터 변수
    Animator m_Anim = null;

    // 파티클 변수
    ParticleSystem m_Dust = null;

    // 타겟 변수
    [HideInInspector] public GameObject m_Target = null;

    E_CharCtrl m_ECharCtrl;
    BaseCtrl m_BaseCtrl;
    bool m_IsCheck = false;

    // 스킬 변수
    float m_Delta = 0.0f;
    float m_Timer = 1.0f;
    bool m_FindEnemy = false;

    CharAnimType m_CharAnimType = CharAnimType.MOVE;

    void Start()
    {
        // 초기화
        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();
        if (m_Dust == null)
            m_Dust = GetComponentInChildren<ParticleSystem>();

        if (m_CrType == CharType.Char_AR || m_CrType == CharType.Char_ARU)
        {
            m_Arrow = (GameObject)Resources.Load("P_Arrow");
        }

        if (m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
        {
            m_Lightning = (GameObject)Resources.Load("Lightning_strike");
        }

        PassiveSkill();

        m_CurMove = m_MvPower;
        m_CurAttack = m_Attack;
        m_CurAttSpeed = m_AtkSpeed;
        m_Delta = m_SkillCoolTime;
        m_MaxHp = m_Hp;
        m_SkTick_Txt.gameObject.SetActive(false);

        m_Dust.Play();

        AnimState(CharAnimType.MOVE);
    }

    void Update()
    {
        if (GameMgr.Inst.m_GameOver == true)
        {
            StopAllCoroutines();
            return;
        }

        if (GameMgr.Inst.m_DlgActive == true)
        {
            StopAllCoroutines();
            return;
        }

        if (m_CharAnimType == CharAnimType.DIE)
            return;

        if(m_Target == null)
            TargetDistance();

        transform.position += Vector3.right * m_MvPower * Time.deltaTime;

        if(m_Delta <= m_SkillCoolTime + 1.0f)
            m_Delta += Time.deltaTime;

        // 스킬 사용
        if(m_SkillCoolTime < m_Delta && m_FindEnemy == true && m_Target != null)
        {
            StopCoroutine(UseSkill(m_CrType));

            if (m_CrType == CharType.Char_GU || m_CrType == CharType.Char_GUU)
            {
                if (m_Hp <= m_MaxHp * 0.6f)
                {
                    m_Delta = 0.0f;
                    StartCoroutine(UseSkill(m_CrType));
                }
                else
                    return;
            }
            else
            {
                m_Delta = 0.0f;
                StartCoroutine(UseSkill(m_CrType));
            }
        }

        if(m_SkTick_Txt.gameObject.activeSelf == true)
        {
            m_Timer -= Time.deltaTime;

            if(m_Timer <= 0.0f)
            {
                m_SkTick_Txt.gameObject.SetActive(false);
                m_Timer = 1.0f;
            }
        }
    }

    void PassiveSkill()
    {
        if (GameMgr.Inst.m_AtkGemLevel <= 0)
            return;

        m_Attack += (m_Attack * (0.05f * GameMgr.Inst.m_AtkGemLevel));

        if (GameMgr.Inst.m_HpGemLevel <= 0)
            return;

        m_Hp += (m_Hp * (0.05f * GameMgr.Inst.m_HpGemLevel));
    }

    void AnimState(CharAnimType a_CrAnimType)
    {
        if (a_CrAnimType < CharAnimType.MOVE && CharAnimType.COUNT <= a_CrAnimType)
            return;

        if (m_CharAnimType == CharAnimType.DIE)
            return;

        if (m_Anim == null)
            m_Anim = GetComponent<Animator>();
            
        if (a_CrAnimType == CharAnimType.MOVE)
            m_Anim.SetTrigger("doMove");
        else if (a_CrAnimType == CharAnimType.STOP)
            m_Anim.SetTrigger("doStop");
        else if (a_CrAnimType == CharAnimType.ATTACK)
            m_Anim.SetTrigger("doAttack");
        else if (a_CrAnimType == CharAnimType.HIT)
            m_Anim.SetTrigger("doHit");
        else if (a_CrAnimType == CharAnimType.DIE)
            m_Anim.SetTrigger("doDie");

        m_CharAnimType = a_CrAnimType;
    }

    // 스킬 사용
    IEnumerator UseSkill(CharType a_CrType)
    {
        // 이상한 캐릭터 타입이라면
        if (a_CrType < CharType.Char_SW || CharType.CrCount <= a_CrType)
            yield break;

        if (m_Target == null)
            yield break;

        m_SkTick_Txt.gameObject.SetActive(true);

        // 공격력 증가
        if (a_CrType == CharType.Char_SW || a_CrType == CharType.Char_SWU)
        {
            if (a_CrType == CharType.Char_SW)
            {
                m_SkTick_Txt.text = "스킬 사용!\n 공격력 30% 증가!";
                m_Attack += m_Attack * 0.3f;
            }
            else
            {
                m_SkTick_Txt.text = "스킬 사용!\n 공격력 60% 증가!";
                m_Attack += m_Attack * 0.6f;
            }

            yield return new WaitForSeconds(30.0f);

            m_Attack = m_CurAttack;

            yield break;
        }
        // 공격속도 증가
        else if (a_CrType == CharType.Char_AR || a_CrType == CharType.Char_ARU)
        {
            if (a_CrType == CharType.Char_AR)
            {
                m_SkTick_Txt.text = "스킬 사용!\n 공격속도 30% 증가!";
                m_AtkSpeed -= m_AtkSpeed * 0.3f;
            }
            else
            {
                m_SkTick_Txt.text = "스킬 사용!\n 공격속도 60% 증가!";
                m_AtkSpeed -= m_AtkSpeed * 0.6f;
            }

            yield return new WaitForSeconds(30.0f);

            m_AtkSpeed = m_CurAttSpeed;

            yield break;
        }
        // 체력 회복
        else if(a_CrType == CharType.Char_GU || a_CrType == CharType.Char_GUU)
        {
            if (a_CrType == CharType.Char_GU)
            {
                m_SkTick_Txt.text = "스킬 사용!\n 체력 50% 회복!";
                m_Hp += m_MaxHp / 2.0f;

                if (m_MaxHp <= m_Hp)
                    m_Hp = m_MaxHp;
            }
            else
            {
                m_SkTick_Txt.text = "스킬 사용!\n 체력 100% 회복!";
                m_Hp = m_MaxHp;
            }

            yield break;
        }
        // 스킬 사용
        else if (a_CrType == CharType.Char_WI || a_CrType == CharType.Char_WIU)
        {
            AnimState(CharAnimType.ATTACK);

            m_SkTick_Txt.text = "스킬 사용!";
            GameObject a_Lightning = Instantiate(m_Lightning, new Vector3(transform.position.x + 1.5f, 2.1f, transform.position.z), Quaternion.identity);
            m_Lightning_ST = a_Lightning.GetComponent<Lightning_Strike>();
            m_Lightning_ST.m_Damage = m_Attack * 10f;
        }
    }

    // 공격판정
    IEnumerator Attack()
    {
        m_IsCheck = true;
        m_MvPower = 0.0f;
        m_Dust.Stop();
        m_FindEnemy = true;

        if (m_Target.tag == "E_Unit")
        {
            m_ECharCtrl = m_Target.GetComponent<E_CharCtrl>();
        }
        else if (m_Target.tag == "E_Base")
        {
            m_BaseCtrl = m_Target.GetComponent<BaseCtrl>();
            StartCoroutine(E_BaseAttack());
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (m_ECharCtrl.m_Hp <= 0.0f)
                break;

            if (m_Hp < 0.0f)
                break;

            // 공격 처리
            AnimState(CharAnimType.ATTACK);
            AudioMgr.Inst.PlayEffSound("Atk Sword", 0.5f);
            Char_Attacks();

            if (m_ECharCtrl.m_Hp <= 0.0f)
                break;

            if (m_Hp < 0.0f)
                break;

            yield return new WaitForSeconds(m_AtkSpeed / 2);

            AnimState(CharAnimType.STOP);

            if (m_ECharCtrl.m_Hp <= 0.0f)
                break;

            if (m_Hp < 0.0f)
                break;

            yield return new WaitForSeconds(m_AtkSpeed);
        }

        m_Target = null;
        m_MvPower = m_CurMove;
        m_Dust.Play();
        m_FindEnemy = false;

        if (m_Hp < 0.0f)
            AnimState(CharAnimType.DIE);
        else
            AnimState(CharAnimType.MOVE);
    }

    // 공격 사거리 판정
    void TargetDistance()
    {
        // Ray 시각적 표현
        Debug.DrawRay(new Vector2(transform.position.x - 0.1f, transform.position.y + 0.5f), Vector2.right * m_AtkDistance, Color.red);

        // Ray의 맞은 게임 오브젝트의 정보를 받아올 변수
        RaycastHit2D a_Hit = Physics2D.Raycast(new Vector2(transform.position.x - 0.1f, transform.position.y + 0.5f), Vector2.right, m_AtkDistance, LayerMask.GetMask("RED"));

        // 충돌체의 정보가 있고 m_Target에 내용이 없다면
        if (a_Hit.collider != null && m_Target == null)
        {
            Vector2 a_CacVec = a_Hit.collider.transform.position - transform.position;
            float a_Length = a_CacVec.magnitude;

            if (a_Length < m_AtkDistance)
            {
                m_Target = a_Hit.collider.gameObject;
                StartCoroutine(Attack());
            }
        }
        // 충돌체의 정보가 있고 m_Target에 내용이 있고 m_IsCheck가 false라면
        else if (a_Hit.collider != null && m_Target != null && m_IsCheck == false)
        {
            if (a_Hit.collider.gameObject.tag != "E_Unit")
                return;

            m_IsCheck = true;
            m_Target = a_Hit.collider.gameObject;
            StartCoroutine(Attack());
        }
    }

    // 적 베이스 공격
    IEnumerator E_BaseAttack()
    {
        m_IsCheck = false;
        m_MvPower = 0.0f;

        while (true)
        {
            TargetDistance();
            yield return new WaitForSeconds(0.1f);

            if (m_IsCheck == true)
                break;

            if (m_BaseCtrl.m_CurHp <= 0.0f)
                break;

            if (m_Hp < 0.0f)
                break;

            // 공격 처리
            AnimState(CharAnimType.ATTACK);
            Char_Attacks(true);

            if (m_BaseCtrl.m_CurHp <= 0.0f)
                break;

            if (m_Hp < 0.0f)
                break;

            yield return new WaitForSeconds(m_AtkSpeed / 2);

            AnimState(CharAnimType.STOP);

            if (m_BaseCtrl.m_CurHp <= 0.0f)
                break;

            if (m_Hp < 0.0f)
                break;

            yield return new WaitForSeconds(m_AtkSpeed);
        }
        m_Target = null;
    }

    // 공격 종류
    void Char_Attacks(bool a_IsCheck = false)
    {
        // 몬스터 공격일 때
        if(a_IsCheck == false)
        {
            if (m_CrType == CharType.Char_AR || m_CrType == CharType.Char_ARU)
            {
                GameObject a_Arrow = Instantiate(m_Arrow,new Vector3(transform.position.x + 0.5f, transform.position.y +0.2f, transform.position.z), Quaternion.identity);
                m_ArrowCtrl = a_Arrow.GetComponent<Arrow>();
                m_ArrowCtrl.m_AtkDamage = m_Attack;
            }
            else if(m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
            {
                Instantiate(GameMgr.Inst.m_BWZ_Particle, m_Target.transform.position, Quaternion.identity);
                m_ECharCtrl.TakeDamage((int)m_Attack);
            }
            else
                m_ECharCtrl.TakeDamage((int)m_Attack);
        }
        // 기지 공격일 때
        else
        {
            if (m_CrType == CharType.Char_AR || m_CrType == CharType.Char_ARU)
            {
                GameObject a_Arrow = Instantiate(m_Arrow, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.2f, transform.position.z), Quaternion.identity);
                m_ArrowCtrl = a_Arrow.GetComponent<Arrow>();
                m_ArrowCtrl.m_AtkDamage = m_Attack;
            }
            else if (m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
            {
                Instantiate(GameMgr.Inst.m_BWZ_Particle, m_Target.transform.position, Quaternion.identity);
                m_BaseCtrl.TakeDamage((int)m_Attack);
            }
            else
                m_BaseCtrl.TakeDamage((int)m_Attack);
        }        
    }

    // 죽음 판정
    void Die()
    {
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        m_MvPower = 0.0f;

        if(m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
        {
           GetComponent<SpriteRenderer>().material = GameMgr.Inst.m_ChangeMtrl;
        }

        if(m_Dust != null)
            m_Dust.Stop();

        StopAllCoroutines();
        AnimState(CharAnimType.DIE);

        Destroy(this.gameObject, 3.0f);
    }

    // 데미지 받기
    public void TakeDamage(float a_Value)
    {
        if (m_Hp < 0.0f)
            return;

        GameMgr.DamageTxt((int)a_Value, this.transform);
        AnimState(CharAnimType.HIT);

        m_Hp -= a_Value;
        m_HpBar.fillAmount = m_Hp / m_MaxHp;

        if (m_Hp < 0.0f)
            Die();
    }

    // 능력치 할당
    public void CharInit(CharInfo a_CrInfo)
    {
        m_CrType = a_CrInfo.m_CrType;
        m_Attack = a_CrInfo.m_Attack;
        m_AtkDistance = a_CrInfo.m_AtkDistance;
        m_AtkSpeed = a_CrInfo.m_AtkSpeed;
        m_Hp = a_CrInfo.m_Hp;
        m_MvPower = a_CrInfo.m_MvPower;
        m_SkillCoolTime = a_CrInfo.m_SkillCoolTime;
    }
}
