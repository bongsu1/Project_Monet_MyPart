using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BS
{
    public class Draw3DObjectOutline : MonoBehaviour
    {
        [SerializeField] Material outlineMat;
        [SerializeField] MeshRenderer mesh;
        [SerializeField] XRGrabInteractable interactable;

        private void Awake()
        {
            if (outlineMat == null)
            {
                outlineMat = Manager.Resource.Load<Material>("Shaders/SimpleOutline");
            }

            if (mesh == null)
            {
                mesh = GetComponent<MeshRenderer>();
            }

            if (interactable == null)
            {
                interactable = GetComponent<XRGrabInteractable>();
                interactable?.firstHoverEntered.AddListener((args) => DrawOutline(true));
                interactable?.lastHoverExited.AddListener((args) => DrawOutline(false));
            }

        }

        // 3D오브젝트의 메시렌더러에 메테리얼을 추가 삭제하는 방식을 아웃라인을 그리기
        public void DrawOutline(bool onHover)
        {
            if (onHover)
            {
                Material[] sharedMaterials = mesh.sharedMaterials;
                int size = sharedMaterials.Length;
                Material[] newArray = new Material[size + 1];
                Array.Copy(sharedMaterials, newArray, size);
                newArray[size] = outlineMat;
                mesh.sharedMaterials = newArray;
            }
            else
            {
                Material[] sharedMaterials = mesh.sharedMaterials;
                int size = sharedMaterials.Length;
                Material[] newArray = new Material[size - 1];
                Array.Copy(sharedMaterials, newArray, size - 1);
                mesh.sharedMaterials = newArray;
            }
        }
    }
}
