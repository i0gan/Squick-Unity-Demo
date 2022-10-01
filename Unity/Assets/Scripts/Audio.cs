using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public class Audio : MonoBehaviour
    {
        private static Audio m_Instance = null;
        public static Audio GetInstance
        {
            get { return m_Instance; }
        }

        [System.Serializable]
        public struct AudioStruct
        {
            public string audioName;
            public AudioClip audioClip;
        }
        public AudioStruct[] asArray; // 音效
        public AudioStruct[] abArray; // 背景音乐
        public AudioStruct[] as2Array; // 音效2

        //背景音乐播放器
        private AudioSource audioSource_BGM;
        //音效播放器
        private AudioSource audioSource_Sound;
        private AudioSource audioSource2_Sound;
        public void Awake()
        {
            if (m_Instance != null) return; // 如果已经创建对象，则将直接返回。

            Audio[] audioManagers = GameObject.FindObjectsOfType<Audio>();
            if (audioManagers.Length == 1)
                DontDestroyOnLoad(gameObject);

            m_Instance = this;

            audioSource_Sound = gameObject.AddComponent<AudioSource>();
            audioSource_Sound.loop = false;
            audioSource_Sound.volume = 1.8f;

            audioSource2_Sound = gameObject.AddComponent<AudioSource>();
            audioSource2_Sound.loop = false;
            audioSource2_Sound.volume = 1.0f;

            audioSource_BGM = gameObject.AddComponent<AudioSource>();
            audioSource_BGM.loop = true;
            audioSource_BGM.volume = 0.4f;
        }

        private void OnDestroy()
        {
            //Debug.Log("AudioManager OnDestroy");
            //m_Instance = null;
            if(this == m_Instance) //如果销毁的是当前对象，即清空当前静态实例。
            {
                Debug.Log("AudioManager OnDestroy");
                m_Instance = null;
            }
        }

        public void AB_PlayAudio(string audioName)
        {
            for (int i = 0; i < abArray.Length; i ++)
            {
                if (audioName == abArray[i].audioName)
                {
                    audioSource_BGM.clip = abArray[i].audioClip;
                    audioSource_BGM.Play();
                    break;
                }
            }
        }

        public void AS_PlayAudio(string audioName)
        {
            for (int i = 0; i < asArray.Length; i++)
            {

                if (audioName == asArray[i].audioName)
                {
                    audioSource_Sound.clip = asArray[i].audioClip;
                    audioSource_Sound.Play();
                    break;
                }
            }
        }

        public void AS2_PlayAudio(string audioName)
        {
            for (int i = 0; i < as2Array.Length; i++)
            {

                if (audioName == as2Array[i].audioName)
                {
                    audioSource2_Sound.clip = as2Array[i].audioClip;
                    audioSource2_Sound.Play();
                    break;
                }
            }
        }

        public void AudioVolume(float value, bool isBGM)
        {
            if (isBGM)
            {
                audioSource_BGM.volume = value;
            }
            else
            {
                audioSource_Sound.volume = value;
            }
        }

        public void StopSoundAudio()
        {
            audioSource_Sound.Stop();
            audioSource2_Sound.Stop();
        }

        public void StopBGMAudio()
        {
            audioSource_BGM.Stop();

        }
    }
}