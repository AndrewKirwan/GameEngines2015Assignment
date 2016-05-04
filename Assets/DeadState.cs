using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class DeadState : State
    {

        // Use this for initialization
        public DeadState(ResourceFSM owner) : base(owner)
        {

        }

        // Update is called once per frame
        public override string Description()
        {
            return "Dead State";
        }

        public override void Enter()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.TurnOffAll();
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }
    }
}