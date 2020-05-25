using System.Collections.Generic;
using UnityEngine;

namespace OpenEoB.Views.AnimationViews
{
    public class AnimationView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private List<Texture> _textures;
        [SerializeField] private float _timeBetweenSprites;

        private int _currentSpriteIndex;
        [SerializeField] private bool _animationPlaying;
        private float _animationTimer;

        private void Start()
        {
            ResetAnimation();
        }
        
        private void Update()
        {
            if (_animationPlaying)
            {
                _animationTimer += Time.deltaTime;
                if (_animationTimer >= _timeBetweenSprites)
                {
                    _animationTimer -= _timeBetweenSprites;
                    if (_currentSpriteIndex + 1 >= _textures.Count)
                    {
                        _animationPlaying = false;
                        return;
                    }

                    _currentSpriteIndex++;
                    _renderer.material.mainTexture = _textures[_currentSpriteIndex];
                }
            }
        }

        public void ResetAnimation()
        {
            _animationPlaying = false;
            _currentSpriteIndex = 0;
            _renderer.material.mainTexture = _textures[_currentSpriteIndex];
        }

        public void PlayAnimation()
        {
            _animationPlaying = true;
            _animationTimer = 0;
        }
    }
}
