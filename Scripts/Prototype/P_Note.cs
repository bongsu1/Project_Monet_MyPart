using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    public class P_Note : PooledObject
    {
        [Header("Note")]
        [SerializeField] float noteSpeed;
        [SerializeField] int noteScore;

        [Header("Sound")]
        [SerializeField] AudioClip clip;
        public AudioClip Clip { set { clip = value; } }

        public UnityEvent<int> onHitNote;

        private int curLine;
        public int CurLine { get { return curLine; } set { curLine = value; } }
        [SerializeField] private bool canHit;
        public bool CanHit { get { return canHit; } set { canHit = value; } }

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(ScrollRoutine());
        }

        private void OnDisable()
        {
            // 추가되어 있는 이벤트 전부 제거
            onHitNote.RemoveAllListeners();
        }

        // 노트의 움직임 구현
        IEnumerator ScrollRoutine()
        {
            while (true)
            {
                transform.position = transform.position + transform.forward * noteSpeed * Time.deltaTime;
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!canHit)
                return;

            P_Instrument instrument = other.GetComponent<P_Instrument>();
            if (instrument == null)
                return;

            switch (instrument)
            {
                // 지금 악기가 핸드벨인데 잡혀있지 않으면 쳐지지 않음
                case P_Handbell handbell:
                    if (!handbell.IsSelect || handbell.Rigid.velocity.magnitude < 2f)
                        return;
                    break;
                // 지금 악기가 하프인데 줄을 튕기지 않으면 쳐지지 않음
                case P_Harp harp:
                    if (!harp.OnBounce)
                        return;
                    // 지금의 라인과 악기의 번호와 맞지 않으면 리턴
                    if (instrument.CurrentLine != curLine)
                        return;
                    break;
                // 지금 악기가 피아노이고, 버튼 한번누를때 하나의 노트만 칠 수 있도록
                case P_Piano piano:
                    piano.HitColl.enabled = false;
                    break;
            }

            PlayNoteSound();
            onHitNote?.Invoke(noteScore); // 노트를 파괴 시 점수 추가 이벤트
            Release();
        }

        public void PlayNoteSound()
        {
            if (clip == null)
                return;

            Manager.Sound.PlaySFX(clip);
        }
    }
}
