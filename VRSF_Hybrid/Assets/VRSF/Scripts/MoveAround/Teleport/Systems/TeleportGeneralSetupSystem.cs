using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils.ButtonActionChoser;
using VRSF.Interactions;

namespace VRSF.MoveAround.Teleport
{
    public class TeleportGeneralSetupSystem : ComponentSystem
    {
        private struct Filter
        {
            public TeleportGeneralComponent TeleportGeneral;
            public BACGeneralComponent BACGeneral;
        }

        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += InitValues;
            base.OnCreateManager();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= InitValues;
        }

        void InitValues(OnSetupVRReady setupVRReady)
        {
            InteractionVariableContainer interactionVariable = InteractionVariableContainer.Instance;

            foreach (var e in GetEntities<Filter>())
            {
                switch (e.BACGeneral.ButtonHand)
                {
                    case Core.Controllers.EHand.LEFT:
                        e.TeleportGeneral.RaycastHitVar = interactionVariable.LeftHit;
                        e.TeleportGeneral.RayVar = interactionVariable.LeftRay;
                        break;
                    case Core.Controllers.EHand.RIGHT:
                        e.TeleportGeneral.RaycastHitVar = interactionVariable.RightHit;
                        e.TeleportGeneral.RayVar = interactionVariable.RightRay;
                        break;
                    default:
                        Debug.LogError("[b]VRSF :[/b] Please specifiy a correct hand on the BAC General Component for the object " + e.BACGeneral.transform.name, e.BACGeneral.gameObject);
                        break;
                }
            }
        }
    }
}