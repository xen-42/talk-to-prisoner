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
            Log($"{nameof(TalkToPrisoner)} is loaded!");

            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                // NH really can't handle ghost birds well and can't find the prisoner on the first frame when you do this normally
                FireOnNextUpdate(() =>
                {
                    var prisoner = GameObject.FindObjectOfType<PrisonerBrain>().gameObject.GetComponentInChildren<CharacterDialogueTree>();
                    var path = GetPath(prisoner.transform).Remove(0, "DreamWorld_Body/".Length);
                    
                    var id = prisoner._xmlCharacterDialogueAsset.name;
                    var xml = File.ReadAllText(ModHelper.Manifest.ModFolderPath + "/dialogue/prisoner.xml");
                    var dialogueInfo = "{ \"pathToExistingDialogue\": \"" + path + "\", \"xmlFile\": \"planets/dialogue.xml\" }";
                    var dreamworld = Locator.GetAstroObject(AstroObject.Name.DreamWorld).gameObject;

                    NewHorizons.CreateDialogueFromXML(id, xml, dialogueInfo, dreamworld);
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