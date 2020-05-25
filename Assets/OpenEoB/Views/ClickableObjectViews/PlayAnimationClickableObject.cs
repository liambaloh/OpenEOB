using OpenEoB.Views.AnimationViews;
using UnityEngine;

namespace OpenEoB.Views.ClickableObjectViews
{
    public class PlayAnimationClickableObject : AbstractClickableObjectView
    {
        [SerializeField] private AnimationView _animationView;

        protected override void Clicked()
        {
            _animationView.PlayAnimation();
        }
    }
}
