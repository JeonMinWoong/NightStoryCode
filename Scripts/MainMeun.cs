using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMeun : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartMain();
        if (GameController.Instance != null)
        {
            GameController.Instance.reset = true;

            GameController.Instance.playIng = false;
        }
    }

    // Update is called once per frame
    
    void StartMain()
    {
        animator.SetTrigger("Main");
    }

    public void LoadButton()
    {
        AudioManager.Instance.PlaySound("MainMenuStart", Camera.main.transform.position);
        DataManager.Instance.OnLoadUI();
    }
}
