using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<CharacterInfo> players;
    public bool gameFinished;
    public LineRenderer line;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        players =  FindObjectsOfType<CharacterInfo>().ToList();
    }

    // Start is called before the first frame update
    public void WinCheck()
    {
        if (GameCanvasControl.Instance.gameState == GameState.LevelCompleted || GameCanvasControl.Instance.gameState == GameState.LevelFailed || gameFinished)
        {
            return;
        }
        if (players.Count == 1)
        {
            line.enabled = false;
            gameFinished = true;
            if (players[0].characterType != RopeManager.CharacterType.Enemy)
            {
                HapticManager.Instance.TriggerTaptic(HapticManager.HapticType.Success);
                GameOverPanelControl.Instance.EnablePanel(GameState.LevelCompleted, 0.5f);
                GetComponents<AudioSource>()[0].Play();
            }
            else
            {
                HapticManager.Instance.TriggerTaptic(HapticManager.HapticType.Failure);
                GameOverPanelControl.Instance.EnablePanel(GameState.LevelFailed, 0.5f);
                GetComponents<AudioSource>()[1].Play();
            }

        }
        else
        {
            if (HasWon() || LostAlready())
            {
                line.enabled = false;
                for (int i = RopeManager.Instance.Ropes.Count - 1; i >= 0; i--)
                {

                    StartCoroutine(CutAllRopes(  RopeManager.Instance.Ropes[i].gameObject.GetComponent<RopeSweepCut>()));
                }
            }
        }
    }

    IEnumerator CutAllRopes(RopeSweepCut ropeSweepCut)
    {
        yield return new WaitForSeconds(0.6f);
        if (ropeSweepCut.gameObject.activeInHierarchy)
        {

            ropeSweepCut.gameObject.GetComponent<RopeSweepCut>().AICut();

        }

    }

    private bool HasWon()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].characterType==RopeManager.CharacterType.Enemy)
            {
                return false;
            }
        }
        return true;

    }

    private bool LostAlready()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].characterType == RopeManager.CharacterType.Player)
            {
                return false;

            }
        }

        return true;
    }
}
