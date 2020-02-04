using System;
using MEMCore;
using MEMCore.Input.Interactables;
using TinyMessenger;
using UnityEngine;
using UnityEngine.UI;

namespace Input.Buttons
{
    [RequireComponent(typeof(Button))]
    public class ColorButton: MonoBehaviour, ISelectable
    {
        public float TimeToActivateInSeconds { get; set; } = 3f; //ToDo
        private Button button;
        private ITinyMessengerHub eventBus;
        private float timePassed;
        private Color baseColor;
        private bool activated;
        
        private void Awake()
        {
            eventBus = MemoreCoreApplication.Container.Resolve<ITinyMessengerHub>();
            button = GetComponent<Button>();
            baseColor = button.image.color;
        }
        
        public void ClickDown()
        {
            if (activated) return;
            
            button.image.color = Color.green;
            timePassed += Time.deltaTime;
            TryActivate();
        }

        public void ClickUp()
        {
            if (activated) return;
            
            button.image.color = Color.cyan;
            timePassed = 0;
        }

        public void Enter()
        {
            if (activated) return;
            
            button.image.color = Color.cyan;
        }

        public void Hover()
        {
        }

        public void Exit()
        {
            if (activated) return;
            
            button.image.color = baseColor;
            timePassed = 0;
        }

        /// <summary>
        /// Check if TimeToActivate treshold has been succeeded & execute function
        /// </summary>
        private void TryActivate()
        {
            if (timePassed < TimeToActivateInSeconds)
                return;

            activated = true;
            button.image.color = Color.gray;
            //eventBus.Publish(new ColorButtonMessage(this, Color.green.ToString()));
        }

        private void OnDestroy()
        {
            Destroy(this);
        }
    }
}