using UnityEngine;
using UnityEngine.Events;

namespace BS
{
    public class SelfPortrait : MonoBehaviour
    {
        [Header("Piece")]
        [SerializeField] GameObject[] potraitPiece;

        [Header("Event")]
        public UnityEvent onComplete;

        private int pieceCount;

        private void Start()
        {
            pieceCount = 0;
            for (int i = 0; i < potraitPiece.Length; i++)
            {
                if (Manager.Game.ChapterData.clear[i])
                {
                    potraitPiece[i].gameObject.SetActive(true);
                    pieceCount++;
                }
                else
                {
                    potraitPiece[i].gameObject.SetActive(false);
                }
            }
            if (pieceCount == potraitPiece.Length)
                onComplete?.Invoke();
        }
    }
}
