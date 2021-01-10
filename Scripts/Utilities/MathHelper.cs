
using UnityEngine;

namespace Mech
{
    public static class MathHelper
    {
        /// <summary>
        /// A curve to interpolate from to.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startHandle"></param>
        /// <param name="endHandle"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Vector3 CubicBezierLerp(Vector3 start, Vector3 end, Vector3 startHandle, Vector3 endHandle, float time)
        {
            Vector3 a = start;
            Vector3 b = startHandle;
            Vector3 c = endHandle;
            Vector3 d = end;
            Vector3 lerpAB = a + (b - a) * time;
            Vector3 lerpBC = b + (c - b) * time;
            Vector3 lerpABC = lerpAB + (lerpBC - lerpAB) * time;

            Vector3 lerpCD = c + (d - c) * time;
            Vector3 lerpBCD = lerpBC + (lerpCD - lerpBC) * time;
            return lerpABC + (lerpBCD - lerpABC) * time;
        }
    }
}
