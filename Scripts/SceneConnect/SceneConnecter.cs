using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    public class SceneConnecter : MonoBehaviour
    {
        [Header("Rotate Object")]
        [SerializeField] Transform r90;
        [SerializeField] Transform r180;
        [SerializeField] Transform npc;
        [SerializeField] Transform notTurnObject;

        [Header("Rotate Time")]
        [SerializeField] float minStartTime = 0f;
        [SerializeField] float maxStartTime = 5f;
        [SerializeField] float minRotateTime = 1f;
        [SerializeField] float maxRotateTime = 2f;

        [Header("Start")]
        [SerializeField] bool isDownStart;

        [Header("LoadScene")]
        [SerializeField] string nextScene;

        [Header("Event")]
        public UnityEvent onAwake;
        public UnityEvent onLoadNextScene;

        private Dictionary<string, List<Transform>> transformDic;
        private Dictionary<string, List<Vector3>> rotationDic;
        private Dictionary<string, List<Renderer>> renderDic;
        private Dictionary<string, List<Renderer[]>> notTurnRenderDic;
        private const string originKey = "OriginRotation";
        private const string downKey = "DownRotiation";
        private const string fadeProperty = "_FullGlowDissolveFade"; // shader Property name

        private void Awake()
        {
            #region GameObject Find
            /*if (r90 == null)
            {
                r90 = GameObject.Find("R90")?.transform;
            }
            if (r180 == null)
            {
                r180 = GameObject.Find("R180")?.transform;
            }
            if (npc == null)
            {
                npc = GameObject.Find("NPC")?.transform;
            }
            if (notTurnObject == null)
            {
                notTurnObject = GameObject.Find("NotTurnObject")?.transform;
            }*/
            #endregion

            #region Add Dictionary
            transformDic = new Dictionary<string, List<Transform>>();
            rotationDic = new Dictionary<string, List<Vector3>>();
            //renderDic = new Dictionary<string, List<Renderer>>();

            List<Transform> childList;
            List<Vector3> originRotList;
            List<Vector3> downRotList;
            //List<Renderer> renderList;
            if (r90 != null)
            {
                childList = new List<Transform>();
                originRotList = new List<Vector3>();
                downRotList = new List<Vector3>();
                //renderList = new List<Renderer>();

                foreach (Transform child in r90)
                {
                    // 회전 시킬 오브젝트의 트랜스폼을 저장
                    childList.Add(child);

                    // 원래 회전값을 저장
                    Vector3 originRot = child.eulerAngles;
                    originRotList.Add(originRot);

                    // 누워 있을 회전값 저장
                    Vector3 downRot = new Vector3(originRot.x + 90, originRot.y, originRot.z);
                    downRotList.Add(downRot);

                    /*Renderer childRender = child.GetComponentInChildren<Renderer>();
                    if (childRender == null)
                        continue;
                    renderList.Add(childRender);*/

                    if (isDownStart)
                    {
                        child.rotation = Quaternion.Euler(downRot);
                        //childRender.material.SetFloat(fadeProperty, 0f);
                    }
                }
                transformDic.Add($"{r90.name}", childList);
                rotationDic.Add($"{r90.name}{originKey}", originRotList);
                rotationDic.Add($"{r90.name}{downKey}", downRotList);
                //renderDic.Add($"{r90.name}", renderList);
            }

            if (r180 != null)
            {
                childList = new List<Transform>();
                originRotList = new List<Vector3>();
                downRotList = new List<Vector3>();
                // 180도 돌아갈 물체는 렌더러를 쓸 필요없음
                //renderList = new List<Renderer>();

                foreach (Transform child in r180)
                {
                    childList.Add(child);

                    Vector3 originRot = child.eulerAngles;
                    originRotList.Add(originRot);

                    Vector3 downRot = new Vector3(originRot.x - 180, originRot.y, originRot.z);
                    downRotList.Add(downRot);

                    /*Renderer childRender = child.GetComponentInChildren<Renderer>();
                    if (childRender == null)
                        continue;
                    renderList.Add(childRender);*/

                    if (isDownStart)
                    {
                        child.rotation = Quaternion.Euler(downRot);
                        //childRender.material.SetFloat(fadeProperty, 0f);
                    }
                }
                transformDic.Add($"{r180.name}", childList);
                rotationDic.Add($"{r180.name}{originKey}", originRotList);
                rotationDic.Add($"{r180.name}{downKey}", downRotList);
                //renderDic.Add($"{r180.name}", renderList);
            }

            if (npc != null)
            {
                childList = new List<Transform>();
                originRotList = new List<Vector3>();
                downRotList = new List<Vector3>();
                ///renderList = new List<Renderer>();

                foreach (Transform child in npc)
                {
                    childList.Add(child);

                    Vector3 originRot = child.eulerAngles;
                    originRotList.Add(originRot);

                    Vector3 downRot = new Vector3(originRot.x + 90, originRot.y, originRot.z);
                    downRotList.Add(downRot);

                    /*Renderer childRender = child.GetComponentInChildren<Renderer>();
                    if (childRender == null)
                        continue;
                    renderList.Add(childRender);*/

                    if (isDownStart)
                    {
                        child.rotation = Quaternion.Euler(downRot);
                        //childRender.material.SetFloat(fadeProperty, 0f);
                    }
                }
                transformDic.Add($"{npc.name}", childList);
                rotationDic.Add($"{npc.name}{originKey}", originRotList);
                rotationDic.Add($"{npc.name}{downKey}", downRotList);
                //renderDic.Add($"{npc.name}", renderList);
            }

            if (notTurnObject != null)
            {
                /*notTurnRenderDic = new Dictionary<string, List<Renderer[]>>();
                List<Renderer[]> renderArrList = new List<Renderer[]>();*/

                childList = new List<Transform>();
                // 돌아가지 않는 물체는 회전값이 필요없음
                /*originRotList = new List<Vector3>();
                downRotList = new List<Vector3>();*/

                foreach (Transform child in notTurnObject)
                {
                    childList.Add(child);

                    Vector3 originRot = child.eulerAngles;
                    /*originRotList.Add(originRot);
                    downRotList.Add(originRot);*/

                    /*Renderer[] childRenders = child.GetComponentsInChildren<Renderer>();
                    if (childRenders == null)
                        continue;
                    renderArrList.Add(childRenders);*/

                    /*if (isDownStart)
                    {
                        for (int i = 0; i < childRenders.Length; i++)
                        {
                            childRenders[i].material.SetFloat(fadeProperty, 0f);
                        }
                    }*/
                }
                transformDic.Add($"{notTurnObject.name}", childList);
                /*rotationDic.Add($"{notTurnObject.name}{originKey}", originRotList);
                rotationDic.Add($"{notTurnObject.name}{downKey}", downRotList);*/
                //notTurnRenderDic.Add($"{notTurnObject.name}", renderArrList);
            }
            #endregion

            if (isDownStart)
            {
                ChangeStage(false);
            }

            onAwake?.Invoke();
        }

        public void ChangeStage(bool isDown)
        {
            foreach (var key in transformDic.Keys)
            {
                for (int i = 0; i < transformDic[key].Count; i++)
                {
                    StartCoroutine(UpAndDownRoutine(key, i, isDown));
                }
            }

            if (isDown)
            {
                if (sceneChangeRoutine != null)
                    return;

                sceneChangeRoutine = StartCoroutine(SceneChangeRoutine());
            }
        }

        IEnumerator UpAndDownRoutine(string key, int index, bool isDown)
        {
            // 돌아가는 오브젝트들만 돌리기
            if (!key.Equals(notTurnObject == null ? "" : notTurnObject.name))
            {
                Transform child = transformDic[key][index];
                Vector3 originRot = rotationDic[$"{key}{originKey}"][index];
                Vector3 downRot = rotationDic[$"{key}{downKey}"][index];
                yield return new WaitForSeconds(Random.Range(minStartTime, maxStartTime)); // 랜덤으로 넘어지기위한 랜덤 지연
                // 넘어가는 연출
                if (isDown)
                {
                    float rate = 0;
                    float randomTime = Random.Range(minRotateTime, maxRotateTime); // 넘어지고 일어서는 시간도 랜덤
                    while (rate <= 1)
                    {
                        rate += Time.deltaTime / randomTime;
                        child.rotation = Quaternion.Lerp(Quaternion.Euler(originRot), Quaternion.Euler(downRot), rate);
                        yield return null;
                    }
                    // 눕혀지고 나서 사라지는 연출
                    /*if (!key.Equals(r180 == null ? "" : r180.name))
                    {
                        Renderer render = renderDic[key][index];
                        StartCoroutine(DissolveRoutine(render, isDown));
                    }*/
                }
                // 일어서는 연출
                else
                {
                    // 일어서기 전에 나타나는 연출
                    /*if (!key.Equals(r180 == null ? "" : r180.name))
                    {
                        Renderer render = renderDic[key][index];
                        yield return DissolveRoutine(render, isDown);
                    }*/
                    float rate = 0;
                    float randomTime = Random.Range(minRotateTime, maxRotateTime);
                    while (rate <= 1)
                    {
                        rate += Time.deltaTime / randomTime;
                        child.rotation = Quaternion.Lerp(Quaternion.Euler(downRot), Quaternion.Euler(originRot), rate);
                        yield return null;
                    }
                }
            }
            // 넘어가지 않는 사물은 따로 진행(렌더러를 여러개 가지고 있다)
            /*else
            {
                Transform child = transformDic[key][index];
                Renderer[] renderArr = notTurnRenderDic[key][index];
                yield return new WaitForSeconds(Random.Range(minStartTime, maxStartTime)); // 랜덤으로 넘어지기위한 랜덤 지연
                for (int i = 0; i < renderArr.Length; i++)
                {
                    StartCoroutine(DissolveRoutine(renderArr[i], isDown));
                }
            }*/
        }

        /*IEnumerator DissolveRoutine(Renderer render, bool isDown)
        {
            float rate = 0;

            while (rate <= 1)
            {
                rate += Time.deltaTime;
                // 사라지는 연출
                if (isDown)
                {
                    render.material.SetFloat(fadeProperty, 1 - rate);
                }
                // 나타나는 연출
                else
                {
                    render.material.SetFloat(fadeProperty, rate);
                }
                yield return null;
            }
        }*/

        Coroutine sceneChangeRoutine;
        IEnumerator SceneChangeRoutine()
        {
            onLoadNextScene?.Invoke();
            yield return new WaitForSeconds(6f);
            Manager.Scene.LoadScene(nextScene);
        }

        public void NextChapter()
        {
            if (sceneChangeRoutine != null)
                return;

            sceneChangeRoutine = StartCoroutine(SceneChangeRoutine());
        }

        /*public void LoadXROriginScene()
        {
            Manager.Scene.LoadSceneAddtive("XROriginScene");
        }

        public void StartChapter()
        {
            StartCoroutine(StartSceneLoadRoutine());
        }

        IEnumerator StartSceneLoadRoutine()
        {
            AsyncOperation oper = UnitySceneManager.LoadSceneAsync(nextSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            yield return new WaitUntil(() => oper.isDone);

            UnityEngine.SceneManagement.Scene scene = UnitySceneManager.GetSceneByName(nextSceneName);
            UnitySceneManager.SetActiveScene(scene);
        }

        public void NextChapter()
        {
            UnitySceneManager.LoadSceneAsync(nextSceneName);

            Manager.Pool.ClearPool();
            Manager.Sound.StopSFX();
            Manager.UI.ClearPopUpUI();
            Manager.UI.ClearWindowUI();
        }*/
    }
}
