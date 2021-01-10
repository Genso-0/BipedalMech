using System.Collections.Generic;
using UnityEngine;

namespace Mech
{
    public class PatrolRoute : MonoBehaviour
    {
        public bool debug;
        public List<GameObject> waypoints;

        void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, 1.5f);
                Gizmos.color = Color.magenta;
                for (int i = 0; i < waypoints.Count; i++)
                {
                    Gizmos.DrawSphere(waypoints[i].transform.position, 1f);
                    if (i < waypoints.Count - 1)
                        Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
                }
            }
        }
    }
}