using System;
using System.Collections;
using System.ComponentModel;
using CameraMod;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;


[assembly: MelonInfo(typeof(CameraMod.Camera.Patches.HarmonyPatches), PluginInfo.Name, PluginInfo.Version, PluginInfo.Author)]
[assembly: MelonGame("Another Axiom", "Gorilla Tag")]
namespace CameraMod.Camera.Patches {
    
    public class HarmonyPatches : MelonMod {
        public string modName = "PokruksCameraModMLE";
        public string updateUrl = "https://github.com/mindwastoggled/PokRuksCameraModMelonLoader";

        private bool showNotification = true;
        private Rect windowRect = new Rect(100, 100, 400, 200);

        void UpdateNotification() {
            if (!showNotification) return;

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            GUIStyle versionStyle = new GUIStyle(GUI.skin.label);
            versionStyle.fontSize = 14;
            versionStyle.alignment = TextAnchor.MiddleCenter;

            windowRect = GUI.Window(0, windowRect, DrawWindow, "Mod Update (Might be) Available!");
        }

        void DrawWindow(int id) {
            GUILayout.Space(10);

            GUILayout.Label(modName, new GUIStyle(GUI.skin.label) {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            });

            GUILayout.Space(5);

            GUILayout.Label($"Update: {PluginInfo.Version} → {desiredVersion}.", new GUIStyle(GUI.skin.label) {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            });

            GUILayout.Space(15);
            
            GUILayout.Label("NOTE: The Update might not be released on MelonLoader Edition, complain to @mindwastoggled NOT Pokruk.", new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            });

            if (GUILayout.Button("Download Update", GUILayout.Height(40))) {
                Application.OpenURL(updateUrl);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Close", GUILayout.Height(30))) {
                showNotification = false;
            }

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void OnGUI() {
            if (isVersionChecked && updateIsNeeded) {
                UpdateNotification();
            }
        }

        public static bool isVersionChecked = false;
        public static bool updateIsNeeded = false;
        public static string desiredVersion = "";
        
        
        private static IEnumerator FetchForceUpdate() {
            UnityWebRequest request = UnityWebRequest.Get("https://pastebin.com/raw/LwXaTiFH");
            yield return request.SendWebRequest();
            var errored = request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError;
            if (errored) {
                Debug.LogError("FetchForceUpdate filed: " + request.error);
                Debug.LogWarning("Skipping version check");
                isVersionChecked = true;
            } else {
                desiredVersion = request.downloadHandler.text;
                isVersionChecked = true;

                updateIsNeeded = desiredVersion != "-" && new Version(PluginInfo.Version) < new Version(desiredVersion);
            }
        }

        private static IEnumerator CameraStartCoroutine() {
            //yield return FetchForceUpdate();
            yield return null;
            if (isVersionChecked && !updateIsNeeded) {
                HarmonyPatcher.ApplyHarmonyPatches();
            }
        }
        
        public void OnEnable() {
            MelonCoroutines.Start(CameraStartCoroutine());
        }

        public void OnDisable() {
            HarmonyPatcher.RemoveHarmonyPatches();
        }
    }
}