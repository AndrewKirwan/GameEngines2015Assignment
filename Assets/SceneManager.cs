using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {
    public GameObject tship;
    public GameObject hunters;
    // Use this for initialization
    void Start () {
        tship = GameObject.FindGameObjectWithTag("tship");
        hunters = GameObject.FindGameObjectWithTag("hunters");
        StartCoroutine(Delay());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(15);
        SpawnEnemies();
    }
    void SpawnEnemies()
    {
        tship.transform.position = new Vector3(200, 100, -500);
        hunters.transform.position = new Vector3(200, 120, -500);
        Instantiate(hunters, new Vector3 (200, 140, -500), hunters.transform.rotation);
        Instantiate(hunters, new Vector3(200, 160, -500), hunters.transform.rotation);
        Instantiate(hunters, new Vector3(200, 180, -500), hunters.transform.rotation);
    }
}
