using ScriptableFramework.Events;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Gaze;
using UnityEngine;

namespace VRSF.Interactions
{
    /// <summary>
    /// Script placed on the SetupVR Prefab.
    /// Check if a ray is over a collider.
    /// </summary>
    public class OnColliderOverHandler : MonoBehaviour
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        //References to the Controllers and the Gaze Parameters
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameter;

        //References to the Interaction and the Input Variables and GameEvents
        private InteractionVariableContainer _interactionContainer;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        void Start()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameter = GazeParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            // Set to true to avoid error on the first frame.
            _interactionContainer.RightHit.isNull = true;
            _interactionContainer.LeftHit.isNull = true;
            _interactionContainer.GazeHit.isNull = true;

            // if we don't use the controllers and the gaze
            if (!_controllersParameter.UseControllers && !_gazeParameter.UseGaze)
                this.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            CheckIsOver();
        }
        #endregion MONOBEHAVIOUR_METHODS


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS


        #region PRIVATE_METHODS
        /// <summary>
        /// Check if each raycast (Controllers and Gaze) are over something
        /// </summary>
        void CheckIsOver()
        {
            if (_controllersParameter.UseControllers)
            {
                HandleOver(_interactionContainer.IsOverSomethingRight, _interactionContainer.RightHit, _interactionContainer.RightOverObject);
                HandleOver(_interactionContainer.IsOverSomethingLeft, _interactionContainer.LeftHit, _interactionContainer.LeftOverObject);
            }

            if (_gazeParameter.UseGaze)
            {
                HandleOver(_interactionContainer.IsOverSomethingGaze, _interactionContainer.GazeHit, _interactionContainer.GazeOverObject);
            }
        }

        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        /// <param name="isOver">the BoolVariable to set if something got hit</param>
        /// <param name="hit">The Hit Point where the raycast collide</param>
        /// <param name="objectOver">The GameEvent to raise with the transform of the hit</param>
        private void HandleOver(BoolVariable isOver, RaycastHitVariable hit, GameEventTransform objectOver)
        {
            //If nothing is hit, we set the isOver value to false
            if (hit.isNull)
            {
                isOver.SetValue(false);
            }
            else
            {
                if (hit.Value.collider != null)
                {
                    var hitTransform = hit.Value.collider.transform;
                    objectOver.Raise(hitTransform);

                    isOver.SetValue(true);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}