using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    public class RayShooter : MonoBehaviour
    {
        [Header("Ray Shooter")]
        [SerializeField] LineRenderer line;
        [SerializeField] Transform muzzlePoint;
        [SerializeField] float rayDistance;
        [SerializeField] int reflectCount;
        [Header("Layer Mask")]
        [SerializeField] LayerMask reactionLayer;
        [SerializeField] LayerMask wallLayer;
        [Header("Complete")]
        [SerializeField] Material completeMat;
        public UnityEvent onClear;

        [Header("Debug")] // test
        [SerializeField] bool doShotRay;

        private void FixedUpdate()
        {
            ShootRay();
        }

        public void PuzzleEnable(bool isActive)
        {
            doShotRay = isActive;
            line.enabled = isActive;
        }

        private void ShootRay()
        {
            if (!doShotRay)
                return;

            // 처음 라인렌더러의 포인트를 두개로 설정해서 일직선만 그린다, 0번째 점은 레이져를 쏘는곳으로 지정
            line.positionCount = 2;
            line.SetPosition(0, muzzlePoint.position);

            if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out RaycastHit hitInfo, rayDistance, reactionLayer))
            {
                line.SetPosition(1, hitInfo.point);

                // 벽에 맞으면 레이져가 반사를 안함
                // 도착지점에 레이져가 맞으면 퍼즐 종료
                if (((1 << hitInfo.collider.gameObject.layer) & wallLayer) != 0)
                {
                    if (hitInfo.collider.CompareTag("EndPoint"))
                    {
                        MeshRenderer mesh = hitInfo.transform.gameObject.GetComponent<MeshRenderer>();
                        mesh.sharedMaterial = completeMat;

                        if (completeRoutine == null)
                            completeRoutine = StartCoroutine(CompleteRoutine());
                    }
                    return;
                }

                // 다음 시작지점을 레이가 맞은곳, 다음 방향은 물체의 표면에 대하여 정반사 방향
                // 레이의 남은 거리는 최대 거리에서 이동한 거리만큼 감소
                Vector3 nextPoint = hitInfo.point;
                Vector3 nextDir = Vector3.Reflect(muzzlePoint.forward, hitInfo.normal);
                float amountDistance = rayDistance - hitInfo.distance;

                // 반사될 수 있는 숫자만큼 반복하여 레이를 반사시킴, 다음 라인렌더러 포인트는 2번째부터 시작
                for (int i = 2; i <= reflectCount + 1; i++)
                {
                    // 포인트갯수를 증가
                    line.positionCount++;

                    if (Physics.Raycast(nextPoint, nextDir, out RaycastHit hit, amountDistance, reactionLayer))
                    {
                        line.SetPosition(i, hit.point);

                        // 벽에 맞으면 더이상 반사하지 않고 종료
                        // 도착지점에 걸리면 종료
                        if (((1 << hit.collider.gameObject.layer) & wallLayer) != 0)
                        {
                            if (hit.collider.CompareTag("EndPoint"))
                            {
                                MeshRenderer mesh = hit.transform.gameObject.GetComponent<MeshRenderer>();
                                mesh.sharedMaterial = completeMat;

                                if (completeRoutine == null)
                                    completeRoutine = StartCoroutine(CompleteRoutine());
                            }
                            return;
                        }

                        // 다음 시작지점을 레이가 맞은곳, 다음 방향은 물체의 표면에 대하여 정반사 방향
                        // 레이의 남은 거리는 최대 거리에서 이동한 거리만큼 감소
                        amountDistance = amountDistance - hit.distance;
                        nextPoint = hit.point;
                        nextDir = Vector3.Reflect(nextDir, hit.normal);
                    }
                    // 더이상 레이캐스트에 아무것도 걸리지 않으면 남은거리만큼의 지점에 마지막점으로 설정
                    else
                    {
                        line.SetPosition(i, nextPoint + nextDir * amountDistance);
                        break;
                    }
                }
            }
            // 레이캐스트에 아무것도 맞지 않으면 레이의 최대거리 지점에 1번째 점으로 설정하여 일직선만 그림
            else
            {
                line.SetPosition(1, muzzlePoint.position + muzzlePoint.forward * rayDistance);
            }
        }

        Coroutine completeRoutine;
        IEnumerator CompleteRoutine()
        {
            yield return new WaitForSeconds(2f);
            PuzzleEnable(false);
            onClear?.Invoke();
            completeRoutine = null;
        }
    }
}
