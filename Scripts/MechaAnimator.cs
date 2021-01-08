
using UnityEngine;
namespace Assets.Mechas
{
    public class MechaAnimator : MonoBehaviour
    {
        Hips hips;
        Mecha mecha;
        public Transform target;
        void Start()
        {
            hips = GetComponentInChildren<Hips>();
            mecha = GetComponent<Mecha>();
            mecha.controledByAnimator = true;
        }
        void Update()
        {
            if (target != null)
            {
                Move();
                RotateUpperBody();
                hips.transform.LookAt(target);
            }
        }

        private void RotateUpperBody()
        {
            mecha.upperBody.transform.LookAt(target);
        }

        private void Move()
        {
            if ((transform.position - target.transform.position).magnitude > 50f)
                mecha.velocityDirection = hips.transform.forward;
            else if ((transform.position - target.transform.position).magnitude < 25f)
                mecha.velocityDirection = -hips.transform.forward;
        }
    }
}