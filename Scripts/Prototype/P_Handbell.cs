using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Prototype
{
    public class P_Handbell : P_Instrument
    {
        [Header("Handbell")]
        [SerializeField] Transform returnPoint;
        [SerializeField] float returnTime;

        [SerializeField] Rigidbody rigid;
        public Rigidbody Rigid { get { return rigid; } }

        [SerializeField] XRGrabInteractable grabInteractable;

        private bool isSelect; // 잡혀있는지 확인
        public bool IsSelect { get { return isSelect; } }

        private void Awake()
        {
            if (rigid == null)
                rigid = GetComponent<Rigidbody>();

            if (grabInteractable == null)
                grabInteractable = GetComponent<XRGrabInteractable>();

            if (grabInteractable != null)
            {
                grabInteractable.firstSelectEntered.AddListener((args) => OnSelect(true));
                grabInteractable.lastSelectExited.AddListener((args) => OnSelect(false));
            }
        }

        public void OnSelect(bool onSelect)
        {
            isSelect = onSelect;
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
            while (rate <= 1)
            {
                rate += Time.deltaTime / returnTime;
                transform.position = Vector3.Lerp(transform.position, returnPoint.position, rate);
                yield return null;
            }
            transform.rotation = returnPoint.rotation;
            returnRoutine = null;
        }
    }
}
