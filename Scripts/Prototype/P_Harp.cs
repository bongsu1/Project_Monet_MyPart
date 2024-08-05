using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Prototype
{
    public class P_Harp : P_Instrument
    {
        [Header("Line")]
        [SerializeField] Transform returnPoint;
        [SerializeField] Transform[] lineLinkPoint; // 줄이 연결될 부분
        [SerializeField] LineRenderer lineReder;
        [Header("Rhythm")]
        [SerializeField] float returnTime;

        private XRGrabInteractable grabInteractable;

        private bool onBounce;
        public bool OnBounce { get { return onBounce; } }

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();

            if (grabInteractable != null)
            {
                grabInteractable.firstSelectEntered.AddListener((args) => OnSelect(true));
                grabInteractable.lastSelectExited.AddListener((args) => OnSelect(false));
            }

            lineReder.SetPosition(0, lineLinkPoint[0].position);
            lineReder.SetPosition(2, lineLinkPoint[1].position);
        }

        private void OnEnable()
        {
            StartCoroutine(LinePositionUpdateRoutine());
        }

        private void OnSelect(bool onSelect)
        {
            onBounce = !onSelect;
            if (onSelect && returnRoutine != null)
            {
                StopCoroutine(returnRoutine);
                returnRoutine = null;
            }
            else if (!onSelect && returnRoutine == null)
            {
                returnRoutine = StartCoroutine(ReturnRoutine());
            }
        }

        Coroutine returnRoutine;
        IEnumerator ReturnRoutine()
        {
            float rate = 0;

            // 튕기는 효과를 주기 위한 다음 포인트
            Vector3 nextPoint = transform.position + (returnPoint.position - transform.position) * 1.4f;
            while (rate <= 1)
            {
                rate += Time.deltaTime / returnTime;
                transform.position = Vector3.Lerp(transform.position, nextPoint, rate);
                yield return null;
            }

            rate = 0;
            while (rate <= 1)
            {
                rate += Time.deltaTime / returnTime;
                transform.position = Vector3.Lerp(transform.position, returnPoint.position, rate);
                yield return null;
            }
            returnRoutine = null;
            onBounce = false;
        }

        IEnumerator LinePositionUpdateRoutine()
        {
            while (true)
            {
                yield return null;
                lineReder.SetPosition(1, transform.position);
            }
        }
    }
}
