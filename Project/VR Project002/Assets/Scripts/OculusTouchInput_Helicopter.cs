using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

static public class Constant
{
    public const float MAXSPEED = 20.0f;
    public const float MAXANGLE = 10.0f;
    public const float MAXALTITUDE = 20.0f;
}
public class OculusTouchInput_Helicopter : MonoBehaviour
{
    //Controller Input Sources
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
    public SteamVR_Action_Boolean turnLeft = SteamVR_Actions.default_SnapTurnLeft;
    public SteamVR_Action_Boolean turnRight = SteamVR_Actions.default_SnapTurnRight;
    public float maxSpeed;
    public float LCtrlDeltaRestriction;

    private GameObject helicopter;

    private GameObject lCtrlobj;
    private GameObject rCtrlobj;

    //left Altitude lever
    private Vector3 lCtrl;
    //right Angle controller
    private Quaternion rCtrl;

    //altitude
    private float prevAlt;
    private float currAlt;
    private float altDelta;

    private Quaternion prevAngle;
    private Quaternion currAngle;
    private Quaternion angleDelta;

    private Rigidbody heliRigidbody;

    private float altLever;

    private GameObject lLever;
    private GameObject rLever;

    // Start is called before the first frame update
    void Start()
    {
        altLever = 0.5f;
        angleDelta = Quaternion.Euler(new Vector3(0, 0, 0));

        lCtrlobj = GameObject.Find("Controller (left)");
        rCtrlobj = GameObject.Find("Controller (right)");

        helicopter = GameObject.Find("Helicopter_main");
        lLever = GameObject.Find("AltitudeController");
        rLever = GameObject.Find("AngleController");

        heliRigidbody = helicopter.GetComponent<Rigidbody>();

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

        // 헬기 방향 전환 레버
        if (turnLeft.GetState(leftHand)||turnRight.GetState(leftHand)) { ChangeDirection(); }

        // 상하 고도 조절 레버
        if (gripbutton.GetState(leftHand) && isLeverActive())
        {
            altDelta = (currAlt - prevAlt)/LCtrlDeltaRestriction;
            lLever.transform.Translate(0,altDelta,0);
            altLever += altDelta;
            altLever = Mathf.Clamp(altLever, 0, 1);
        }
        //Debug.Log(isLeverActive());
        // 전후좌우 조절 레버
        if (gripbutton.GetState(rightHand) &&isHandleActive())
        {
            //고정된 Y축을 기준으로 회전량을 계산한다.
            Quaternion prevAngleMin = prevAngle;
            prevAngleMin.x = -prevAngle.x;
            prevAngleMin.y = -prevAngle.y;
            prevAngleMin.z = -prevAngle.z;
            //angleDelta *= (currAngle * prevAngleMin);

            //angleDelta = Quaternion.Euler(new Vector3(angleDelta.x + currAngle.x - prevAngle.x, 0, angleDelta.z + currAngle.z - prevAngle.z));
            angleDelta = Quaternion.Euler(new Vector3(currAngle.x - prevAngle.x, 0, currAngle.z - prevAngle.z));
        }
        Debug.Log(isHandleActive());
        float xRotate = helicopter.transform.localRotation.x + angleDelta.x;
        float zRotate = helicopter.transform.localRotation.z + angleDelta.z;

        if(heliRigidbody.velocity.magnitude > Constant.MAXSPEED)
            heliRigidbody.velocity = heliRigidbody.velocity.normalized * Constant.MAXSPEED;

        heliRigidbody.transform.localRotation = heliRigidbody.transform.localRotation * angleDelta;

        Vector3 tmp = heliRigidbody.transform.localEulerAngles;

        if (tmp.x > 180) tmp.x -= 360;
        if (tmp.z > 180) tmp.z -= 360;
        tmp.x = Mathf.Clamp(tmp.x, -Constant.MAXANGLE, Constant.MAXANGLE);
        tmp.z = Mathf.Clamp(tmp.z, -Constant.MAXANGLE, Constant.MAXANGLE);
        tmp.y = 0;

        helicopter.transform.Rotate(zRotate, 0, xRotate);
        heliRigidbody.transform.localEulerAngles = tmp;

        prevAlt = currAlt;
        prevAngle = currAngle;
    }

    bool isLeverActive() {
        if (gripbutton.GetState(leftHand) && Mathf.Abs(Vector3.Distance(lCtrlobj.transform.position, lLever.transform.position)) < 1.0f)
            return true;
        else
            return false;
    }

    bool isHandleActive()
    {
        if (gripbutton.GetState(rightHand) && Mathf.Abs(Vector3.Distance(rCtrlobj.transform.position, rLever.transform.position)) < 1.0f)
            return true;
        else
            return false;
    }

    public void ChangeAltitude(float altLever)
    {
        altLever = Mathf.Clamp(altLever, 0, 1);
        //Debug.Log(altLever);
        float altVec = altLever * Mathf.Abs(Physics.gravity.y) * 2;
        float gravVec = Mathf.Abs(Physics.gravity.y);
        float airResistance = 1 - helicopter.transform.position.y/Constant.MAXALTITUDE*altLever;
        airResistance = Mathf.Clamp(airResistance, 0.4f, 1.0f);
        Vector3 heliVec = heliRigidbody.transform.up;
        heliRigidbody.AddForce(heliVec* altVec*airResistance);
        //heliRigidbody.AddForce(-heliVec* altVec*(helicopter.transform.position.y/Constant.MAXALTITUDE*altLever));
        //Debug.Log(altVec);
    }

    public void ChangeAngle(Quaternion angLever)
    {

    }

    public void ChangeDirection() { helicopter.transform.Rotate(0, stick.GetAxis(leftHand).x, 0); }
}
