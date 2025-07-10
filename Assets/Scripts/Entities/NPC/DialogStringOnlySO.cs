using System.Collections.Generic;
using UnityEngine;

namespace ToB
{
    [CreateAssetMenu(fileName = "DialogStringOnlySO", menuName = "Scriptable Objects/DialogStringOnlySO")]
    public class DialogStringOnlySO : ScriptableObject
    {
        [field:SerializeField] public List<string> Dialogs { get; private set; }
    }
}
