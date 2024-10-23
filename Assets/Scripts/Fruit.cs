using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public float distance;
    public bool chosen = false;
    public bool prejuicy = false;
    public bool fake_prejuicy = false;
    public bool classic_look = false;
    public bool obviously_wrong_but_possible = false;
    public bool juicy;
    public bool fake_juicy;
    public bool eaten = false;
    public bool multiple_fruit_mode = false;
    //public bool Reward_for_rotation;
    GameObject player;
    GameObject experiment;

    public Material neutral_mat;
    public Material red_mat;
    public Material chosen_mat;
    public Material prejuicy_mat;
    public Material juicy_mat;
    public Material eaten_mat;

    //float min_distance = 0;
    //float min_time2get_reward = 0;

    // Start is called before the first frame update
    void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        player = GameObject.Find("Player");
        experiment = GameObject.Find("Experiment");

        //min_distance = experiment.GetComponent<MainTask>().minimumDistance;
        //min_time2get_reward = experiment.GetComponent<MainTask>().minimumRewardTime;
        
        //beginTimeJuicy = Time.time;

        //GetComponent<Renderer>().material = neutral_mat;
    }

    //Vector3 forwardPosition;
    //Vector2 pos, tar, fwd;
    //public float playerAngle = 90;

    //float beginTimeJuicy;
    //public float timeJuicy = 0;
    //float releasetime = -1;
    //float nulltime = 0;
    //bool finished = false;
    // Update is called once per frame
    void Update()
    {
        //pos = new Vector2(player.transform.position.x, player.transform.position.z);
        //tar = new Vector2(transform.position.x, transform.position.z);
        //forwardPosition = player.transform.position + player.transform.rotation * Vector3.forward;
        //fwd = new Vector2(forwardPosition.x, forwardPosition.z);
        //playerAngle = Vector2.Angle(tar - pos, fwd - pos);

        //distance = Vector3.Distance(player.transform.position, transform.position);

        //if (!eaten) //can get juicy or unjuicy
        //{
        //    if (distance < min_distance && prejuicy)
        //    {
        //        if (player.GetComponent<Movement>().presstime > min_time2get_reward && !juicy && !player.GetComponent<Movement>().is_eating) //get juicy
        //        {
        //            GetComponent<Renderer>().material = juicy_mat;
        //            //if (experiment.GetComponent<MainTask>().immediateReward) { experiment.GetComponent<Ardu>().SendReward(experiment.GetComponent<MainTask>().reward_length); }
        //            beginTimeJuicy = Time.time;
        //            juicy = true;
        //            player.GetComponent<Movement>().is_eating = true;
        //        }
        //    } else if (distance < min_distance && fake_prejuicy)
        //    {
        //        if (player.GetComponent<Movement>().presstime > min_time2get_reward && !fake_juicy && !player.GetComponent<Movement>().is_eating) //get juicy
        //        {
        //            GetComponent<Renderer>().material = juicy_mat;
        //            beginTimeJuicy = Time.time;
        //            fake_juicy = true;
        //            player.GetComponent<Movement>().is_eating = true;
        //        }
        //    }
        //    else if (distance < 1f && obviously_wrong_but_possible)
        //    {
        //        if (player.GetComponent<Movement>().presstime > min_time2get_reward && !fake_juicy && !player.GetComponent<Movement>().is_eating) //get juicy
        //        {
        //            /*GetComponent<Renderer>().material = juicy_mat;
        //            beginTimeJuicy = Time.time;
        //            fake_juicy = true;
        //            player.GetComponent<Movement>().is_eating = true;*/

        //            //experiment.GetComponent<Saver>().addObject(transform.name,                                                                                                                                %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //            //                                        transform.position.x,
        //            //                                        transform.position.z,
        //            //                                        transform.eulerAngles.y, "ObviouslyWrongFruitTrigger");

        //            obviously_wrong_but_possible = false;
        //            // experiment.GetComponent<MainTask>().phase = 98;                                                                                                                                          %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //        }
        //    }
        //    else if (chosen)
        //    {
        //        GetComponent<Renderer>().material = chosen_mat;
        //        if (juicy || fake_juicy) { player.GetComponent<Movement>().is_eating = false; }
        //        juicy = false;
        //        fake_juicy = false;
        //        timeJuicy = 0;
        //    }
        //    else if (prejuicy || fake_prejuicy)
        //    {
        //        if (classic_look)
        //        {
        //            GetComponent<Renderer>().material = red_mat;
        //        } else
        //        {
        //            GetComponent<Renderer>().material = prejuicy_mat;
        //        }
        //        if (juicy || fake_juicy) { player.GetComponent<Movement>().is_eating = false; }
        //        juicy = false;
        //        fake_juicy = false;
        //        timeJuicy = 0;
        //    }
        //    else
        //    {
        //        GetComponent<Renderer>().material = neutral_mat;
        //        if (juicy || fake_juicy) { player.GetComponent<Movement>().is_eating = false; }
        //        juicy = false;
        //        fake_juicy = false;

        //        timeJuicy = 0;
        //    }
        //} else if (distance > min_distance && !finished)
        //{
        //    player.GetComponent<Movement>().is_eating = false;
        //    finished = true;
        //}

        //if (juicy || fake_juicy)
        //{
        //    timeJuicy = (Time.time - beginTimeJuicy) * 1000;

        //    if (!player.GetComponent<Movement>().keypressed) //gotit
        //    {
        //        if (releasetime == -1)
        //        {
        //            releasetime = Time.time;
        //        }


        //        //if released for nullpositionTime
        //        // nulltime = (float)experiment.GetComponent<MainTask>().nullpositionTime / 1000;                                                                                      %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //        if ((Time.time - releasetime) > nulltime && !eaten)
        //        {
        //            GetComponent<Renderer>().material = eaten_mat;
        //            eaten = true;

        //            //eating
        //            if (juicy) 
        //            {
        //                //experiment.GetComponent<Saver>().addObject(transform.name,                                                                                                    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //                //                                        transform.position.x,
        //                //                                        transform.position.z,
        //                //                                        transform.eulerAngles.y, "FruitTrigger");
        //                juicy = false;
        //                experiment.GetComponent<Ardu>().SendReward(experiment.GetComponent<MainTask>().RewardLength);

        //                if (multiple_fruit_mode)
        //                {
        //                    player.GetComponent<Movement>().is_eating = false;
        //                    // experiment.GetComponent<MainTask>().fruit_eaten_notification();                                                                                          %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //                }
        //                else
        //                {
        //                    // experiment.GetComponent<MainTask>().phase = 99;                                                                                                      %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //                }


        //            } else if (fake_juicy)
        //            {
        //                //experiment.GetComponent<Saver>().addObject(transform.name,                                                                                                    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //                //                                        transform.position.x,
        //                //                                        transform.position.z,
        //                //                                        transform.eulerAngles.y, "WrongFruitTrigger");
        //                fake_juicy = false;
        //                // experiment.GetComponent<MainTask>().phase = 98;                                                                                                              %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        //            }


        //            releasetime = -1;
        //        }
                

                
        //    } else
        //    {
        //        releasetime = -1;
        //    }
        //}
    }

}
