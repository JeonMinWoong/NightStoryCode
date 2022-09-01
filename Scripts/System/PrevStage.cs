using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrevStage : MonoBehaviour
{
    public int currentCount;
    public int count;

    [SerializeField]
    TextMeshProUGUI stageText;

    private void Start()
    {
        stageText = GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(CoStart());
    }

    IEnumerator CoStart()
    {
        yield return new WaitForSeconds(0.1f);
        stageText.text = ObjectManager.Instance.sceneNick + "\n" + GameController.Instance.lvCount + "-" + count;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            currentCount = GameController.Instance.StageCount;
            PlayerController pA = collision.GetComponent<PlayerController>();

            if (pA.coIntro == null)
            {
                if (GameController.Instance.stageClear[currentCount-1].isStageClear == true)
                {
                    GameController.Instance.StartCoroutine(GameController.Instance.Prev_Stage(count, stageText.text));
                }
                else
                {
                    GameController.Instance.StartCoroutine(GameController.Instance.PlayerReDoor(false));
                }

            }
        }

    }

}
