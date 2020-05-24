using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OpenEoB.Views.TileObjectViews
{
    public class PlayerSpawnPoint : TileObjectView
    {
        protected override void Start()
        {
            base.Start();

            var playerView = FindObjectOfType<PlayerView>();
            playerView.Teleport(Tile);
        }
    }
}
