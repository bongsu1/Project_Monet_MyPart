using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    public class SoliloquyNPC : NPC
    {
        [Header("Soliloquy")]
        [SerializeField] BubbleUI bubbleUI;

        [Header("Serialize Debug")] // test 인스펙터 창으로 볼려고 serializeField를 붙임
        [SerializeField] private bool inArea1; // 플레이어가 첫번째 영역에 들어오면 true
        [SerializeField] private bool inArea2; // 두번째 영역에 들어오면 true
        [SerializeField] private int triggerCount;
        [SerializeField] private string curArea; // 혼잣말 데이터테이블에서 사용할 항목이름

        public UnityEvent onStopSoliloquyRoutine;

        protected override void Awake()
        {
            base.Awake();

            if (bubbleUI == null)
            {
                bubbleUI = GetComponentInChildren<BubbleUI>(true);
            }
        }

        protected void StartSoliloquy(InArea area)
        {
            switch (area)
            {
                case InArea.Area1:
                    curArea = "Area1_ID";
                    break;
                case InArea.Area2:
                    curArea = "Area2_ID";
                    break;
            }
            if (npcDT[npcID][curArea] is int soliloquyID)
            {
                // 혼잣말을 가지고 있지 않으면 -1, 그래서 0이하면 리턴
                if (soliloquyID < 0)
                    return;

                soliloquyRoutine = StartCoroutine(SoliloquyRoutine(area, soliloquyID.ToString()));
            }
        }

        Coroutine soliloquyRoutine;
        private IEnumerator SoliloquyRoutine(InArea area, string soliloquyID)
        {
            // 중복 실행 되지않게 전에 실행되는 루틴을 종료
            StopSoliloquyRoutine();

            // 한프레임이 실행되고 나서 할당되기 때문에 처음 실행은 문제 없이 된다
            yield return new WaitUntil(() => soliloquyRoutine == null);
            yield return null;

            // voiceID를 리스트로 받아서 관리, 랜덤으로 진행
            var voiceIDList = new List<string>();
            foreach (var id in soliloquyDT[soliloquyID].Values)
            {
                // 자신의 아이디와 같은건 제거
                if (id.ToString() == soliloquyID)
                    continue;

                // -1, id가 존재하지 않음
                if ((int)id < 0)
                    continue;

                voiceIDList.Add(id.ToString());
            }

            // area에서 나가면 반복종료
            while ((inArea1 && area == InArea.Area1) || (inArea2 && area == InArea.Area2))
            {
                int random = Random.Range(0, voiceIDList.Count);
                npcVoice.PlayVoice(voiceDT[voiceIDList[random]]["ID"].ToString()); // todo: 데이터 테이블 변경 예정 그때 수정바람
                bubbleUI.gameObject.SetActive(true);
                bubbleUI.SetMessage(voiceDT[voiceIDList[random]]["Text_Subtitle"].ToString());

                yield return new WaitUntil(() => !npcVoice.Voice.isPlaying); // 음성이 종료될 때까지 기다리기
                yield return new WaitForSeconds(3f); // 그리고 2초정도 더 기다리고 반복
            }
            // 혼자 종료될 때에는 null
            bubbleUI.gameObject.SetActive(false);
            soliloquyRoutine = null;
        }

        protected void StopSoliloquyRoutine()
        {
            if (soliloquyRoutine == null)
                return;

            StopCoroutine(soliloquyRoutine);
            // 종료될 때 이벤트 호출
            onStopSoliloquyRoutine?.Invoke();
            bubbleUI.gameObject.SetActive(false);
            soliloquyRoutine = null;
        }

        // 플레이어와 충돌시 triggerCount증가
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 3) // 3 = playerLayer
                return;

            /*XROrigin origin = other.GetComponent<XROrigin>();
            if (origin == null)
                return;*/

            triggerCount++;
            switch (triggerCount)
            {
                case 1:
                    inArea1 = true;
                    inArea2 = false;
                    StartSoliloquy(InArea.Area1);
                    break;
                case 2:
                    inArea1 = false;
                    inArea2 = true;
                    StartSoliloquy(InArea.Area2);
                    break;
                default:
                    inArea1 = false;
                    inArea2 = false;
                    break;
            }
        }

        // 플레이어가 영역에서 나갈시 triggerCount감소
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 3)
                return;

            /*XROrigin origin = other.GetComponent<XROrigin>();
            if (origin == null)
                return;*/

            triggerCount--;
            switch (triggerCount)
            {
                case 1:
                    inArea1 = true;
                    inArea2 = false;
                    StartSoliloquy(InArea.Area1);
                    break;
                default:
                    inArea1 = false;
                    inArea2 = false;
                    break;
            }
        }
    }
}
