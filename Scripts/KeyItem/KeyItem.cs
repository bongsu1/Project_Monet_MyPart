using UnityEngine;

namespace BS
{
    public class KeyItem : MonoBehaviour
    {
        [SerializeField] int keyID;
        public int KeyID { get { return keyID; } }
    }
}
