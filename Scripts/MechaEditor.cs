using Assets.Mechas.InverseKinematics;
using UnityEditor;
using UnityEngine;

namespace Assets.Mechas
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Mecha))]
    class MechaEditor : Editor
    {

        void OnSceneGUI()
        {
            Mecha m = (Mecha)target;
            //Handles.color = Handles.zAxisColor;
            //Handles.ArrowHandleCap(0, m.transform.position, Quaternion.LookRotation(m.transform.forward, m.arrowAngle), arrowSize, EventType.Repaint);
            WhileRunning(m);

        }

        private void WhileRunning(Mecha m)
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < m.legsData.Count; i++)
                {
                    var leg = m.legsData[i];
                    DrawCurve(leg.bezierStartPosition,
                        leg.bezierEndPosition,
                       leg.bezierHandle1,
                        leg.bezierHandle2);
                }
           
            }
        }

        void DrawCurve(Vector3 start, Vector3 end, Vector3 handle1, Vector3 handle2)
        {
            Handles.DrawBezier(start, end, handle1, handle2, Color.magenta, null, 20);
        }
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmoForLimbs(IKLimb limb, GizmoType gizmoType)
        {
            DrawLimb(limb);
        }
        //[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        //static void DrawGizmoForMecha(Mecha mecha, GizmoType gizmoType)
        //{
        //    for (int i = 0; i < mecha.legsData.Count; i++)
        //    {
        //        var limb = mecha.legsData[i];
        //        DrawLimb(limb);
        //    }
        //    //for (int i = 0; i < mecha.armsIK.Count; i++)
        //    //{
        //    //    var limb = mecha.armsIK[i];
        //    //    DrawLimb(limb);
        //    //}
        //}
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawForces(Mecha mecha, GizmoType gizmoType)
        {
            Debug.DrawRay(mecha.transform.position, mecha.velocity);
            Debug.DrawRay(mecha.transform.position, mecha.legForce, Color.magenta);
            Debug.DrawRay(mecha.transform.position, mecha.gravityForce, Color.yellow);
            if(Application.isPlaying)
            {
                var p0 = mecha.legsData[0].bezierStartPosition;
                var p1 = mecha.legsData[0].bezierHandle1;
                var p2 = mecha.legsData[0].bezierHandle2;
                var p3 = mecha.legsData[0].bezierEndPosition;

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(p1, 0.1f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(p2, 0.1f);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(mecha.legsData[0].target.position, 0.1f);
            }
          
        }
        static void DrawLimb(IKLimb limb)
        {
            var current = limb.transform;
            for (int y = 0;   current != null && current.parent != null; y++)//limb.chainLength &&
            {
                var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
                Handles.matrix = Matrix4x4.TRS(current.position,
                    Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                    new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
                Handles.color = Color.green;
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(current.position, 1f);
                current = current.parent;
            }
           // Gizmos.DrawWireSphere(limb.pole.position, 1f);
        }
    }
#endif
}
