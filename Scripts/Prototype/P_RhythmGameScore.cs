using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    public class P_RhythmGameScore : MonoBehaviour
    {
        [SerializeField] int finishScore;

        [Header("Test")]
        [SerializeField] TMP_Text scoreText;

        public UnityEvent<int> onChangeScore;
        public UnityEvent finished;

        private int score;

        public void AddScore(int score)
        {
            this.score += score;
            onChangeScore?.Invoke(this.score);

            if (scoreText != null)
                scoreText.text = $"Score : {this.score}";

            if (this.score >= finishScore)
            {
                finished?.Invoke();
            }
        }

        public void ResetScore()
        {
            score = 0;
        }
    }
}
