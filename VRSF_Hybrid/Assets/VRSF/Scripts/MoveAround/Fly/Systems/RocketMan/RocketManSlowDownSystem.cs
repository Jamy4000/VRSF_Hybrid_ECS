using HPA_Boat.VR.Component;
using Unity.Entities;
using UnityEngine;
using VRSF.MoveAround.Components;

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
                if (e.RocketManComp._RocketSlowingDown)
                {
                    if (e.FlyParameters.FlyingSpeed > e.RocketManComp._BaseSpeedValue)
                    {
                        e.FlyParameters.FlyingSpeed -= (Time.deltaTime * e.FlyAcceleration.DecelerationEffectFactor * 10);
                        e.RocketManComp._SpeedHasBeenSet = false;
                    }
                    else
                    {
                        e.FlyParameters.FlyingSpeed = e.RocketManComp._BaseSpeedValue;
                        e.RocketManComp._RocketSlowingDown = false;
                    }
                }
            }
        }
    }
}