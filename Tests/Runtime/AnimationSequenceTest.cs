using System.Collections;
using MEMCore.Sequence.Messages;
using MEMCore.Sequence.Sequence;
using Sequence.Animation;
using NUnit.Framework;
using TinyMessenger;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Tests
{
    public class AnimationSequenceTest
    {
        private float updateDelay = 0.1f;
        private GameObject animation1Go;
        private GameObject animation2Go;
        private Animation animation1;
        private Animation animation2;
        private AnimationSequence animationSequence1;
        private AnimationSequence animationSequence2;

        private TinyMessengerHub eventBus;

        [SetUp]
        public void SetUp()
        {
            eventBus = new TinyMessengerHub();

            GameObject animation1Prefab = Resources.Load("TestObjects/Helper/Animation/Animation1Prefab") as GameObject; // What happens if a project imports this package and doesn't have these prefabs in its resources folder? (PH)
            GameObject prefabInstance1 = Object.Instantiate(animation1Prefab);
            animation1Go = Object.Instantiate(prefabInstance1);
            animation1 = animation1Go.GetComponent<Animation>();
            animationSequence1 = new AnimationSequence(animation1, "animationSequence1", SequenceType.Animation, eventBus);
            
            GameObject animation2Prefab = Resources.Load("TestObjects/Helper/Animation/Animation1Prefab") as GameObject; // What happens if a project imports this package and doesn't have these prefabs in its resources folder? (PH)
            GameObject prefabInstance2 = Object.Instantiate(animation2Prefab);
            animation2Go = Object.Instantiate(prefabInstance2);
            animation2 = animation2Go.GetComponent<Animation>();
            animationSequence2 = new AnimationSequence(animation2, "animationSequence2", SequenceType.Animation, eventBus);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
        
        /// <summary>
        /// Wait for Animation to be finished
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator PlayFull()
        {
            bool sequenceFinished = false;
            eventBus.Subscribe<SequenceFinishedMessage>(
                message => sequenceFinished = true
                );

            //Grant some additional time due to inaccuracy
            float animationLength = animation1.clip.length * 2;
            animationSequence1.Play();
            while (animationLength > 0)
            {
                animationSequence1.Update(updateDelay);
                animationLength -= updateDelay;
                yield return new WaitForSeconds( updateDelay);
            }
            Assert.True(sequenceFinished);
            yield return null;
        }
        
        /// <summary>
        /// Assure that Animation has not finished after half of the time
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator PlayHalf()
        {
            bool sequenceFinished = false;
            //animationSequence1.OnSequenceFinished += delegate { sequenceFinished = true; };
            eventBus.Subscribe<SequenceFinishedMessage>((message) => sequenceFinished = true);

            float animationLength = animation1.clip.length / 2f;
            animationSequence1.Play();
            while (animationLength > 0)
            {
                animationSequence1.Update(updateDelay);
                animationLength -= updateDelay;
                yield return new WaitForSeconds( updateDelay);
            }
            Assert.False(sequenceFinished);
            yield return null;
        }
        
        /// <summary>
        /// Test Pausing the animationSequence
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Pause()
        {
            Assert.False(animationSequence1.IsPlaying); 
            animationSequence1.Play();
            Assert.True(animationSequence1.IsPlaying); 
            animationSequence1.Pause();
            Assert.False(animationSequence1.IsPlaying); 
            yield return null;
        }
        
        /// <summary>
        /// Test UnPausing the animationSequence
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator UnPause()
        {
            Assert.False(animationSequence1.IsPlaying); 
            animationSequence1.Play();
            Assert.True(animationSequence1.IsPlaying); 
            animationSequence1.Pause();
            Assert.False(animationSequence1.IsPlaying); 
            animationSequence1.Play();
            Assert.True(animationSequence1.IsPlaying); 
            yield return null;
        }
        
        /// <summary>
        /// Test UnPausing the animationSequence
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Interrupt()
        {
            animationSequence1.Play(); 
            animationSequence1.Interrupt();
            Assert.False(animationSequence1.IsPlaying);
            Assert.False(animation1.enabled);
            yield return null;
        }
        
        /// <summary>
        /// Test Disposing
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Dispose()
        {
            animationSequence1.Dispose();
            Assert.False(animationSequence1.IsPlaying);
            Assert.False(animation1.enabled);
            yield return null;
        }
        
    }
}
