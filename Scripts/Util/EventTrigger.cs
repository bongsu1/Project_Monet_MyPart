using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    public class EventTrigger : MonoBehaviour
    {
        public enum CheckType { Layer, Tag, Both }
        [Header("TriggerCheckType")]
        [SerializeField] CheckType checkType;
        [SerializeField] LayerMask triggerAbleLayer;
        [SerializeField] string triggerAbleTag;

        [Header("Event")]
        public UnityEvent triggerEntered;
        public UnityEvent triggerExited;

        private void OnTriggerEnter(Collider other)
        {
            switch (checkType)
            {
                case CheckType.Layer:
                    if (((1 << other.gameObject.layer) & triggerAbleLayer) != 0)
                    {
                        triggerEntered?.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.CompareTag(triggerAbleTag))
                    {
                        triggerEntered?.Invoke();
                    }
                    break;
                case CheckType.Both:
                    if (((1 << other.gameObject.layer) & triggerAbleLayer) != 0)
                    {
                        if (other.CompareTag(triggerAbleTag))
                        {
                            triggerEntered?.Invoke();
                        }
                    }
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (checkType)
            {
                case CheckType.Layer:
                    if (((1 << other.gameObject.layer) & triggerAbleLayer) != 0)
                    {
                        triggerExited?.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.CompareTag(triggerAbleTag))
                    {
                        triggerExited?.Invoke();
                    }
                    break;
                case CheckType.Both:
                    if (((1 << other.gameObject.layer) & triggerAbleLayer) != 0)
                    {
                        if (other.CompareTag(triggerAbleTag))
                        {
                            triggerExited?.Invoke();
                        }
                    }
                    break;
            }
        }
    }
}
