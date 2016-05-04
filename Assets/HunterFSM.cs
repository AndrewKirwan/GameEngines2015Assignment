using UnityEngine;
using System.Collections;

namespace BGE
{

    public class HunterFSM : MonoBehaviour
    {
        public float hitpoints = 5.0f;
        float theta = 0;
        public GameObject leader;
        public GameObject target;
        State3 state = null;

        // Use this for initialization
        void Start()
        {
            SwitchState(new DriftState(this));
            StartCoroutine("Att");
            StartCoroutine("fireProjectile");
        }

        System.Collections.IEnumerator Att()
        {
            while (hitpoints > 0)
            {
                yield return new WaitForSeconds(1.0f);
            }
            Destroy(gameObject);
            SwitchState(new RIPState(this));
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 toTarget = target.transform.position - transform.position;
            toTarget.Normalize();
            float dot = Vector3.Dot(transform.forward, toTarget);
            theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (state != null)
            {
                state.Update();
            }
        }
        public void SwitchState(State3 state)
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
            if ((other.gameObject.tag == "leader" || other.gameObject.tag == "defender" || other.gameObject.tag == "harvester"))
            {
                target = other.gameObject;
                SwitchState(new SeekTargetState(this, other.gameObject));
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
                lazer.GetComponent<BoxCollider>().size = new Vector3(2,2,2);
                lazer.GetComponent<BoxCollider>().isTrigger = true;
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