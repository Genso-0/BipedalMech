
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Mechas
{
    public class MechaAnimator : MonoBehaviour
    {
        Hips hips;
        Mecha mecha;
        public Transform hipsTarget;
        public Transform headTarget;
        public MoveType moveType;
        public List<GameObject> waypoints;
        public int waypointIndex;
        public float distanceBeforeMovingTowardsTarget = 30;
        public float distanceBeforeMovingAwayFromTarget = 15;
        public float lookAtSpeed = 1f;
        public bool debug;

        public enum MoveType
        {
            None,
            FollowTarget,
            FollowWaypoints,
        }
        void Start()
        {
            hips = GetComponentInChildren<Hips>();
            mecha = GetComponent<Mecha>();
            mecha.controledByAnimator = true;
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
                    break;
                case MoveType.FollowWaypoints:
                    MoveAlongWaypoints();
                    break;
            }
        }

        private void MoveAlongWaypoints()
        {
            var position = waypoints[waypointIndex].transform.position;
             
            LookAt(hips.transform, position);
            LookAt(mecha.upperBody.transform, headTarget != null ? headTarget.transform.position : position);
            mecha.velocityDirection = hips.transform.forward;
            if ((transform.position - position).magnitude < 10)
                waypointIndex++;
            if (waypointIndex >= waypoints.Count)
                waypointIndex = 0;
        }

        void MoveWithTarget()
        {
            LookAt(hips.transform, hipsTarget.position);
            LookAt(mecha.upperBody.transform, headTarget.position);

            if ((transform.position - hipsTarget.transform.position).magnitude > distanceBeforeMovingTowardsTarget)
                mecha.velocityDirection = hips.transform.forward;
            else if ((transform.position - hipsTarget.transform.position).magnitude < distanceBeforeMovingAwayFromTarget)
                mecha.velocityDirection = -hips.transform.forward;
        }
        void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < waypoints.Count; i++)
                {
                    Gizmos.DrawSphere(waypoints[i].transform.position, 1f);
                    if (i < waypoints.Count - 1)
                        Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
                }
            }
        }
     
        void LookAt(Transform transform, Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, lookAtSpeed * Time.deltaTime);
        }
    }
}