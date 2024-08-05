using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BS
{
    public enum SelectButtonType { Yes, No, Quest }

    public class SubtitleUI : BaseUI
    {
        [SerializeField] TMP_Text wordText;
        [SerializeField] Button[] buttons;

        [Header("Subtitle Debug")]
        //[SerializeField] int wordIndex;
        [SerializeField] bool isClicked;
        public bool IsClicked { get { return isClicked; } }

        protected override void Awake()
        {
            base.Awake();

            if (wordText == null)
            {
                // 자막으로 사용할 text만 가져옴
                wordText = GetUI("Background").GetComponentInChildren<TMP_Text>(true);
            }

            if (buttons.Length <= 0)
            {
                buttons = GetComponentsInChildren<Button>(true);
            }
        }

        /// <summary>
        /// doActive 버튼 활성화 여부. connectPairList 버튼을 활성화 할때 참고할 리스트, 버튼을 활성화 할때 꼭 넣어주세요.
        /// setSelectConnect 선택지를 변경시켜주는 메서드, 이것도 버튼을 활성화 할때 꼭 넣어주세요.
        /// </summary>
        /// <param name="doActive"></param>
        /// <param name="connectPairList"></param>
        /// <param name="setSelectConnect"></param>
        public void SetActiveButton(bool doActive, List<ConnectPair> connectPairList = null, UnityAction<int> setSelectConnect = null)
        {
            if (doActive)
            {
                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);

                if (connectPairList == null || setSelectConnect == null)
                    return;

                for (int i = 0; i < connectPairList.Count; i++)
                {
                    int curIndex = i;
                    buttons[i].gameObject.SetActive(true);

                    // 매개변수가 없는 이벤트에 매개변수가 필요한 메서드를 람다식으로 추가 
                    buttons[i].onClick.AddListener(() => setSelectConnect(curIndex));
                    buttons[i].onClick.AddListener(() => StartCoroutine(ClickRoutine()));

                    TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = connectPairList[i].Button_Text;
                    }

                    // 활성화되는 버튼이 레이아웃 그룹에서 정렬이 안되서 재정렬을 해주는 기능
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)buttons[i].transform);

                    // 구버전
                    #region ButtonType
                    /*switch (connectPairList[i].type)
                    {
                        case SelectButtonType.Yes:
                            buttons[0].gameObject.SetActive(true);
                            // 매개변수가 없는 이벤트에 매개변수가 필요한 메서드를 람다식으로 추가 
                            buttons[0].onClick.AddListener(() => setSelectConnect(curIndex));
                            buttons[0].onClick.AddListener(() => StartCoroutine(ClickRoutine()));
                            break;
                        case SelectButtonType.No:
                            buttons[1].gameObject.SetActive(true);
                            buttons[1].onClick.AddListener(() => setSelectConnect(curIndex));
                            buttons[1].onClick.AddListener(() => StartCoroutine(ClickRoutine()));
                            break;
                        case SelectButtonType.Quest:
                            buttons[2].gameObject.SetActive(true);
                            buttons[2].onClick.AddListener(() => setSelectConnect(curIndex));
                            buttons[2].onClick.AddListener(() => StartCoroutine(ClickRoutine()));
                            break;
                    }*/
                    #endregion
                }
            }
            else
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i].gameObject.activeSelf)
                    {
                        buttons[i].onClick.RemoveAllListeners();
                        buttons[i].gameObject.SetActive(false);

                        TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
                        if (buttonText != null)
                        {
                            buttonText.text = "";
                        }
                    }
                }
            }
        }

        IEnumerator ClickRoutine()
        {
            isClicked = true;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].gameObject.activeSelf)
                {
                    buttons[i].onClick.RemoveAllListeners();
                    buttons[i].gameObject.SetActive(false);
                }
            }
            yield return new WaitForEndOfFrame();
            isClicked = false;
        }

        public void SendSubtitleMessage(string message)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            wordText.gameObject.SetActive(true);
            wordText.text = message;
            wordText.transform.SetAsLastSibling();

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)wordText.transform);

            /*wordIndex++;
            // 순환을 다 돌면 처음으로
            if (wordIndex >= wordText.Length)
            {
                wordIndex = 0;
            }*/
        }

        // 비활성화되면 text 비우고 비활성화
        private void OnDisable()
        {
            wordText.text = "";
            wordText.gameObject.SetActive(false);

            /*for (int i = 0; i < wordText.Length; i++)
            {
                wordText[i].text = "";
                wordText[i].gameObject.SetActive(false);
            }*/
        }
    }
}
