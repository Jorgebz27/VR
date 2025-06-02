using System;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
    public class XRPullInteractable : XRBaseInteractable
    {
        public event Action<float> PullActionReleased;
        public event Action<float> PullUpdated;
        public event Action PullStarted;
        public event Action PullEnded;
        
        public ArrowLauncherScript launcher;

        [Header("Pull Settings")] 
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private GameObject notch;
        

        public float pullAmount { get; private set; } = 0.0f;
        
        private LineRenderer lineRenderer;
        private IXRSelectInteractor pullInteractor = null;

        protected override void Awake()
        {
            base.Awake();
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void SetPullInteractor(SelectEnterEventArgs args)
        {
            Debug.Log("Pull");
            pullInteractor = args.interactorObject;
            PullStarted?.Invoke();
        }

        public void Release()
        {
            launcher = FindFirstObjectByType<ArrowLauncherScript>();
            Debug.Log("Release");
            PullActionReleased?.Invoke(pullAmount);
            float finalPullAmount = pullAmount;
            PullEnded?.Invoke();
            pullInteractor = null;
            pullAmount = 0f;
            notch.transform.localPosition = new Vector3(notch.transform.localPosition.x, notch.transform.localPosition.y, 0f);
            UpdateString();
            if (launcher != null)
            {
                launcher.Release(finalPullAmount);
                
            }
            else
            {
                Debug.LogError("Not found");
            }
            
        }
        
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected & pullInteractor != null)
                {
                    Vector3 pullPosition = pullInteractor.GetAttachTransform(this).position;
                    float previousPull = pullAmount;
                    pullAmount = CalculatePull(pullPosition);

                    if (previousPull != pullAmount)
                    {
                        PullUpdated?.Invoke(pullAmount);
                    }
                    
                    UpdateString();
                    HandleHaptics();
                }
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            SetPullInteractor(args);
        }
        
        private float CalculatePull(Vector3 pullPosition)
        {
            Vector3 pullDirection = pullPosition - startPoint.position;
            Vector3 targetDirection = endPoint.position - startPoint.position;
            float maxLength = targetDirection.magnitude;
    
            targetDirection.Normalize();
            float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
            return Mathf.Clamp(pullValue, 0,1);
        }
        
        private void UpdateString()
        {
            Vector3 linePosition = Vector3.Lerp(startPoint.localPosition, endPoint.localPosition, pullAmount);
            notch.transform.localPosition = linePosition;
            lineRenderer.SetPosition(1, linePosition);
        }

        private void HandleHaptics()
        {
            if (pullInteractor != null && pullInteractor is XRBaseInputInteractor controllerInteractor)
            {
                controllerInteractor.SendHapticImpulse(pullAmount, 0.1f);
            }
        }
    }
}
