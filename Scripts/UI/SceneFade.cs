using UnityEngine;

namespace BS
{
    public class SceneFade : MonoBehaviour
    {
        [SerializeField] float offset;

        private void LateUpdate()
        {
            transform.position = Camera.main.transform.forward * offset + Camera.main.transform.position;

            transform.forward = Camera.main.transform.forward;
        }
    }
}
