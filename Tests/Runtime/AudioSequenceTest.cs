using System.Collections;
using MEMCore.Sequence.Messages;
using MEMCore.Sequence.Sequence;
using MEMModuleMenuView.Sequence.Audio;
using NUnit.Framework;
using TinyMessenger;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Tests
{
    public class AudioSequenceTest
    {
        private float updateDelay = 0.01f;
        private GameObject audio1Go;
        private GameObject audio2Go;
        private AudioSource audioSource1;
        private AudioSource audioSource2;
        private AudioSequence audioSequence1;
        private AudioSequence audioSequence2;

        private TinyMessengerHub eventBus;

        [SetUp]
        public void SetUp()
        {
            eventBus = new TinyMessengerHub();

            GameObject audio1Prefab = Resources.Load("TestObjects/Helper/Audio/Audio1Prefab") as GameObject; // What happens if a project imports this package and doesn't have these prefabs in its resources folder? (PH)
            GameObject prefabInstance1 = Object.Instantiate(audio1Prefab);
            audio1Go = Object.Instantiate(prefabInstance1);
            audioSource1 = audio1Go.GetComponent<AudioSource>();
            audioSequence1 = new AudioSequence(audioSource1, "audioSequence1", SequenceType.Audio, eventBus);
            
            GameObject audio2Prefab = Resources.Load("TestObjects/Helper/Audio/Audio1Prefab") as GameObject; // What happens if a project imports this package and doesn't have these prefabs in its resources folder? (PH)
            GameObject prefabInstance2 = Object.Instantiate(audio2Prefab);
            audio2Go = Object.Instantiate(prefabInstance2);
            audioSource2 = audio2Go.GetComponent<AudioSource>();
            audioSequence2 = new AudioSequence(audioSource2, "audioSequence2", SequenceType.Audio, eventBus);
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
            eventBus.Subscribe<SequenceFinishedMessage>((message) => sequenceFinished = true);

            float audioLength = audioSource1.clip.length;
            audioSequence1.Play();
            while (audioLength > 0)
            {
                audioSequence1.Update(updateDelay);
                audioLength -= updateDelay;
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
            eventBus.Subscribe<SequenceFinishedMessage>((message) => sequenceFinished = true);

            float audioLength = audioSource1.clip.length / 2f;
            audioSequence1.Play();
            while (audioLength > 0)
            {
                audioSequence1.Update(updateDelay);
                audioLength -= updateDelay;
                yield return new WaitForSeconds( updateDelay );
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
            Assert.False(audioSequence1.IsPlaying); 
            audioSequence1.Play();
            Assert.True(audioSequence1.IsPlaying); 
            audioSequence1.Pause();
            Assert.False(audioSequence1.IsPlaying); 
            yield return null;
        }
        
        /// <summary>
        /// Test UnPausing the animationSequence
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator UnPause()
        {
            Assert.False(audioSequence1.IsPlaying); 
            audioSequence1.Play();
            Assert.True(audioSequence1.IsPlaying); 
            audioSequence1.Pause();
            Assert.False(audioSequence1.IsPlaying); 
            audioSequence1.Play();
            Assert.True(audioSequence1.IsPlaying); 
            yield return null;
        }
        
        /// <summary>
        /// Test UnPausing the animationSequence
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Interrupt()
        {
            audioSequence1.Play(); 
            audioSequence1.Interrupt();
            Assert.False(audioSequence1.IsPlaying);
            Assert.False(audioSource1.enabled);
            yield return null;
        }
        
        /// <summary>
        /// Test Disposing
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Dispose()
        {
            audioSequence1.Dispose();
            Assert.False(audioSequence1.IsPlaying);
            Assert.False(audioSource1.enabled);
            yield return null;
        }

        
    }
}
