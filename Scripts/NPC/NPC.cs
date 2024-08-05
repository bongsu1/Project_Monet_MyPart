using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    public class NPC : MonoBehaviour
    {
        //protected enum DTType { NPC, Soliloquy, Conversation, Voice, ConverEffect } // DataTable Type
        protected enum InArea { Area1, Area2 } // 플레이어가 어디까지 들어왔는지 상태 큰범위가 1, 좁은범위가 2

        protected Dictionary<string, Dictionary<string, object>> npcDT;
        protected Dictionary<string, Dictionary<string, object>> soliloquyDT;
        protected Dictionary<string, Dictionary<string, object>> conversationDT;
        protected Dictionary<string, Dictionary<string, object>> voiceDT;
        protected Dictionary<string, Dictionary<string, object>> convertEffectDT;

        [Header("Data Table Path")]
        [SerializeField] string npcDTPath = "Scenario/DataTable/NPCDT";
        [SerializeField] string soliloquyDTPath = "Scenario/DataTable/SoliloquyDT";
        [SerializeField] string conversationDTPath = "Scenario/DataTable/ConversationDT";
        [SerializeField] string voiceDTPath = "Scenario/DataTable/TextDT";
        [SerializeField] string convertEffectDTPath = "Scenario/DataTable/ConvertEffectDT";

        [Header("NPC")]
        [SerializeField] protected string npcID; // 입력해주는 id값으로 데이터테이블 참조
        public string NPCID { get { return npcID; } }

        [Header("Voice")]
        [SerializeField] protected NPCVoice npcVoice; // 보이스를 재생시켜주는 컴포넌트

        [Header("Enable Event")]
        public UnityEvent onEnabled;
        public UnityEvent onDisabled;

        protected virtual void OnEnable()
        {
            onEnabled?.Invoke();
        }

        protected virtual void OnDisable()
        {
            onDisabled?.Invoke();
        }

        protected virtual void Awake()
        {
            if (npcVoice == null)
            {
                npcVoice = GetComponentInChildren<NPCVoice>(true);
            }

            npcDT = NPCDataTable.GetDataTable(DTType.NPC, npcDTPath);
            soliloquyDT = NPCDataTable.GetDataTable(DTType.Soliloquy, soliloquyDTPath);
            conversationDT = NPCDataTable.GetDataTable(DTType.Conversation, conversationDTPath);
            voiceDT = NPCDataTable.GetDataTable(DTType.Voice, voiceDTPath);
            convertEffectDT = NPCDataTable.GetDataTable(DTType.ConverEffect, convertEffectDTPath);
        }
    }
}