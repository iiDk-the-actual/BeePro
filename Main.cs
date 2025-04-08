using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;
using Valve.VR;

namespace BeePro
{
	[BepInPlugin(ModInfo._id, ModInfo._name, ModInfo._ver)]
	public class Main : BaseUnityPlugin
	{
		private void Start()
		{
			Main.instance = this;
		}

        private bool hasInit;
        private void Update()
        {
            if (GorillaLocomotion.GTPlayer.Instance != null && !hasInit)
            {
                hasInit = true;
                OnGameInitialized();
            }
        }

		public static bool IsSteam = true;
		private void OnGameInitialized()
		{
            bpro = EasyAssetLoading.InstantiateAsset(Assembly.GetExecutingAssembly(), "BeePro.beepro", "BeeProParent");
            bpro.transform.position = new Vector3(-66.49865f, 11.908f, -84.76817f);
            bpro.AddComponent<BeeProManager>();
            bui = EasyAssetLoading.InstantiateAsset(Assembly.GetExecutingAssembly(), "BeePro.beepro", "BeeProUI");
            bui.AddComponent<BeeUIManager>();

            IsSteam = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
            if (bui != null)
                bui.SetActive(true);

            if (bpro != null)
                bpro.SetActive(true);

            HarmonyPatches.ApplyHarmonyPatches();
        }

        public bool GetLeftJoystickDown()
        {
            if (IsSteam)
                return SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
            else
            {
                bool leftJoystickClick;
                ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out leftJoystickClick);
                return leftJoystickClick;
            }
        }

        public bool GetRightJoystickDown()
        {
            if (IsSteam)
                return SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
            else
            {
                bool rightJoystickClick;
                ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out rightJoystickClick);
                return rightJoystickClick;
            }
        }

        public static Main instance;
		public GameObject bpro;
		public GameObject bui;
	}
}
