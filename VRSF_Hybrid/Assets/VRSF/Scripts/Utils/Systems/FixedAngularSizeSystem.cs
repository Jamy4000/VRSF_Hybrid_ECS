using Unity.Entities;
using UnityEngine;

namespace VRSF.Utils.Hybrid
{
     public class FixedAngularSizeSystem : ComponentSystem
     {
        private struct Components
        {
            public Transform transform;
            public FixedAngularSizeComponents AngularSize;
        }

        private float startingDistance;
        private Vector3 startingScale;
        private GameObject CameraRig;

        protected override void OnUpdate()
        {
            foreach (var comp in GetEntities<Components>())
            {
                float distanceToHologram = Vector3.Distance(CameraRig.transform.position, comp.transform.position);
                float curvedRatio = 1.0f - startingDistance * comp.AngularSize.SizeRatio;
                comp.transform.localScale = startingScale * (distanceToHologram * comp.AngularSize.SizeRatio + curvedRatio);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private void Start()
        {
            foreach (var comp in GetEntities<Components>())
            {
                CameraRig = VRSF.Utils.VRSF_Components.CameraRig;
                // Calculate the XYZ ratios for the transform's localScale over its initial distance from the camera.

                startingDistance = Vector3.Distance(CameraRig.transform.position, comp.transform.position);

                startingScale = comp.transform.localScale;

                if (comp.AngularSize.SizeRatio == 0)
                {
                    // Avoid 0 division
                    if (startingDistance > 0.0f)
                    {
                        // Set to a linear scale ratio
                        comp.AngularSize.SizeRatio = 1.0f / startingDistance;
                    }
                }
            }
        }
     }
}
