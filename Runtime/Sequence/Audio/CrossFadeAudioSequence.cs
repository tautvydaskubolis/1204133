using MEMCore.Sequence.Sequence;
using System.Threading;
using TinyMessenger;
using UnityEngine;

namespace MEMModuleMenuView.Sequence.Audio
{
    public class CrossFadeAudioSequence: SequenceBase, ICrossFadeable
    {
        public ICrossFadeable CrossFadeable { get; }
        
        private readonly AudioSource audioSource;
        private bool audioStarted;

        public CrossFadeAudioSequence(AudioSource audioSource, string name, SequenceType sequenceType, ITinyMessengerHub eventBus) : base(name, sequenceType, eventBus)
        {
            this.audioSource = audioSource;
        }
        
        public override void Play()
        {
            if (audioStarted)
            {
                audioSource.UnPause();
                return;
            }
            
            audioStarted = true;
            //ToDo Try to avoid using a thread
            new Thread(OnAudioClipFinished).Start();
            
            //ToDo implement Crossfade
            audioSource.Play();
        }
        
        private void OnAudioClipFinished()
        {
            //Convert Sleep time from seconds to Milliseconds
            Thread.Sleep((int)audioSource.clip.length*1000);
            SequenceFinished();
        }
        
        public override void Pause()
        {
            audioSource.Pause();
        }

        public override void Interrupt()
        {
            Dispose();
        }

        public override void Dispose()
        {
            audioSource.Stop();
        }

    }
}