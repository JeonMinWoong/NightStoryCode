using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerCinema : MonoBehaviour
{
    PlayerStat playerStat;
    Canvas logCanvas;


    private void Start()
    {
        playerStat = GetComponent<PlayerStat>();
        logCanvas = transform.Find("LogCanvas").GetComponent<Canvas>();
    }

    private void Update()
    {
        if (transform.localScale.x < 0)
            logCanvas.transform.localScale = new Vector2(-Mathf.Abs(logCanvas.transform.localScale.x), logCanvas.transform.localScale.y);
        else
            logCanvas.transform.localScale = new Vector2(Mathf.Abs(logCanvas.transform.localScale.x), logCanvas.transform.localScale.y);
    }

    public void PlayerCinemaCheck(int _cinemaCount)
    {
        QuestList currentQuest = null;
        if (playerStat.playerAbility.mapInpo == 1)
        {
            switch (_cinemaCount)
            {
                case 0:
                    if (playerStat.playerAbility.playerQuests.Count == 0)
                    {
                        CinemaManager.Instance.EventSet(_cinemaCount);
                        CinemaManager.Instance.CinemaLog();
                        QuestWindow.Instance.MainQuestAdd(101);
                        UIManager.Instance.ActiveWindow(3);
                    }
                    break;
                case 1:
                    if (playerStat.playerAbility.cinemaCount == _cinemaCount && playerStat.Level == 1)
                    {
                        CinemaManager.Instance.EventSet(_cinemaCount);
                        CinemaManager.Instance.CinemaLog();
                    }
                    break;
                case 2:
                    if (playerStat.playerAbility.cinemaCount == _cinemaCount && playerStat.Level == 1)
                    {
                        CinemaManager.Instance.EventSet(_cinemaCount);
                        CinemaManager.Instance.CinemaLog();
                    }
                    break;
                case 3:
                    if (playerStat.playerAbility.cinemaCount == _cinemaCount && playerStat.Level > 1)
                    {
                        for (int i = 0; i < playerStat.playerAbility.playerQuests.Count; i++)
                        {
                            currentQuest = playerStat.playerAbility.playerQuests[i];
                            if (currentQuest.questNubmer == 101 && !currentQuest.isEnd)
                            {
                                CinemaManager.Instance.EventSet(_cinemaCount);
                                CinemaManager.Instance.CinemaLog();
                                playerStat.QuestCheck(101);
                                QuestWindow.Instance.SetToggle(currentQuest);
                                UIManager.Instance.ActiveWindow(3);
                                break;
                            }
                        }
                    }
                    break;
                case 4:
                    if (playerStat.playerAbility.cinemaCount == _cinemaCount)
                    {
                        CinemaManager.Instance.EventSet(_cinemaCount);
                        CinemaManager.Instance.CinemaLog();
                    }
                    break;
                case 6:
                    if (playerStat.playerAbility.cinemaCount == _cinemaCount)
                    {
                        for (int i = 0; i < playerStat.playerAbility.playerQuests.Count; i++)
                        {
                            currentQuest = playerStat.playerAbility.playerQuests[i];
                            if (currentQuest.questNubmer == 104 && !currentQuest.isEnd)
                            {
                                CinemaManager.Instance.EventSet(_cinemaCount);
                                CinemaManager.Instance.CinemaLog();
                                playerStat.QuestCheck(104);
                                UIManager.Instance.ActiveWindow(3);
                                if (playerStat.MapInpo == 1)
                                    playerStat.MapInpo++;
                                QuestWindow.Instance.MapToggleInit();
                                QuestWindow.Instance.SetToggle(currentQuest);
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("CinemaCheck"))
        {
            collision.GetComponent<BoxCollider2D>().enabled = false;
            string cinemaName = Regex.Replace(collision.name, @"\D", "");
            int.TryParse(cinemaName, out int cinemaCount);
            PlayerCinemaCheck(cinemaCount);
        }
    }
}
