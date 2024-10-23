using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocks : MonoBehaviour
{
    public GameObject Rock1;
    public GameObject Rock2;
    public GameObject Rock3;
    public GameObject Rock4;
    public GameObject Rock5;
    GameObject experiment;

    void Start()
    {
        experiment = GameObject.Find("Experiment");
    }

    public void MakeRock(float x, float z)
    {
        int rock_nr = Random.Range(1, 6);
        switch (rock_nr)
        {
            case 1:
                GameObject rock1 = Instantiate(Rock1, new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(-180f, 180f), 0), transform);
                rock1.transform.localScale *= 1.5f;
                rock1.tag = "Rock1";
                break;
            case 2:
                GameObject rock2 = Instantiate(Rock2, new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(-180f, 180f), 0), transform);
                rock2.transform.localScale *= 1.5f;
                rock2.tag = "Rock2";
                break;
            case 3:
                GameObject rock3 = Instantiate(Rock3, new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(-180f, 180f), 0), transform);
                rock3.transform.localScale *= 1.5f;
                rock3.tag = "Rock3";
                break;
            case 4:
                GameObject rock4 = Instantiate(Rock4, new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(-180f, 180f), 0), transform);
                rock4.transform.localScale *= 1.5f;
                rock4.tag = "Rock4";
                break;
            case 5:
                GameObject rock5 = Instantiate(Rock5, new Vector3(x, 0.5f, z), Quaternion.Euler(0, Random.Range(-180f, 180f), 0), transform);
                rock5.transform.localScale *= 1.5f;
                rock5.tag = "Rock5";
                break;
            default:
                Debug.Log("Error with Rock Number.");
                break;
        }
        

    }

    public void saveObjects()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Rock5" || child.tag == "Rock4" || child.tag == "Rock3" || child.tag == "Rock2" || child.tag == "Rock1")
            {
                experiment.GetComponent<Saver>().addObject(child.GetInstanceID().ToString(), child.tag,
                                        child.transform.position.x, child.transform.position.y, child.transform.position.z,
                                        child.transform.eulerAngles.x, child.transform.eulerAngles.y, child.transform.eulerAngles.z,
                                        child.transform.localScale.x, child.transform.localScale.y, child.transform.localScale.z); // transform.eulerAngles.y
            }
        }
    }

    public void deleteRocks()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Rock5" || child.tag == "Rock4" || child.tag == "Rock3" || child.tag == "Rock2" || child.tag == "Rock1")
            {
                experiment.GetComponent<Saver>().addObjectEnd(child.GetInstanceID().ToString());
                Destroy(child.gameObject);
            }
        }
    }
}
