using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class RIPState : State3
    {

        // Use this for initialization
        public RIPState(HunterFSM owner) : base(owner)
        {

        }

        // Update is called once per frame
        public override string Description()
        {
            return "RIP State";
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