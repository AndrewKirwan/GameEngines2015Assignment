using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class DriftState : State3
    {
        public GameObject target;

        // Use this for initialization
        public DriftState(HunterFSM owner) : base(owner)
        {

        }

        // Update is called once per frame
        public override string Description()
        {
            return "Drift State";
        }

        public override void Enter()
        {
            if (owner.gameObject.tag == "leader1")
            {
                Boid boid = owner.GetComponent<Boid>();
                boid.arriveEnabled = true;
            }
            if (owner.gameObject.tag == "hunter")
            {
                Boid boid = owner.GetComponent<Boid>();
                boid.offsetPursuitEnabled = true;
            }
        }

        public override void Exit()
        {
            Boid boid = owner.GetComponent<Boid>();
            if(owner.gameObject.tag == "leader1")
            {
                boid.arriveEnabled = false;
            }

            if (owner.gameObject.tag == "hunter")
            {
                boid.offsetPursuitEnabled = true;
            }
        }

        public override void Update()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.arriveTargetPos = GameObject.FindGameObjectWithTag("mothership").transform.position;
        }
        void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.tag == "leader" || other.gameObject.tag == "defender" || other.gameObject.tag == "harvester" ))
            {
                target = other.gameObject;
                owner.SwitchState(new SeekTargetState(owner, other.gameObject));
            }
        }
    }
}