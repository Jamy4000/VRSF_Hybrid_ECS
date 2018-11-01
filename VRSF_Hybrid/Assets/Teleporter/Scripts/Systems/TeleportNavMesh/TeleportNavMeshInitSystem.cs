using Unity.Entities;
using UnityEngine;

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

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            foreach (var e in GetEntities<Filter>())
            {
                InitValues(e);
            }
        }

        protected override void OnUpdate()
        {
        }

        private void InitValues(Filter e)
        {
            if (e.TeleportNavMesh.SelectableMesh == null)
                e.TeleportNavMesh.SelectableMesh = new Mesh();
            if (e.TeleportNavMesh._SelectableMeshBorder == null)
                e.TeleportNavMesh._SelectableMeshBorder = new BorderPointSet[0];
            
            e.TeleportNavMesh.AlphaShaderID = Shader.PropertyToID("_Alpha");

            if (e.TeleportNavMesh._GroundMaterialSource != null)
                e.TeleportNavMesh.GroundMaterial = new Material(e.TeleportNavMesh._GroundMaterialSource);

#if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
#endif
        }
    }
}