using UnityEngine;

namespace VRSF.Controllers.Components
{
    [System.Serializable]
    public struct OptionalLaserObjects
    {
        // LineRenderer attached to the right and left controllers
        public ParticleSystem[] PointersParticles;
        public Transform PointersEndPoint;
    }
}