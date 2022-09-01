using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NextStage : MonoBehaviour
{
    public int currentCount;
    public int count;
    public bool isNoStage;

    public int mapInpoCheck;
    public int questCheckCount;

    [SerializeField]
    TextMeshProUGUI stageText;

    private void Start()
    {
        stageText = GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(CoStart());
    }

    IEnumerator CoStart()
    {
        if (isNoStage == true)
        {
            yield return new WaitForSeconds(0.1f);
            //Enums.MapStory mapStory = (Enums.MapStory)GameController.Instance.lvCount;
            //switch (mapStory)
            //{
            //    case Enums.MapStory.None:
            //        break;
            //    case Enums.MapStory.Map_1:
            //        stageText.text = "유적 수호자의 길목";
            //        break;
            //    case Enums.MapStory.Map_2:
            //        break;
            //    case Enums.MapStory.Map_3:
            //        break;
            //    case Enums.MapStory.Map_4:
            //        break;
            //    case Enums.MapStory.Map_5:
            //        break;
            //    case Enums.MapStory.Map_6:
            //        break;
            //    case Enums.MapStory.Map_7:
            //        break;
            //    default:
            //        break;
            //}
        }
        else
        {
            yield return new WaitForSeconds(0.1f);

            stageText.text = ObjectManager.Instance.sceneNick + "\n" + GameController.Instance.lvCount + "-" + count;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            currentCount = GameController.Instance.StageCount;
            PlayerController pA = collision.GetComponent<PlayerController>();

            if (pA.coIntro == null)
            {
                if (GameController.Instance.stageClear[currentCount - 1].isStageClear == true)
                {
                    if (PlayerProgressCheck(collision.GetComponent<PlayerStat>()))
                    {
                        GameController.Instance.StartCoroutine(GameController.Instance.Next_Stage(count, stageText.text));
                        if (name.Contains("Training"))
                        {
                            GameController.Instance.TutorialEnd();
                        }
                    }
                    else
                    {
                        GameController.Instance.StartCoroutine(GameController.Instance.PlayerReDoor(true));
                    }
                }
                else
                {
                    GameController.Instance.StartCoroutine(GameController.Instance.PlayerReDoor(true));
                }
            }
        }

    }

    bool PlayerProgressCheck(PlayerStat playerStat)
    {
        int mapInpo = playerStat.playerAbility.mapInpo;
        bool isQuestCheck = false;

        if (playerStat.playerAbility.playerQuests.Count > 0 && questCheckCount != 0)
        {
            for (int i = 0; i < playerStat.playerAbility.playerQuests.Count; i++)
            {
                if (playerStat.playerAbility.playerQuests[i].questNubmer == questCheckCount)
                {
                    isQuestCheck = playerStat.playerAbility.playerQuests[i].isEnd;
                    break;
                }
            }
        }
        else
        {
            isQuestCheck = true;
        }
            


        if (mapInpo < mapInpoCheck || !isQuestCheck)
        {
            Debug.Log("진행할 수 없습니다.");
            return false;
        }
        else
            return true;
    }

}
