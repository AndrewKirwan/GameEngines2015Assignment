using UnityEngine;
using System.Collections;

namespace BGE
{

    public class FighterFSM : MonoBehaviour
    {
        public float hitpoints = 5.0f;
        public GameObject leader;
        public GameObject hunter;
        State2 state = null;
        float theta = 0;

        // Use this for initialization
        void Start()
        {
            SwitchState(new IdleState(this));
            StartCoroutine("Def");
        }

        System.Collections.IEnumerator Def()
        {
            while (hitpoints > 0)
            {
                yield return new WaitForSeconds(1.0f);
            }
            Destroy(gameObject);
            SwitchState(new KillState(this));
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 toTarget = hunter.transform.position - transform.position;
            toTarget.Normalize();
            float dot = Vector3.Dot(transform.forward, toTarget);
            theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (state != null)
            {
                state.Update();
            }
        }
        public void SwitchState(State2 state)
        {
            if (this.state != null)
            {
                this.state.Exit();
            }
            this.state = state;
            if (this.state != null)
            {
                this.state.Enter();
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.tag == "hunter"))
            {
                hunter = other.gameObject;
                SwitchState(new SeekHunterState(this, other.gameObject));
            }
            if ((other.gameObject.tag == "lazer"))
            {
                hitpoints--;
                Destroy(other.gameObject);
            }
        }
        System.Collections.IEnumerator fireProjectile()
        {
            while (true)
            {
                // Use a line renderer
                GameObject lazer = new GameObject();
                lazer.transform.position = transform.position;
                lazer.transform.rotation = transform.rotation;
                LineRenderer line = lazer.AddComponent<LineRenderer>();
                lazer.AddComponent<Shoot>();
                lazer.AddComponent<BoxCollider>();
                lazer.GetComponent<BoxCollider>().size = new Vector3(2, 2, 2);
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