using TMPro;
using UnityEngine;

namespace BS
{
    public class BubbleUI : BillboardUI
    {
        [SerializeField] TMP_Text npcWord;

        protected override void Awake()
        {
            base.Awake();

            if (npcWord == null)
            {
                npcWord = GetUI<TMP_Text>("Word Text");
            }
        }

        // 메시지 변경
        public void SetMessage(string message)
        {
            npcWord.text = message;
        }

        // 비활성화되면 text를 비우기
        private void OnDisable()
        {
            npcWord.text = "";
        }
    }
}
