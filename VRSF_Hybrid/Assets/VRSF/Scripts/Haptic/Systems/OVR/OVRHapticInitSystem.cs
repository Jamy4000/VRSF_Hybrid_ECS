using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Utils.Components;

namespace VRSF.Controllers.Haptic
{
    /// <summary>
    /// Init the OVRHapticComponent when a scene is loaded
    /// </summary>
    public class OVRHapticInitSystem : ComponentSystem
    {
        struct Filter
        {
            public OVRHapticComponent OVRHapticComp;
        }

        /// <summary>
        /// Used to wait for init
        /// </summary>
        struct SetupVRFilter
        {
            public SetupVRComponents SetupVRComp;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<SetupVRFilter>())
                e.SetupVRComp.StartCoroutine(WaitForSetupVR());

            SceneManager.sceneLoaded += OnSceneLoaded;
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Initialize the necessary values for the OVR Haptics component
        /// </summary>
        /// <param name="hapticComp"></param>
        private void InitializeOVRHaptics(OVRHapticComponent hapticComp)
        {
            int cnt = 20;
            hapticComp.LightClip = new OVRHapticsClip(cnt);
            hapticComp.MediumClip = new OVRHapticsClip(cnt);
            hapticComp.HardClip = new OVRHapticsClip(cnt);

            for (int i = 0; i < cnt; i++)
            {
                hapticComp.LightClip.Samples[i] =/* i % 2 == 0 ? (byte)0 : */(byte)50;
                hapticComp.MediumClip.Samples[i] = /*i % 2 == 0 ? (byte)0 : */(byte)110;
                hapticComp.HardClip.Samples[i] = /*i % 2 == 0 ? (byte)0 : */(byte)255;
            }

            hapticComp.LightClip = new OVRHapticsClip(hapticComp.LightClip.Samples, hapticComp.LightClip.Samples.Length);
            hapticComp.MediumClip = new OVRHapticsClip(hapticComp.MediumClip.Samples, hapticComp.MediumClip.Samples.Length);
            hapticComp.HardClip = new OVRHapticsClip(hapticComp.HardClip.Samples, hapticComp.HardClip.Samples.Length);
        }

        private IEnumerator<WaitForEndOfFrame> WaitForSetupVR()
        {
            while (!Utils.VRSF_Components.SetupVRIsReady)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();

            foreach (var e in GetEntities<Filter>())
            {
                InitializeOVRHaptics(e.OVRHapticComp);
            }
        }

        private void OnSceneLoaded(Scene newScene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    InitializeOVRHaptics(e.OVRHapticComp);
                }
            }
        }
    }
}