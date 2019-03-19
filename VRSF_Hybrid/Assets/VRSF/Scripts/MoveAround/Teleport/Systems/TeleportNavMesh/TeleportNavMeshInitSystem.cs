using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Init the values for the Teleport Nav Mesh Classes
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class TeleportNavMeshInitSystem : ComponentSystem
    {
        private struct Filter
        {
            public TeleportNavMeshComponent TeleportNavMesh;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += InitValues;
            base.OnCreateManager();
        }

        protected override void OnUpdate() { }
        
        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= InitValues;
            base.OnDestroyManager();
        }

        private void InitValues(OnSetupVRReady setupVRReady)
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.TeleportNavMesh.SelectableMesh == null)
                    e.TeleportNavMesh.SelectableMesh = new Mesh();

                e.TeleportNavMesh.AlphaShaderID = Shader.PropertyToID("_Alpha");

                if (e.TeleportNavMesh._GroundMaterialSource != null)
                    e.TeleportNavMesh.GroundMaterial = new Material(e.TeleportNavMesh._GroundMaterialSource);
            }

#if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
#endif
        }
    }
}