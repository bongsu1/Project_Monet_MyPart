using UnityEngine;

namespace Prototype
{
    public class P_Instrument : MonoBehaviour
    {
        [SerializeField] int currentLine;
        public int CurrentLine { get { return currentLine; } }
    }
}
