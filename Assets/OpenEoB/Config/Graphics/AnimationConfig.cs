using System.Collections.Generic;
using UnityEngine;

namespace OpenEoB.Config.Graphics
{
    [CreateAssetMenu(fileName = "AnimationConfig", menuName = "Config/AnimationConfig", order = 1)]
    public class AnimationConfig : ScriptableObject
    {
        [SerializeField] private List<Texture> _frames;

        public int FrameCount => _frames.Count;

        public Texture GetFrame(int frameIndex)
        {
            return _frames[frameIndex];
        }
    }
}