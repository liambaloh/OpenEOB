using System;
using System.Collections.Generic;
using OpenEoB.Views;
using UnityEngine;

namespace OpenEoB.Config
{
    [CreateAssetMenu(fileName = "NpcConfig", menuName = "Config/NpcConfig", order = 1)]
    public class NpcConfig : ScriptableObject
    {
        [SerializeField] private List<NpcDatum> _npcDatums;

        public NpcDatum GetNpcDatum(string id)
        {
            foreach (var npcDatum in _npcDatums)
            {
                if (npcDatum.Id == id)
                {
                    return npcDatum;
                }
            }

            throw new Exception("Could not find tile object with id " + id);
        }
    }
}