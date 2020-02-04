using MEMCore;
using MEMCore.Input.Interactables;
using MEMCore.ModuleState;
using MEMCore.ModuleState.Interfaces;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Input.Buttons
{
    [RequireComponent(typeof(Button))]
    public class LoadModuleButton: MonoBehaviour, ILoadModuleSelectable
    {
        [SerializeField] private Text text;
        [SerializeField] private string moduleAssemblyName;
        [SerializeField] private string moduleClassQualifier; // e.g. "Example.ExampleModuleLogic";
        [SerializeField] private string moduleName;
        public string ModuleName
        {
            set
            {
                moduleName = value;
                text.text = moduleName;
            }
        }

        public float TimeToActivateInSeconds { get; set; } = 2f; //ToDo
        private Button button;
        private float timePassed;
        private Color baseColor;
        private bool activated;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            baseColor = button.image.color;
        }
        public void ClickDown()
        {
            if (activated) return;
            
            Debug.Log("ClickDown: " + timePassed);
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
            
            DiContainer container = MemoreCoreApplication.Container;
            try
            {
                var assembly = Assembly.Load(moduleAssemblyName);
                var fullQualifier = moduleAssemblyName + "." + moduleClassQualifier;
                var type = assembly?.GetType(fullQualifier);

                if (type != null)
                {
                    //Initialize ExampleModuleLogic via Injection //ToDo deprecated?
                    if (!container.HasBinding(type))
                        container.Bind(type).AsTransient();

                    var exampleModuleLogic = container.Resolve(type) as ModuleStateBase;
                    IModuleStateSystem moduleStateSystem = MemoreCoreApplication.Container.Resolve<IModuleStateSystem>(); //ToDo use EventBus instead
                    moduleStateSystem.PushModuleState(exampleModuleLogic);
                }
                else
                {
                    Debug.LogError("type: " + fullQualifier + " can't be found in assembly: " + moduleAssemblyName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        
        private void OnDestroy()
        {
            Destroy(this);
        }
    }
}