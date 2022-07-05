using System.Collections.Generic;
using DG.Tweening;
using Obi;
using UnityEngine;
using TMPro;

public class CharacterInfo : MonoBehaviour
{
    //public bool isOpponent;
    public RopeManager.CharacterType characterType;
    public int Level;
    public List<ObiRope> ropesAttached;
    public TMP_Text levelText;
    public List<GameObject> attachedPlayer;
    public Animator anim;
    private void Start()
    {
        if (Level > 0)
        {
            levelText.text = Level.ToString();
        }

        anim = GetComponent<Animator>();

    }

    public void ChangeLevel(int toIncrease)
    {
        Level += toIncrease;
        if (Level > 0)
        {
            levelText.transform.parent.gameObject.SetActive(true);
            levelText.text = Level.ToString();
        }
        else
        {
            levelText.transform.parent.gameObject.SetActive(false);
        }
    }

    public void FindAttachedPlayer()
    {
        attachedPlayer = new List<GameObject>();
        for (int i = 0; i < ropesAttached.Count; i++)
        {
            for (int j = 0; j < ropesAttached[i].GetComponents<ObiParticleAttachment>().Length; j++)
            {
                if (!attachedPlayer.Contains(ropesAttached[i].GetComponents<ObiParticleAttachment>()[j].target
                        .gameObject) && ropesAttached[i].GetComponents<ObiParticleAttachment>()[j].target.gameObject !=
                    this.gameObject)
                {
                    attachedPlayer.Add(ropesAttached[i].GetComponents<ObiParticleAttachment>()[j].target.gameObject);
                }
            }
        }

        if (RopeManager.Instance.objectType == RopeManager.ObjectType.character)
        {
            transform.LookAt(attachedPlayer[0].transform.position);
        }
      
    }

    public void Rotate(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
        transform.GetChild(0).DORotate(left * 15, 0.05f);

    }

    public void ResetRotation()
    {
        transform.GetChild(0).DORotate(new Vector3(0, 0, 0), 0.05f);
    }

    public void LookAtCharacter(Transform character)
    {
        Vector3 target = character.position;
        transform.LookAt(target);

    }

}
