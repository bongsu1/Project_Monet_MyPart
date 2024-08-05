using UnityEngine;

namespace BS
{
    public class OutlineRender : MonoBehaviour
    {
        private enum OutlineType { None, Target, Conversation }

        [SerializeField] Renderer render;
        [SerializeField] Shader outlineShader;
        [SerializeField] Color targetColor = Color.white, ConversationColor = Color.green;

        private Shader originShader;

        private void Start()
        {
            originShader = render.material.shader;
        }

        // UnityEvent에서 enum을 매개변수로 사용하는 함수를 사용하기 편하게 해주는 라이브러리 사용
        [VisibleEnum(typeof(OutlineType))]
        public void ShowOutline(int type)
        {
            switch ((OutlineType)type)
            {
                case OutlineType.None:
                    render.material.shader = originShader;
                    break;
                case OutlineType.Target:
                    render.material.shader = outlineShader;
                    render.material.SetColor("_OuterOutlineColor", targetColor);
                    break;
                case OutlineType.Conversation:
                    render.material.shader = outlineShader;
                    render.material.SetColor("_OuterOutlineColor", ConversationColor);
                    break;
            }
        }

        // test
        [SerializeField] OutlineType type;
        [ContextMenu("Show Outline")]
        private void Test()
        {
            ShowOutline((int)type);
        }
    }
}
