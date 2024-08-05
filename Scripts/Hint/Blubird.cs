using System.Collections;
using UnityEngine;

namespace BS
{
    public class Blubird : MonoBehaviour
    {
        [Header("Blue bird")]
        [SerializeField] Vector3 offset;
        [SerializeField] TrailRenderer trailRender;

        [Header("Quest")]
        [SerializeField] Transform[] targetTransforms;

        private int targetIndex;

        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }

        public void MoveToNextTarget()
        {
            ClearTrail();
            if (targetIndex >= targetTransforms.Length)
                return;

            if (toNextTargetRoutine != null)
                return;

            toNextTargetRoutine = StartCoroutine(ToNextTargetRoutine(targetTransforms[targetIndex]));
            targetIndex++;
        }

        Coroutine toNextTargetRoutine;
        IEnumerator ToNextTargetRoutine(Transform target)
        {
            float time = 0;
            float arrivalTime = Vector3.Distance(target.position, transform.position);
            while (time <= arrivalTime)
            {
                time += Time.deltaTime * 8f;
                transform.position = Vector3.MoveTowards(transform.position, target.position + offset, Time.deltaTime * 8f);
                yield return null;
            }
            toNextTargetRoutine = null;
        }

        public void ClearTrail()
        {
            trailRender.Clear();
        }
    }
}
