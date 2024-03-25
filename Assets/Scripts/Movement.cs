using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public bool reverse_Xaxis;
    public bool reverse_Yaxis;

    public float restrict_horizontal = 1;
    public float restrict_backwards = 1;
    public float restrict_forwards = 1;
    [System.NonSerialized] public bool keypressed = false;
    
    public float presstime = 0;
    GameObject Exp;

    Vector3 CamPosition;
    Vector3 CamRotation;
    float arduX = 0;
    float arduY = 0;

    float lastTimeStatic;
    int x_inversion = 1;
    int y_inversion = 1;

    Rigidbody rb;
    GameObject target;

    // MANAGE COLLISIONS
    [System.NonSerialized] public bool HasCollided = false;
    [System.NonSerialized] public float CollisionTime = 0f;
    [System.NonSerialized] public GameObject CollidedObject;

    // Start is called before the first frame update
    void Start()
    {
        lastTimeStatic = Time.time;
        if (reverse_Xaxis) { x_inversion = -1; }
        if (reverse_Yaxis) { y_inversion = -1; }
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("Target");
        Exp = GameObject.Find("Experiment");
    }

    
    void OnCollisionEnter(Collision collision)
    {
        // Acknowledge collision for the maintask
        HasCollided = true;
        CollidedObject = collision.gameObject;

    }

    void OnCollisionExit(Collision other)
    {
        // Acknowledge collision for the maintask
        HasCollided = false;

        // Reset collision time
        CollisionTime = 0f;
    }

    void FixedUpdate()
    {
        // Increase collision time
        if (HasCollided)
        {
            CollisionTime += Time.deltaTime;
        }

        arduX = x_inversion * Exp.GetComponent<Ardu>().ax1;
        arduY = y_inversion * Exp.GetComponent<Ardu>().ax2;

        // Initialize vars to reassign ardu vars in case they are NaN
        var arduX_notNaN = arduX;
        var arduY_notNaN = arduY;

        // Set ardu vars to 0 to do operations because NaN is "Not a Number"
        if (float.IsNaN(arduX) && float.IsNaN(arduY))
        {
            arduX_notNaN = (int)0;
            arduY_notNaN = (int)0;
        }

        if (Input.anyKey || arduX_notNaN != 0 || arduY_notNaN != 0)
        {
            keypressed = true;

            //Movement
            CamRotation = transform.localEulerAngles;
            
            CamRotation.y += Input.GetAxis("Horizontal") * Time.deltaTime * restrict_horizontal * 40 * speed;

            CamRotation.y += (arduX_notNaN / 512f) * Time.deltaTime * restrict_horizontal * 40 * speed;

            transform.localEulerAngles = CamRotation;

            Vector3 moveVector = ((transform.rotation * Camera.main.transform.localRotation) * Vector3.forward * Input.GetAxis("Vertical") * 4) + ((transform.rotation * Camera.main.transform.localRotation) * Vector3.forward * (arduY_notNaN / 512f) * 4);

            if (Input.GetAxis("Vertical") > 0 || arduY_notNaN > 0) //
            {
                rb.MovePosition(transform.position + Vector3.Normalize(moveVector) * speed * restrict_forwards * Time.deltaTime);
            }
            else //if (Input.GetAxis("Vertical") < 0 || arduY < 0)
            {
                rb.MovePosition(transform.position + Vector3.Normalize(moveVector) * speed * restrict_backwards * Time.deltaTime); // if backwards is restricted (0),everything is set to 0
            }


            presstime = (Time.time - lastTimeStatic) * 1000;
        }
        else
        {
            keypressed = false;
            lastTimeStatic = Time.time;
            presstime = 0;
        }

    }
}
