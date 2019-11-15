using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controler : MonoBehaviour
{
    public GameObject body;
    public float speed = 20;
    public float power =1;
    public float AngleRimit = 10;
    public float torq = 1;
    public float WingTorq = 1;


    Rigidbody bodyRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        bodyRigidbody = body.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Jump")>0)
        {
            Up(power);
        }
        
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
        rotateDir(hor, ver);
        float mh = Input.GetAxis("Mouse X");
        turn(mh);


        //속도 제한
        if (bodyRigidbody.velocity.magnitude > speed)
            bodyRigidbody.velocity = bodyRigidbody.velocity.normalized * speed;

        //회전 제한
        Vector3 tmp = bodyRigidbody.transform.eulerAngles ;
        if (tmp.x > 180) tmp.x -= 360;
        if (tmp.z > 180) tmp.z -= 360;
        tmp.x = Mathf.Clamp(tmp.x, -AngleRimit, AngleRimit);
        tmp.z = Mathf.Clamp(tmp.z, -AngleRimit, AngleRimit);
        bodyRigidbody.transform.eulerAngles = tmp;
        Debug.Log(tmp);
    }

    void Up(float k)
    {
        Vector3 v = bodyRigidbody.transform.up;
        bodyRigidbody.AddForce(v*k);
    }
    void rotateDir(float hor, float ver)
    {
        Vector3 bodyRight = bodyRigidbody.transform.right;
        Vector3 bodyback = -bodyRigidbody.transform.forward;
        bodyRigidbody.AddTorque(bodyback * hor * torq);
        bodyRigidbody.AddTorque(bodyRight * ver * torq);
    }
    void turn(float m)
    {
        //Vector3 v = bodyRigidbody.transform.up;
        bodyRigidbody.AddTorque(Vector3.up * m * WingTorq);
    }
}
