using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class WorkState : State
    {
        GameObject asteroid;

        public WorkState(ResourceFSM owner, GameObject asteroid) : base(owner)
        {
            this.asteroid = asteroid;
        }

        public override string Description()
        {
            return "Work State";
        }

        public override void Enter()
        {
            owner.transform.LookAt(asteroid.transform);
        }

        public override void Exit()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.TurnOffAll();
        }

        public override void Update()
        {
            owner.transform.LookAt(asteroid.transform);
            LineRenderer line = owner.GetComponent<LineRenderer>();
            line.SetPosition(0, owner.transform.position);
            line.SetPosition(1, asteroid.transform.position);

        }
    }
}