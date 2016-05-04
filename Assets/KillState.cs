using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class KillState : State2
    {

        // Use this for initialization
        public KillState(FighterFSM owner) : base(owner)
        {

        }

        // Update is called once per frame
        public override string Description()
        {
            return "Kill State";
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