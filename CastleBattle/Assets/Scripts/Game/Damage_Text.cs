using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage_Text : MonoBehaviour
{
    float totalEnd = 0.0f;

    public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[]
                                                             { 
                                                             // new Keyframe(시간,크기)
                                                             new Keyframe(0.0f, 0.01f), new Keyframe(0.0f, 0.01f)
                                                             });

    public AnimationCurve moveCurve = new AnimationCurve(new Keyframe[]
                                                       { 
                                                             // new Keyframe(시간,크기)
                                                             new Keyframe(0.19f, 0.0f), new Keyframe(0.65f, 0.5f)
                                                       });

    public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[]
                                                     { 
                                                             // new Keyframe(시간,크기)
                                                             new Keyframe(0.4f, 1.0f), new Keyframe(1.0f,0.0f)
                                                     });

    // 연출 계산용 변수
    float m_StartTime = 0.0f;
    float m_CurTime = 0.0f;
    float a_CacScale = 0.0f;
    Vector3 a_CacScVec = Vector3.zero;

    // 이동 계산용 변수
    Vector3 a_CacCurPos = Vector3.zero;
    float a_MvOffSet = 0.0f;
    Vector3 m_BaseWdPos = Vector3.zero;

    // 투명도 계산용 변수
    Text m_RefText = null;
    Color _color = new Color(200, 0, 0, 255);
    Image m_ImgColor = null;
    [HideInInspector] public float m_DamageVal = 0.0f;
    float a_alpha = 0.0f;

    void Start()
    {
        if (gameObject.tag == "Gold")
        {
            m_ImgColor = GetComponent<Image>();
            m_RefText = this.gameObject.GetComponentInChildren<Text>();

            scaleCurve = new AnimationCurve(new Keyframe[]
                                                            { 
                                                             // new Keyframe(시간,크기)
                                                             new Keyframe(0.0f, 0.01f), new Keyframe(0.0f, 0.01f)
                                                             }
                                                            );
        }
        else
            m_RefText = this.gameObject.GetComponent<Text>();

        // 색 설정
        if (m_RefText != null)
        {
            _color = m_RefText.color;

            if (gameObject.tag == "Gold")
                m_RefText.text = "+" + m_DamageVal.ToString();
            else
                m_RefText.text = "-" + m_DamageVal.ToString();
        }

        m_StartTime = Time.realtimeSinceStartup;
        m_BaseWdPos = this.transform.position;

        // 종료시간 계산 코드

        Keyframe[] m_Scales;
        m_Scales = scaleCurve.keys;
        float scalesEnd = m_Scales[m_Scales.Length - 1].time;

        Keyframe[] m_MoveKey;
        m_MoveKey = moveCurve.keys;
        float moveEnd = m_MoveKey[m_MoveKey.Length - 1].time;

        Keyframe[] m_Alphas;
        m_Alphas = alphaCurve.keys;
        float alphaEnd = m_Alphas[m_Alphas.Length - 1].time;

        totalEnd = Mathf.Max(scalesEnd, Mathf.Max(moveEnd, alphaEnd));

        a_CacScVec.x = 0.001f;
        a_CacScVec.y = 0.001f;
        transform.localScale = a_CacScVec;

        Destroy(this.gameObject, totalEnd + 0.2f);  // 연출이 끝나는 시간에 오브젝트 파괴
    }


    void Update()
    {
        m_CurTime = Time.realtimeSinceStartup;

        // ---- 펀칭 효과 연출
        a_CacScale = scaleCurve.Evaluate(m_CurTime - m_StartTime); // 스케일은 무조건 적용된다.
        a_CacScVec.x = a_CacScale;
        a_CacScVec.y = a_CacScale;
        transform.localScale = a_CacScVec;

        // ---- 이동 효과 연출
        a_MvOffSet = moveCurve.Evaluate(m_CurTime - m_StartTime);  // 얼마나 진행 되었는가
        a_CacCurPos = m_BaseWdPos;
        a_CacCurPos.y = m_BaseWdPos.y + a_MvOffSet;
        this.transform.position = a_CacCurPos;

        // ---- 투명 효과 연출
        a_alpha = alphaCurve.Evaluate(m_CurTime - m_StartTime);

        if (gameObject.tag == "Gold")
        {
            _color = m_ImgColor.color;
            _color.a = a_alpha;
            m_ImgColor.color = _color;
            m_RefText.color = _color;
        }
        else
        {
            _color = m_RefText.color;
            _color.a = a_alpha;
            m_RefText.color = _color;
        }
    }
}
