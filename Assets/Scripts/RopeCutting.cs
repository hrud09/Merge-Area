using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class RopeCutting : MonoBehaviour
{
  
     List<ObiStructuralElement> ropeElements;

     ObiSolver solver;
     ObiRope ropeObject;


    void Start()
    {
        ropeObject = GetComponent<ObiRope>();
        solver = ropeObject.solver;
        solver.OnCollision += Solver_OnCollision;
    }

    void Solver_OnCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
    {
        var world = ObiColliderWorld.GetInstance();
        for (int i = 0; i < e.contacts.Count; ++i)
        {
            if (e.contacts.Data[i].distance < 0.01f)
            {       
                ObiColliderBase collider =world.colliderHandles[e.contacts[i].bodyB].owner;
                    
                if (collider.tag == "Cutter")
                {   
                   // collider.enabled = false;
                    ObiSolver.ParticleInActor pa = solver.particleToActor[e.contacts.Data[i].bodyA];
                    
                    if (pa != null)
                    {
                        ropeObject = pa.actor.GetComponent<ObiRope>();
                        ropeElements = ropeObject.elements;
                    }
                   
                    for (int j = 0; j < ropeElements.Count; j++)
                    {
                        if (ropeElements[j].particle1 == e.contacts.Data[i].bodyA)
                        {
                            ropeObject.Tear(ropeElements[j]);
                            ropeObject.RebuildConstraintsFromElements();
                           // solver = new ObiSolver();
                        }
                    }
                
                }
            }
        }
    }


}
