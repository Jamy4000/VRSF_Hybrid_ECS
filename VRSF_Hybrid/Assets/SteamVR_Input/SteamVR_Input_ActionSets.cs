//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Input_ActionSet_VRSF_Binding p_VRSF_Binding;
        
        public static SteamVR_Input_ActionSet_VRSF_Binding VRSF_Binding
        {
            get
            {
                return SteamVR_Actions.p_VRSF_Binding.GetCopy<SteamVR_Input_ActionSet_VRSF_Binding>();
            }
        }
        
        private static void StartPreInitActionSets()
        {
            SteamVR_Actions.p_VRSF_Binding = ((SteamVR_Input_ActionSet_VRSF_Binding)(SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_VRSF_Binding>("/actions/VRSF_Binding")));
            Valve.VR.SteamVR_Input.actionSets = new Valve.VR.SteamVR_ActionSet[] {
                    SteamVR_Actions.VRSF_Binding};
        }
    }
}
