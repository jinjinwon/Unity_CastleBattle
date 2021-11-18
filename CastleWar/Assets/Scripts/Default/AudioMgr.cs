using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    public static AudioMgr Inst;

    Dictionary<string, AudioClip> m_ADClipList = new Dictionary<string, AudioClip>();
    AudioSource m_AudioSrc = null;

    // 효과음 최적화를 위한 변수 (Buffer 돌려쓰기)
    int m_EffSdCount = 4;                                              
    int m_iSndCount = 0;
    GameObject[] m_sndObjList = new GameObject[10];                    
    AudioSource[] m_sndSrcList = new AudioSource[10];                  
    float[] m_EffVolume = new float[10];

    void Awake()
    {
        var a_Go = FindObjectsOfType<AudioMgr>();

        if (a_Go.Length == 1)
            DontDestroyOnLoad(this.gameObject);
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        LoadChildGameObj();                                             

        Inst = this;                                                    

        m_AudioSrc = GetComponent<AudioSource>();                       

        // 사운드 리소스를 전부 불러오는 구문
        AudioClip a_GAudioClip = null;                                  

        object[] temp = Resources.LoadAll("Sounds");                    

        for (int a_ii = 0; a_ii < temp.Length; a_ii++)
        {
            a_GAudioClip = temp[a_ii] as AudioClip;                     
            m_ADClipList.Add(a_GAudioClip.name, a_GAudioClip);          
        }
    }

    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;

        if (m_ADClipList.ContainsKey(a_FileName) == true)                 
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else                                                              
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);                  
        }

        if (m_AudioSrc == null)
            return;

        m_AudioSrc.clip = a_GAudioClip;
        m_AudioSrc.volume = fVolume;
        m_AudioSrc.loop = true;
        m_AudioSrc.Play();
    }

    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;

        if (m_ADClipList.ContainsKey(a_FileName) == true)                      
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else                                                                   
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);                        
        }

        if (a_GAudioClip != null && m_sndSrcList[m_iSndCount] != null)
        {
            m_sndSrcList[m_iSndCount].clip = a_GAudioClip;
            m_sndSrcList[m_iSndCount].volume = fVolume;
            m_sndSrcList[m_iSndCount].loop = false;
            m_sndSrcList[m_iSndCount].Play();

            m_iSndCount++;

            if (m_EffSdCount <= m_iSndCount)
                m_iSndCount = 0;
        }
    }

    public void LoadChildGameObj()
    {
        for (int a_ii = 0; a_ii < m_EffSdCount; a_ii++)
        {
            GameObject newSoundObj = new GameObject();
            newSoundObj.transform.SetParent(this.transform);
            newSoundObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundObj.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            newSoundObj.name = "SoundEffObj";

            m_sndSrcList[a_ii] = a_AudioSrc;
            m_sndObjList[a_ii] = newSoundObj;
        }
    }

    public void PlayGUISound(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;

        if (m_ADClipList.ContainsKey(a_FileName) == true)                      
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else                                                                   
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);                        
        }

        if (m_AudioSrc == null)
            return;

        m_AudioSrc.PlayOneShot(a_GAudioClip, fVolume);
    }
}
