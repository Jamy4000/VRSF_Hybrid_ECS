﻿using UnityEngine;

public static class LayerMaskExtension
{
    public static LayerMask NamesToMask(params string[] layerNames)
    {
        LayerMask ret = (LayerMask)0;
        foreach (var name in layerNames)
        {
            ret |= (1 << LayerMask.NameToLayer(name));
        }
        return ret;
    }


    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask">The layerMask to check</param>
    /// <param name="layer">The layer to check</param>
    /// <returns>true if the layer is in the layerMask</returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    /// <summary>
    /// Add a Layer to a LayerMask
    /// </summary>
    /// <param name="mask">The layerMask to check</param>
    /// <param name="layerName">The layer to add</param>
    /// <returns>true if the layer is in the layerMask</returns>
    public static LayerMask AddToMask(this LayerMask original, string layerName)
    {
        return original | (1 << LayerMask.NameToLayer(layerName));
    }

    /// <summary>
    /// Add a Layer to a LayerMask
    /// </summary>
    /// <param name="mask">The layerMask to check</param>
    /// <param name="layer">The layer to add</param>
    /// <returns>true if the layer is in the layerMask</returns>
    public static LayerMask AddToMask(this LayerMask original, int layer)
    {
        return original | (1 << layer);
    }


    /// <summary>
    /// Remove a Layer from a LayerMask
    /// </summary>
    /// <param name="mask">The layerMask to check</param>
    /// <param name="layerName">The layer to remove</param>
    /// <returns>true if the layer is in the layerMask</returns>
    public static LayerMask RemoveFromMask(this LayerMask original, string layerName)
    {
        return original & ~(1 << LayerMask.NameToLayer(layerName));
    }


    /// <summary>
    /// Remove a Layer from a LayerMask
    /// </summary>
    /// <param name="mask">The layerMask to check</param>
    /// <param name="layer">The layer to remove</param>
    /// <returns>true if the layer is in the layerMask</returns>
    public static LayerMask RemoveFromMask(this LayerMask original, int layer)
    {
        return original & ~(1 << layer);
    }


    public static LayerMask Inverse(this LayerMask original)
    {
        return ~original;
    }
}