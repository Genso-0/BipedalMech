using Assets.Mechas;
using UnityEngine;

public class Hips : MonoBehaviour
{
    [Header("How much the hips will move when walking")]
    public float Zmagnitude = 5;
    public float Ymagnitude = 5;
    private Mecha mecha;

    public float rotateHipsOnAxis;
    public enum HipsType
    {
        ForwardRotation,
        InverseRotation,
    };
    [Header("Which way the hips will rotate with relation to the feet")]
    public HipsType type = HipsType.ForwardRotation;
    void Start()
    {
        mecha = GetComponentInParent<Mecha>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mecha.controledByAnimator)
            UpdateHips();
    }
    public float bodyRotationSpeed = 0.1f;
    float InputX;
    private void UpdateHips()
    {
        InputX += Input.GetAxis("Horizontal") * Time.deltaTime * bodyRotationSpeed;
        var leftLeg = Quaternion.Euler(-transform.rotation.eulerAngles) * (mecha.legsData[0].foot.transform.position - transform.position) + transform.position;
        var rightLeg = Quaternion.Euler(-transform.rotation.eulerAngles) * (mecha.legsData[1].foot.transform.position - transform.position) + transform.position;
        var leftLegVector =
            (transform.position - leftLeg);
        var rightLegVector =
           (transform.position - rightLeg);
        var vectorDifferenceBetweenBothSides = rightLegVector - leftLegVector;

        switch (type)
        {
            case HipsType.ForwardRotation:
                transform.rotation = Quaternion.Euler(0, InputX - vectorDifferenceBetweenBothSides.z * Ymagnitude, vectorDifferenceBetweenBothSides.y * Zmagnitude);
                break;
            case HipsType.InverseRotation:
                transform.rotation = Quaternion.Euler(0, InputX + vectorDifferenceBetweenBothSides.z * Ymagnitude, vectorDifferenceBetweenBothSides.y * Zmagnitude);
                break;
        }
    }
}
