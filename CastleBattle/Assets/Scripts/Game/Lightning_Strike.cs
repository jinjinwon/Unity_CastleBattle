using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning_Strike : MonoBehaviour
{
    public float m_Damage = 0.0f;

    void Start()
    {
        Destroy(gameObject, 1.0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        AudioMgr.Inst.PlayEffSound("Atk Magic", 0.5f);

        if (other.tag == "E_Base")
            other.GetComponent<BaseCtrl>().TakeDamage(m_Damage);
        else if(other.tag == "E_Unit")
            other.GetComponent<E_CharCtrl>().TakeDamage((int)m_Damage);
    }
}
