using HarmonyLib;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TalkToPrisoner
{
    public class TalkToPrisoner : ModBehaviour
    {
        public static TalkToPrisoner Instance { get; private set; }

        public INewHorizons NewHorizons { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public static void Log(string message) => Instance.ModHelper.Console.WriteLine(message);
        public static void FireOnNextUpdate(Action action) => Instance.ModHelper.Events.Unity.FireOnNextUpdate(action);

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(TalkToPrisoner)} is loaded!", MessageType.Success);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                FireOnNextUpdate(() =>
                {
                    var prisoner = GameObject.FindObjectOfType<PrisonerBrain>().gameObject.GetComponentInChildren<CharacterDialogueTree>();
                    var dialogueAssetName = prisoner._xmlCharacterDialogueAsset.name;
                    var xml = File.ReadAllText(ModHelper.Manifest.ModFolderPath + "/planets/dialogue.xml");
                    var dialogueInfo = "{ \"pathToExistingDialogue\": \"" + GetPath(prisoner.transform).Remove(0, "DreamWorld_Body/".Length) + "\", \"xmlFile\": \"planets/dialogue.xml\" }";
                    NewHorizons.CreateDialogueFromXML(dialogueAssetName, xml, dialogueInfo, Locator.GetAstroObject(AstroObject.Name.DreamWorld).gameObject);
                });

            };
        }

        private string GetPath(Transform t)
        {
            if (t.parent == null)
            {
                return t.name;
            }
            else
            {
                return GetPath(t.parent) + "/" + t.name;
            }
        }
    }
}