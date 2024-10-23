using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    GameObject experiment;

    void Start()
    {
        experiment = GameObject.Find("Experiment");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            experiment.GetComponent<Saver>().addObject(transform.tag, "TouchedARock",
                                                    transform.position.x, transform.position.y, transform.position.z,
                                                    transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z,
                                                    transform.localScale.x, transform.localScale.y, transform.localScale.z); // transform.eulerAngles.y

            // Do something upon contact
            Debug.LogWarning("PLAYER TOUCHED ROCK, BUT ROCK HAS NO BEHAVIOUR SET UP. MIND THAT.");
            // experiment.GetComponent<MainTask>().phase = 103;
        }
    }
}
