using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObj : MonoBehaviour
{
    public float smooth = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
  

    }
    void MoveTo(float x, float y, float z)
    {
        Vector3 t = transform.position+ new Vector3(x,y,z);
        transform.position = Vector3.MoveTowards(transform.position, t, smooth * Time.deltaTime);
    }
}
