using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Obi;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    public static RopeManager Instance;
    public List<ObiRope> Ropes; 
    public List<RopeSweepCut> enemyRopeSweepCutScript;
    public int onRopeCount;
    public GameObject[] vfx;
    public GameObject vfxExplosion;
    public Material lineMaterial;
    public ObjectType objectType;
    
    public enum  ObjectType
    {
        castle,
        ball,
        character
    }
    public enum CharacterType
    {
        Player,
        Enemy,
        Normal
    }
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Ropes = FindObjectsOfType<ObiRope>().ToList();
      
        StartCoroutine(SetEnemyRopes());
    }

    IEnumerator SetEnemyRopes()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < Ropes.Count; i++)
        {
            for (int j = 0; j < Ropes[i].GetComponent<RopeSweepCut>().attachedPlayer.Count; j++)
            {
                if (Ropes[i].GetComponent<RopeSweepCut>().attachedPlayer[j].GetComponent<CharacterInfo>().characterType == CharacterType.Enemy)
                {
                    if (!enemyRopeSweepCutScript.Contains(Ropes[i].GetComponent<RopeSweepCut>()))
                    {
                        enemyRopeSweepCutScript.Add(Ropes[i].GetComponent<RopeSweepCut>());
                    }
              
                }
            }
           
        }
    }
   
    public void CheckDoubleAttachment(CharacterInfo strongPlayer)
    {

        for (int i = 0; i < strongPlayer.attachedPlayer.Count; i++)
        {
            int RopeCount = 0;
            List<ObiRope> ropes = new List<ObiRope>();
            for (int j = 0; j < strongPlayer.ropesAttached.Count; j++)
            {
                if (strongPlayer.attachedPlayer[i].GetComponent<CharacterInfo>().ropesAttached.Contains(strongPlayer.ropesAttached[j]))
                {
                    RopeCount += 1;
                    ropes.Add(strongPlayer.ropesAttached[j]);
                }
            }
            if (ropes.Count>1)
            {
                for (int k = 1; k < ropes.Count; k++)
                {

                    strongPlayer.ropesAttached.Remove(ropes[k]);
                    for (int l = 0; l < strongPlayer.attachedPlayer.Count; l++)
                    {
                        if (strongPlayer.attachedPlayer[l].GetComponent<CharacterInfo>().ropesAttached.Contains(ropes[k]))
                        {
                            strongPlayer.attachedPlayer[l].GetComponent<CharacterInfo>().ropesAttached.Remove(ropes[k]);
                        }
                    }

                    if (Ropes.Contains(ropes[k]))
                    {
                        Ropes.Remove(ropes[k]);
                    }

                    if (enemyRopeSweepCutScript.Contains(ropes[k].GetComponent<RopeSweepCut>()))
                    {
                        enemyRopeSweepCutScript.Remove(ropes[k].GetComponent<RopeSweepCut>());
                    }
                    ropes[k].gameObject.SetActive(false);
                }
            }
     
        }
      
    }

}