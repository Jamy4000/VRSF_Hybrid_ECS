using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRSF.MoveAround.Teleport
{
    public class ParaboleAnimatorInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public ParaboleAnimatorComponent NavMeshAnimator;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            SceneManager.sceneLoaded += OnSceneLoaded;

            foreach (var e in GetEntities<Filter>())
            {
                InitValues(e);
            }

            this.Enabled = false;
        }
        
        protected override void OnUpdate() { }

        void InitValues(Filter e)
        {
            // Set some standard variables for the TeleportNavMeshComponent
            e.NavMeshAnimator._NavmeshAnimator = e.NavMeshAnimator.GetComponent<Animator>();
            e.NavMeshAnimator._EnabledAnimatorID = Animator.StringToHash("Enabled");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            foreach (var e in GetEntities<Filter>())
            {
                InitValues(e);
            }
        }
    }
}