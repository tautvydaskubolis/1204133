using MEMCore.Sequence.Sequence;
using System.Collections.Generic;
using TinyMessenger;
using UnityEngine;

namespace MEMModuleMenuView.Sequence.Animation
{
    public class CrossFadeAnimationSequence: SequenceBase, ICrossFadeable
    {
        public ICrossFadeable CrossFadeable { get; }
        
        private readonly UnityEngine.Animation animation;
        private readonly UnityEngine.Animation toCrossFade;
        private readonly float fadeLength;
        private bool animationStarted;

        public CrossFadeAnimationSequence(UnityEngine.Animation animation, UnityEngine.Animation toCrossFade, float fadeLength, string name, SequenceType sequenceType, ITinyMessengerHub eventBus) : base(name, sequenceType, eventBus)
        {
            this.animation = animation;
            this.toCrossFade = toCrossFade;
            this.fadeLength = fadeLength;
        }
        
        public override void Play()
        {
            if (animationStarted)
            {
                animation.enabled = true;
                toCrossFade.enabled = true;
                return;
            }
            
            animationStarted = true;
            AddAnimationEvents();
            //ToDo i am not sure if CrossFade starts on 1-fadeLength or immediately
            //ToDo if it starts immediately just add another animationEvent on fadeLEngth on animation clip with cb like below
            animation.CrossFade(toCrossFade.name, fadeLength);
        }
        
        /// <summary>
        /// Add an AnimationEvent to the end of the animation
        /// </summary>
        private void AddAnimationEvents()
        {
            AnimationEvent onFinishedEvent = new AnimationEvent();
            onFinishedEvent.functionName = "OnSequenceFinished";
            onFinishedEvent.time = animation.clip.length;

            List<AnimationEvent> events = new List<AnimationEvent>(animation.clip.events);
            events.Add(onFinishedEvent);
            
            animation.clip.events = events.ToArray();
        }
        
        public override void Pause()
        {
            animation.enabled = false;
            toCrossFade.enabled = false;
        }

        public override void Interrupt()
        {
            Dispose();
        }

        public override void Dispose()
        {
            animation.enabled = false;
            toCrossFade.enabled = false;
        }
    }
}