using System;
using OpenEoB.Config.Graphics;

namespace OpenEoB.Config
{
    [System.Serializable]
    public class NpcAnimationDatum
    {
        public NpcState NpcState;
        public float AnimationFrameTime = 0.4f;
        public AnimationConfig AnimationGeneral;
        public AnimationConfig AnimationNorth;
        public AnimationConfig AnimationSouth;
        public AnimationConfig AnimationEast;
        public AnimationConfig AnimationWest;

        public AnimationConfig GetPlayerFacingAnimation()
        {
            if (AnimationSouth != null)
            {
                return AnimationSouth;
            }
            if (AnimationGeneral != null)
            {
                return AnimationGeneral;
            }
            if (AnimationEast != null)
            {
                return AnimationEast;
            }
            if (AnimationWest != null)
            {
                return AnimationWest;
            }
            if (AnimationNorth != null)
            {
                return AnimationNorth;
            }
            throw new Exception("Could not find Player Facing animation");
        }
    }
}