using System.Collections.Generic;
using MEMCore.Sequence.Text;
using MEMCore.Utility;
using MEMCoreData.Sequence.Text;

namespace MEMModuleMenuView.Sequence.Text
{
    public class ScreenTextManager: ScreenTextManagerBase
    {
        private List<ScreenText> screenTexts;
        private int curScreenTextIndex;
        private float timePassed;
        private float lastTime;

        public override void SetText(string text, ITextField textField, int durationInMilliSeconds, int maxChars, double fontCharSize = FontCharSize.Default)
        {
            this.durationInMilliSeconds = durationInMilliSeconds;
            this.textField = textField;
            
            //Create ScreenTexts
            int maxLength = (int) (maxChars * fontCharSize);
            screenTexts = CreateScreenTexts(text, maxLength, durationInMilliSeconds);
        }

        public override void Display()
        {
            Restart();
        }

        /// <summary>
        /// Create texts that logically split and are as close below the maxLength as possible
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        private List<ScreenText> CreateScreenTexts(string text, int maxLength, int durationInMilliSeconds)
        {
            //Split sentences based on syntax
            List<string> sentences = StringUtility.SplitByOperators(text);
            
            //Split sentences that are too long on whitespaces
            List<string> shortenedSentences = StringUtility.ShortenSentences(sentences, maxLength, sentences.Count);
            
            //Combine sentences that are too short with their upcoming text & add them to a new list
            //Does nothing most of time but would concatenate shorter sentences 
            List<string> combinedSentences = StringUtility.CombineSentences(shortenedSentences, maxLength);
            
            //Create ScreenTexts by assigning duration to strings based on their length
             return AssignDurations(combinedSentences, durationInMilliSeconds, text.Length);
        }

        /// <summary>
        /// Create a list of ScreenTexts from given strings & assign each string a time treshold after which he has to be displayed
        /// until its treshold (EndTime) is reached
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="durationInMilliSeconds"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        private List<ScreenText> AssignDurations(List<string> texts, int durationInMilliSeconds, int charCount)
        {
            List<ScreenText> screenTexts = new List<ScreenText>();
            float timeperChar = durationInMilliSeconds / charCount;
            float triggerTime = 0;
            foreach (string text in texts)
            {
                triggerTime += text.Length * timeperChar;
                ScreenText screenText = new ScreenText(text, triggerTime);
                screenTexts.Add(screenText);
            }

            return screenTexts;
        }

        public override void Update(float deltaTime)
        {
            if (!IsPlaying)
                return;
            
            timePassed += deltaTime;

            UpdateDisplayedScreenText();
        }

        /// <summary>
        /// Check if a ScreenText exceeded its EndTime
        /// </summary>
        private void UpdateDisplayedScreenText()
        {
            if (curScreenTextIndex > screenTexts.Count-1)
            {
                base.ScreenTextFinished();
                return;
            }
            
            ScreenText screenText = screenTexts[curScreenTextIndex];
            if (timePassed < screenText.EndTime)
                return;
            
            //Increment Index for active ScreenText & check for out of bounds
            if (curScreenTextIndex >= screenTexts.Count)
            {
                //ToDo throw error as this should have already triggered in duration check
                return;
            }
            
            textField.SetText(screenTexts[curScreenTextIndex].Text);
            curScreenTextIndex++;
        }

        public override void Pause()
        {
            IsPlaying = false;
        }

        public override void Continue()
        {
            IsPlaying = true;
        }

        /// <summary>
        /// Set base values for Update Loop & set first text
        /// </summary>
        public override void Restart()
        {
            IsPlaying = true;
            timePassed = 0;
            curScreenTextIndex = 0;
            textField.SetText(screenTexts[curScreenTextIndex].Text);
        }

        public override void Interrupt()
        {
            Dispose();
        }
        
        public override void Dispose()
        {
            IsPlaying = false;
            screenTexts = null;
        }
    }
}