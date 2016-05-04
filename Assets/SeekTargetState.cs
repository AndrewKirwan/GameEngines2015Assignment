using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class SeekTargetState : State3
    {
        GameObject target;
        float theta = 0;

        public SeekTargetState(HunterFSM owner, GameObject target) : base(owner)
        {
            this.target = target;
            
        }

        public override string Description()
        {
            return "Seek Target State";
        }

        public override void Enter()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.pursuitEnabled = true;
            
        }

        public override void Exit()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.pursuitEnabled = false;
        }

        public override void Update()
        {

            Boid boid = owner.GetComponent<Boid>();
            boid.pursuitTarget = target;

        }

    }
}