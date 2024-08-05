using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BS
{
    public class Mirror : MonoBehaviour
    {
        [Header("Mirror")]
        [SerializeField] Transform mirrorTransform;
        [SerializeField] Transform mirrorCollider;
        [SerializeField] MeshRenderer afterimage;
        [SerializeField] XRGrabInteractable interactable;

        private Transform interactorTransform;
        private ActionBasedController controllerInputAction;
        private IXRSelectInteractor curSocket;

        private bool onSoket;
        public bool OnSoket { set { onSoket = value; } }

        private void Awake()
        {
            if (interactable == null)
            {
                interactable = GetComponent<XRGrabInteractable>();
            }

            if (interactable != null)
            {
                interactable.hoverEntered.AddListener(HoverEnter);
                interactable.hoverExited.AddListener(HoverExit);
                interactable.selectEntered.AddListener(SelectEnter);
                interactable.selectExited.AddListener(SelectExit);
            }
        }

        public void HoverEnter(HoverEnterEventArgs args)
        {
            if (interactorTransform != null)
                return;

            interactorTransform = args.interactorObject.transform;
            // 인풋액션 설정을 바꾸지 않고 사용하기위해 가져옴
            controllerInputAction = interactorTransform.parent.GetComponent<ActionBasedController>();

            if (controllerInputAction == null)
            {
                interactorTransform = null;
                return;
            }

            Rotate(true);
        }

        public void HoverExit(HoverExitEventArgs args)
        {
            if (interactorTransform != args.interactorObject.transform)
                return;

            controllerInputAction = null;
            interactorTransform = null;
            Rotate(false);
        }

        public void SelectEnter(SelectEnterEventArgs args)
        {
            // 인터렉터가 소켓이면
            if (args.interactorObject.transform.gameObject.layer == 6 && followRoutine == null)
            {
                followRoutine = StartCoroutine(FollowRoutine());
                afterimage.enabled = false;
                curSocket = args.interactorObject;

                if (returnRoutine == null)
                    return;

                StopCoroutine(returnRoutine);
                returnRoutine = null;
            }
        }

        public void SelectExit(SelectExitEventArgs args)
        {
            if (args.interactorObject.transform.gameObject.layer == 6 && followRoutine != null)
            {
                StopCoroutine(followRoutine);
                followRoutine = null;
                afterimage.enabled = true;
            }
            else if (returnRoutine == null && curSocket != null)
            {
                // 원래의 소켓으로 돌아가기
                returnRoutine = StartCoroutine(ReturnRoutine(args.manager, curSocket, args.interactableObject));
            }
        }

        Coroutine returnRoutine;
        IEnumerator ReturnRoutine(XRInteractionManager manager, IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            yield return new WaitForEndOfFrame();
            manager.SelectEnter(interactor, interactable);
        }

        Coroutine followRoutine;
        // 거울의 콜라이더와 실제거울이 따로 있어서 소켓에 붙어 있을때만 콜라이더가 거울을 따라다니도록
        IEnumerator FollowRoutine()
        {
            yield return new WaitForSeconds(0.1f); // 돌아가는 루틴을 기다리기
            while (true)
            {
                yield return null;
                mirrorCollider.position = mirrorTransform.position;
                mirrorCollider.rotation = mirrorTransform.rotation;
            }
        }

        private void Rotate(bool onHover)
        {
            if (onHover && rotateRoutine == null)
            {
                rotateRoutine = StartCoroutine(RotateRoutine());
            }
            else if (!onHover && rotateRoutine != null)
            {
                StopCoroutine(rotateRoutine);
                rotateRoutine = null;
            }
        }
        Coroutine rotateRoutine;
        IEnumerator RotateRoutine()
        {
            Vector3 thisAngle = Vector3.zero;
            Vector3 interactorAngle = Vector3.zero;
            while (true)
            {
                // 소켓에 들어가 있을때만 활성화
                if (!onSoket)
                {
                    yield return null;
                    continue;
                }

                // 컨트롤러의 액티베이트 키(트리거 키)를 눌렀을때 (keyDown)
                if (controllerInputAction.activateAction.action.IsPressed() && controllerInputAction.activateAction.action.triggered)
                {
                    // 처음 눌렀을때의 거울과 컨트롤러의 회전값을 캐싱 (오일러로 변환하여 저장)
                    thisAngle = mirrorTransform.eulerAngles;
                    interactorAngle = interactorTransform.eulerAngles;
                }
                // 컨트롤러의 트리거 키를 누르고 있으면
                else if (controllerInputAction.activateAction.action.IsPressed())
                {
                    // 어떤 방향에서 잡아도 회전값을 유지한 다음에 회전 시키기 위해 캐싱을 미리 해둠
                    // 캐싱해둔 회전값을 이용해서 트리거를 누르고 있는 동안 거울을 회전시킴
                    Vector3 result = interactorTransform.eulerAngles - interactorAngle + thisAngle;
                    // 한쪽 축만 회전시키기 위해 나머지는 원래 값으로
                    result.x = mirrorTransform.eulerAngles.x;
                    if (isHorizontal)
                        result.z = mirrorTransform.eulerAngles.z;
                    else
                        result.y = mirrorTransform.eulerAngles.y;
                    mirrorTransform.rotation = Quaternion.Euler(result);
                }
                yield return null;
            }
        }

        // 세로판 가로판 설정이 달라짐
        [Header("Debug")]
        [SerializeField] bool isHorizontal;
    }
}
