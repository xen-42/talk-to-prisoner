using HarmonyLib;
using NewHorizons;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.IO;
using System.Linq;
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

            TextTranslation.Get().OnLanguageChanged += () =>
            {
                TextTranslation.Get().m_table.theUITable[511] = "<Color=orange>Outer Wilds</Color> is best experienced on <Color=orange>mouse and keyboard!</Color>";

                OnTalkToPrisoner();
            };

            GlobalMessenger.AddListener("ExitConversation", OnTalkToPrisoner);

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                // NH really can't handle ghost birds well and can't find the prisoner on the first frame when you do this normally
                // Actually maybe this would have worked normally idk I dont care!!!
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

        private void OnTalkToPrisoner()
        {
            if (PlayerData.GetPersistentCondition("SPOKE_TO_PRISONER_GROMBO"))
            {
                TextTranslation.Get().m_table.theUITable[(int)UITextType.NotificationGhostMatter] = "AHHH! LOOK OUT IT'S SPOOKY MATTER! OH NO!";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.YouEndedRealityMessage] = "GORP!";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.WakeUpPrompt] = "Go back to sleep (JK wake up!)";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.LaunchCodes] = "Launch codes: | || || |_";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.NotificationTrees] = "There's magic trees that somehow make oxygen out of nothing nearby!";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.SignalChert] = "Drum guy";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.SignalEsker] = "Whistle guy";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.SignalFeldspar] = "Harmonica guy";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.SignalGabbro] = "Flute guy? Is it a flute?";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.SignalRiebeck] = "Banjo guy";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.SignalSolanum] = "Solanum guy";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.UIFuel] = "Propane";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.UnidentifiedSignal] = "Ayo wtf is that??";
                TextTranslation.Get().m_table.theUITable[(int)UITextType.CampfireDozeOff] = "Honk shoo mimimi";
                TextTranslation.Get().m_table.theUITable[730] = "PLANETARY CHART of the OUTER WORLDS";
                TextTranslation.Get().m_table.theUITable[403] = "YOU ARE LITERALLY GONNA DIE";
                TextTranslation.Get().m_table.theUITable[261] = "Don't press this button";

                TextTranslation.Get().m_table.theUITable[(int)UITextType.ItemUnknownArtifactPrompt] = "Grombofact";

                TextTranslation.Get().m_table.theUITable[(int)UITextType.TranslatorUntranslatableWarning] = "Grombo";

                TextTranslation.Get().m_table.theUITable[(int)UITextType.LocationIP] = "Gromboville";

                var shipLogs = TextTranslation.Get().m_table.theShipLogTable;
                foreach (var key in shipLogs.Keys.ToArray())
                {
                    if (shipLogs[key].Contains("the Stranger") || shipLogs[key].Contains("The Stranger"))
                    {
                        shipLogs[key] = shipLogs[key].Replace("the Stranger", "Gromboville");
                        shipLogs[key] = shipLogs[key].Replace("The Stranger", "Gromboville");
                    }
                }
                shipLogs["The Stranger"] = "Gromboville";
            }
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