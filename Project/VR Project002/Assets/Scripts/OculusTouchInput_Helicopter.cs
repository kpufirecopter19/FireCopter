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
    private Quaternion rCtrl;

    private float prevAlt;
    private float currAlt;
    private float altDelta;

    private Quaternion prevAngle;
    private Quaternion currAngle;
    private Quaternion angleDelta;

    // Start is called before the first frame update
    void Start()
    {
        altDelta = 0;
        angleDelta = Quaternion.Euler(new Vector3(0, 0, 0));

        helicopter = GameObject.Find("Helicopter") ;
        Camera = GameObject.Find("[CameraRig]");

        lCtrl = pose.GetLocalPosition(leftHand);
        rCtrl = pose.GetLocalRotation(rightHand);
        currAlt = lCtrl.y;
        prevAlt = currAlt;

        currAngle = rCtrl;
        prevAngle = currAngle;
    }

    // Update is called once per frame
    void Update()
    {
        currAlt = pose.GetLocalPosition(leftHand).y;
        currAngle = pose.GetLocalRotation(rightHand);

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
            //고정된 Y축을 기준으로 회전량을 계산한다.
            Quaternion prevAngleMin = prevAngle;
            prevAngleMin.x = -prevAngle.x;
            prevAngleMin.y = -prevAngle.y;
            prevAngleMin.z = -prevAngle.z;
            angleDelta *= (currAngle * prevAngleMin);

            //angleDelta = Quaternion.Euler(new Vector3(angleDelta.x + currAngle.x - prevAngle.x, 0, angleDelta.z + currAngle.z - prevAngle.z));
        }
        helicopter.transform.Rotate(-angleDelta.z/5,0,angleDelta.x/5);
        helicopter.transform.Translate(new Vector3((angleDelta.x)/Mathf.PI/5, altDelta, -(angleDelta.z)/Mathf.PI/5));
        prevAlt = currAlt;
        prevAngle = currAngle;
    }
}
