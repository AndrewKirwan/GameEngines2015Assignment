using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
    public float speed = 35.0f;
    Vector3 updatedPos;
    Vector3 startPos;
	// Use this for initialization
	void Start () {

        startPos = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {
        updatedPos = transform.position;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        LineRenderer line = GetComponent<LineRenderer>();
        line.SetPosition(0, transform.position + transform.forward);
        line.SetPosition(1, transform.position - transform.forward);
        if(Vector3.Distance(startPos, updatedPos) > 350)
        {
            Destroy(gameObject);
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "hunter") || (other.gameObject.tag == "harvester") || (other.gameObject.tag == "defender"))
        {
            Destroy(other.gameObject);
        }

    }
}
