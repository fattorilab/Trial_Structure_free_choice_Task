using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateTreesAndTargets : MonoBehaviour
{
    public GameObject experiment;
    public Saver saver;

    public GameObject tree1;
    public GameObject tree2;
    public GameObject tree3;
    public GameObject lowlevel;
    public GameObject midlevel;
    public GameObject highlevel;
    public GameObject doubleobject;

    void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        experiment = GameObject.Find("Experiment");
        saver = experiment.GetComponent<Saver>();
         
        createGreenery();
    }

    public void createGreenery()
    {

        for (int i = 0; i < 2; i++) //48 + 8 obj /before 45 + 9
        {
            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor
            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor

            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor
            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor

            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor
            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor

            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor
            createAndSaveObject(tree1, "Obstacle"); //Distraktor
            createAndSaveObject(tree2, "Obstacle"); //Distraktor
            createAndSaveObject(tree3, "Obstacle"); //Distraktor

            /*createAndSaveObject(lowlevel, "LowTarget");
            createAndSaveObject(midlevel, "MidTarget");
            createAndSaveObject(highlevel, "HighTarget");
            createAndSaveObject(doubleobject, "DoubleTarget");*/
        }

    }


    void createAndSaveObject(GameObject Prefab, string type)
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-25f, 25f), 0, UnityEngine.Random.Range(-25f, 25f));
        GameObject newObject = Instantiate(Prefab, transform.position + pos, Quaternion.Euler(0, UnityEngine.Random.Range(-180f, 180f), 0), transform);
        newObject.tag = type;

        Vector3 position = newObject.transform.position;
        Vector3 rotation = newObject.transform.eulerAngles;
        Vector3 scale = newObject.transform.localScale;

        saver.addObject(
            newObject.GetInstanceID().ToString(),
            type,
            position[0],
            position[1],
            position[2],
            rotation[0],
            rotation[1],
            rotation[2],
            scale[0],
            scale[1],
            scale[2]
            );
    }

    public void deleteGreenery()
    {
        foreach (Transform child in transform)
        {
            if (child.name != "Ground")
            {
                saver.addObjectEnd(child.GetInstanceID().ToString());
                Destroy(child.gameObject);
            }
        }
    }
}
