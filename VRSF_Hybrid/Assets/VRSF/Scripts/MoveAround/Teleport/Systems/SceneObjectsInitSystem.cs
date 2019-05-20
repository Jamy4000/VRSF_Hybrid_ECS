using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    public class SceneObjectsInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public SceneObjectsComponent SceneObjects;
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
            foreach (var e in GetEntities<Filter>())
            {
                try
                {
                    e.SceneObjects.FadeComponent = VRSF_Components.VRCamera.GetComponentInChildren<TeleportFadeComponent>();
                    e.SceneObjects.FadeComponent._FadingImage = e.SceneObjects.FadeComponent.GetComponent<UnityEngine.UI.Image>();
                }
                catch (System.Exception exception)
                {
                    Debug.Log("<b>[VRSF] :</b> No Fade Component found on the VRCamera. Teleporting user without fade effect.\n" + exception.ToString());
                }

                e.SceneObjects._TeleportNavMesh = GameObject.FindObjectOfType<TeleportNavMeshComponent>();

                if (e.SceneObjects._TeleportNavMesh == null)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to add a TeleportNavMeshComponent in your scene to be able to use the Teleport Feature.");
                }
            }
        }
    }
}