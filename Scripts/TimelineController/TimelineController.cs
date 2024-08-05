using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace BS
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineController : MonoBehaviour
    {
        [SerializeField] PlayableDirector director;
        [SerializeField] TimelineAsset[] timelineControllers;

        public void TimelinePlay(int index)
        {
            director.playableAsset = timelineControllers[index];
            director.Play();
        }
    }
}
