using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toe : MonoBehaviour
{
    public Vector3 rayAngle;
    private Transform pivot;
    Quaternion startingRotation;
    void Start()
    {
        pivot = transform.GetChild(0).transform;
        startingRotation = pivot.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward + rayAngle, out hit))
        {
            if ((transform.position - hit.point).magnitude < 2f)
            {
                float angle = Vector3.Angle(hit.normal, transform.forward) - 90;
                Vector3 newRot = Vector3.zero;
                newRot.x = -angle;
                pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(newRot), Time.deltaTime * 5);
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
            //else
            //    pivot.localRotation = startingRotation;
            
        }
        //else
        //    pivot.localRotation = startingRotation;
    }
}
