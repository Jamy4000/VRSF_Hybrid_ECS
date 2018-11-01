using Unity.Entities;
using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// 
    /// A generic System that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// </summary>
    public class BorderRendererSystem : ComponentSystem
    {
        struct Filter
        {
            public BorderRendererComponent BorderRenderer;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.BorderRenderer.CachedMeshes == null || e.BorderRenderer.BorderMaterial == null)
                    return;

                if (e.BorderRenderer.LastBorderAlpha != e.BorderRenderer.BorderAlpha && e.BorderRenderer.BorderMaterial != null)
                {
                    e.BorderRenderer.BorderMaterial.SetFloat("_Alpha", e.BorderRenderer.BorderAlpha);
                    e.BorderRenderer.LastBorderAlpha = e.BorderRenderer.BorderAlpha;
                }

                foreach (Mesh m in e.BorderRenderer.CachedMeshes)
                    Graphics.DrawMesh(m, e.BorderRenderer.Transpose, e.BorderRenderer.BorderMaterial, e.BorderRenderer.gameObject.layer, null, 0, null, false, false);
            }
        }
    }
}