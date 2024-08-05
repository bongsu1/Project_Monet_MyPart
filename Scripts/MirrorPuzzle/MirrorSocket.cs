using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BS
{
    public class MirrorSocket : MonoBehaviour
    {
        [SerializeField] XRSocketInteractor socket;

        private Mirror mirror;

        private void Awake()
        {
            if (socket == null)
            {
                socket = GetComponent<XRSocketInteractor>();
            }

            if (socket != null)
            {
                socket.selectEntered.AddListener(SelectEnter);
                socket.selectExited.AddListener(SelectExit);
            }
        }

        public void SelectEnter(SelectEnterEventArgs args)
        {
            mirror = args.interactableObject.transform.GetComponent<Mirror>();
            if (mirror == null)
                return;

            mirror.OnSoket = true;
        }

        public void SelectExit(SelectExitEventArgs args)
        {
            if (mirror == null)
                return;

            mirror.OnSoket = false;
            mirror = null;
        }
    }
}
