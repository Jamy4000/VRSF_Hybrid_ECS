using System.Collections.Generic;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// Class that implement the IUISetupScrollable, used in the Scrollable UI Elements.
    /// </summary>
	public class VRUIScrollableSetup : IUISetupScrollable
    {
        public VRUIScrollableSetup(EUIDirection dir, float minVal = 0.0f, float maxVal = 1.0f, bool wholeNum = false)
        {
            _Direction = dir;
            _MinValue = minVal;
            _MaxValue = maxVal;
            _WholeNumbers = wholeNum;
        }

        // EMPTY
        #region PUBLIC_VARIABLES

        #endregion


        #region PRIVATE_VARIABLES
        EUIDirection _Direction;
        float _MinValue = 0.0f;
        float _MaxValue = 1.0f;
        bool _WholeNumbers = false;
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Called in an update method to check if a click is still down
        /// </summary>
        /// <param name="handHoldingHandle">The hand with with the user is clicking</param>
        /// <param name="clickIsDown">The ClickIsDown BoolVariable Value for the corresponding hand</param>
        public void CheckClickStillDown(ref EHand handHoldingHandle, bool clickIsDown)
        {
            if (!clickIsDown)
                handHoldingHandle = EHand.NONE;
        }


        /// <summary>
        /// Move the Scrollable component when the user is holding the handle.
        /// </summary>
        /// <param name="handHoldingHandle">The hand that is holding the Scrollable element</param>
        /// <param name="minPosTransform">The min position of the scrollable element, set at runtime</param>
        /// <param name="maxPosTransform">The max position of the scrollable element, set at runtime</param>
        /// <param name="raycastHitDictionary">The dictionary containing references to the RaycastHitVariable of the controllers</param>
        /// <param name="direction">The direction of the scrollable element</param>
        /// <returns>The new value of the scrollable</returns>
        public float MoveComponent(EHand handHoldingHandle, Transform minPosTransform, Transform maxPosTransform, Dictionary<string, RaycastHitVariable> raycastHitDictionary)
        {
            switch (handHoldingHandle)
            {
                case (EHand.GAZE):
                    return SetComponentNewValue(raycastHitDictionary["Gaze"].Value.point, minPosTransform, maxPosTransform);

                case (EHand.LEFT):
                    return SetComponentNewValue(raycastHitDictionary["Left"].Value.point, minPosTransform, maxPosTransform);

                case (EHand.RIGHT):
                    return SetComponentNewValue(raycastHitDictionary["Right"].Value.point, minPosTransform, maxPosTransform);

                default:
                    Debug.LogError("Error in MoveComponent method");
                    return 0;
            }
        }


        /// <summary>
        /// Set the UI Scrollable component new value according to the hitPoint, the min and max position and the direction of the element
        /// </summary>
        /// <param name="hitPoint">The hitPoint world position</param>
        /// <param name="minPosTransform">The minimum world position of the scrollable element</param>
        /// <param name="maxPosTransform">The maximum world position of the scrollable element</param>
        /// <param name="direction">The direction of the scrollable element</param>
        /// <returns>The new value of the scrollable element (between 0 and 1)</returns>
        public float SetComponentNewValue(Vector3 hitPoint, Transform minPosTransform, Transform maxPosTransform)
        {
            Vector3 minPos = minPosTransform.position;
            Vector3 maxPos = maxPosTransform.position;

            float distanceMinMax = Vector3.Distance(minPos, maxPos);
            float distanceMaxHitPoint;
            float distanceMinHitPoint;

            // IF direction is reverse, we reverse the value. doesn't work if you change the direction while in play mode.
            if (_Direction == EUIDirection.LeftToRight || _Direction == EUIDirection.BottomToTop)
            {
                distanceMaxHitPoint = Vector3.Distance(minPos, hitPoint);
                distanceMinHitPoint = Vector3.Distance(maxPos, hitPoint);
            }
            else
            {
                distanceMaxHitPoint = Vector3.Distance(maxPos, hitPoint);
                distanceMinHitPoint = Vector3.Distance(minPos, hitPoint);
            }

            float toReturn = CheckHitPointInsideComponent(distanceMinMax, distanceMinHitPoint, distanceMaxHitPoint);

            if (_WholeNumbers)
            {
                return (int)toReturn;
            }
            else
            {
                return toReturn;
            }
        }


        /// <summary>
        /// Check the value by comparing the distance between the Min and Max Positions and the hitpoint
        /// </summary>
        /// <param name="distanceMinMax">The distance between the minimum and maximum positions of the scrollable</param>
        /// <param name="distanceMinHitPoint">The distance between the min position and the hitpoint</param>
        /// <param name="distanceMaxHitPoint">The distance between the max position and the hitpoint</param>
        /// <returns>The new value of the scrollable element (between 0 and 1)</returns>
        public float CheckHitPointInsideComponent(float distanceMinMax, float distanceMinHitPoint, float distanceMaxHitPoint)
        {
            if (distanceMaxHitPoint > distanceMinMax)
                return _MinValue;
            else if (distanceMinHitPoint > distanceMinMax)
                return _MaxValue;
            else
                return (distanceMinHitPoint / distanceMinMax) * _MaxValue;
        }


        /// <summary>
        /// Call at runtime, set the min and max pos transform by looking in the children of the Handle Rect
        /// </summary>
        /// <param name="minPos">The min pos transform of the Scrollable element</param>
        /// <param name="maxPos">The max pos transform of the Scrollable element</param>
        /// <param name="handleRectParent">The handle rect of the scrollable element</param>
        public void SetMinMaxPos(ref Transform minPos, ref Transform maxPos, Transform handleRectParent)
        {
            minPos = handleRectParent.Find("MinPos");
            maxPos = handleRectParent.Find("MaxPos");
        }


        /// <summary>
        /// Check if the MinPos and MaxPos of the element are correctly instantiated and Check there RectTransform
        /// </summary>
        /// <param name="handleRectParent">The parent of the HandleRect</param>
        /// <param name="direction">The UIDirection of the element</param>
        public void CheckMinMaxGameObjects(Transform handleRectParent, EUIDirection direction)
        {
            RectTransform minPosRect;
            RectTransform maxPosRect;

            if (handleRectParent.Find("MinPos") == null)
                minPosRect = InstantiateNewPosObject("MinPos", handleRectParent);
            else
                minPosRect = handleRectParent.Find("MinPos").GetComponent<RectTransform>();

            if (handleRectParent.Find("MaxPos") == null)
                maxPosRect = InstantiateNewPosObject("MaxPos", handleRectParent);
            else
                maxPosRect = handleRectParent.Find("MaxPos").GetComponent<RectTransform>();

            if (direction == EUIDirection.BottomToTop || direction == EUIDirection.TopToBottom)
            {
                CheckRectTrans(ref minPosRect, new Vector2(0.5f, 1.0f));
                CheckRectTrans(ref maxPosRect, new Vector2(0.5f, 0.0f));
            }
            else
            {
                CheckRectTrans(ref minPosRect, new Vector2(1.0f, 0.5f));
                CheckRectTrans(ref maxPosRect, new Vector2(0.0f, 0.5f));
            }
        }

        /// <summary>
        /// If the MinPos or MaxPos Gameobject doesn't exist, instantiate them
        /// </summary>
        /// <param name="name">The name of the object to instantiate</param>
        /// <param name="handleRectParent">The parent of the HandleRect</param>
        /// <return>The RectTransform of the new pos object</return>
        public RectTransform InstantiateNewPosObject(string name, Transform handleRectParent)
        {
            GameObject newPos = new GameObject();
            newPos.transform.SetParent(handleRectParent);
            newPos.transform.name = name;
            return newPos.AddComponent<RectTransform>();
        }

        /// <summary>
        /// Check for the both min and max pos objects if there rect is set correctly
        /// </summary>
        /// <param name="rect">The rect to check, passed as reference</param>
        /// <param name="anchor">The Vector2 to which we set the pivot, anchor min and anchor max</param>
        public void CheckRectTrans(ref RectTransform rect, Vector2 anchor)
        {
            rect.anchorMax = anchor;
            rect.anchorMin = anchor;
            rect.pivot = anchor;
            rect.localScale = Vector3.one;
            rect.sizeDelta = Vector2.zero;
            rect.localRotation = Quaternion.Euler(Vector3.zero);
            rect.anchoredPosition3D = Vector3.zero;
        }
        #endregion


        // EMPTY
        #region PRIVATE_METHODS

        #endregion


        // EMPTY
        #region GETTERS_SETTERS

        #endregion
    }
}