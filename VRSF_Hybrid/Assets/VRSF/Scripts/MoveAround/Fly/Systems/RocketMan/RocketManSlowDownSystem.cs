using HPA_Boat.VR.Component;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using VRSF.MoveAround.Components;
using VRSF.MoveAround.Systems;
using VRSF.Utils.Components.ButtonActionChoser;
using VRSF.Utils.Systems.ButtonActionChoser;

namespace HPA_Boat.VR
{
    /// <summary>
    /// Handle the slow down of the user when using rocket man
    /// </summary>
	public class RocketManSlowDownSystem : ComponentSystem
    {
        struct Filter
        {
            public RocketManComponent RocketManComp;
            public FlyParametersComponent FlyParameters;
            public FlyAccelerationComponent FlyAcceleration;
        }
        
        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.RocketManComp.RocketSlowingDown)
                {
                    if (e.FlyParameters.FlyingSpeed > e.RocketManComp.BaseSpeedValue)
                    {
                        e.FlyParameters.FlyingSpeed -= (Time.deltaTime * e.FlyAcceleration.DecelerationEffectFactor * 10);
                        e.RocketManComp.SpeedHasBeenSet = false;
                    }
                    else
                    {
                        e.FlyParameters.FlyingSpeed = e.RocketManComp.BaseSpeedValue;
                        e.RocketManComp.RocketSlowingDown = false;
                    }
                }
            }
        }
    }
}