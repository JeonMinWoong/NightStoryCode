using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    AudioSource mainBGM;
    public AudioSource audioPrefab;     //AudioSource만 붙어있는 빈 게임오브젝트 프리팝
    public AudioClip[] clips;           //모든 오디오클립들 가지고 있음

    public AudioClip[] bgmClips;

    [SerializeField]
    List<GameObject> audioSourceGroup = new List<GameObject>();


    private void OnEnable()
    {
        mainBGM = GetComponent<AudioSource>();
        SceneName_BGM();
        if(audioSourceGroup.Count == 0)
            AuudioClipCreate();
    }

    void SceneName_BGM()
    {
        string _sceneName = GameManager.Instance.sceneName;

        if (_sceneName.Contains("Main"))
        {
            mainBGM.clip = bgmClips[0];
            mainBGM.Play();
        }
        else if(_sceneName.Contains("Map_1"))
        {
            mainBGM.clip = bgmClips[1];
            mainBGM.Play();
        }
        else if(_sceneName.Contains("Map_2"))
        {
            mainBGM.clip = bgmClips[2];
            mainBGM.Play();
        }
        else
        {
            mainBGM.clip = null;
        }
    }

    void AuudioClipCreate()
    {
        for (int i = 0; i < 10; i++)
        {
            audioSourceGroup.Add(Instantiate(audioPrefab).gameObject);
            audioSourceGroup[i].transform.parent = gameObject.transform;
            audioSourceGroup[i].gameObject.SetActive(false);
        }
    }

    //오디오클립을 재생하는 함수
    public void PlaySound(AudioClip clip, Vector2 pos, float pitch = 1f)
    {
        //AudioSource 프리팝을 생성, 값을 전해줌
        if (audioSourceGroup.Count == 0)
        {
            AudioSource audio = Instantiate(audioPrefab, pos, Quaternion.identity);

            audio.clip = clip;
            audio.pitch = pitch;
            audio.Play();
            audio.transform.parent = Camera.main.transform;

            StartCoroutine(ActiveFalse(audio.gameObject, clip.length));     //오디오 재생 이후 게임오브젝트를 삭제해라
        }
        else
        {
            for (int i = 0; i < audioSourceGroup.Count; i++)
            {
                if(audioSourceGroup[i].activeSelf == false)
                {
                    audioSourceGroup[i].transform.position = pos;
                    audioSourceGroup[i].gameObject.SetActive(true);
                    audioSourceGroup[i].transform.parent = Camera.main.transform;

                    AudioSource audio = audioSourceGroup[i].GetComponent<AudioSource>();

                    audio.clip = clip;
                    audio.pitch = pitch;
                    audio.Play();

                    StartCoroutine(ActiveFalse(audioSourceGroup[i].gameObject, clip.length));
                    audioSourceGroup.Remove(audioSourceGroup[i]);
                    break;
                }
            }
        }
    }

    IEnumerator ActiveFalse(GameObject go, float _length)
    {
        yield return new WaitForSeconds(_length);
        audioSourceGroup.Add(go.gameObject);
        go.transform.parent = gameObject.transform;
        go.SetActive(false);
    }

    //이름으로 오디오클립 찾아서 재생
    public void PlaySound(string clipName, Vector2 pos, float pitch = 1f)
    {
        foreach (AudioClip ac in clips)      //clips를 순환하며
            if (ac.name == clipName)         //clipName과 같은 clip이 있으면
                PlaySound(ac, pos, pitch);         //그 클립을 재생하라..고 넘겨줌

    }

    //이 중 랜덤한 사운드를 골라서 재생
    public void PlaySound(string[] clipNames, Vector2 pos, float pitch = 1f)
    {
        int r = Random.Range(0, clipNames.Length);

        PlaySound(clipNames[r], pos, pitch);
    }

}
