using UnityEngine;
using System.Collections;
using System;

namespace BGE
{

    public class SeekHunterState : State2
    {
        GameObject hunter;
        float theta = 0;

        public SeekHunterState(FighterFSM owner, GameObject hunter) : base(owner)
        {
            this.hunter = hunter;
        }

        public override string Description()
        {
            return "Seek Hunter State";
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
            boid.offsetPursuitTarget = hunter;
            Vector3 toTarget = hunter.transform.position - owner.transform.position;
            toTarget.Normalize();
            float dot = Vector3.Dot(owner.transform.forward, toTarget);
            float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;

        }

        System.Collections.IEnumerator fireProjectile()
        {
            while (true)
            {
                // Use a line renderer
                GameObject lazer = new GameObject();
                lazer.transform.position = owner.transform.position;
                lazer.transform.rotation = owner.transform.rotation;
                LineRenderer line = lazer.AddComponent<LineRenderer>();
                lazer.AddComponent<Shoot>();
                lazer.AddComponent<BoxCollider>();
                lazer.tag = "lazer";
                line.material = new Material(Shader.Find("Particles/Additive"));
                line.SetColors(Color.red, Color.blue);
                line.SetWidth(0.1f, 0.1f);
                line.SetVertexCount(2);
                yield return new WaitForSeconds(2.0f);
            }
        }
    }
}