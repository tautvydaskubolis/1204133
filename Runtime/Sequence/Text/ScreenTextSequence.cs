using MEMCore.Sequence.Sequence;
using MEMCore.Sequence.Text;
using MEMCore.Sequence.Text.Messages;
using TinyMessenger;

namespace MEMModuleMenuView.Sequence.Text
{
    public class ScreenTextSequence: SequenceBase
    {
        private readonly IScreenTextManager screenTextManager;
        private bool textStarted;

        private TinyMessageSubscriptionToken sequenceFinishedSubscription;
        
        public ScreenTextSequence(IScreenTextManager screenTextManager, string name, SequenceType sequenceType, ITinyMessengerHub eventBus) : base(name, sequenceType, eventBus)
        {
            this.screenTextManager = screenTextManager;

            sequenceFinishedSubscription = EventBus?.Subscribe<ScreenTextFinishedMessage>((message) => SequenceFinished()); 
        }
        
        public override void Play()
        {
            IsPlaying = true;
            if (textStarted)
            {
                screenTextManager.Continue();
                return;
            }

            textStarted = true;
            screenTextManager.Display();
        }

        public override void Update(float deltaTime)
        {
            screenTextManager.Update(deltaTime);
        }

        public override void Pause()
        {
            screenTextManager.Pause();
            IsPlaying = false;
        }

        public override void Interrupt()
        {
            Dispose();
        }

        public override void Dispose()
        {
            screenTextManager.Dispose();
            IsPlaying = false;

            if (sequenceFinishedSubscription != null)
            {
                EventBus?.Unsubscribe(sequenceFinishedSubscription);
            }
        }
    }
}