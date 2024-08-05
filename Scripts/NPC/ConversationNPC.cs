using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    /// <summary>
    /// UI선택형, 모션감지형, 미선택형(연결되는 대화), 나레이션
    /// </summary>
    public enum ConversationType { UI, Gesture, None, Narration }

    public class ConversationNPC : NPC
    {
        [Header("Subtile")]
        [SerializeField] SubtitleUI subtitle;
        [SerializeField] float minScriptReadingTime = 2f; // 음성파일이 없는 경우에도 자막이 최소한 떠있어야 할 시간

        [Header("Trigger Narration")]
        [SerializeField][Tooltip("트리거로 작동될 대사와 NPC는 체크하세요")] bool isTriggerExcute;

        [Header("Player Limit")]
        [SerializeField] GameObject[] moveProviderAndETC;

        public UnityEvent onExitConversation;

        private List<KeyPair> keyPairList;
        private List<ConnectPair> connectPairList;
        private NPC[] npcArray;

        private bool isConversation;
        private string conversationID;
        private string convertEffectID;
        private int selectConnect;

        private WaitForEndOfFrame waitForEndOfFrame;
        private WaitForSeconds waitZeroPointFiveSecond; // 0.5초

        protected override void Awake()
        {
            base.Awake();

            keyPairList = new List<KeyPair>();
            connectPairList = new List<ConnectPair>();

            waitForEndOfFrame = new WaitForEndOfFrame();
            waitZeroPointFiveSecond = new WaitForSeconds(0.5f);

            if (subtitle == null)
                subtitle = FindObjectOfType<SubtitleUI>(true);

            npcArray = transform.root.GetComponentsInChildren<NPC>(true);
        }

        IEnumerator KeyPairLoadingRoutine()
        {
            yield return new WaitUntil(() => npcDT != null);
            keyPairList.Clear();

            int keyObject1_ID = (int)npcDT[npcID]["KeyObject1_ID"];
            int conversation1DT_ID = (int)npcDT[npcID]["Conversation1DT_ID"];
            if (conversation1DT_ID > 0) // conversationDT_ID = -1 대화데이터 없음
            {
                keyPairList.Add(new KeyPair(keyObject1_ID, conversation1DT_ID));
            }

            int keyObject2_ID = (int)npcDT[npcID]["KeyObject2_ID"];
            int conversation2DT_ID = (int)npcDT[npcID]["Conversation2DT_ID"];
            if (conversation2DT_ID > 0)
            {
                keyPairList.Add(new KeyPair(keyObject2_ID, conversation2DT_ID));
            }

            int keyObject3_ID = (int)npcDT[npcID]["KeyObject3_ID"];
            int conversation3DT_ID = (int)npcDT[npcID]["Conversation3DT_ID"];
            if (conversation3DT_ID > 0)
            {
                keyPairList.Add(new KeyPair(keyObject3_ID, conversation3DT_ID));
            }
        }

        // 플레이어가 가지고 있는 아이템마다 다른 대화를 할 수도 있다, 키 아이템이 필요할 수도 있고 아닐 수도 있음
        /// <summary>
        /// NPC와 대화 시작
        /// </summary>
        /// <param name="keyObject_ID"> 가지고 있는 키아이템의 아이디</param>
        public void Conversation(int keyObject_ID = -1)
        {
            // 이미 대화중
            if (isConversation)
                return;

            // 다른사람과 대화중
            if (subtitle.gameObject.activeSelf)
                return;

            isConversation = true;

            StartCoroutine(ConversationRoutine(keyObject_ID));
        }

        IEnumerator ConversationRoutine(int keyObject_ID)
        {
            yield return KeyPairLoadingRoutine();

            KeyPair keyPair = null;
            // 해당 NPC와 관련된 키 아이템을 들고 있는지 확인
            for (int i = 0; i < keyPairList.Count; i++)
            {
                if (keyPairList[i].KeyObject_ID == keyObject_ID)
                {
                    keyPair = keyPairList[i];
                    break;
                }
            }

            // 대화가 없는 친구들은 대화를 할 수 없음
            if (keyPairList.Count == 0)
            {
                isConversation = false;
                yield break;
            }

            // 해당 npc와 관련된 키 아이템이 없고, 해당 npc의 모든 대화에 키 아이템이 필요한 경우
            if ((keyPair == null) && (keyPairList[0].KeyObject_ID != -1))
            {
                // 아이템이 필요하다는 문구를 띄우는 로직
                // 대화 거부
                isConversation = false;
                yield break;
            }
            // 해당 npc와 관련된 키 아이템은 없지만, 해당 npc가 키 아이템이 필요 없는 경우
            else if ((keyPair == null) && (keyPairList[0].KeyObject_ID == -1))
            {
                keyPair = keyPairList[0];
            }
            // 그 외에는 키 아이템에 따라 대화 가능
            // 가지고 있는게 없으면 keyObjectID = -1이라서 키 아이템이 필요없는 npc의 경우는 for문에서 할당됨

            conversationID = keyPair.ConversationDT_ID.ToString(); // 시작하는 대화ID 설정

            // 플레이의 움직임 제한
            for (int i = 0; i < moveProviderAndETC.Length; i++)
            {
                moveProviderAndETC[i].SetActive(false);
            }

            float scriptReadingTime = 0;
            // 다음 진행되는 대화가 있고, 2영역에서 나가지않으면 반복하여 진행
            while (!conversationID.Equals("-1"))
            {
                convertEffectID = conversationDT[conversationID]["Effect_ID"].ToString();
                switch ((ConversationType)conversationDT[conversationID]["Type"])
                {
                    #region Select Conversation
                    // 선택지가 나오는 대화
                    case ConversationType.UI:
                        // npc대화 재생
                        // 선택지가 있는 대화에 상대 대화 가 없으면 선택지를 바로 띄운다
                        if (!conversationDT[conversationID]["Voice_ID"].ToString().Equals("-1"))
                        {
                            string npcSubtitle = voiceDT[conversationDT[conversationID]["Voice_ID"].ToString()]["Text_Subtitle"].ToString();
                            string npcVoicePath = voiceDT[conversationDT[conversationID]["Voice_ID"].ToString()]["ID"].ToString();
                            string npcName = voiceDT[conversationDT[conversationID]["Voice_ID"].ToString()]["Npc_Name"].ToString();

                            npcVoice.PlayVoice(npcVoicePath);
                            subtitle.SendSubtitleMessage($"{npcName} : {npcSubtitle}");

                            scriptReadingTime = Time.time;
                            yield return new WaitUntil(() => !npcVoice.Voice.isPlaying); // 보이스가 다 재생 될때까지
                            scriptReadingTime = Time.time - scriptReadingTime;

                            if (scriptReadingTime < minScriptReadingTime)
                                yield return new WaitForSeconds(minScriptReadingTime - scriptReadingTime);

                            yield return waitZeroPointFiveSecond;
                        }

                        yield return ConnectPairLoadingRoutine();
                        // 선택지가 없으면 반복종료
                        if (connectPairList.Count <= 0)
                        {
                            conversationID = "-1";
                            break;
                        }
                        subtitle.SetActiveButton(true, connectPairList, SetSelectConnect);

                        // 버튼이 클릭되면 계속
                        // 영역
                        yield return new WaitUntil(() => subtitle.IsClicked);
                        yield return waitForEndOfFrame;

                        // 선택한 선택지에 따라서 다음 대화ID가 결정됨
                        Manager.Sound.PlayVoice(connectPairList[selectConnect].Voice_ID.ToString());

                        string playerSubtitle = voiceDT[connectPairList[selectConnect].Voice_ID.ToString()]["Text_Subtitle"].ToString();
                        subtitle.SendSubtitleMessage($"{playerSubtitle}");
                        conversationID = connectPairList[selectConnect].Connect_ID.ToString();

                        scriptReadingTime = Time.time;
                        yield return new WaitUntil(() => !Manager.Sound.PlayerVoice.isPlaying); // 나의 대사가 끝날때까지
                        scriptReadingTime = Time.time - scriptReadingTime;

                        if (scriptReadingTime < minScriptReadingTime)
                            yield return new WaitForSeconds(minScriptReadingTime - scriptReadingTime);

                        yield return waitZeroPointFiveSecond;

                        // 영역에서 나가면 대화 취소
                        /*else
                        {
                            subtitle.gameObject.SetActive(false);
                            yield break;
                        }*/

                        yield return waitForEndOfFrame;

                        break;
                    case ConversationType.Gesture:
                        // 기본적으로 UI와 같은 구조
                        conversationID = "-1";
                        yield break; // 아직은 제스처를 만들지 않아서 일단은 종료
                    #endregion
                    #region Noneselect Conversation
                    // 미선택형 대화(쭉 이어지는 대화)
                    case ConversationType.None:
                        // npc의 대사 id가 있고 -1이 아닐때
                        if (conversationDT[conversationID]["Voice_ID"] is int voiceID && voiceID > 0)
                        {
                            npcVoice.PlayVoice(voiceID.ToString());
                            string npcSubtitle = voiceDT[voiceID.ToString()]["Text_Subtitle"].ToString();
                            string npcName = voiceDT[voiceID.ToString()]["Npc_Name"].ToString();
                            subtitle.SendSubtitleMessage($"{npcName}: {npcSubtitle}");

                            // 보이스 재생이 끝날때까지 기다리기
                            scriptReadingTime = Time.time;
                            yield return new WaitUntil(() => !npcVoice.Voice.isPlaying);
                            scriptReadingTime = Time.time - scriptReadingTime;

                            if (scriptReadingTime < minScriptReadingTime)
                                yield return new WaitForSeconds(minScriptReadingTime - scriptReadingTime);

                            yield return waitZeroPointFiveSecond;
                        }
                        yield return waitForEndOfFrame;

                        // 나의 대사 id가 있고 -1이 아닐때
                        if (conversationDT[conversationID]["Voice1_ID"] is int voice1ID && voice1ID > 0)
                        {
                            Manager.Sound.PlayVoice(voice1ID.ToString());
                            subtitle.SendSubtitleMessage($"{voiceDT[voice1ID.ToString()]["Text_Subtitle"]}");

                            scriptReadingTime = Time.time;
                            yield return new WaitUntil(() => !Manager.Sound.PlayerVoice.isPlaying);
                            scriptReadingTime = Time.time - scriptReadingTime;

                            if (scriptReadingTime < minScriptReadingTime)
                                yield return new WaitForSeconds(minScriptReadingTime - scriptReadingTime);

                            yield return waitZeroPointFiveSecond;
                        }
                        yield return waitForEndOfFrame;

                        // 다음 대화로 넘어감
                        conversationID = conversationDT[conversationID]["Connect1_ID"].ToString();
                        yield return waitForEndOfFrame;

                        break;
                    // 나레이션형 (나레이션다음에는 플레이어의 혼잣말 재생, 또는 플레이어의 혼잣말)
                    case ConversationType.Narration:
                        // 나레이션의 voice id는 데이터테이블의 Voice1_ID로 설정되어있다 (시스템 기획자의 구조)
                        //  나레이션의 voice id가 있고 -1이 아닐때
                        if (conversationDT[conversationID]["Voice1_ID"] is int narrationVoiceID && narrationVoiceID > 0)
                        {
                            Manager.Sound.PlayVoice(narrationVoiceID.ToString());
                            subtitle.SendSubtitleMessage(voiceDT[narrationVoiceID.ToString()]["Text_Subtitle"].ToString());

                            scriptReadingTime = Time.time;
                            yield return new WaitUntil(() => !Manager.Sound.PlayerVoice.isPlaying);
                            scriptReadingTime = Time.time - scriptReadingTime;

                            if (scriptReadingTime < minScriptReadingTime)
                                yield return new WaitForSeconds(minScriptReadingTime - scriptReadingTime);

                            yield return waitZeroPointFiveSecond;
                        }
                        yield return waitForEndOfFrame;

                        conversationID = conversationDT[conversationID]["Connect1_ID"].ToString();
                        yield return waitForEndOfFrame;

                        break;
                        #endregion
                }
                // npc를 찾아서 바꾸기
                if (!convertEffectID.Equals("-1"))
                    StartCoroutine(ActiveFalseRoutine(convertEffectID));
            }
            yield return waitZeroPointFiveSecond; // 대화가 끝나고 잠시 기다린 후에 종료
            subtitle.gameObject.SetActive(false);

            isConversation = false;
            for (int i = 0; i < moveProviderAndETC.Length; i++)
            {
                moveProviderAndETC[i].SetActive(true);
            }

            yield return waitForEndOfFrame;

            onExitConversation?.Invoke();
        }

        IEnumerator ActiveFalseRoutine(string converEffectID)
        {
            string prevNPCID = convertEffectDT[converEffectID]["NPC_Bfo_ID"].ToString();
            string nextNPCID = convertEffectDT[converEffectID]["NPC_Aft_ID"].ToString();

            int count = 0;

            NPC prevNPC = null;
            NPC nextNPC = null;
            for (int i = 0; i < npcArray.Length; i++)
            {
                // 비활성화 될 npc
                if (npcArray[i].NPCID.Equals(prevNPCID))
                {
                    count++;
                    prevNPC = npcArray[i];
                }
                // 활성화 될 npc
                else if (npcArray[i].NPCID.Equals(nextNPCID))
                {
                    count++;
                    nextNPC = npcArray[i];
                }
                if (count > 1)
                    break;
            }
            yield return new WaitUntil(() => !isConversation);

            if (nextNPC != null)
                nextNPC.gameObject.SetActive(true);

            if (prevNPC != null)
                prevNPC.gameObject.SetActive(false);
        }

        IEnumerator ConnectPairLoadingRoutine()
        {
            yield return new WaitUntil(() => conversationDT != null);
            connectPairList.Clear();

            int voice1_ID = (int)conversationDT[conversationID]["Voice1_ID"];
            int connect1_ID = (int)conversationDT[conversationID]["Connect1_ID"];
            if (connect1_ID > 0)
            {
                string buttonText1 = voiceDT[voice1_ID.ToString()]["Text_Button"].ToString().Equals("-1") ? voiceDT[voice1_ID.ToString()]["Text_Subtitle"].ToString() : voiceDT[voice1_ID.ToString()]["Text_Button"].ToString();
                connectPairList.Add(new ConnectPair(voice1_ID, connect1_ID, buttonText1));
            }

            int voice2_ID = (int)conversationDT[conversationID]["Voice2_ID"];
            int connect2_ID = (int)conversationDT[conversationID]["Connect2_ID"];
            if (connect2_ID > 0)
            {
                string buttonText2 = voiceDT[voice2_ID.ToString()]["Text_Button"].ToString().Equals("-1") ? voiceDT[voice2_ID.ToString()]["Text_Subtitle"].ToString() : voiceDT[voice2_ID.ToString()]["Text_Button"].ToString();
                connectPairList.Add(new ConnectPair(voice2_ID, connect2_ID, buttonText2));
            }

            int voice3_ID = (int)conversationDT[conversationID]["Voice3_ID"];
            int connect3_ID = (int)conversationDT[conversationID]["Connect3_ID"];
            if (connect3_ID > 0)
            {
                string buttonText3 = voiceDT[voice3_ID.ToString()]["Text_Button"].ToString().Equals("-1") ? voiceDT[voice3_ID.ToString()]["Text_Subtitle"].ToString() : voiceDT[voice3_ID.ToString()]["Text_Button"].ToString();
                connectPairList.Add(new ConnectPair(voice3_ID, connect3_ID, buttonText3));
            }
        }

        // 버튼이나 제스쳐와 연결해서 선택지를 고르기
        private void SetSelectConnect(int select)
        {
            selectConnect = select;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isTriggerExcute)
                return;

            if (other.gameObject.layer != 3)
                return;

            Conversation(SearchKeyID());
        }

        public void StartConversation()
        {
            Conversation(SearchKeyID());
        }

        Collider[] colliders = new Collider[20];
        // 키아이템의 아이디 검색
        private int SearchKeyID()
        {
            int keyObjectID = -1;

            int size = Physics.OverlapSphereNonAlloc(transform.position, 5f, colliders);
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    KeyItem key = colliders[i].gameObject.GetComponent<KeyItem>();
                    if (key != null)
                    {
                        keyObjectID = key.KeyID;
                        break;
                    }
                }
            }
            return keyObjectID;
        }
    }

    // NpcDT
    // KeyObject_ID와 ConversationDT_ID 페어
    public class KeyPair
    {
        public int KeyObject_ID;
        public int ConversationDT_ID;

        public KeyPair(int KeyObject_ID, int ConversationDT_ID)
        {
            this.KeyObject_ID = KeyObject_ID;
            this.ConversationDT_ID = ConversationDT_ID;
        }
    }

    // ConversationDT
    // Voice_ID와 Connect_ID 페어
    public class ConnectPair
    {
        public int Voice_ID;
        public int Connect_ID;

        public string Button_Text;

        public ConnectPair(int Voice_ID, int Connect_ID, string Button_Text)
        {
            this.Voice_ID = Voice_ID;
            this.Connect_ID = Connect_ID;
            this.Button_Text = Button_Text;
        }
    }
}
