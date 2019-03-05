using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRSF.MoveAround.Teleport
{
    public class SceneObjectsInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public SceneObjectsComponent SceneObjects;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            SceneManager.sceneLoaded += OnSceneLoaded;

            foreach (var e in GetEntities<Filter>())
            {
                e.SceneObjects.StartCoroutine(InitValues(e));
            }
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        IEnumerator InitValues(Filter e)
        {
            while (!Utils.VRSF_Components.SetupVRIsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            try
            {
                e.SceneObjects.FadeComponent = Utils.VRSF_Components.VRCamera.GetComponentInChildren<TeleportFadeComponent>();
                e.SceneObjects.FadeComponent._FadingImage = e.SceneObjects.FadeComponent.GetComponent<UnityEngine.UI.Image>();
            }
            catch (System.Exception exception)
            {
                Debug.Log("VRSF : No Fade Component found on the VRCamera. Teleporting user without fade effect.\n" + exception.ToString());
            }

            e.SceneObjects._TeleportNavMesh = GameObject.FindObjectOfType<TeleportNavMeshComponent>();
            if (e.SceneObjects._TeleportNavMesh == null)
            {
                Debug.LogError("VRSF : You need to add a TeleportNavMeshComponent in your scene to be able to use the Teleport Feature.");
            }

            this.Enabled = false;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    e.SceneObjects.StartCoroutine(InitValues(e));
                }
            }
        }
    }
}