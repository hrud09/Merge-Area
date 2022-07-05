using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public TMP_Text turnText;
    //public GameObject hand;
    public int TurnCount;
    [Range(1,5)]
    public int AILevel;
    int maxRange;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        switch (AILevel)
        {
            case 1:
                maxRange = 10;
                break;
            case 2:
                maxRange = 8;
                break;
            case 3:
                maxRange = 6;
                break;
            case 4:
                maxRange = 4;
                break;
            case 5:
                maxRange = 1;
                break;
        }
    }
    //Turn Count "0" means, Player's Turn
    public void ResetTurn( int turnCount)
    {
        if (GameManager.Instance.gameFinished)
        {
            return;
        }
        if (GameManager.Instance.players.Count>1)
        {
            if (turnCount == 0)
            {
                //Player's Turn
                turnText.text = "YOUR TURN";
                TurnCount = 0;
            }
            else if (turnCount == 1)
            {
                TurnCount = 1;
                turnText.text = "OPPONENT's TURN";
                StartCoroutine(OpponentsMove());
            }
        }
        
    }

    public IEnumerator OpponentsMove()
    {
        yield return new WaitForSeconds(2f);
        if (GameManager.Instance.players.Count > 1)
        {

            List<RopeSweepCut> enemyRopes = new List<RopeSweepCut>();
            for (int i = 0; i < RopeManager.Instance.enemyRopeSweepCutScript.Count; i++)
            {
                if (RopeManager.Instance.enemyRopeSweepCutScript[i].gameObject.activeInHierarchy)
                {
                    enemyRopes.Add(RopeManager.Instance.enemyRopeSweepCutScript[i]);
                }
                else
                {
                    RopeManager.Instance.enemyRopeSweepCutScript.Remove(RopeManager.Instance.enemyRopeSweepCutScript[i]);
                }
            }
            int index = Random.Range(0, enemyRopes.Count);
            if (Random.Range(0, maxRange) == 0)
            {
                for (int i = 0; i < enemyRopes.Count; i++)
                {
                    if (enemyRopes[i].attachedPlayer[0].GetComponent<CharacterInfo>().characterType == enemyRopes[i].attachedPlayer[1].GetComponent<CharacterInfo>().characterType)
                    {
                        index = i;
                    }
                }
            }
           
            
            enemyRopes[index].AICut();
        }

    }
}
