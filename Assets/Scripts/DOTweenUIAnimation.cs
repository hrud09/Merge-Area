using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DOTweenUIAnimation : MonoBehaviour
{
    public Ease easeType;
    public Vector3 targetPos;
    public GameObject nextActiveObject;
    [SerializeField]private float timeToCome;
    private void Start()
    {
        transform.DOLocalMove(targetPos, timeToCome).SetEase(easeType).OnComplete(()=> {

            if (nextActiveObject)
            {
                nextActiveObject.SetActive(true);
            }
        });
    }
}
