using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    public class MirrorPuzzleManager : MonoBehaviour
    {
        [SerializeField] RayShooter[] rayShooters; // 퍼즐 레벨 개수

        public UnityEvent allClear;

        private int clearCount;

        private void Awake()
        {
            for (int i = 0; i < rayShooters.Length; i++)
            {
                rayShooters[i].onClear.AddListener(ClearCount);
            }
        }

        private void ClearCount()
        {
            clearCount++;
            if (clearCount == rayShooters.Length)
            {
                allClear?.Invoke();
            }
        }
    }
}
