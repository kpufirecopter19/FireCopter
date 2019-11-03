using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OculusTouchInput_Helicopter : MonoBehaviour
{
    public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Input_Sources any = SteamVR_Input_Sources.Any;

    // Action Buttons
    public SteamVR_Action_Pose pose = SteamVR_Actions.default_Pose;
    public SteamVR_Action_Boolean trigger = SteamVR_Actions.default_Trigger;
    public SteamVR_Action_Vector2 stick = SteamVR_Actions.default_Stick;
    public SteamVR_Action_Boolean grab = SteamVR_Actions.default_Grab;
    public SteamVR_Action_Boolean gripbutton = SteamVR_Actions.default_GripButton;
    public SteamVR_Action_Boolean ybbutton = SteamVR_Actions.default_YBButton;
    public SteamVR_Action_Boolean xabutton = SteamVR_Actions.default_XAButton;
    public SteamVR_Action_Boolean start = SteamVR_Actions.default_StartButton;
    public float maxSpeed;

    private GameObject helicopter;
    private GameObject Camera;

    private Vector3 lCtrl;
    private Vector3 rCtrl;

    private float prevAlt;
    private float currAlt;
    private float altDelta;

    // Start is called before the first frame update
    void Start()
    {
        altDelta = 0;
        helicopter = GameObject.Find("Helicopter") ;
        Camera = GameObject.Find("[CameraRig]");

        lCtrl = pose.GetLocalPosition(leftHand);
        rCtrl = pose.GetLocalPosition(rightHand);
        currAlt = lCtrl.y;
        prevAlt = currAlt;
    }

    // Update is called once per frame
    void Update()
    {
        currAlt = pose.GetLocalPosition(leftHand).y;

        // 상하 고도 조절 레버
        if (stick.GetActive(leftHand))
        {
            Vector2 movement = stick.GetAxis(leftHand);
            helicopter.transform.Translate(new Vector3(movement.x, 0, movement.y));
        }
        if (gripbutton.GetState(leftHand))
        {
            altDelta += (currAlt - prevAlt)/2;
        }
        // 전후좌우 조절 레버
        if (gripbutton.GetState(rightHand))
        {

        }

        helicopter.transform.Translate(new Vector3(0, altDelta, 0));
        prevAlt = currAlt;
    }
}
