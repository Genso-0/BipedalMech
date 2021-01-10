using UnityEngine;

namespace Assets.Mechas.Legs
{
    public class LegData
    {
        public LegData(Transform anklePosition, Transform legRoot, Transform target)
        {
            this.foot = anklePosition;
            this.legRoot = legRoot;
            this.target = target;
        }
        /// <summary>
        ///Lets the leg move if true.
        /// </summary>
        public bool moveLeg;
        public bool footGrounded = true;
        public RaycastHit targetPositionRayHit;
        public RaycastHit underFootPositionRayHit;
        public float timeStep;
        public Vector3 bezierStartPosition;
        public Vector3 bezierHandle1;
        public Vector3 bezierHandle2;
        public Vector3 bezierEndPosition;
        public Transform foot;
        /// <summary>
        /// The target used to move the foot towards.
        /// </summary>
        public Transform target;
        /// <summary>
        /// The top of the leg.
        /// </summary>
        public Transform legRoot;
    }
}
