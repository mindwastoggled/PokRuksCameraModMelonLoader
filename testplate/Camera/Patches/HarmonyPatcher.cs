using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using GorillaTag;
namespace CameraMod.Camera.Patches {
    public class HarmonyPatcher : MonoBehaviour {
        public static HarmonyLib.Harmony instance;
        public static bool IsPatched { get; private set; }

        internal static void ApplyHarmonyPatches() {
            if (!IsPatched) {
                if (instance == null) instance = new HarmonyLib.Harmony(PluginInfo.GUID);
                instance.PatchAll(Assembly.GetExecutingAssembly());

                IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches() {
            if (instance != null && IsPatched) IsPatched = false;
        }
    }
}
