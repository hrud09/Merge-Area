using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Obi;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ObiRope))]
public class RopeSweepCut : MonoBehaviour
{

    public Camera cam;
    ObiParticleAttachment[] attach;
    public ObiRope rope;
    Material dotLineMat;
    LineRenderer lineRenderer;
    Vector3 cutStartPosition;
    public List<GameObject> attachedPlayer;
    public bool cuttingThisRope;
    RopeManager.ObjectType _objectType;
  
    private void Start()
    {
        _objectType = RopeManager.Instance.objectType;
        dotLineMat = RopeManager.Instance.lineMaterial;
        rope = GetComponent<ObiRope>();
      
        attach = rope.GetComponents<ObiParticleAttachment>();
        for (int j = 0; j < attach.Length; j++)
        {
            attach[j].target.GetComponent<CharacterInfo>().ropesAttached.Add(this.rope);
            attachedPlayer.Add(attach[j].target.gameObject);
        }

        for (int i = 0; i < attachedPlayer.Count; i++)
        {
            attachedPlayer[i].GetComponent<CharacterInfo>().FindAttachedPlayer();
        }
        lineRenderer = FindObjectOfType<LineRenderer>();
        if (!lineRenderer)
        {
            AddMouseLine();
        }
    
    }

    private void OnDestroy()
    {
        DeleteMouseLine();
    }

    private void AddMouseLine()
    {

        GameObject line = new GameObject("Mouse Line");
        lineRenderer = line.AddComponent<LineRenderer>();
        GameManager.Instance.line = lineRenderer;
        lineRenderer.enabled = false;
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.numCapVertices = 2;
        //  lineRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        // lineRenderer.sharedMaterial.color = Color.red;
        lineRenderer.material = dotLineMat;
        lineRenderer.enabled = false;


    }

    private void DeleteMouseLine()
    {
        if (lineRenderer != null)
            Destroy(lineRenderer.gameObject);
    }

    private void LateUpdate()
    {

        if (cam == null) return;
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (TurnManager.Instance.TurnCount == 1)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            cutStartPosition = Input.mousePosition;
            lineRenderer.SetPosition(0, cam.ScreenToWorldPoint(new Vector3(cutStartPosition.x, cutStartPosition.y, 0.5f)));
            lineRenderer.enabled = true;
        }
        else if(Input.GetMouseButton(0))
        {
            for (int i = 0; i < rope.elements.Count; ++i)
            {

                Vector3 screenPos1 = cam.WorldToScreenPoint(rope.solver.positions[rope.elements[i].particle1]);
                Vector3 screenPos2 = cam.WorldToScreenPoint(rope.solver.positions[rope.elements[i].particle2]);
                if (SegmentSegmentIntersection(screenPos1, screenPos2, cutStartPosition, Input.mousePosition, out float o, out float p) && !cuttingThisRope && RopeManager.Instance.onRopeCount==0)
                {
                    cuttingThisRope = true;
                    RopeManager.Instance.onRopeCount = 1;
                }

            }
        }

        if (lineRenderer.enabled)
            lineRenderer.SetPosition(1, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f)));

    
        if (Input.GetMouseButtonUp(0))
        {
            RopeManager.Instance.onRopeCount = 0;
            ScreenSpaceCut(cutStartPosition, Input.mousePosition);
            lineRenderer.enabled = false;
        }
    }

    public void ScreenSpaceCut(Vector2 lineStart, Vector2 lineEnd)
    {
   
        bool cut = false;

        for (int i = 0; i < rope.elements.Count; ++i)
        {

            Vector3 screenPos1 = cam.WorldToScreenPoint(rope.solver.positions[rope.elements[i].particle1]);
            Vector3 screenPos2 = cam.WorldToScreenPoint(rope.solver.positions[rope.elements[i].particle2]);

         
            if (SegmentSegmentIntersection(screenPos1, screenPos2, lineStart, lineEnd, out float r, out float s) && cuttingThisRope)
            {
                HapticManager.Instance.TriggerTaptic(HapticManager.HapticType.Light);
                RopeManager.Instance.onRopeCount = 0;
                cut = true;
                PlayerMove();
              
                rope.Tear(rope.elements[i]);
                RopeManager.Instance.Ropes.Remove(this.GetComponent<ObiRope>());
                if (RopeManager.Instance.enemyRopeSweepCutScript.Contains(this))
                {
                    RopeManager.Instance.enemyRopeSweepCutScript.Remove(this);
                }
             
                //  this.enabled = false;
            }
        }


        if (cut) rope.RebuildConstraintsFromElements();
        else cuttingThisRope = false;
    }

    public void AICut()
    {
        try
        {
            RopeManager.Instance.onRopeCount = 0;
            rope.Tear(rope.elements[Random.Range(0, rope.elements.Count)]);
            rope.RebuildConstraintsFromElements();
            PlayerMove();
   
        }
        catch (NullReferenceException)
        {

            print(gameObject.name);
        }
       
    }
    GameObject weakCharacter,strongCharacter;
    private void PlayerMove()
    {
        if (GameManager.Instance.players.Count <= 1)
        {
            return;
        }

        if ( RopeManager.Instance.Ropes.Contains(this.GetComponent<ObiRope>()))
        {
            RopeManager.Instance.Ropes.Remove(this.GetComponent<ObiRope>());
        }
      
        if (RopeManager.Instance.enemyRopeSweepCutScript.Contains(this))
        {
            RopeManager.Instance.enemyRopeSweepCutScript.Remove(this);
        }
        try
        {
            CharacterInfo c1, c2;
            c1 = attachedPlayer[0].GetComponent<CharacterInfo>();
            c2 = attachedPlayer[1].GetComponent<CharacterInfo>();

            if (c1.characterType == c2.characterType)
            {
                weakCharacter = attachedPlayer[1];
                strongCharacter = attachedPlayer[0];
              //c1.ChangeLevel(c2.Level);
            }
            else
            {
                if (c1.Level > c2.Level)
                {
                    weakCharacter = attachedPlayer[1];
                    strongCharacter = attachedPlayer[0];
                     // attachedPlayer[0].GetComponent<CharacterInfo>().ChangeLevel( -attachedPlayer[1].GetComponent<CharacterInfo>().Level);
                }
                else if (c1.Level < c2.Level)
                {
                    weakCharacter = attachedPlayer[0];
                    strongCharacter = attachedPlayer[1];
                   // attachedPlayer[1].GetComponent<CharacterInfo>().ChangeLevel( -attachedPlayer[0].GetComponent<CharacterInfo>().Level);
                }
                else
                {
                    if (c1.characterType == RopeManager.CharacterType.Enemy)
                    {
                        weakCharacter = attachedPlayer[0];
                        strongCharacter = attachedPlayer[1];
                    //    c1.ChangeLevel( -attachedPlayer[0].GetComponent<CharacterInfo>().Level);
                    }
                    else
                    {
                        weakCharacter = attachedPlayer[1];
                        strongCharacter = attachedPlayer[0];
                      //  attachedPlayer[0].GetComponent<CharacterInfo>().ChangeLevel( -attachedPlayer[1].GetComponent<CharacterInfo>().Level);

                    }


                }
            }

            float timeToDestroy = 0;
            if (_objectType == RopeManager.ObjectType.character)
            {
                c1.LookAtCharacter(c2.transform);
                c2.LookAtCharacter(c1.transform);
                if (c1.characterType != c2.characterType)
                {
                    strongCharacter.GetComponent<CharacterInfo>().anim.Play("Smash");
                    weakCharacter.GetComponent<CharacterInfo>().anim.SetLayerWeight(1, 1);
                    strongCharacter.GetComponent<CharacterInfo>().anim.SetLayerWeight(1, 1);
                    timeToDestroy = 1f;
                }
                else
                {
                    timeToDestroy = 0f;
                    strongCharacter.GetComponent<CharacterInfo>().anim.SetLayerWeight(1, 1);
                    weakCharacter.GetComponent<CharacterInfo>().anim.SetLayerWeight(1, 1);
                }

            }
            else if (_objectType == RopeManager.ObjectType.castle)
            {
                c1.Rotate(attachedPlayer[1].transform.position);
                c2.Rotate(attachedPlayer[0].transform.position);
            }

            Vector3 targetPos = MidPos(attachedPlayer[0].transform.position, attachedPlayer[1].transform.position);
          
            attachedPlayer[1].transform.DOMove(targetPos, 0.5f).SetEase(Ease.InBack);

            attachedPlayer[0].transform.DOMove(targetPos, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                strongCharacter.GetComponent<CharacterInfo>().anim.SetLayerWeight(1, 0);
                weakCharacter.GetComponent<CharacterInfo>().anim.SetLayerWeight(1, 0);
                HapticManager.Instance.TriggerTaptic(HapticManager.HapticType.Medium);
                if (c1.characterType == c2.characterType)
                {
                    strongCharacter.GetComponent<CharacterInfo>().ChangeLevel(weakCharacter.GetComponent<CharacterInfo>().Level);
                }
                else
                {
                    strongCharacter.GetComponent<CharacterInfo>().ChangeLevel(weakCharacter.GetComponent<CharacterInfo>().Level);
                }
                if (_objectType == RopeManager.ObjectType.character)
                {
                    strongCharacter.transform.LookAt(strongCharacter.GetComponent<CharacterInfo>().attachedPlayer[0].transform.position);

                }
                else
                {
                    strongCharacter.GetComponent<CharacterInfo>().ResetRotation();
                }

                //Vfx
                Destroy(Instantiate(RopeManager.Instance.vfx[Random.Range(0, RopeManager.Instance.vfx.Length)], targetPos + Vector3.up * 4f, Quaternion.identity), 3);
                Destroy(Instantiate(RopeManager.Instance.vfxExplosion, targetPos + Vector3.up * 4f, Quaternion.identity), 3);

                List<ObiRope> attachedRope = weakCharacter.GetComponent<CharacterInfo>().ropesAttached;

                for (int i = 0; i < attachedRope.Count; i++)
                {

                    ObiParticleAttachment[] attachment = attachedRope[i].GetComponents<ObiParticleAttachment>();
                    for (int j = 0; j < attachment.Length; j++)
                    {
                        if (attachment[j].target == weakCharacter.transform)
                        {
                            attachment[j].target = strongCharacter.transform;

                            if (!strongCharacter.GetComponent<CharacterInfo>().ropesAttached.Contains(attachedRope[i]))
                            {
                                strongCharacter.GetComponent<CharacterInfo>().ropesAttached.Add(attachedRope[i]);
                            }
                        }

                    }

                    for (int k = 0; k < attachedRope[i].GetComponent<RopeSweepCut>().attachedPlayer.Count; k++)
                    {
                        if (attachedRope[i].GetComponent<RopeSweepCut>().attachedPlayer[k] == weakCharacter)
                        {
                            attachedRope[i].GetComponent<RopeSweepCut>().attachedPlayer[k] = strongCharacter;
                        }
                    }
                }
                for (int i = 0; i < GameManager.Instance.players.Count; i++)
                {
                    if (GameManager.Instance.players[i].attachedPlayer.Contains(weakCharacter))
                    {
                        if (GameManager.Instance.players[i].attachedPlayer.Contains(strongCharacter) || GameManager.Instance.players[i].gameObject == strongCharacter)
                        {
                            GameManager.Instance.players[i].attachedPlayer.Remove(weakCharacter);
                        }
                        else
                        {
                            for (int J = 0; J < GameManager.Instance.players[i].attachedPlayer.Count; J++)
                            {
                                if (GameManager.Instance.players[i].attachedPlayer[J] == weakCharacter)
                                {
                                    GameManager.Instance.players[i].attachedPlayer[J] = strongCharacter;
                                }

                            }
                        }

                        // 

                    }
                }
                for (int i = 0; i < GameManager.Instance.players.Count; i++)
                {
                    if (GameManager.Instance.players[i].ropesAttached.Contains(this.rope))
                    {
                        GameManager.Instance.players[i].ropesAttached.Remove(this.rope);
                    }
                }
                //strongCharacter.GetComponent<CharacterInfo>().ropesAttached.Remove(this.rope);
               // weakCharacter.GetComponent<CharacterInfo>().ropesAttached.Remove(this.rope);

                for (int i = 0; i < weakCharacter.GetComponent<CharacterInfo>().attachedPlayer.Count; i++)
                {
                    if (weakCharacter.GetComponent<CharacterInfo>().attachedPlayer[i] != strongCharacter && !strongCharacter.GetComponent<CharacterInfo>().attachedPlayer.Contains(weakCharacter.GetComponent<CharacterInfo>().attachedPlayer[i]))
                    {
                        strongCharacter.GetComponent<CharacterInfo>().attachedPlayer.Add(weakCharacter.GetComponent<CharacterInfo>().attachedPlayer[i]);

                    }
                }

                if (_objectType == RopeManager.ObjectType.character && c1.characterType != c2.characterType)
                {
                    weakCharacter.GetComponent<CharacterInfo>().anim.Play("Smashed");
                }

                GameManager.Instance.players.Remove(weakCharacter.GetComponent<CharacterInfo>());



                if (_objectType == RopeManager.ObjectType.ball)
                {
                    // Vector3 initScale = strongCharacter.transform.GetChild(0).localScale + Vector3.one*0.5f;
                    float targetScale =0.5f * strongCharacter.GetComponent<CharacterInfo>().Level;
                    targetScale = Mathf.Clamp(targetScale, 0.4f, 3.4f);
                    strongCharacter.transform.GetChild(0).DOScale(Vector3.one * targetScale, 0.3f);
                }


                Destroy(weakCharacter, timeToDestroy);

                if (TurnManager.Instance.TurnCount == 0)
                {
                    TurnManager.Instance.ResetTurn(1);
                }
                else
                {
                    TurnManager.Instance.ResetTurn(0);
                }
                RopeManager.Instance.CheckDoubleAttachment(strongCharacter.GetComponent<CharacterInfo>());
                GameManager.Instance.WinCheck();
                gameObject.SetActive(false);

            });
        }
        catch (UnityEngine.MissingReferenceException)
        {
            print(gameObject.name);
        }




    }
    private bool SegmentSegmentIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D, out float r, out float s)
    {
        float denom = (B.x - A.x) * (D.y - C.y) - (B.y - A.y) * (D.x - C.x);
        float rNum = (A.y - C.y) * (D.x - C.x) - (A.x - C.x) * (D.y - C.y);
        float sNum = (A.y - C.y) * (B.x - A.x) - (A.x - C.x) * (B.y - A.y);

        if (Mathf.Approximately(rNum, 0) || Mathf.Approximately(denom, 0))
        {  r = -1; s = -1; return false; }

        r = rNum / denom;
        s = sNum / denom;

        return (r >= 0 && r <=1  && s >= 0 && s <= 1);
    }

    Vector3 MidPos (Vector3 a, Vector3 b)
    {
        Vector3 m = new Vector3();
        m.x = a.x + (b.x - a.x)/2;
        m.y = a.y;
        m.z = a.z + (b.z - a.z)/2;

        return m;
    }
}
