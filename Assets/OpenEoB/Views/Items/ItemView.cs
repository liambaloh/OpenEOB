using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB.Views.Items
{
    public class ItemView
    {
        private readonly ItemDatum _itemDatum;
        
        public string Id => _itemDatum.Id;
        public Sprite ItemSprite => _itemDatum.ItemSprite;

        public ItemView(ItemDatum itemDatum)
        {
            _itemDatum = itemDatum;
        }
    }
}