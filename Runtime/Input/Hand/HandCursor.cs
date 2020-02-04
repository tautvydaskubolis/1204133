using MEMCore.Input.Hand;
using MEMCore.Input.Interactables;
using MEMCoreData.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Input.Hand
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class HandCursor: MonoBehaviour, IHandCursor
    {
        public HandStatus HandStatus { get; private set; }
        public HandSide HandSide => handSide;
        
        [SerializeField] private HandSide handSide;
        [SerializeField] private Image handOpen;
        [SerializeField] private Image handClosed;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ISelectable selectable = collision.GetComponent<ISelectable>();
            if (selectable == null)
                return;
            
            selectable.Enter();
        }

        private HandStatus lastHandStatus;//todo move
        private void OnTriggerStay2D(Collider2D collision)
        {
            //Check if collided object is interactable
            ISelectable selectable = collision.GetComponent<ISelectable>();
            if (selectable == null)
                return;

            if (HandStatus == HandStatus.Closed)
            {
                selectable.ClickDown();
            }
            else if(lastHandStatus == HandStatus.Closed)
            {
                selectable.ClickUp();
            }
            lastHandStatus = HandStatus;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            //Check if collided object is interactable
            ISelectable selectable = collision.GetComponent<ISelectable>();
            if (selectable == null)
                return;
            
            selectable.Exit();
        }

        public void SetPosition(float x, float y)
        {
            Vector3 vec3 = new Vector3(x, y, transform.position.z);
            transform.position = vec3;
        }
        
        public void SetHandStatus(HandStatus handStatus)
        {
            HandStatus = handStatus;
            //Update hand
            if(handStatus == HandStatus.Open)
                Open();
            else if(HandStatus == HandStatus.Closed)
                Close();
            else if(HandStatus == HandStatus.Hidden)
                Hide();

        }

        public void Open()
        {
            handClosed.enabled = false;
            handOpen.enabled = true;
        }

        public void Close()
        {
            handOpen.enabled = false;
            handClosed.enabled = true;
        }

        public void Hide()
        {
            handOpen.enabled = false;
            handClosed.enabled = false;
        }

        public void SetColor(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out var color);
            handOpen.color = color;
            handClosed.color = color;
        }

        public void Destroy()
        {
            Destroy(this);
        }
    }
}