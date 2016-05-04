using UnityEngine;
using System.Collections;

namespace BGE
{

    public class FleeHunterState : State
    {
        GameObject hunter;

        // Use this for initialization
        public FleeHunterState(ResourceFSM owner, GameObject hunter) : base(owner)
        {
            this.hunter = hunter;
        }

        // Update is called once per frame
        public override string Description()
        {
            return "Fleeing State";
        }

        public override void Enter()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.fleeEnabled = true;
        }

        public override void Exit()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.fleeEnabled = false;
        }

        public override void Update()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.fleeTarget = hunter;
        }
    }
}
