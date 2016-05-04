using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class IdleState : State2
    {
        GameObject hunter;

        public IdleState(FighterFSM owner) : base(owner)
        {
        }

        public override string Description()
        {
            return "Idle State";
        }

        public override void Enter()
        {
            Boid boid = owner.GetComponent<Boid>();
            if(owner.tag == "leader")
            {
                boid.randomWalkEnabled = true;
            }
            if(owner.tag == "defender")
            {
                boid.offsetPursuitEnabled = true;
            }
        }

        public override void Exit()
        {
            Boid boid = owner.GetComponent<Boid>();
            if (owner.tag == "leader")
            {
                boid.randomWalkEnabled = false;
            }
            if (owner.tag == "defender")
            {
                boid.offsetPursuitEnabled = false;
            }
        }

        public override void Update()
        {
            Boid boid = owner.GetComponent<Boid>();
            if (owner.tag == "defender")
            {
                boid.offsetPursuitTarget = owner.leader;
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.tag == "hunter"))
            {
                hunter = other.gameObject;
                owner.SwitchState(new SeekHunterState(owner, other.gameObject));
            }
        }
    }
}