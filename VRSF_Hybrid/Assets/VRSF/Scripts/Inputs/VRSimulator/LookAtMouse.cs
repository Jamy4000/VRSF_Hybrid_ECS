using UnityEngine;

namespace VRSF.Inputs.Simulator
{
    /// <summary>
    /// Script attached to the Camera rig of the SimulatorSDK.
    /// Allow the user to rotate the camera, which will follow the mouse position.
    /// To use this feature, press the Space bar when you want to rotate the camera.
    /// </summary>
    public class LookAtMouse : MonoBehaviour
    {
        // speed is the rate at which the object will rotate
        public float speed;

        void FixedUpdate()
        {
            // Generate a ray from the cursor position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetKey(KeyCode.Space))
            {
                // Get the point along the ray that hits the calculated distance.
                Vector3 targetPoint = ray.GetPoint(100);

                // Determine the target rotation.  This is the rotation if the transform looks at the target point.
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }
    }
}