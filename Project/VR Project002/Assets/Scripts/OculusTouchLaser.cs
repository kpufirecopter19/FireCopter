using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class OculusTouchLaser : MonoBehaviour
{
    private SteamVR_Behaviour_Pose pose;
    private SteamVR_Input_Sources hand;
    private LineRenderer line;

    public SteamVR_Action_Boolean trigger = SteamVR_Actions.default_Trigger;

    public float maxDistance = 30.0f;

    public Color color = Color.blue;
    public Color clickedColor = Color.green;

    private RaycastHit hit;
    private Transform tr;

    private GameObject prevObject;
    private GameObject currObject;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

        pose = GetComponent<SteamVR_Behaviour_Pose>();
        hand = pose.inputSource;

        CreateLineRenderer();
    }

    void CreateLineRenderer()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.receiveShadows = false;

        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, new Vector3(0, 0, maxDistance));

        line.startWidth = 0.03f;
        line.endWidth = 0.005f;

        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = this.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(tr.position,tr.forward,out hit, maxDistance))
        {
            line.SetPosition(1, new Vector3(0, 0, hit.distance));

            currObject = hit.collider.gameObject;

            if(currObject != prevObject)
            {
                ExecuteEvents.Execute(currObject,
                    new PointerEventData(EventSystem.current),
                    ExecuteEvents.pointerEnterHandler);
                ExecuteEvents.Execute(prevObject,
                    new PointerEventData(EventSystem.current),
                    ExecuteEvents.pointerExitHandler);
                prevObject = currObject;
            }
            if (trigger.GetStateDown(hand))
            {
                ExecuteEvents.Execute(currObject,
                    new PointerEventData(EventSystem.current),
                    ExecuteEvents.pointerClickHandler);

            }
        }
        else
        {
            if(prevObject != null)
            {
                ExecuteEvents.Execute(prevObject,
                    new PointerEventData(EventSystem.current),
                    ExecuteEvents.pointerExitHandler);
                prevObject = null;
            }
        }
    }
}
