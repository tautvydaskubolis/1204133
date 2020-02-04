using MEMCore.Sequence.Sequence;
using TinyMessenger;
using UnityEngine;

namespace MEMModuleMenuView.Sequence.Audio
{
    public class AudioSequence: SequenceBase
    {
        private readonly AudioSource audioSource;
        private bool audioStarted;

        public AudioSequence(AudioSource audioSource, string name, SequenceType sequenceType, ITinyMessengerHub eventBus) : base(name, sequenceType, eventBus)
        {
            this.audioSource = audioSource;
        }
        
        public override void Play()
        {
            IsPlaying = true;
            if (audioStarted)
            {
                audioSource.UnPause();
                return;
            }
            
            audioStarted = true;
            audioSource.Play();
        }
        
        public override void Update(float deltaTime)
        {
            if (audioSource.isPlaying) return;
            
            //Audio finished as it stopped playing
            SequenceFinished();
        }
        
        public override void Pause()
        {
            IsPlaying = false;
            audioSource.Pause();
        }

        public override void Interrupt()
        {
            Dispose();
        }

        public override void Dispose()
        {
            IsPlaying = false;
            audioSource.Stop();
            audioSource.enabled = false;
        }
    }
}