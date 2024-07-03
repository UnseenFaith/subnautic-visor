using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace VisorSubnautica
{
    [BepInPlugin(MyGuid, PluginName, VersionString)]
    public class VisorPlugin : BaseUnityPlugin
    {
        private const string MyGuid = "com.blobyblob.visormod";
        private const string PluginName = "Visor Mod";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGuid);

        public static ManualLogSource Log;

        private void Awake()
        {
            Harmony.PatchAll();
            Logger.LogInfo(PluginName + " " + VersionString + " " + "loaded.");
            Log = Logger;
        }


        private static Texture2D LoadPNGFromFile(string filePath)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadRawTextureData(fileData);
            return texture;
        }

        [HarmonyPatch(typeof(uGUI))]
        internal class UIPatch
        {
            [HarmonyPatch(nameof(uGUI.main.overlays.Awake))]
            [HarmonyPostfix]
            public static void Awake_Postfix()
            {
                string pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string pngFilePath = Path.Combine(pluginDirectory, "overlay.png");

                
                GameObject imageGO = new GameObject("VisorHUD");
                Image imageComponent = imageGO.AddComponent<Image>();
                var canvasComponent = imageGO.AddComponent<CanvasGroup>();
                canvasComponent.blocksRaycasts = false;
                canvasComponent.interactable = false;

                Texture2D texture = new Texture2D(2, 2);

                ImageConversion.LoadImage(texture, File.ReadAllBytes(pngFilePath));

                imageComponent.material.mainTexture = texture;

                imageGO.transform.SetParent(uGUI.main.hud.transform, false);

                imageGO.transform.SetAsFirstSibling();

                RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, 0); // Example position
                rectTransform.sizeDelta = new Vector2(1920f, 1080f); // Example size

                Log.LogInfo("Overlay Added To Screen");
            }
        }

    }
}
