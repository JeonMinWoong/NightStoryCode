using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public string sceneName;

    [SerializeField]
    List<GameObject> mangerGroup = new List<GameObject>();

    [SerializeField]
    Texture2D cursorTexture;
    protected override void Awake()
    {
        base.Awake();
        
        sceneName = SceneManager.GetActiveScene().name;

        Application.targetFrameRate = 60;

        ManagerInit("AudioManager");
        ManagerInit("UIManager");
        ManagerInit("ObjectManager");
        ManagerInit("SettingManager");

        //Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.Equals("Main") == false && sceneName.Equals("MapSelect") == false)
            {
                ManagerCreate();
                ManagerReset();
            }
            else
            {
                Init();
                ManagerSet();
            }
        }
    }

    public void Init()
    {
        if (mangerGroup.Count <= 0)
        {
            ManagerInit("AudioManager");
            ManagerInit("UIManager");
            ManagerInit("ObjectManager");
            ManagerInit("SettingManager");
        }
    }

    void ManagerInit(string Manager)
    {
        GameObject go = GameObject.Find(Manager);

        if (go != null)
        {
            Debug.Log(go.name);
            return;
        }
        if (go == null)
        {
            go = Instantiate(Resources.Load("Prefabs/Manager/" + Manager)) as GameObject;
            mangerGroup.Add(go);
            //Debug.Log(go.name);
            go.name = Manager;
            go.transform.parent = gameObject.transform;
        }
    }

    void ManagerCreate()
    {
        ManagerInit("ScoreManager");
        ManagerInit("GameController");
        ManagerInit("CinemaManager");
    }

    void ManagerSet()
    {
        if (mangerGroup.Count > 4)
        {
            mangerGroup.Remove(GameController.Instance.gameObject);
            mangerGroup.Remove(ScoreManager.Instance.gameObject);
            mangerGroup.Remove(CinemaManager.Instance.gameObject);
            Destroy(GameController.Instance.gameObject);
            Destroy(ScoreManager.Instance.gameObject);
            Destroy(CinemaManager.Instance.gameObject);
        }
    }

    void ManagerReset()
    {
        for (int i = 0; i < mangerGroup.Count; i++)
        {
            mangerGroup[i].SetActive(false);
            mangerGroup[i].SetActive(true);
        }
    }

    public void ManagerDestroy()
    {
        for (int i = 0; i < mangerGroup.Count; i++)
        {
            Destroy(mangerGroup[i]);
        }

        mangerGroup.Clear();

        DBManager.Instance.ReDB();

        SceneManager.LoadScene("Main");
        
    }

    public void ManagerDestroyLoad()
    {
        StartCoroutine(ReManager());
    }

    public IEnumerator ReManager()
    {
        yield return null;

        ManagerInit("AudioManager");
        ManagerInit("UIManager");
        ManagerInit("ObjectManager");
        ManagerInit("SettingManager");
        PlayGame();
    }

    public void Load()
    {
        StartCoroutine(CoLoad());
    }

    public IEnumerator CoLoad()
    {
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.1f);
        ManagerDestroy();
        ManagerDestroyLoad();
    }

    public void PlayGame()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene("Map_1");
#else
        LoadingManager.Instance.LoadScene("Map_1");
#endif
    }
}
