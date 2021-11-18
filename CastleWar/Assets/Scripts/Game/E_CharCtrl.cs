using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class E_CharCtrl : MonoBehaviour
{
    public CharType m_CrType = CharType.Char_SW;
    float m_Attack = 100.0f;
    float m_AtkDistance = 0.5f;
    float m_AtkSpeed = 3.0f;
    public float m_Hp = 100.0f;
    float m_MvPower = 0.5f;
    float m_MaxHp = 0.0f;

    public int m_Level = 1;
    float m_CurMove = 0.0f;
    float m_UpAttack = 0.0f;
    float m_UpAtkDistance = 0.0f;
    float m_UpAtkSpeed = 0.0f;
    float m_UpHp = 0.0f;
    float m_UpMvPower = 0.0f;

    public Image m_HpBar = null;

    // 애니메이터 변수
    Animator m_Anim = null;

    // 파티클 변수
    ParticleSystem m_Dust = null;

    // 타겟 변수
    GameObject m_Target = null;
    bool m_IsCheck = false;
    BaseCtrl m_BaseCtrl;
    CharCtrl m_CharCtrl;

    // 화살 프리팹 변수
    GameObject m_Arrow = null;
    Arrow m_ArrowCtrl = null;

    CharAnimType m_CharAnimType = CharAnimType.MOVE;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        m_Dust = GetComponentInChildren<ParticleSystem>();
        m_CurMove = m_MvPower;
        m_MaxHp = m_Hp;

        if (m_CrType == CharType.Char_AR || m_CrType == CharType.Char_ARU)
        {
            m_Arrow = (GameObject)Resources.Load("E_Arrow");
        }

        m_Dust.Stop();

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

        if (m_Target == null)
            TargetDistance();

        transform.position += Vector3.left * m_MvPower * Time.deltaTime;
    }

    public void LevelSetting(int a_Level)
    {
        float m_UpAttack = m_Attack;
        float m_UpAtkSpeed = m_AtkSpeed;
        float m_UpHp = m_Hp;
        float m_UpMvPower = m_MvPower;

        for (int i = 0; i < a_Level; i++)
        {
            if (m_CrType == CharType.Char_AR || m_CrType == CharType.Char_ARU || m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
                m_AtkDistance = 3.0f;

            m_Attack += m_UpAttack * 0.5f;
            m_AtkSpeed += m_UpAtkSpeed * 0.5f;
            m_Hp += m_UpHp * 0.5f;
            m_MvPower += m_UpMvPower * 0.5f;
        }
        m_MaxHp = m_Hp;
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

    // 공격 사거리 판정
    void TargetDistance()
    {
        // Ray 시각적 표현
        Debug.DrawRay(new Vector2(transform.position.x + 0.1f, transform.position.y + 0.5f), Vector2.left * m_AtkDistance, Color.blue);

        // Ray의 맞은 게임 오브젝트의 정보를 받아올 변수
        RaycastHit2D a_Hit = Physics2D.Raycast(new Vector2(transform.position.x + 0.1f, transform.position.y + 0.5f), Vector2.left, m_AtkDistance, LayerMask.GetMask("BLUE"));

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
            if (a_Hit.collider.gameObject.tag != "P_Unit")
                return;

            m_IsCheck = true;
            m_Target = a_Hit.collider.gameObject;
            StartCoroutine(Attack());
        }
    }

    // 공격판정
    IEnumerator Attack()
    {
        m_IsCheck = true;
        m_MvPower = 0.0f;
        m_Dust.Stop();

        if (m_Target.tag == "P_Unit")
        {
            m_CharCtrl = m_Target.GetComponent<CharCtrl>();
        }
        else if (m_Target.tag == "P_Base")
        {
            m_BaseCtrl = m_Target.GetComponent<BaseCtrl>();
            StartCoroutine(P_BaseAttack());
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (m_CharCtrl.m_Hp <= 0.0f)
                break;

            if (m_Hp <= 0.0f)
            {
                AnimState(CharAnimType.DIE);
                break;
            }

            // 공격 처리
            AnimState(CharAnimType.ATTACK);
            Char_Attacks();

            if (m_CharCtrl.m_Hp <= 0.0f)
                break;

            if (m_Hp <= 0.0f)
            {
                AnimState(CharAnimType.DIE);
                break;
            }

            yield return new WaitForSeconds(m_AtkSpeed / 2);

            AnimState(CharAnimType.STOP);

            if (m_CharCtrl.m_Hp <= 0.0f)
                break;

            if (m_Hp <= 0.0f)
            {
                AnimState(CharAnimType.DIE);
                break;
            }

            yield return new WaitForSeconds(m_AtkSpeed);
        }
        m_Target = null;
        m_Dust.Play();
        m_MvPower = m_CurMove;

        if (m_Hp <= 0.0f)
            AnimState(CharAnimType.DIE);
        else
            AnimState(CharAnimType.MOVE);
    }

    // 적 베이스 공격
    IEnumerator P_BaseAttack()
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

            if (m_Hp <= 0.0f)
                break;

            // 공격 처리
            AnimState(CharAnimType.ATTACK);
            Char_Attacks(true);

            if (m_BaseCtrl.m_CurHp <= 0.0f)
                break;

            if (m_Hp <= 0.0f)
                break;

            yield return new WaitForSeconds(m_AtkSpeed / 2);

            AnimState(CharAnimType.STOP);

            if (m_BaseCtrl.m_CurHp <= 0.0f)
                break;

            if (m_Hp <= 0.0f)
                break;

            yield return new WaitForSeconds(m_AtkSpeed);
        }
        m_Target = null;
    }

    // 공격 종류
    void Char_Attacks(bool a_IsCheck = false)
    {
        // 몬스터 공격일 때
        if (a_IsCheck == false)
        {
            if (m_CrType == CharType.Char_AR || m_CrType == CharType.Char_ARU)
            {
                GameObject a_Arrow = Instantiate(m_Arrow, new Vector3(transform.position.x - 0.5f, transform.position.y + 0.2f, transform.position.z), Quaternion.identity);
                m_ArrowCtrl = a_Arrow.GetComponent<Arrow>();
                m_ArrowCtrl.m_AtkDamage = m_Attack;
            }
            else if (m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
            {
                Instantiate(GameMgr.Inst.m_BWZ_Particle, m_Target.transform.position, Quaternion.identity);
                m_CharCtrl.TakeDamage(m_Attack);
            }
            else
                m_CharCtrl.TakeDamage(m_Attack);
        }
        // 기지 공격일 때
        else
        {
            if (m_CrType == CharType.Char_AR || m_CrType == CharType.Char_ARU)
            {
                GameObject a_Arrow = Instantiate(m_Arrow, new Vector3(transform.position.x - 0.5f, transform.position.y + 0.2f, transform.position.z), Quaternion.identity);
                m_ArrowCtrl = a_Arrow.GetComponent<Arrow>();
                m_ArrowCtrl.m_AtkDamage = m_Attack;
            }
            else if (m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
            {
                Instantiate(GameMgr.Inst.m_RWZ_Particle, m_Target.transform.position, Quaternion.identity);
                m_BaseCtrl.TakeDamage(m_Attack);
            }
            else
                m_BaseCtrl.TakeDamage(m_Attack);
        }
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

    void Die()
    {
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        m_MvPower = 0.0f;

        if (m_CrType == CharType.Char_WI || m_CrType == CharType.Char_WIU)
        {
            GetComponent<SpriteRenderer>().material = GameMgr.Inst.m_ChangeMtrl;
        }

        if (m_Dust != null)
        m_Dust.Stop();

        StopAllCoroutines();
        AnimState(CharAnimType.DIE);

        Destroy(this.gameObject, 3.0f);
    }
}
