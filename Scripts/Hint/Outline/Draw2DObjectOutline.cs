using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BS
{
    public class Draw2DObjectOutline : MonoBehaviour
    {
        private Material originMat;

        [SerializeField] Material outlineMat;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] XRGrabInteractable interactable;

        private void Awake()
        {
            if (outlineMat == null)
            {
                outlineMat = Manager.Resource.Load<Material>("Shaders/SpriteOutline");
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (interactable == null)
            {
                interactable = GetComponent<XRGrabInteractable>();
                interactable?.firstHoverEntered.AddListener((args) => DrawOutline(true));
                interactable?.lastHoverExited.AddListener((args) => DrawOutline(false));
            }

            if (spriteRenderer != null)
            {
                originMat = spriteRenderer.sharedMaterial;
            }
        }

        // 2D 오브젝트의 스프라이트렌더러의 메테리얼을 미리 만들어준 아웃라인 셰이더로 교체하여 아웃라인 그리기
        public void DrawOutline(bool onHover)
        {
            if (onHover)
            {
                spriteRenderer.sharedMaterial = outlineMat;
            }
            else
            {
                spriteRenderer.sharedMaterial = originMat;
            }
        }
    }
}
