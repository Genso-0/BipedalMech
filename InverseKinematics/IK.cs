//Credit goes to DitzelGames for this script. More information at https://www.youtube.com/watch?v=qqOAzn05fvk
//The original script has been modified to be a singleton. Keep track of all IK objects and handle them via one call. And less null checking so use carefully.
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Mechas.InverseKinematics
{
    public sealed class IK
    {
        private IK()
        {
        }
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly IK instance = new IK();
                
        }
        public static IK Instance { get { return Nested.instance; } }

        /// <summary>
        /// Solver iterations per update
        /// </summary>
        private readonly int iterations = 10;
        /// <summary>
        /// Distance when the solver stops
        /// </summary>
        private readonly float delta = 0.001f;
        /// <summary>
        /// Strength of going back to the start position.
        /// </summary>
        private readonly float snapBackStrength = 0.1f;
        private List<IKLimb> iks = new List<IKLimb>();
        public void Init(IKLimb ik)
        {
            //initial array
            ik.bones = new Transform[ik.chainLength + 1];
            ik.positions = new Vector3[ik.chainLength + 1];
            ik.bonesLength = new float[ik.chainLength];
            ik.startDirectionSucc = new Vector3[ik.chainLength + 1];
            ik.startRotationBone = new Quaternion[ik.chainLength + 1];
            //init target
            if (ik.target == null)
            {
                ik.target = new GameObject(ik.gameObject.name + " Target").transform;
                SetPositionRootSpace(ik.target, GetPositionRootSpace(ik.transform, ik.root), ik.root);
            }
            ik.startRotationTarget = GetRotationRootSpace(ik.target, ik.root);


            //init data
            var current = ik.transform;
            ik.completeLength = 0;
            for (var i = ik.bones.Length - 1; i >= 0; i--)
            {
                ik.bones[i] = current;
                ik.startRotationBone[i] = GetRotationRootSpace(current, ik.root);
                if (i == ik.bones.Length - 1)
                {
                    //leaf
                  ik.startDirectionSucc[i] = GetPositionRootSpace(ik.target, ik.root) - GetPositionRootSpace(current, ik.root);
                }
                else
                {
                    //mid bone
                    ik.startDirectionSucc[i] = GetPositionRootSpace(ik.bones[i + 1], ik.root) - GetPositionRootSpace(current, ik.root);
                    ik.bonesLength[i] = ik.startDirectionSucc[i].magnitude;
                    ik.completeLength += ik.bonesLength[i];
                }
                current = current.parent;
            }
            iks.Add(ik);
        }
        public void ResolveAllIK()
        {
            for (int i = 0; i < iks.Count; i++)
            {
                ResolveIK(iks[i]);
            }
        }
        private void ResolveIK(IKLimb limb)
        {
            //if (Target == null)
            //    return;

            //if (BonesLength.Length != ChainLength)
            //    Init();

            //Fabric

            //  root
            //  (bone0) (bonelen 0) (bone1) (bonelen 1) (bone2)...
            //   x--------------------x--------------------x---...

            //get position
            for (int i = 0; i < limb.bones.Length; i++)
                limb.positions[i] = GetPositionRootSpace(limb.bones[i], limb.root);

            var targetPosition = GetPositionRootSpace(limb.target, limb.root);
            var targetRotation = GetRotationRootSpace(limb.target, limb.root);


            //1st is possible to reach?
            if ((targetPosition - GetPositionRootSpace(limb.bones[0], limb.root)).sqrMagnitude >= limb.completeLength * limb.completeLength)
            {
                //just strech it
                var direction = (targetPosition - limb.positions[0]).normalized;
                //set everything after root
                for (int i = 1; i < limb.positions.Length; i++)
                    limb.positions[i] = limb.positions[i - 1] + direction * limb.bonesLength[i - 1];
            }
            else
            {
                for (int i = 0; i < limb.positions.Length - 1; i++)
                    limb.positions[i + 1] = Vector3.Lerp(limb.positions[i + 1], limb.positions[i] + limb.startDirectionSucc[i], snapBackStrength);

                for (int iteration = 0; iteration < iterations; iteration++)
                {
                    //https://www.youtube.com/watch?v=UNoX65PRehA
                    //back
                    for (int i = limb.positions.Length - 1; i > 0; i--)
                    {
                        if (i == limb.positions.Length - 1)
                            limb.positions[i] = targetPosition; //set it to target
                        else
                            limb.positions[i] = limb.positions[i + 1] + (limb.positions[i] - limb.positions[i + 1]).normalized * limb.bonesLength[i]; //set in line on distance
                    }

                    //forward
                    for (int i = 1; i < limb.positions.Length; i++)
                        limb.positions[i] = limb.positions[i - 1] + (limb.positions[i] - limb.positions[i - 1]).normalized * limb.bonesLength[i - 1];

                    //close enough?
                    if ((limb.positions[limb.positions.Length - 1] - targetPosition).sqrMagnitude < delta * delta)
                        break;
                }
            }

            //move towards pole
            //if (limb.pole != null)
            //{
            var polePosition = GetPositionRootSpace(limb.pole, limb.root);
            for (int i = 1; i < limb.positions.Length - 1; i++)
            {
                var plane = new Plane(limb.positions[i + 1] - limb.positions[i - 1], limb.positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(limb.positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - limb.positions[i - 1], projectedPole - limb.positions[i - 1], plane.normal);
                limb.positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (limb.positions[i] - limb.positions[i - 1]) + limb.positions[i - 1];
            }
            // }

            //set position & rotation
            for (int i = 0; i < limb.positions.Length; i++)
            {
                if (i == limb.positions.Length - 1)
                    SetRotationRootSpace(limb.bones[i],
                        Quaternion.Inverse(targetRotation) * limb.startRotationTarget * Quaternion.Inverse(limb.startRotationBone[i]), limb.root);
                else
                    SetRotationRootSpace(limb.bones[i],
                        Quaternion.FromToRotation(limb.startDirectionSucc[i], limb.positions[i + 1] - limb.positions[i])
                        * Quaternion.Inverse(limb.startRotationBone[i]), limb.root);
                SetPositionRootSpace(limb.bones[i], limb.positions[i], limb.root);
            }
        }
        private Vector3 GetPositionRootSpace(Transform current, Transform root)
        {
            return Quaternion.Inverse(root.rotation) * (current.position - root.position);
        }
        private void SetPositionRootSpace(Transform current, Vector3 position, Transform root)
        {
            current.position = root.rotation * position + root.position;
        }
        private Quaternion GetRotationRootSpace(Transform current, Transform root)
        {
            return Quaternion.Inverse(current.rotation) * root.rotation;
        }
        private void SetRotationRootSpace(Transform current, Quaternion rotation, Transform root)
        {
            current.rotation = root.rotation * rotation;
        }
    }
}

