using VRSF.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// To use the TeleportFarAway feature, add a Collider and a Layer named "Teleport" on the Objects where the user can Teleport.
    /// </summary>
    public class TeleportFarAway : ButtonActionChoser
    {
        #region PUBLIC_VARIABLES
        [Header("OPTIONAL : Loading Slider")]
        [Tooltip("Those are used to display the loading slider on the controller when charging the teleport feature.")]
        [HideInInspector] public bool UseLoadingSlider;
        [HideInInspector] public Image FillRect;
        [HideInInspector] public Text TeleportText;
        #endregion


        #region PRIVATE_VARIABLES
        // Scriptable Parameter for the teleport script
        private TeleportParametersVariable _teleportParameters;

        private LayerMask _teleportLayer = -1;

        private bool _canTeleport;
        #endregion


        #region MONOBEHAVIOUR_METHODS
        // Use this for initialization
        public void Awake()
        {
            _teleportParameters = TeleportParametersVariable.Instance;

            _teleportLayer = LayerMask.NameToLayer("Teleport");

            if (_teleportLayer == -1)
            {
                Debug.Log("VRSF : You won't be able to teleport on the floor, as you didn't set the Ground Layer");
            }
        }

        public override void Start()
        {
            base.Start();

            if (UseLoadingSlider)
            {
                FillRect.gameObject.SetActive(false);
                TeleportText.gameObject.SetActive(false);
            }
        }
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Method to call from the StartTouching or StartClicking Method, set the Loading Slider values if used.
        /// </summary>
        public void OnStartInteracting()
        {
            if (UseLoadingSlider)
            {
                if (FillRect != null)
                {
                    FillRect.gameObject.SetActive(true);
                    FillRect.fillAmount = 0.0f;
                }

                if (TeleportText != null)
                {
                    TeleportText.gameObject.SetActive(true);
                    TeleportText.text = "Preparing Teleport ...";
                }
            }
        }


        /// <summary>
        /// To call from the IsClicking or IsTouching event
        /// </summary>
        public void OnIsInteracting()
        {
            CheckTeleport();

            if (UseLoadingSlider)
            {
                if (FillRect != null && FillRect.fillAmount < 1.0f)
                {
                    FillRect.fillAmount += Time.deltaTime / _teleportParameters.TimerBeforeTeleport;
                }
                else if (TeleportText != null && !_canTeleport)
                {
                    TeleportText.text = "Waiting for ground ...";
                }
                else if (TeleportText != null)
                {
                    TeleportText.text = "Release To Teleport !";
                }
            }
        }

        /// <summary>
        /// Method to call from the StopTouching or StopClicking Event.
        /// </summary>
        public void OnStopInteracting()
        {
            Teleport();
            DeactivateTeleportSlider();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if the Teleport ray is on a Teleport Layer, and set the _canTeleport bool and the color of the Loading Slider accordingly.
        /// </summary>
        private void CheckTeleport()
        {
            Color32 fillRectColor;

            if (!HitVar.isNull && HitVar.Value.collider.gameObject.layer == _teleportLayer.value)
            {
                _canTeleport = true;
                fillRectColor = new Color32(100, 255, 100, 255);
            }
            else
            {
                _canTeleport = false;
                fillRectColor = new Color32(0, 180, 255, 255);
            }

            if (UseLoadingSlider && FillRect != null)
            {
                FillRect.color = fillRectColor;
            }

            if (UseLoadingSlider && TeleportText != null)
            {
                TeleportText.color = fillRectColor;
            }
        }


        /// <summary>
        /// Handle the Teleport when the user is releasing the button.
        /// </summary>
        private void Teleport()
        {
            if (_canTeleport)
            {
                Vector3 newPos = new Vector3();

                if (_teleportParameters.AdjustHeight)
                {
                    float y = HitVar.Value.point.y + _teleportParameters.HeightAboveGround + VRSF_Components.CameraRig.transform.localScale.y;
                    newPos = new Vector3(HitVar.Value.point.x, y, HitVar.Value.point.z);
                }
                else
                {
                    newPos = new Vector3(HitVar.Value.point.x, VRSF_Components.CameraRig.transform.position.y, HitVar.Value.point.z);
                }

                VRSF_Components.CameraRig.transform.position = CheckNewPosLongTeleportBoundaries(newPos);
            }
        }


        /// <summary>
        /// Check the newPos for the Long Teleport feature depending on the Teleport Boundaries
        /// </summary>
        private Vector3 CheckNewPosLongTeleportBoundaries(Vector3 PosToCheck)
        {
            if (_teleportParameters.UseBoundaries)
            {
                float newX = Mathf.Clamp(PosToCheck.x, _teleportParameters.MinAvatarPosition.x, _teleportParameters.MaxAvatarPosition.x);
                float newY = Mathf.Clamp(PosToCheck.y, _teleportParameters.MinAvatarPosition.y, _teleportParameters.MaxAvatarPosition.y);
                float newZ = Mathf.Clamp(PosToCheck.z, _teleportParameters.MinAvatarPosition.z, _teleportParameters.MaxAvatarPosition.z);
                PosToCheck = new Vector3(newX, newY, newZ);
            }
            return PosToCheck;
        }


        /// <summary>
        /// If used, we deactivate the Teleport Slider and Text when the user release the button.
        /// </summary>
        private void DeactivateTeleportSlider()
        {
            if (UseLoadingSlider)
            {
                if (FillRect != null)
                {
                    Debug.Log("Hello");
                    FillRect.gameObject.SetActive(false);
                }

                if (TeleportText != null)
                {
                    Debug.Log("Hello 2");
                    TeleportText.gameObject.SetActive(false);
                }
            }
        }
        #endregion


        //EMPTY
        #region GETTERS_SETTERS

        #endregion
    }

}