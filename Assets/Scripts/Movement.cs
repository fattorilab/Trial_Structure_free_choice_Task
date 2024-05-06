using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    #region Variables Declaration

    // Time vars
    public float presstime = 0;
    float lastTimeStatic;
    public float speed;

    // Movement vars
    Vector3 CamPosition;
    Vector3 CamRotation;
    float arduX = 0;
    float arduY = 0;

    // Control movement vars
    public float restrict_horizontal = 1;
    public float restrict_backwards = 1;
    public float restrict_forwards = 1;
    [System.NonSerialized] public bool keypressed = false;
    
    // Axes inversion
    public bool reverse_Xaxis = false;
    public bool reverse_Yaxis = false;
    int x_inversion = 1;
    int y_inversion = 1;

    // GameObjects
    GameObject experiment;
    Rigidbody rb;
    GameObject target;

    // Collisions
    [System.NonSerialized] public bool HasCollided = false;
    [System.NonSerialized] public float CollisionTime = 0f;
    [System.NonSerialized] public GameObject CollidedObject;
    private Vector3 lastPosition;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Track time
        lastTimeStatic = Time.time;

        // GameObjects
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("Target");
        experiment = GameObject.Find("Experiment");

        // In case of inverted axes
        if (reverse_Xaxis) { x_inversion = -1; }
        if (reverse_Yaxis) { y_inversion = -1; }

    }
    
    void FixedUpdate()
    {
        // In case of collision, increase collision time
        if (HasCollided)
        {
            CollisionTime += Time.deltaTime;
        }

        // Initialize vars to reassign ardu vars in case they are NaN
        arduX = x_inversion * experiment.GetComponent<Ardu>().ax1;
        arduY = y_inversion * experiment.GetComponent<Ardu>().ax2;
        var arduX_notNaN = arduX;
        var arduY_notNaN = arduY;

        // Set Ardu vars to 0 to do operations because NaN is "Not a Number"
        if (float.IsNaN(arduX) && float.IsNaN(arduY))
        {
            arduX_notNaN = (int)0;
            arduY_notNaN = (int)0;
        }

        // Player moves
        if (Input.anyKey || arduX_notNaN != 0 || arduY_notNaN != 0)
        {
            keypressed = true;

            #region Horizontal and vertical movement

            // Horizontal (i.e. rotation) movement
            CamRotation = transform.localEulerAngles;
            CamRotation.y += Input.GetAxis("Horizontal") * Time.deltaTime * restrict_horizontal * 40 * speed;
            CamRotation.y += (arduX_notNaN / 512f) * Time.deltaTime * restrict_horizontal * 40 * speed;
            transform.localEulerAngles = CamRotation;

            // Vertical (i.e. forward/backward) movement
            Vector3 moveVector = ((transform.rotation * Camera.main.transform.localRotation) * Vector3.forward * Input.GetAxis("Vertical") * 4) + ((transform.rotation * Camera.main.transform.localRotation) * Vector3.forward * (arduY_notNaN / 512f) * 4);
            if (Input.GetAxis("Vertical") > 0 || arduY_notNaN > 0) //
            {
                rb.MovePosition(transform.position + Vector3.Normalize(moveVector) * speed * restrict_forwards * Time.deltaTime);
            }
            else //if (Input.GetAxis("Vertical") < 0 || arduY_notNaN < 0)
            {
                rb.MovePosition(transform.position + Vector3.Normalize(moveVector) * speed * restrict_backwards * Time.deltaTime); // if backwards is restricted (0),everything is set to 0
            }

            #endregion

            // Record time spent moving
            presstime = (Time.time - lastTimeStatic) * 1000;
        }
        else
        {
            keypressed = false;
            lastTimeStatic = Time.time;
            presstime = 0;
        }

    }

    #region Methods

    void OnCollisionEnter(Collision collision)
    {
        // Acknowledge collision start for the maintask
        lastPosition = transform.position;
        HasCollided = true;
        CollidedObject = collision.gameObject;

    }

    void OnCollisionExit(Collision other)
    {
        if (Vector3.Distance(transform.position, lastPosition) > 0.1f)
        {
            // Acknowledge collision end for the maintask
            HasCollided = false;

            // Reset collision time
            CollisionTime = 0f;
        }
    }

    public void resetCollision()
    {
        // Reset collision to false
        if (HasCollided)
        {
            HasCollided = false;
        }

        // Reset collision time
        CollisionTime = 0f;
    }

    #endregion

}
