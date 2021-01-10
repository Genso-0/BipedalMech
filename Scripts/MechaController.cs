
using Mech.Utilities;
using UnityEngine;
namespace Mech
{
    [RequireComponent(typeof(Mecha))]
    public class MechaController : MonoBehaviour
    {
        Hips hips;
        Mecha mech;
        public Transform hipsTarget;
        public Transform headTarget;
        public MoveType moveType;
        public PatrolRoute patrolRoute;
        public int waypointIndex;
        public float distanceBeforeMovingTowardsTarget = 30;
        public float distanceBeforeMovingAwayFromTarget = 15;
        public float distanceBeforeChangingWaypointTarget = 10;
        public float lookAtSpeed = 1f;
        public float rotationSpeed_Yaw = 2.0f;
        private KeyCode previousKeyPress;
        public enum MoveType
        {
            None,
            FollowTarget,
            FollowPatrolRoute,
            PlayerControlled,
        }
        void Start()
        {
            hips = GetComponentInChildren<Hips>();
            mech = GetComponent<Mecha>();
        }
        void Update()
        {
            Move();
        }
        private void Move()
        {
            switch (moveType)
            {
                case MoveType.FollowTarget:
                    if (hipsTarget != null)
                        MoveWithTarget();
                    else
                    {
                        Debug.Log($"{name} :This move type requires a target! Setting move type to none.");
                        moveType = MoveType.None;
                    }
                    break;
                case MoveType.FollowPatrolRoute:
                    if (patrolRoute != null)
                        MoveAlongWaypoints();
                    else
                    {
                        Debug.Log($"{name} :This move type requires a patrol route! Setting move type to none.");
                        moveType = MoveType.None;
                    }
                    break;
                case MoveType.PlayerControlled:
                    MoveViaPlayer();
                    break;
            }
        }
        #region follow route
        private void MoveAlongWaypoints()
        {
            var position = patrolRoute.waypoints[waypointIndex].transform.position;

            mech.LookAt(hips.transform, position, lookAtSpeed);
            mech.LookAt(mech.upperBody.transform, headTarget != null ? headTarget.transform.position : position, lookAtSpeed);
            mech.velocityDirection = hips.transform.forward;
            if ((transform.position - position).magnitude < distanceBeforeChangingWaypointTarget * mech.scaleFactor)
                waypointIndex++;
            if (waypointIndex >= patrolRoute.waypoints.Count)
                waypointIndex = 0;
        }
        #endregion
        #region follow target
        void MoveWithTarget()
        {
            mech.LookAt(hips.transform, hipsTarget.position, lookAtSpeed);
            mech.LookAt(mech.upperBody.transform, headTarget != null ? headTarget.transform.position : hipsTarget.position, lookAtSpeed);

            if ((transform.position - hipsTarget.transform.position).magnitude > distanceBeforeMovingTowardsTarget * mech.scaleFactor)
                mech.velocityDirection = hips.transform.forward;
            else if ((transform.position - hipsTarget.transform.position).magnitude < distanceBeforeMovingAwayFromTarget * mech.scaleFactor)
                mech.velocityDirection = -hips.transform.forward;
        }
        #endregion
        #region PlayerControlled
        private void MoveViaPlayer()
        {
            hips.UpdateHips();
            RotateUpperBody();
            UserInput();
        }
        void RotateUpperBody()
        {
            if (Input.GetMouseButton(1))
            {
                mech.upperBodyRotation += rotationSpeed_Yaw * Input.GetAxis("Mouse X");
                var pos = new Vector3(0, mech.upperBodyRotation, 0.0f);
                mech.upperBody.transform.eulerAngles = pos;
            }
        }
        private void UserInput()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                mech.legMoveSpeed = mech.speed * 2 * mech.scaleFactor;
            }
            else
                mech.legMoveSpeed = mech.speed * mech.scaleFactor;
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
                    mech.legInterface.FootPathingWasInterupted();
                }
                mech.velocityDirection = vDirection;
                previousKeyPress = key;
                return true;
            }
            return false;
        }
        #endregion

    }
}