using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Component handling the fading effects. Need to be placed on the Camera Eye
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class TeleportFadeComponent : MonoBehaviour
    {
        /// How long, in seconds, the fade-in/fade-out animation should take
        [Tooltip("Duration of the \"blink\" animation (fading in and out upon teleport) in seconds.")]
        public float TeleportFadeDuration = 0.2f;

        /// Material used to render the fade in/fade out quad
        [Tooltip("Material used to render the fade in/fade out quad.")]
        public Material _fadeMaterial;

        [System.NonSerialized] public Material _fadeMaterialInstance;
        [System.NonSerialized] public int _materialFadeID;
        [System.NonSerialized] public bool _fadingIn = false;

        [System.NonSerialized] public Mesh _planeMesh;

        [System.NonSerialized] public ETeleportState TeleportState = ETeleportState.None;

        [System.NonSerialized] public bool _IsSetup = false;

        [System.NonSerialized] public float _teleportTimeMarker = -1;

        /// <summary>
        /// As onPostRenderer is not implement in the ComponentSystem for now, we handle it here.
        /// </summary>
        void OnPostRender()
        {
            if (TeleportState == ETeleportState.Teleporting)
            {
                // Perform the fading in/fading out animation, if we are teleporting.  This is essentially a triangle wave
                // in/out, and the user teleports when it is fully black.
                float alpha = Mathf.Clamp01((Time.time - _teleportTimeMarker) / (TeleportFadeDuration / 2));

                if (_fadingIn)
                    alpha = 1 - alpha;

                Matrix4x4 local = Matrix4x4.TRS(Vector3.forward, Quaternion.identity, Vector3.one);
                _fadeMaterialInstance.SetPass(0);
                _fadeMaterialInstance.SetFloat(_materialFadeID, alpha);
                Graphics.DrawMeshNow(_planeMesh, transform.localToWorldMatrix * local);
            }
        }
    }
} 