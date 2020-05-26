using System;
using System.Collections.Generic;
using OpenEoB.Views;
using UnityEngine;

namespace OpenEoB.Config
{
    [System.Serializable]
    public class NpcDatum
    {
        public string Id;
        public NpcView Prefab;
        public NpcState InitialNpcState;

        [SerializeField] private List<NpcAnimationDatum> _animations;

        public NpcAnimationDatum GetAnimationForState(NpcState npcState)
        {
            foreach (var npcAnimationDatum in _animations)
            {
                if (npcAnimationDatum.NpcState == npcState)
                {
                    return npcAnimationDatum;
                }
            }

            throw new Exception("Could not find NPC Animation for NPC state " + npcState);
        }
    }
}