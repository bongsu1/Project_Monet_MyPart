using UnityEngine;

namespace Prototype
{
    public class P_Piano : P_Instrument
    {
        [SerializeField] Collider hitColl;
        public Collider HitColl { get { return hitColl; } }

        public void PushButton(bool isPush)
        {
            hitColl.enabled = isPush;
        }
    }
}
