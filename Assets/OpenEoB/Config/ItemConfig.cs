using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenEoB.Config
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Config/ItemConfig", order = 1)]
    public class ItemConfig : ScriptableObject
    {
        [SerializeField]private List<ItemDatum> _items;

        public ItemDatum GetItemDatum(string id)
        {
            foreach (var itemDatum in _items)
            {
                if (itemDatum.Id == id)
                {
                    return itemDatum;
                }
            }

            throw new Exception("Could not find item with id "+id);
        }
    }
}