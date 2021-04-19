using System.Collections.Generic;
using UnityEngine;

namespace Assets.SS13.ByondLanguage
{
    [CreateAssetMenu(fileName = "ByondLanguageNode", menuName = "Byond/ByondLanguageNode", order = 1)]
    public class ByondLanguageNode : ScriptableObject
    {
        public string NodeDescriptor;
        public bool CanEscapeCharacters = true;
        public List<ByondLanguageNode> CanParseInto;

        [SerializeField] private string _bracketStart;
        [SerializeField] private string _bracketEnd;

        public bool StartConditionFulfilled(string blockContent)
        {
            return blockContent.EndsWith(_bracketStart);
        }

        public bool EndConditionFulfilled(string blockContent)
        {
            return blockContent.EndsWith(_bracketEnd);
        }

    }
}