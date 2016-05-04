using UnityEngine;
using System.Collections;

namespace BGE {

    public class ResourceFSM : MonoBehaviour {

        public float hitpoints = 10.0f;
        Vector3 offset = new Vector3(0, 40, 0);
        public GameObject hunter;
        public GameObject asteroid;

        State state = null;

        // Use this for initialization
        void Start() {
            SwitchState(new MoveState(this, asteroid.transform.position + offset));
            StartCoroutine("Harvest");
        }

        System.Collections.IEnumerator Harvest()
        {
            while (hitpoints > 0)
            {
                yield return new WaitForSeconds(1.0f);
            }
            Destroy(gameObject);
            SwitchState(new DeadState(this));
        }

        // Update is called once per frame
        void Update() {
            if (state != null)
            {
                state.Update();
            }
        }

        public void SwitchState(State state)
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
                SwitchState(new FleeHunterState(this, other.gameObject));
            }
            if ((other.gameObject.tag == "asteroid"))
            {
                asteroid = other.gameObject;
                SwitchState(new WorkState(this, other.gameObject));
            }
            if ((other.gameObject.tag == "lazer"))
            {
                hitpoints--;
                Destroy(other.gameObject);
            }
        }
    }
}