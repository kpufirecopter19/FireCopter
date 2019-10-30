using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OculusTouchInput : MonoBehaviour
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

    // Start is called before the first frame update
    public float distance = 0.1f;

    private Transform tr;
    private Transform tr_lc;//왼쪽 컨트롤러

    private Transform cameraPos;
    private Vector3 currLeftctrl;
    private Vector3 currRightctrl;

    private Vector3 prevLeftctrl;
    private Vector3 prevRightctrl;

    private float currDistance;
    private float prevDistance;

    private float currDegree;
    private float prevDegree;

    public GameObject obj;

    private bool keyDown;


    void Start()
    {
        cameraPos = GetComponent<Transform>();
        currLeftctrl = pose.GetLocalPosition(leftHand);
        prevLeftctrl = currLeftctrl;
        currRightctrl = pose.GetLocalPosition(rightHand);
        prevRightctrl = currRightctrl;

        currDistance = Vector3.Distance(currLeftctrl,currRightctrl);
        prevDistance = currDistance;

        currDegree = getSlope(currLeftctrl, currRightctrl);
        prevDegree = currDegree;
        keyDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        currDistance = Vector3.Distance(pose.GetLocalPosition(leftHand), pose.GetLocalPosition(rightHand));
        currDegree = getSlope(pose.GetLocalPosition(leftHand), pose.GetLocalPosition(rightHand));

        // 카메라의 xz좌표 이동
        if(stick.GetActive(leftHand))
        {
            Vector2 pos = stick.GetAxis(leftHand);
            Debug.LogFormat("Stick Position = {0}", pos);
            cameraPos.Translate(new Vector3(pos.x, 0, pos.y) * distance * Time.deltaTime, Space.Self);
        }

        // 양쪽 트리거 클릭 시 오브젝트의 확대/축소,회전
        if (trigger.GetState(leftHand) && trigger.GetState(rightHand)) {
            float distanceDelta = currDistance / prevDistance;
            Vector3 objScale = obj.GetComponent<Transform>().localScale;
            Vector3 newObjscale = objScale * distanceDelta;
            Debug.Log(newObjscale);

            float degreeDelta = currDegree - prevDegree;
            Debug.Log(degreeDelta);

            obj.GetComponent<Transform>().localScale = newObjscale;
            obj.GetComponent<Transform>().Rotate(0, degreeDelta, 0);
        }

        prevDegree = currDegree;
        prevDistance = currDistance;
    }

    float getSlope(Vector3 c1, Vector3 c2)
    {
        float degree = Mathf.Atan2(c2.z - c1.z, c2.x - c1.x)*180/Mathf.PI;
        // Dot (내적) Cross (외적)
        return - degree;
    }


    void MoveBody(Vector2 pos)
    {

        Vector3 t = transform.position + new Vector3(pos.x, 0, pos.y);
        transform.position = Vector3.MoveTowards(transform.position, t, distance * Time.deltaTime);
    }
}