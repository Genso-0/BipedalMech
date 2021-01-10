
using System.Collections.Generic;
using UnityEngine;

namespace Mech
{
    public class Mecha : MonoBehaviour
    {
        [HideInInspector]public float scaleFactor;
        [Header("How far ahead the mech will look to place feet")]
        [Range(0.1f, 10)] public float scanDistance = 5;
        public float NewPositionForFootToLerpToMagnitude = 1;
        [Header("How long the feet wait until they start transitioning to a new position")]
        public float legUpdateDistance = 4;
        [Header("How fast the legs transition from one spot to the next")]
        public float speed = 1.5f;
        public Vector3 legRaiseFromGround = new Vector3(0, 4, 0);

        [Header("How strong the legs will push off the ground")] public float legLiftStrength = 250;
        [Header("Bounciness")] [Range(0.001f, 0.9f)] public float dampener = 0.2f;
        [Header("Distance from hips to ankles")] [Range(1, 15)] public float legHeight = 15;
        [Header("Distance from ankles to floor")] [Range(0.01f, 5)] public float ankleHeight = 1;

        //Play with these at your own risk
        [HideInInspector] public float LookAtGroundBeneathMagnitude = 2;
        [HideInInspector] public float legMoveSpeed = 0.1f;
        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public Vector3 acceleration;
        [HideInInspector] public bool canMove;
        [HideInInspector] public Vector3 legForce;
        [HideInInspector] public Vector3 gravityForce;
        [HideInInspector] public bool bothLegsGrounded = true;
        [HideInInspector] public Vector3 velocityDirection;
        [HideInInspector] public Transform upperBody;
        [HideInInspector] public float upperBodyRotation = 0.0f;
        CharacterController controller;
        //Legs
        [HideInInspector] public List<LegData> legsData;
        [HideInInspector] public List<IKLimb> ik_Legs;
        public IMechLegs legInterface;
        public IK_Solver ikSolver;
        void Awake()
        {
            controller = GetComponent<CharacterController>();
            legsData = new List<LegData>();
            upperBody = transform.GetChild(0).transform;
            legInterface = new IMechLegs(this);
            ikSolver = new IK_Solver();
            scaleFactor = transform.localScale.x;
            //CalibrateHeight();
        }
        void Update()
        {
            canMove = true;
            legInterface.HandleLegs();
            Move();
            for (int i = 0; i < ik_Legs.Count; i++)
            {
                ikSolver.ResolveIK(ik_Legs[i]);
            }
        }
        public void LookAt(Transform transform, Vector3 targetPosition, float lookAtSpeed)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, lookAtSpeed * Time.deltaTime);
        }

        private void Move()
        {
            Vector3 forceUpDirection = Vector3.zero;
            acceleration = Vector3.zero;
            if (canMove)
                acceleration = velocityDirection;
            for (int i = 0; i < legsData.Count; i++)
            {
                var leg = legsData[i];
                forceUpDirection += (leg.foot.position - transform.position);
            }
            forceUpDirection.y += legHeight * scaleFactor;
            forceUpDirection *= legLiftStrength * scaleFactor;

            legForce = forceUpDirection;
            acceleration += -Vector3.up;
            acceleration += forceUpDirection;
            velocity += acceleration * Time.deltaTime;
            velocity -= velocity * dampener *scaleFactor;
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// This auto sets the legHeight.Use with caution!
        /// </summary>
        //void CalibrateHeight()
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity))
        //    {
        //        legHeight = Vector3.Distance(transform.position, hit.point);
        //    }
        //}
        //void RotateUpperBody()
        //{
        //    if (Input.GetMouseButton(1))
        //    {
        //        upperBodyRotation += rotator.rotateSpeedH * Input.GetAxis("Mouse X");
        //        var pos = new Vector3(0, upperBodyRotation, 0.0f);
        //        upperBody.transform.eulerAngles = pos;
        //    }
        //}

        //private void UserInput()
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        legMoveSpeed = speed * 2;
        //    }
        //    else
        //        legMoveSpeed = speed;
        //    if (NewInput(KeyCode.W, hips.transform.forward))
        //        return;
        //    if (NewInput(KeyCode.S, -hips.transform.forward))
        //        return;
        //    if (NewInput(KeyCode.Q, -hips.transform.right))
        //        return;
        //    if (NewInput(KeyCode.E, hips.transform.right))
        //        return;
        //}

        //private bool NewInput(KeyCode key, Vector3 vDirection)
        //{
        //    if (Input.GetKey(key))
        //    {
        //        if (previousKeyPress != key)
        //        {
        //            legInterface.FootPathingWasInterupted();
        //        }
        //        velocityDirection = vDirection;
        //        previousKeyPress = key;
        //        return true;
        //    }
        //    return false;
        //}

    }
}
