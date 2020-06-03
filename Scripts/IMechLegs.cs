﻿using Assets.Mechas.Legs;
using UnityEngine;

namespace Assets.Mechas
{
    public class IMechLegs
    {
        private Mecha parent;
        public IMechLegs(Mecha parent)
        {
            this.parent = parent;
        }

        public void HandleLegs()
        {
            for (int i = 0; i < parent.legsData.Count; i++)
            {
                var legData = parent.legsData[i];
                LookAtGroundAhead(legData);
                LookAtGroundBeneath(legData);
                MoveLegJoints(legData);
            }
        }
        /// <summary>
        /// Will move a leg from a to b if given permission.
        /// </summary>
        /// <param name="ik"></param>
        private void MoveLegJoints(LegData leg)
        {
            if (leg.moveLeg)
            {
                Vector3 newPos = MathHelper.CubicBezierLerp(leg.bezierStartPosition, leg.bezierEndPosition, leg.bezierHandle1, leg.bezierHandle2, leg.timeStep);

                RotateFoot(leg);
               
                leg.target.position = newPos;
                leg.timeStep += Time.deltaTime * parent.legMoveSpeed;

               
                if (leg.timeStep >= 1)
                {
                    NewFootPositionToLerpFrom(leg);
                    leg.moveLeg = false;
                    parent.bothLegsGrounded = true;
                    parent.velocityDirection = Vector3.zero;
                }
            }
        }
        void RotateFoot(LegData leg)
        {
            var footRotation = leg.hips.transform.rotation;
            footRotation.x = 0;
            footRotation.z = 0;
            leg.foot.rotation = Quaternion.Lerp(leg.foot.rotation,
                  Quaternion.FromToRotation(Vector3.up, leg.targetPositionRayHit.normal) * footRotation, Time.deltaTime * 5);
        }
        public void FootNeedsReseting()
        {
            for (int i = 0; i < parent.legsData.Count; i++)
            {
                var legData = parent.legsData[i];
                if (legData.moveLeg && legData.timeStep > 0)
                {
                    legData.timeStep = 1 - legData.timeStep;
                    var start = legData.bezierStartPosition;
                    var handle1 = legData.bezierHandle1;
                    var handle2 = legData.bezierHandle2;
                    var end = legData.bezierEndPosition;
                    legData.bezierStartPosition = end;
                    legData.bezierEndPosition = start;
                    legData.bezierHandle1 = handle2;
                    legData.bezierHandle2 = handle1;
                }
            }
        }
        public void FootPathingWasInterupted()
        {
            for (int i = 0; i < parent.legsData.Count; i++)
            {
                NewFootPositionToLerpFrom(parent.legsData[i]);
            }
        }
        void NewFootPositionToLerpFrom(LegData leg)
        {
            leg.timeStep = 0;
            leg.bezierStartPosition = leg.foot.position;
            leg.bezierHandle1 = leg.underFootPositionRayHit.point + parent.legRaiseFromGround;
        }
        void NewPositionForFootToLerpTo(LegData legData, Vector3 rayEndPoint)
        {
            rayEndPoint.y += parent.ankleHeight;
            if ((legData.bezierEndPosition - rayEndPoint).sqrMagnitude > parent.NewPositionForFootToLerpToMagnitude)
            {
                NewFootPositionToLerpFrom(legData);
            }
            legData.bezierEndPosition = rayEndPoint;
            legData.bezierHandle2 = rayEndPoint + parent.legRaiseFromGround;
            if (parent.bothLegsGrounded)
            {
                parent.bothLegsGrounded = false;
                legData.moveLeg = true;
            }
        }
        /// <summary>
        /// Detects ground in desired direction via a RayCast. Will stop parent move if ray fails.
        /// </summary>
        /// <param name="ik"></param>
        private void LookAtGroundAhead(LegData leg)
        {
            Vector3 rayEndPoint;
            Vector3 rayStartPoint;
            if (ShootRayFromBodyToDirection(leg, out rayStartPoint, out rayEndPoint))
            {
                if ((rayEndPoint - leg.foot.position).sqrMagnitude > parent.legUpdateDistance)
                {
                    NewPositionForFootToLerpTo(leg, rayEndPoint);
                }
                Debug.DrawLine(rayStartPoint, rayEndPoint, Color.yellow);
            }
            else
            {
                parent.canMove = false;
            }
        }
        private void LookAtGroundBeneath(LegData leg)
        {
            RaycastHit hit;
            if (Physics.Raycast(leg.foot.position, -leg.foot.up, out hit, Mathf.Infinity))
            {
                Debug.DrawLine(leg.foot.position, hit.point, Color.magenta);
                leg.underFootPositionRayHit = hit;
                var dist = (hit.point - leg.foot.position).sqrMagnitude;
                if (dist > parent.LookAtGroundBeneathMagnitude)
                {
                    leg.footGrounded = false;
                }
                else
                {

                    leg.footGrounded = true;
                }
            }
            else
                leg.footGrounded = false;
        }
        /// <summary>
        /// Shoots a ray perpendicular to the ground, towards the ground, at an offset from the main body.
        /// </summary>
        /// <param name="rayStartPoint"></param>
        /// <param name="rayEndPoint"></param>
        /// <returns></returns>
        private bool ShootRayFromBodyToDirection(LegData leg, out Vector3 rayStartPoint, out Vector3 rayEndPoint)
        {
            //Cast a ray over a few times until a max is reached or a viable spot is found.
            for (float i = 1; i > 0.1f; i -= 0.1f)
            {
                RaycastHit hit;
                var startOfRay = GetRayStartDirectionFromBody(leg, i);
                var endDirection = startOfRay;
                endDirection.y = -parent.transform.position.y;
                endDirection -= startOfRay;
                if (Physics.Raycast(startOfRay, endDirection, out hit, Mathf.Infinity))
                {
                    var angle = Vector3.Angle(hit.normal, leg.hips.transform.forward) - 90;
                    //Debug.Log(angle);
                    if (angle > -30 && angle < 30)
                    {
                        leg.targetPositionRayHit = hit;
                        rayEndPoint = hit.point;
                        rayStartPoint = startOfRay;
                        return true;
                    }
                }
            }
            rayEndPoint = Vector3.zero;
            rayStartPoint = Vector3.zero;
            return false;
        }
        /// <summary>
        /// Shoot a ray from the parent body towards the leg and offset by the speed of the body.
        /// </summary>
        /// <param name="leg"></param>
        /// <param name="detail">Affects the distance from the body</param>
        /// <returns></returns>
        private Vector3 GetRayStartDirectionFromBody(LegData leg, float detail)
        {
            Vector3 direction = Vector3.zero;
            direction += leg.hips.transform.position;
            direction += parent.velocityDirection * detail * parent.scanDistance;
            return direction;
        }
    }
}

