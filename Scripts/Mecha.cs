using Assets.Mechas.InverseKinematics;
using Assets.Mechas.Legs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Mechas
{
    public class Mecha : MonoBehaviour
    {
        [Header("How far ahead the mech will look to place feet")]
        [Range(1, 10)] public sbyte scanDistance = 1;
        public float NewPositionForFootToLerpToMagnitude = 1;
        [Header("How long the feet wait until they start transitioning to a new position")]
        public float legUpdateDistance;
        [Header("How fast the legs transition from one spot to the next")]
        public float speed = 0.1f;
        public Vector3 legRaiseFromGround;

        [Header("How strong the legs will push off the ground")] public float legLiftStrength = 150;
        public float gravity = 300f;
        [Header("Bounciness")] [Range(0.001f, 0.9f)] public float dampener;
        [Header("Distance from hips to ankles")] [Range(8, 15)] public float legHeight;
        [Header("Distance from ankles to floor")] [Range(0.01f, 5)] public float ankleHeight;

        //Play with these at your own risk
        [HideInInspector] public float MoveLegJointsTargetToEndEffectorMagnitude = 2;

        [HideInInspector] public float LookAtGroundBeneathMagnitude = 2;
        [HideInInspector] public float legMoveSpeed = 0.1f;
        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public Vector3 acceleration;
        [HideInInspector] public bool canMove;
        [HideInInspector] public Vector3 legForce;
        [HideInInspector] public Vector3 gravityForce;

        [HideInInspector] public bool bothLegsGrounded = true;
        [HideInInspector] public Vector3 velocityDirection;
        [HideInInspector] CameraRotator rotator;
        [HideInInspector] public Transform upperBody;
        [HideInInspector] Hips hips;
        float upperBodyRotation = 0.0f;
        CharacterController controller;
        [HideInInspector] public IK iKinematics;
        //Legs
        //[HideInInspector] public List<IKLimb> legsIK;
        [HideInInspector] public List<LegData> legsData;
        public IMechLegs legInterface;

        private KeyCode previousKeyPress;
        void Awake()
        {
            iKinematics = IK.Instance;
            controller = GetComponent<CharacterController>();
            hips = GetComponentInChildren<Hips>();
            legsData = new List<LegData>();
            rotator = FindObjectOfType<CameraRotator>();
            upperBody = transform.GetChild(0).transform;
            legInterface = new IMechLegs(this);
            //CalibrateHeight();
        }
        void Update()
        {
            canMove = true;
            legInterface.HandleLegs();
            RotateUpperBody();
            UserInput();
            CalculateVelocityV2();
            iKinematics.ResolveAllIK();
        }
      
        private void CalculateVelocityV2()
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
            forceUpDirection.y += legHeight;
            forceUpDirection *= legLiftStrength;

            legForce = forceUpDirection;
            acceleration += -Vector3.up;
            acceleration += forceUpDirection;
            velocity += acceleration * Time.deltaTime;
            velocity -= velocity * dampener;
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// This auto sets the legHeight.Use with caution!
        /// </summary>
        void CalibrateHeight()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity))
            {
                legHeight = Vector3.Distance(transform.position, hit.point);
            }
        }
        void RotateUpperBody()
        {
            if (Input.GetMouseButton(1))
            {
                upperBodyRotation += rotator.rotateSpeedH * Input.GetAxis("Mouse X");
                var pos = new Vector3(0, upperBodyRotation, 0.0f);
                upperBody.transform.eulerAngles = pos;
            }
        }

        private void UserInput()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                legMoveSpeed = speed * 2;
            }
            else
                legMoveSpeed = speed;
            if (NewInput(KeyCode.W, hips.transform.forward))
                return;
            if (NewInput(KeyCode.S, -hips.transform.forward))
                return;
            if (NewInput(KeyCode.Q, -hips.transform.right))
                return;
            if (NewInput(KeyCode.E, hips.transform.right))
                return;
        }

        private bool NewInput(KeyCode key, Vector3 vDirection)
        {
            if (Input.GetKey(key))
            {
                if (previousKeyPress != key)
                {
                    legInterface.FootPathingWasInterupted();
                }
                velocityDirection = vDirection;
                previousKeyPress = key;
                return true;
            }
            return false;
        }
    }
}
