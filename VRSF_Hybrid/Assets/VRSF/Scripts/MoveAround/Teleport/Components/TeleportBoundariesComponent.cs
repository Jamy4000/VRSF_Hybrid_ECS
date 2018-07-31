using System.Collections.Generic;
using UnityEngine;
using VRSF.MoveAround.Teleport.Interfaces;

namespace VRSF.MoveAround.Teleport.Components
{
    /// <summary>
    /// Contains all variable necessary for the BezierTeleportSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(Unity.Entities.GameObjectEntity))]
    public class TeleportBoundariesComponent : MonoBehaviour, ITeleportComponent
    {
        [Tooltip("Wheter you wanna use boundaries for the flying mode or not.")]
        [HideInInspector] [SerializeField] public bool _UseBoundaries = false;

        [Tooltip("The minimun position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] [SerializeField] public List<Bounds> _Boundaries = new List<Bounds>();


        #region Getters_ITeleportComponent
        public bool UseBoundaries()
        {
            return _UseBoundaries;
        }

        public List<Bounds> Boundaries()
        {
            return _Boundaries;
        }
        #endregion Getters_ITeleportComponent
    }
}