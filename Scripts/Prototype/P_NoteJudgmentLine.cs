using UnityEngine;

namespace Prototype
{
    public class P_NoteJudgmentLine : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            P_Note note = other.GetComponent<P_Note>();
            if (note == null)
                return;

            note.CanHit = true;
        }

        private void OnTriggerExit(Collider other)
        {
            P_Note note = other.GetComponent<P_Note>();
            if (note == null)
                return;

            note.CanHit = false;
        }
    }
}
