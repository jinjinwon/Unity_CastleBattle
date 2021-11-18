using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogCtrl : MonoBehaviour
{
    public delegate void DLT_Response();             
    DLT_Response DltMethod;                          

    public Button m_OK_Btn = null;
    public Button m_Cancel_Btn = null;
    public Text m_Contents_Txt = null;

    void Start()
    {
        if (m_OK_Btn != null)
            m_OK_Btn.onClick.AddListener(() =>
            {
                if (DltMethod != null)
                    DltMethod();

                AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
                Destroy(gameObject);
            });

        if (m_Cancel_Btn != null)
            m_Cancel_Btn.onClick.AddListener(() =>
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "InGameScene")
                    GameMgr.Inst.m_DlgActive = false;

                AudioMgr.Inst.PlayEffSound("Buy", 0.5f);
                Destroy(gameObject);
            });
    }

    public void SetMessage(string a_Mess, DLT_Response a_DltMtd = null)
    {
        m_Contents_Txt.text = a_Mess;
        DltMethod = a_DltMtd;
    }
}
