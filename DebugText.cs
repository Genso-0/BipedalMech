using Assets.Mechas;
using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    Mecha parent;
    Hips hips;
    public TextMeshProUGUI t;
    void Start()
    {
        parent = GetComponent<Mecha>();
        hips = parent.GetComponentInChildren<Hips>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // t.text =
            //$"Velocity: {parent.velocity}\n" +
            //$"Acceleration: {parent.acceleration}\n" +
            //$"Leg force: {parent.legForce}\n" +
            //$"Old Angle: {hips.transform.rotation.eulerAngles}\n" +
           // $"hips.transform.rotation: {hips.transform.rotation}\n" +
           // $"hips.transform.localRotation: {hips.transform.localRotation}\n" +
         //   $"vectorFromLeftLeg: {hips.leftLegVector} & magnitude {hips.leftLegMag}\n" +
          //  $"vectorFromRightLeg: {hips.rightLegVector} & magnitude {hips.rightLegMag}\n" +
          //  $"vectorDifferenceBetweenBothSides: {hips.vectorDifferenceBetweenBothSides}\n"+
          //  $"vectorDifferenceBetweenBothSidesManitude: {hips.vectorDifferenceBetweenBothSidesManitude}\n"+
           // $"initialRotation: {hips.initialRotation}\n"+
            //$"left: {hips.magLeft}\n"+
            //$"right: {hips.magRight}\n"+
    //    $"dif: {hips.magDif}\n" +
          //  $"difZSin: {hips.z}\n" +
           // $"difYCos: {hips.y}\n"
            ;
    }
}
