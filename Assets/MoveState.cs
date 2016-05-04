using UnityEngine;
using System.Collections;

namespace BGE
{

    public class MoveState : State
    {
        Vector3 target;

        // Use this for initialization
        public MoveState(ResourceFSM owner, Vector3 target) : base(owner)
        {
            this.target = target;
        }

        // Update is called once per frame
        public override string Description()
        {
            return "Moving State";
        }

        public override void Enter()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.arriveEnabled = true;
        }

        public override void Exit()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.arriveEnabled = false;
        }

        public override void Update()
        {
            Boid boid = owner.GetComponent<Boid>();
            boid.arriveTargetPos = target;
        }
        void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.tag == "hunter"))
            {
                owner.SwitchState(new FleeHunterState(owner, other.gameObject));
            }
            if ((other.gameObject.tag == "asteroid"))
            {
                target = other.gameObject.transform.position;
                owner.SwitchState(new WorkState(owner, other.gameObject));
            }
        }
    }
}
