using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lever : MonoBehaviour
{
    [Range(0.1f,2)] public float sensitivityH = 1;
    [Range(0.1f,2)] public float sensitivityL = 1;
    public Transform heliBody;
    public Transform handleControlor;
    public Transform leverControlor;

    private Vector3 leverOldPos;

    // Start is called before the first frame update
    void Start()
    {
        leverOldPos = leverControlor.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //조종간
        Vector3 tmp = heliBody.localEulerAngles;
        if (tmp.x > 180) tmp.x -= 360;
        if (tmp.z > 180) tmp.z -= 360;
        tmp.y = 0;

        handleControlor.transform.localRotation = Quaternion.Euler(tmp*sensitivityH);

        //레버
        float curV = leverControlor.localPosition.y - leverOldPos.y;
        curV /= sensitivityL;
        curV = Mathf.Clamp01(curV);

        Debug.Log(curV);//여기다가 상승 함수 호출
        heliBody.GetComponent<OculusTouchInput_Helicopter>().ChangeAltitude(curV);
   
    }
}
