using System.Collections;
using UnityEngine;

namespace BS
{
    public class Compass : MonoBehaviour
    {
        [SerializeField] Transform compassArrow;
        [SerializeField] Transform targetPoint;
        public Transform TargetPoint { get { return targetPoint; } set { targetPoint = value; } }

        private void OnEnable()
        {
            EnableIndicateDirection(true);
        }

        private void OnDisable()
        {
            EnableIndicateDirection(false);
        }

        private void EnableIndicateDirection(bool doActive)
        {
            if (doActive && indicateDirectionRoutine == null)
            {
                indicateDirectionRoutine = StartCoroutine(IndicateDirectionRoutine());
            }
            else if (!doActive && indicateDirectionRoutine != null)
            {
                StopCoroutine(indicateDirectionRoutine);
                indicateDirectionRoutine = null;
            }
        }

        Coroutine indicateDirectionRoutine;
        // 방향을 가르키는 루틴
        IEnumerator IndicateDirectionRoutine()
        {
            Vector3 targetDirection = Vector3.zero;
            while (true)
            {
                yield return null;
                targetDirection = targetPoint.position - transform.position;
                targetDirection.y = 0f;
                targetDirection = targetDirection.normalized;
                compassArrow.forward = targetDirection;
            }
        }
    }
}
