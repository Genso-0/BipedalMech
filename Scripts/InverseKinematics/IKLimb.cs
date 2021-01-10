using UnityEngine;

namespace Mech
{
    public class IKLimb : MonoBehaviour
    {
        public enum LimbType
        {
            None,
            Leg,
            Arm,
        }
        /// <summary>
        /// Chain length of bones.
        /// </summary>
        public int chainLength = 2;
        /// <summary>
        /// Target the chain should bent to.
        /// </summary>
        [HideInInspector] public Transform target;
        public Transform pole;
        public LimbType type;
        [HideInInspector] public float[] bonesLength; //Target to Origin
        [HideInInspector] public float completeLength;
        [HideInInspector] public Transform[] bones;
        [HideInInspector] public Vector3[] positions;

        [HideInInspector] public Vector3[] startDirectionSucc;
        [HideInInspector] public Quaternion[] startRotationBone;
        [HideInInspector] public Quaternion startRotationTarget;
        /// <summary>
        /// Root parent.
        /// </summary>
        [HideInInspector] public Transform root;
        /// <summary>
        /// End effector transform
        /// </summary>.
        [HideInInspector] public Transform endEffector;

        void Start()
        {
            endEffector = transform.GetChild(0).transform;
            FindRoot();
            Init();
        }

        private void Init()
        {
            Mecha mecha = GetComponentInParent<Mecha>();
            if (mecha == null)
            {
                Debug.LogError("Mecha script was not found in chain. Please add!");
                return;
            }
            mecha.ikSolver.Init(this);
            mecha.ik_Legs.Add(this);
            switch (type)
            {
                case LimbType.None:
                    break;
                case LimbType.Leg:
                    var legData = new LegData(endEffector, root, target);
                    legData.bezierStartPosition = endEffector.position;
                    legData.bezierHandle1 = endEffector.position + mecha.legRaiseFromGround * mecha.scaleFactor;
                    legData.bezierEndPosition = endEffector.position;
                    legData.bezierHandle2 = endEffector.position + mecha.legRaiseFromGround * mecha.scaleFactor;
                    mecha.legsData.Add(legData);
                    break;
            }
        }
        /// <summary>
        /// Goes up the leg to find the root joint.
        /// </summary>
        void FindRoot()
        {
            root = transform;
            for (var i = 0; i <= chainLength; i++)
            {
                if (root == null)
                    throw new UnityException("The chain value is longer than the ancestor chain!");
                root = root.parent;
            }
        }

    }
}
