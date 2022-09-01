using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingManager : MonoBehaviour
{
    protected static LoadingManager instance;
    public static LoadingManager Instance
    {
        get
        {
            if (instance == null)
            { var obj = FindObjectOfType<LoadingManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = Create();
                }
            } return instance;
        }
        private set 
        {
            instance = value; 
        } 
    }
    public static LoadingManager Create() 
    { 
        var SceneLoaderPrefab = Resources.Load<LoadingManager>("Prefabs/Manager/LoadingManager");
        return Instantiate(SceneLoaderPrefab); 
    }

    private void Awake() 
    { 
        if (Instance != this) 
        { 
            Destroy(gameObject); return; 
        }
        DontDestroyOnLoad(gameObject); 
    }

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Image progressBar;

    private string loadSceneName;

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        progressBar.fillAmount = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while(op.isDone == false)
        {
            yield return null;
            if(progressBar.fillAmount < 0.9f)
            {
                timer += Time.unscaledDeltaTime * UnityEngine.Random.Range(0.01f, 0.05f);
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, progressBar.fillAmount +UnityEngine.Random.Range(0.01f,0.1f), timer);
            }
            else if(progressBar.fillAmount >= 0.9f && op.progress >= 0.9f)
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 0.95f, timer);
                if(progressBar.fillAmount == 0.95f)
                {
                    // 95에서 대기
                    //yield return new WaitForSeconds(1);
                    progressBar.fillAmount = 1;
                    // 완성일 때 대기
                    //yield return new WaitForSeconds(1);
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
            canvasGroup.blocksRaycasts = isFadeIn ? true : false;
        }

        if (isFadeIn == false)
        {
            gameObject.SetActive(false);
        }
    }
}