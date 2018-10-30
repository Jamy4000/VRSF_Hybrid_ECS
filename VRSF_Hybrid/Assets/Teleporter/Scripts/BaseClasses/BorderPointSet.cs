using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Class containing the Collection for the Border's Points
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    [System.Serializable]
    public class BorderPointSet
    {
        public Vector3[] Points;

        public BorderPointSet(Vector3[] Points)
        {
            this.Points = Points;
        }
    }
}