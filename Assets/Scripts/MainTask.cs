using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using PupilLabs;


public class MainTask : MonoBehaviour
{
    #region Variables Declaration

    [SerializeField]

    [HideInInspector]
    [Header("GameObjects and components")]

    // Cams
    [System.NonSerialized] Camera camM;
    [System.NonSerialized] Camera camL;
    [System.NonSerialized] Camera camR;

    // Pupil
    [System.NonSerialized] public PupilDataStream PupilDataStreamScript;
    private RequestController RequestControllerScript;
    private bool PupilDataConnessionStatus;

    // Game
    Rigidbody player_rb;
    [HideInInspector] GameObject environment;
    [HideInInspector] GameObject experiment;
    [HideInInspector] GameObject player;

    [Header("Saving info")]
    public string MEF;
    public string path_to_data = "C:/Users/admin/Desktop/Registrazioni_VR/";
    public bool RECORD_EYES;
    [System.NonSerialized] public int lastIDFromDB;
    [HideInInspector] public int seed = 12345;
    [HideInInspector] public long starttime = 0;
    [HideInInspector] public int frame_number = 0;

    [Header("Reward")]
    public int RewardLength = 50;
    float RewardLength_in_sec; // Formatting
    public int reward_counter = 0; //just for having this information readibily accessible
    public int minimumRewardTime; //delay before juicy ??unnecessary??
    [HideInInspector] public float minimumDistance; //to get juicy

    [Header("Trials Info")]
    public string file_name_positions;
    // States
    [System.NonSerialized] public int current_state;
    private int last_state;
    [System.NonSerialized] public string error_state;
    // Trials
    [System.NonSerialized] public int current_trial;
    public int trials_win;
    public int trials_lose;
    public int[] trials_for_target;
    public int trials_for_cond = 1;
    // Conditions
    private int randomIndex;
    [System.NonSerialized] public List<int> condition_list;
    [System.NonSerialized] public int current_condition;
    //
    private float lastevent;
    private string identifier;
    private bool isMoving = false;
    private bool first_frame;

    [Header("Target Info")]
    // List, because it is changing size during the runtime
    public List<Vector3> target_positions = new List<Vector3>();
    GameObject[] targets;
    [System.NonSerialized] public GameObject TargetPrefab;
    [System.NonSerialized] public Vector3 CorrectTargetCurrentPosition;

    #region Materials
    [Header("Target Materials")]
    [System.NonSerialized] public Material initial_grey;
    [System.NonSerialized] public Material red;
    [System.NonSerialized] public Material green_dot;
    [System.NonSerialized] public Material red_dot;
    [System.NonSerialized] public Material final_grey;
    [System.NonSerialized] public Material white;
    #endregion

    [Header("Epoches Info")]
    // Array, because is not changing size during the runtime
    public float[] FREE_timing = { 0.3f, 0.6f, 0.9f };
    public float[] DELAY_timing = { 0.3f, 0.6f, 0.9f };
    public float[] RT_timing = { 0.3f, 0.6f, 0.9f };

    private List<int> FREE_timing_list;
    private List<int> DELAY_timing_list;
    private List<int> RT_timing_list;

    public float BASELINE_duration = 2f;
    public float INTERTRIAL_duration = 2f;
    private float FREE_duration;
    private float DELAY_duration;
    private float RT_maxduration;
    public float MOVEMENT_maxduration = 6f;
    public float second_RT_maxduration = 2f;

    [Header("Arduino Info")]
    [System.NonSerialized] public Ardu ardu;
    [System.NonSerialized] public float arduX;
    [System.NonSerialized] public float arduY;

    [Header("PupilLab Info")]
    [System.NonSerialized] public Vector2 centerRightPupilPx = new Vector2(float.NaN, float.NaN);  
    [System.NonSerialized] public Vector2 centerLeftPupilPx = new Vector2(float.NaN, float.NaN);  
    [System.NonSerialized] public float diameterRight = float.NaN;                 
    [System.NonSerialized] public float diameterLeft = float.NaN;                   
    [System.NonSerialized] public bool pupilconnection;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Setup
        UnityEngine.Random.InitState(seed);
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        RewardLength_in_sec = RewardLength / 1000f;
        first_frame = true;

        // States
        current_state = -2;
        last_state = -2;
        error_state = "";

        // Trials
        current_trial = 0;
        trials_win = 0;
        trials_lose = 0;
   
        // GameObjects
        ardu = GetComponent<Ardu>(); 
        player = GameObject.Find("Player");
        player_rb = player.GetComponent<Rigidbody>();
        experiment = GameObject.Find("Experiment");
        environment = GameObject.Find("Environment");

        // Init cameras
        camM = GameObject.Find("Main Camera").GetComponent<Camera>();
        camL = GameObject.Find("Left Camera").GetComponent<Camera>();
        camR = GameObject.Find("Right Camera").GetComponent<Camera>();

        // PupilLab
        PupilDataStreamScript = GameObject.Find("PupilDataManagment").GetComponent<PupilDataStream>();
        RequestControllerScript = GameObject.Find("PupilDataManagment").GetComponent<RequestController>();

        // Materials
        initial_grey = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/fruit/neutralgrey.mat");
        red = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/fruit/red_fruit.mat");
        green_dot = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/fruit/green_dot.mat");
        red_dot = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/fruit/reddot.mat");
        final_grey = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/fruit/grey_fruit.mat");
        white = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/fruit/white_fruit.mat");

        // Target Prefab
        TargetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Material/Fruit_Prefab.prefab");

        // Import targets coordinates from csv file into target_positions list
        // and initiate the targets in a disable state (i.e invisible)
        InstantiateTargets(target_positions);

        // Define number of trials per each target
        trials_for_target = new int[target_positions.Count];

        // Generate condition and timing vectors
        condition_list = CreateRandomSequence(target_positions.Count, trials_for_cond * target_positions.Count);
        FREE_timing_list = CreateRandomSequence(FREE_timing.Length, trials_for_cond * target_positions.Count);
        DELAY_timing_list = CreateRandomSequence(DELAY_timing.Length, trials_for_cond * target_positions.Count);
        RT_timing_list = CreateRandomSequence(RT_timing.Length, trials_for_cond * target_positions.Count);

    }

    void Update()
    {
        frame_number++;

        // Start on first operating frame
        if (first_frame) 
        {
            Debug.Log("START TASK");
            // Start time main task unity
            starttime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            // Send START trigger
            ardu.SendStartRecordingOE();           
            first_frame = false;
        }

        // Check if the player is moving the joystick
        isMoving = player.GetComponent<Movement>().keypressed;

        #region StateMachine

        switch (current_state)
        {
            case -2: // TASK BEGINS

                if (PupilDataStreamScript.subsCtrl.IsConnected || RequestControllerScript.ans)
                {
                    foreach (Camera cam in player.GetComponentsInChildren<Camera>())
                    {
                        cam.backgroundColor = Color.black;
                    }
                    current_state = -1;
                }

                break;

            case -1: // INTERTRIAL

                #region State Beginning (executed once upon entering)

                if (last_state != current_state)
                {
                    Debug.Log($"Current state: {current_state}");

                    current_condition = -1;

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                    error_state = "";

                }
                #endregion

                #region State Body (executed every frame while in state)

                current_condition = -1;

                #endregion

                #region State End (executed once upon exiting)
                if (!isMoving && ((Time.time - lastevent) > INTERTRIAL_duration))
                {
                    // Enable movement
                    player.GetComponent<Movement>().restrict_backwards = 1;
                    player.GetComponent<Movement>().restrict_forwards = 1;
                    player.GetComponent<Movement>().restrict_horizontal = 1;

                    // Move to state 0
                    current_state = 0;

                }
                #endregion

                break;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            case 0: // BASELINE

                #region State Beginning (executed once upon entering)

                if (last_state != current_state)
                {
                    Debug.Log($"Current state: {current_state}");

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                    error_state = "";
                }
                #endregion

                #region State Body (executed every frame while in state)
                // Prevent entering FREE state from a position different from initial
                if (isMoving)
                {
                    reset_position();
                }
                #endregion

                #region State End (executed once upon exiting)
                if (!isMoving && ((Time.time - lastevent) > BASELINE_duration))
                {
                    // Prepare everything for next trial

                    // Change target material (neutral mat is the initial grey ball )
                    for (int i = 0; i < targets.Length; i++)
                    {
                        changeTargetMaterial(targets[i], initial_grey);      
                    }

                    // Choose the correct target
                    current_condition = condition_list[0];
                    CorrectTargetCurrentPosition = target_positions[current_condition];

                    // Picking first time from the timing list to select epoch durations in this trial
                    FREE_duration = FREE_timing[FREE_timing_list[0]];
                    DELAY_duration = DELAY_timing[DELAY_timing_list[0]];
                    RT_maxduration = RT_timing[RT_timing_list[0]];

                    // Move to state 1
                    current_state = 1;

                    // Trial starts
                    current_trial++;

                }
                #endregion

                break;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            case 1: // FREE

                #region State Beginning
                if (last_state != current_state) 
                {
                    Debug.Log($"Current state: {current_state}");

                    // Enable targets
                    showTargets(targets);

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                    error_state = "";
                }
                #endregion

                #region State Body
                if (isMoving)
                {
                    error_state = "ERR: Moving in FREE";
                    current_state = -99;
                }
                #endregion

                #region State End
                // MEF required to be static for a minimum time (i.e. FREE_duration)
                if ((Time.time - lastevent) >= FREE_duration && !isMoving)
                {
                    current_state = 2;
                }
                #endregion

                break;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            case 2: //DELAY

                #region State Beginning
                if (last_state != current_state)
                {
                    Debug.Log($"Current state: {current_state}");

                    // Change target material (green dot)
                    for (int i = 0; i < targets.Length; i++)
                    {
                        changeTargetMaterial(targets[i], green_dot);       
                    }

                    // Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                    error_state = "";
                }
                #endregion

                #region State Body
                if (isMoving)
                {
                    error_state = "ERR: Moving in DELAY";
                    current_state = -99;
                }
                #endregion

                #region State End
                if ((Time.time - lastevent) >= DELAY_duration && !isMoving)
                {
                    current_state = 3;
                }
                #endregion

                break;


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            case 3: //RT

                #region State Beginning
                if (last_state != current_state)
                {
                    Debug.Log($"Current state: {current_state}");

                    // Change target material (red dot)
                    for (int i = 0; i < targets.Length; i++)
                    {
                        changeTargetMaterial(targets[i], red_dot);
                    }

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                    error_state = "";
                }
                #endregion

                #region State Body
                if (isMoving) 
                {
                    current_state = 4;
                }
                #endregion

                #region State End
                if ((Time.time - lastevent) >= RT_maxduration && !isMoving)
                {
                    error_state = "ERR: Not Moving in RT";
                    current_state = -99;
                }
                #endregion

                break;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            case 4: // MOVEMENT

                #region State Beginning
                if (last_state != current_state)
                {
                    Debug.Log($"Current state: {current_state}");

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                    error_state = "";
                }
                #endregion

                #region State Body

                if (player.GetComponent<Movement>().HasCollided) // If collision happened
                {

                    // Check if collided object is the correct one
                    if (player.GetComponent<Movement>().CollidedObject.transform.position == CorrectTargetCurrentPosition)
                    {
                        // Go to second RT
                        current_state = 5;
                    }
                    else
                    {
                        error_state = $"ERR: Selected target at {player.GetComponent<Movement>().CollidedObject.transform.position} but correct position: {CorrectTargetCurrentPosition}";
                        current_state = -99;
                    }


                    // Change target material (juicy mat is grey ball)
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (targets[i].name == player.GetComponent<Movement>().CollidedObject.name)
                        {
                            changeTargetMaterial(targets[i], final_grey);
                        }
                    }

                    // Go to second RT
                    current_state = 5;
                }

                #endregion

                #region State End
                if ((Time.time - lastevent) >= MOVEMENT_maxduration)
                {
                    error_state = "ERR: Not Finding Target in MOVEMENT";
                    current_state = -99;
                }
                #endregion

                break;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            case 5: // 2ND RT

                #region State Beginning
                if (last_state != current_state)
                {
                    Debug.Log($"Current state: {current_state}");

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                    error_state = "";
                }
                #endregion

                #region State Body
                // MEF stops moving
                if (!isMoving)
                {
                    // Change target material (eaten mat is white ball)
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (targets[i].name == player.GetComponent<Movement>().CollidedObject.name)
                        {
                            changeTargetMaterial(targets[i], white);
                        }
                    }

                    // Go to reward
                    if ((Time.time - lastevent) >= 1f)
                    {
                        current_state = 99;
                    }
                }


                // If player exits the collision (i.e. contact time lower than reaction time)
                if (!player.GetComponent<Movement>().HasCollided)
                {
                    error_state = "ERR: Collision ended early in 2nd RT";
                    current_state = -99;
                }

                #endregion

                #region State End

                if ((Time.time - lastevent) >= second_RT_maxduration)
                {
                    error_state = "ERR: Not Moving in RT";
                    current_state = -99;
                }

                #endregion

                break;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            case -99: //ERROR

                #region State Beginning
                if (last_state != current_state)
                {
                    Debug.Log($"Current state: {current_state}");
                    Debug.Log(error_state);

                    reset_lose();

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;
                }
                #endregion

                #region State Body

                #endregion

                #region State End
                if (true)
                {
                    current_state = -1;
                    error_state = "";
                }
                #endregion

                break;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            case 99: //WIN

                #region State Beginning
                if (last_state != current_state)
                {

                    Debug.Log("TRIAL DONE");
                    GetComponent<Saver>().addObjectEnd(player.GetComponent<Movement>().CollidedObject.name);

                    reset_win();

                    //Beginning routine
                    lastevent = Time.time;
                    last_state = current_state;

                }
                #endregion

                #region State Body

                if (condition_list.Count == 0) { QuitGame();  }

                #endregion

                #region State End
                if ((Time.time - lastevent) >= RewardLength)
                {
                    current_state = -1;
                }
                #endregion

                break;

        }

        #endregion

        // Manual reward
        if (Input.GetKeyDown("space"))        { ardu.SendReward(RewardLength); }
        reward_counter = ardu.reward_counter;

    }

    void OnApplicationQuit()
    {
        ardu.SendStopRecordingOE();
        Debug.Log("END OF SESSION");
        QuitGame();
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Reset methods

    void reset_win()
    {
        // Disable targets
        hideTargets(targets);

        // Send reward
        ardu.SendReward(RewardLength);

        // Reset position
        reset_position();

        // Count trial
        trials_win++;
        trials_for_target[current_condition]++;

        // Remove condition
        condition_list.RemoveAt(0);
        FREE_timing_list.RemoveAt(0);
        DELAY_timing_list.RemoveAt(0);
        RT_timing_list.RemoveAt(0);
    }

    void reset_lose()
    {
        // Disable targets
        hideTargets(targets);

        // Reset position
        reset_position();

        // Reset conditions
        condition_list = SwapVector(condition_list);
        FREE_timing_list = SwapVector(FREE_timing_list); //not strictly necessary, but better for coherence...
        DELAY_timing_list = SwapVector(DELAY_timing_list);
        RT_timing_list = SwapVector(RT_timing_list);

        trials_lose++;
    }

    void reset_position()
    {
        // Move rigidbody back to initial position
        player_rb.position = Vector3.zero;
        player_rb.rotation = Quaternion.identity;

        // Disable player movement 
        player.GetComponent<Movement>().restrict_backwards = 0;
        player.GetComponent<Movement>().restrict_forwards = 0;
        player.GetComponent<Movement>().restrict_horizontal = 0;
    }

    #endregion

    #region Manage conditions

    public List<int> CreateRandomSequence(int n, int k) //n, number of elements; k, length of the required vector
    {
        var vector = new List<int>();
        for (int i = 0; i < Math.Floor((double)k / n) + 1; i++)
        {
            var tmp = Enumerable.Range(0, n).OrderBy(x => UnityEngine.Random.Range(0, n)).ToList();
            vector.AddRange(tmp);
        }

        // If k is not a multiple of n, we need to remove the extra elements
        if (vector.Count > k)
        {
            vector = vector.Take(k).ToList();
        }

        return vector;
    }

    public List<int> SwapVector(List<int> vector)
    {
        // Moves the first half to fifth of the vector to the end of the vector
        int i = vector.Count / UnityEngine.Random.Range(2, 5);  
        if (i > 0)
        {
            vector = vector.Skip(i).Concat(vector.Take(i)).ToList();
        }
        return vector;
    }

    void set_epochs_duration()
    {
        int randomIndex_FREE = UnityEngine.Random.Range(0, FREE_timing.Length);
        int randomIndex_DELAY = UnityEngine.Random.Range(0, DELAY_timing.Length);
        int randomIndex_RT = UnityEngine.Random.Range(0, RT_timing.Length);

        FREE_duration = FREE_timing[randomIndex_FREE];
        DELAY_duration = DELAY_timing[randomIndex_DELAY];
        RT_maxduration = RT_timing[randomIndex_RT];

    }

#endregion

    #region Manage targets

    private void LoadPositionsFromCSV(List<Vector3> target_positions)
    {
        string filePath = Application.dataPath + "/" + file_name_positions + ".csv";
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine(); // Salta la riga degli header se presente
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    string[] fields = line.Split(',');
                    if (fields.Length >= 3)
                    {
                        float x, y, z;
                        if (float.TryParse(fields[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x) &&
                            float.TryParse(fields[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y) &&
                            float.TryParse(fields[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z))
                        {
                            Vector3 position = new Vector3(x, y, z);
                            target_positions.Add(position);
                        }
                        else
                        {
                            Debug.LogWarning("Impossible to convert coordinates in numbers: " + line);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Line has not enough coordinates: " + line);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }
    }

    private void InstantiateTargets(List<Vector3> target_positions)
    {

        // Import targets coordinates from csv file into target_positions list
        LoadPositionsFromCSV(target_positions);
       
        // Instantiate targets (switched off)
        targets = new GameObject[target_positions.Count];
        for (int i = 0; i < targets.Length; i++)
        {
            // Instantiate
            targets[i] = Instantiate(TargetPrefab, target_positions[i], Quaternion.Euler(0, 0, 0), environment.transform); // TargetPrefab.transform.rotation , environment.transform);
            targets[i].name = $"{TargetPrefab.name}_{i.ToString()}";

            // Disable (make invisible)
            targets[i].GetComponent<MeshRenderer>().enabled = false;

            // Add target to data to be saved
            GetComponent<Saver>().addObject(targets[i].name,
                "Target", // target_positions[i].x, target_positions[i].z, 0, "Position");
                targets[i].transform.position.x,
                targets[i].transform.position.y,
                targets[i].transform.position.z,
                TargetPrefab.transform.rotation[0],
                TargetPrefab.transform.rotation[1],
                TargetPrefab.transform.rotation[2],
                targets[i].transform.localScale[0],
                targets[i].transform.localScale[1],
                targets[i].transform.localScale[2]
                );
        }
    }

    private void showTargets(GameObject[] targets)
    {

        for (int i = 0; i < targets.Length; i++)
        {
            // Enable (make visible)
            targets[i].GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void hideTargets(GameObject[] targets)
    { 

        for (int i = 0; i < targets.Length; i++)
        {
            // Disable (make invisible)
            targets[i].GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void changeTargetMaterial(GameObject target, Material mat)
    {
        target.GetComponent<MeshRenderer>().material = mat;
    }

    #endregion

}