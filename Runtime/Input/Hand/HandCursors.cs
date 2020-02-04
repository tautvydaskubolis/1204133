using MEMCore.Input.Hand;
using UnityEngine;

namespace Input.Hand
{
    public class HandCursors: MonoBehaviour, IHandCursors
    {
        [SerializeField] private HandCursor leftHandCursor;
        [SerializeField] private HandCursor rightHandCursor;
        
        public IHandCursor LeftHandCursor { get; private set; }
        public IHandCursor RightHandCursor { get; private set; }

        private void Awake()
        {
            LeftHandCursor = leftHandCursor;
            RightHandCursor = rightHandCursor;
        }
        
        private void OnDestroy()
        {
            Destroy(this);
        }
        
    }
}