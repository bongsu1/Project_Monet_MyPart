using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    // harp가 맞는 표현임 데이터 테이블에 half라고 쓰여있어서 이렇게 사용
    public enum InstrumentType { pianosound, halfsound, handbellsound }
    public enum SelectMusic { LittleStar_Easy, LittleStar_Hard, MoonLight1st, MoonLight3rd }

    public class P_RhythmGameManager : MonoBehaviour
    {
        [Header("Data Path")]
        [SerializeField] string soundDataPath;
        [SerializeField] List<string> musicDataPath;

        [Header("Music BGM")]
        [SerializeField] AudioClip[] musicBGM;
        [SerializeField] float syncTime;

        [Header("Note")]
        [SerializeField] Transform[] spawnPoints;
        [SerializeField] P_Note notePrefab;
        [SerializeField] PooledObject noteDestroyEffetPrefab;

        [Header("Score")]
        [SerializeField] int successScore;
        [SerializeField] KJW.ScoreManager scoreManger;

        public UnityEvent<int> onSuccess; // 성공 시 이벤트

        private Dictionary<string, Dictionary<string, object>> soundData;
        private List<Dictionary<string, Dictionary<string, object>>> musicDatas;

        [Header("Debug")]
        [SerializeField] List<SoundData> soundDataList;
        [SerializeField] List<MusicData> musicDataList;

        [SerializeField] int curScore;
        [SerializeField] AudioClip curBGM;

        [SerializeField] InstrumentType instrument;

        private void Awake()
        {
            soundData = CSVReader.ReadAndReturnDic(soundDataPath);
            musicDatas = new List<Dictionary<string, Dictionary<string, object>>>();
            // 곡을 여러개 담아놔도 전부 불러오기
            for (int i = 0; i < musicDataPath.Count; i++)
            {
                musicDatas.Add(CSVReader.ReadAndReturnDic(musicDataPath[i]));
            }

            LoadSoundData();

            if (!Manager.Pool.ExistPool(notePrefab))
                Manager.Pool.CreatePool(notePrefab, 10, 20);

            if (!Manager.Pool.ExistPool(noteDestroyEffetPrefab))
                Manager.Pool.CreatePool(noteDestroyEffetPrefab, 10, 20);
        }

        private void LoadSoundData()
        {
            soundDataList = new List<SoundData>();
            foreach (var dic in soundData.Values)
            {
                int id = (int)dic["id"];
                string pianoSound = dic["pianosound"].ToString();
                string handbellSound = dic["handbellsound"].ToString();
                string halfSound = dic["halfsound"].ToString();

                soundDataList.Add(new SoundData(id, pianoSound, handbellSound, halfSound));
            }
        }

        [VisibleEnum(typeof(SelectMusic))]
        public void LoadMusicData(int index)
        {
            if (musicDataList == null)
                musicDataList = new List<MusicData>();
            else
                musicDataList.Clear();

            foreach (var dic in musicDatas[index].Values)
            {
                int id = (int)dic["id"];
                float time = Convert.ToSingle(dic["time"]);
                int line = (int)dic["line"];
                int sound = (int)dic["sound"];
                int type = (int)dic["type"];
                float last = Convert.ToSingle(dic["last"]);

                musicDataList.Add(new MusicData(id, time, line, sound, type, last));
            }
            curBGM = musicBGM[index];
        }

        [ContextMenu("Start Game")]
        public void StartRhythmGame()
        {
            // 지금 곡 데이터가 없으면 리턴
            if (musicDataList.Count < 1)
                return;

            // 이미 재생중이면 리턴
            if (rhythmGameRoutine != null)
                return;

            rhythmGameRoutine = StartCoroutine(RhythmGameRoutine());
            StartCoroutine(BGMWaitRoutine(musicDataList[0].time + syncTime));
        }

        Coroutine rhythmGameRoutine;
        IEnumerator RhythmGameRoutine()
        {
            float waitTime = 0f;
            for (int i = 0; i < musicDataList.Count; i++)
            {
                // 처음을 제외하고 전 노트의 타임과 지금 타임을 비교하여 생성
                if (i != 0)
                    waitTime = musicDataList[i].time - musicDataList[i - 1].time;
                else
                    waitTime = musicDataList[i].time;

                yield return new WaitForSeconds(waitTime);
                if (Manager.Pool.GetPool(notePrefab, spawnPoints[musicDataList[i].line - 1].position, spawnPoints[musicDataList[i].line - 1].rotation) is P_Note note)
                {
                    note.onHitNote.AddListener(AddScore);
                    note.onHitNote.AddListener((score) => Manager.Pool.GetPool(noteDestroyEffetPrefab, note.transform.position, note.transform.rotation));
                    note.CurLine = musicDataList[i].line;
                    // scoreManager 가 있으면 점수를 추가해주는 함수를 이벤트에 추가
                    if (scoreManger != null)
                        note.onHitNote.AddListener((score) => scoreManger.AddScore(1));

                    AudioClip clip = Manager.Resource.Load<AudioClip>($"Prototype/{soundData[musicDataList[i].sound.ToString()][instrument.ToString()]}"); // 지금은 피아노소리만 있음
                    note.Clip = clip;
                }
            }

            yield return new WaitForSeconds(5f); // 노트가 내려오는 시간을 고려
            Manager.Sound.StopBGM();
            rhythmGameRoutine = null;

            // 성공점수를 넘었으면 성공
            if (curScore >= successScore)
                onSuccess?.Invoke(curScore);

            // 끝나면 점수 초기화
            curScore = 0;
        }

        IEnumerator BGMWaitRoutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Manager.Sound.PlayBGM(curBGM);
        }

        public void AddScore(int score)
        {
            curScore += score;
        }

        [VisibleEnum(typeof(InstrumentType))]
        public void ChangeInstrument(int instrument)
        {
            this.instrument = (InstrumentType)instrument;
        }
    }

    [Serializable]
    public class SoundData
    {
        public int id;
        public string pianoSound;
        public string handbellSound;
        public string halfSound;

        public SoundData(int id, string pianoSound, string handbellSound, string halfSound)
        {
            this.id = id;
            this.pianoSound = pianoSound;
            this.handbellSound = handbellSound;
            this.halfSound = halfSound;
        }
    }

    [Serializable]
    public class MusicData
    {
        public int id;
        public float time;
        public int line;
        public int sound;
        public int type; // 단타, 장타 구분 // 지금은 단타뿐
        public float last; // 장타일때 지속되는 시간

        public MusicData(int id, float time, int line, int sound, int type, float last)
        {
            this.id = id;
            this.time = time;
            this.line = line;
            this.sound = sound;
            this.type = type;
            this.last = last;
        }
    }
}
