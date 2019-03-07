using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    public class FlyBoundariesComponent : MonoBehaviour
    {
        [Tooltip("Wheter you wanna use boundaries for the flying mode or not.")]
        [HideInInspector] public bool UseHorizontalBoundaries = false;

        [Tooltip("The minimun vertical position at which the user can go if UseHorizontalBoundaries is at false.")]
        [HideInInspector] public float MinAvatarYPosition = 0.0f;

        [Tooltip("The maximum vertical position at which the user can go if UseHorizontalBoundaries is at false.")]
        [HideInInspector] public float MaxAvatarYPosition = 2000.0f;

        [Tooltip("The minimun local position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 MinAvatarPosition = new Vector3(-10.0f, -1.0f, -10.0f);

        [Tooltip("The maximum local position at which the user can go if UseHorizontalBoundaries is at true.")]
        [HideInInspector] public Vector3 MaxAvatarPosition = new Vector3(10.0f, 10.0f, 10.0f);
    }
}