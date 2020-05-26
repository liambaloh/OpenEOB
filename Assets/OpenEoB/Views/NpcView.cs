using OpenEoB.Config;
using UnityEngine;

namespace OpenEoB.Views
{
    public class NpcView : MonoBehaviour
    {
        [SerializeField] private AbstractPlayerTileEnterView _playerTileEnterView;
        [SerializeField] private AbstractPlayerTileBumpView _playerTileBumpView;
        [SerializeField] private Renderer _npcRenderer;

        private NpcDatum _datum;
        private NpcState _state;
        private int _animationFrame;
        private float _animationFrameTimer;

        public void Setup(NpcDatum npcDatum)
        {
            _datum = npcDatum;
            SetNpcState(_datum.InitialNpcState);

            _npcRenderer.material = new Material(_npcRenderer.material);
        }

        private void Update()
        {
            _animationFrameTimer += Time.deltaTime;

            var stateAnimation = _datum.GetAnimationForState(_state);
            if (_animationFrameTimer >= stateAnimation.AnimationFrameTime)
            {
                _animationFrameTimer -= stateAnimation.AnimationFrameTime;
                _animationFrame++;
                _animationFrame %= stateAnimation.GetPlayerFacingAnimation().FrameCount;
                _npcRenderer.material.mainTexture = stateAnimation.GetPlayerFacingAnimation().GetFrame(_animationFrame);
            }
        }

        public bool CanPlayerEnterTile()
        {
            return _playerTileEnterView.CanPlayerEnterTile();
        }

        public void Bump()
        {
            _playerTileBumpView.Bump();
        }

        protected void SetNpcState(NpcState npcState)
        {
            _state = npcState;

            var animationForNewState = _datum.GetAnimationForState(npcState);
            var framesInNewStateAnimation = animationForNewState.GetPlayerFacingAnimation().FrameCount;
            var frameTimeForNewStateAnimation = animationForNewState.AnimationFrameTime;

            _animationFrame = Random.Range(0, framesInNewStateAnimation);
            _animationFrameTimer = Random.Range(0f, frameTimeForNewStateAnimation);
        }
    }
}