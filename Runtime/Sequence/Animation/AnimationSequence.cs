using UnityEngine;
using MEMCore.Sequence.Sequence;
using TinyMessenger;

namespace Sequence.Animation
{
    public class AnimationSequence: SequenceBase
    {
        private readonly UnityEngine.Animation animation;
        private bool animationStarted;
        private float animationSpeed;

        public AnimationSequence(UnityEngine.Animation animation, string name, SequenceType sequenceType, ITinyMessengerHub eventBus) : base(name, sequenceType, eventBus)
        {
            this.animation = animation;
        }

        public override void Update(float deltaTime)
        {
            if (animation.isPlaying) return;
            
            SequenceFinished();
        }

        public override void Play()
        {
            IsPlaying = true;
            if (animationStarted)
            {
                animation[animation.clip.name].speed = animationSpeed;
                return;
            }

            animation.enabled = true;
            animationStarted = true;
            animation.Play(animation.clip.name);
        }

        public override void Pause()
        {
            AnimationState animationState = animation[animation.clip.name];
            animationSpeed = animationState.speed;
            animationState.speed = 0;
            IsPlaying = false;
        }

        public override void Interrupt()
        {
            Dispose();
        }

        public override void Dispose()
        {
            IsPlaying = false;
            animation.enabled = false;
        }
    }
}