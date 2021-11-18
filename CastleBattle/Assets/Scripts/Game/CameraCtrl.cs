using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraCtrl : MonoBehaviour
{
    public Button m_RightMv_Btn = null;
    public Button m_LeftMv_Btn = null;

    bool m_RBtnDown = false;
    bool m_LBtnDown = false;

    float m_MinPos = 0.0f;
    float m_MaxPos = 0.0f;

    void Start()
    {
        m_MinPos = 0.0f;
        m_MaxPos = 18.0f;

        //-----------Right Button 처리 부분
        EventTrigger a_trigger = m_RightMv_Btn.GetComponent<EventTrigger>();
        EventTrigger.Entry a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerDown;
        a_entry.callback.AddListener((data) => { OnRBtnDownDelegate((PointerEventData)data); });
        a_trigger.triggers.Add(a_entry);

        a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerUp;
        a_entry.callback.AddListener((data) => { OnRBtnUpDelegate((PointerEventData)data); });
        a_trigger.triggers.Add(a_entry);

        //-----------Left Button 처리 부분
        a_trigger = m_LeftMv_Btn.GetComponent<EventTrigger>();
        a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerDown;
        a_entry.callback.AddListener((data) => { OnLBtnDownDelegate((PointerEventData)data); });
        a_trigger.triggers.Add(a_entry);
        
        a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerUp;
        a_entry.callback.AddListener((data) => { OnLBtnUpDelegate((PointerEventData)data); });
        a_trigger.triggers.Add(a_entry);
    }

    void Update()
    {
        if(m_RBtnDown == true)
        {
            float a_MvSpeed = 7.0f * Time.deltaTime;
            transform.Translate(a_MvSpeed, 0, 0);
        }

        if (m_LBtnDown == true)
        {
            float a_MvSpeed = -7.0f * Time.deltaTime;
            transform.Translate(a_MvSpeed, 0, 0);
        }

        if (transform.position.x <= m_MinPos)
            transform.position = new Vector3(m_MinPos, transform.position.y, transform.position.z);

        if (transform.position.x >= m_MaxPos)
            transform.position = new Vector3(m_MaxPos, transform.position.y, transform.position.z);
    }

    void OnRBtnDownDelegate(PointerEventData _Data)
    {
        if (_Data.button == PointerEventData.InputButton.Left)
            m_RBtnDown = true;
    }

    void OnRBtnUpDelegate(PointerEventData _Data)
    {
        if (_Data.button == PointerEventData.InputButton.Left)
            m_RBtnDown = false;
    }

    void OnLBtnDownDelegate(PointerEventData _Data)
    {
        if (_Data.button == PointerEventData.InputButton.Left)
            m_LBtnDown = true;
    }

    void OnLBtnUpDelegate(PointerEventData _Data)
    {
        if (_Data.button == PointerEventData.InputButton.Left)
            m_LBtnDown = false;
    }
}
