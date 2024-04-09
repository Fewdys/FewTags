using Il2CppInterop.Common.XrefScans;
using Il2CppInterop.Runtime.XrefScans;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using static FewTags.Json;

namespace FewTags
{
    public class Main : MelonMod
    {
        private static Json._Tags s_tags { get; set; }
        public static string? s_rawTags { get; set; }

        private static MethodInfo? s_joinMethod { get; set; }
        public static bool SnaxyTagsLoaded { get; private set; }
        public static bool AbyssClientLoaded { get; private set; }
        public static bool NameplateStatsLoaded { get; private set; }
        public static bool VanixClientLoaded { get; private set; }
        internal static float Position { get; set; }
        internal static float Position2 { get; set; }
        internal static float Position3 { get; set; }
        internal static float PositionID { get; set; }
        internal static float PositionBigText { get; set; }
        private static string? s_stringInstance { get; set; }
        public static bool overlay = false;

        private delegate IntPtr userJoined(IntPtr _instance, IntPtr _user, IntPtr _methodinfo);
        private static userJoined? s_userJoined;
        public static List<VRC.Player> p = new List<VRC.Player>();

        public static MelonLogger.Instance Log = new("FewTags", System.Drawing.Color.Red);

        public override void OnApplicationLateStart()
        {
            new WaitForSeconds(3f);
            //NativeHook();
            //Task.Run(() => { OnPlayer.InitPatches(); });
            MelonCoroutines.Start(_OnPlayer.InitializeJoinLeaveHooks());
            UpdateTags();
        }

        public override void OnApplicationStart()
        {
            Chatbox.CreateShit();
            SnaxyTagsLoaded = MelonTypeBase<MelonMod>.RegisteredMelons.Any<MelonMod>(m => m.Info.Name.ToLower().Contains("snaxytags"));
            NameplateStatsLoaded = MelonMod.RegisteredMelons.Any(m => m.Info.Name.ToLower().Contains("nameplatestats"));
            //ProPlatesLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "ProPlates");
            //AbyssClientLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "AbyssLoader");
            Main.Log.Msg(System.ConsoleColor.Green, "Started FewTags");
            Main.Log.Msg(System.ConsoleColor.DarkCyan, "To ReFetch Tags Press L + F (Rejoin Required)");
            Main.Log.Msg(System.ConsoleColor.Green, "Finished Fetching Tags (This Message Doesn't Appear When Tags Are ReFetched)");
            Main.Log.Msg(System.ConsoleColor.Green, "Tagged Players - Nameplate ESP On/Off: RightShift + O (Rejoin Required)");

            //Checks For Other Mods (Positions For A Fixed ProPlates and Snaxy Aren't Updated - Abyss Positions Might Not Be Updated Now Due To It Being C++)
            //If Nothing Is Loaded
            if (!FewTags.Main.NameplateStatsLoaded && !FewTags.Main.SnaxyTagsLoaded)
            {
                FewTags.Main.PositionID = -75.95f;
                FewTags.Main.Position = -101.95f;
                FewTags.Main.PositionBigText = 273.75f;
            }
            else if (FewTags.Main.NameplateStatsLoaded || FewTags.Main.SnaxyTagsLoaded)
            {
                FewTags.Main.PositionID = -102.95f;
                FewTags.Main.Position = -130.95f;
                FewTags.Main.PositionBigText = 273.75f;
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            try
            {
                Chatbox.GetColors();
            }
            catch { }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F) & Input.GetKey(KeyCode.L))
            {
                UpdateTags();
            }
        }

        public static void NameplateOverlay(bool set)
        {
            if (platestatic.TextID.isActiveAndEnabled == true)
            {
                overlay = set;
                s_plate.Text.isOverlay = set;
                platestatic.TextID.isOverlay = set;
                platestatic.TextM.isOverlay = set;
                platestatic.TextBP.isOverlay = set;
            }
            if (set == true)
            {
                Log.Msg(System.ConsoleColor.Green, "(Tagged Players) Nameplate ESP On");
            }
            else if (set == false)
            {
                Log.Msg(System.ConsoleColor.Red, "(Tagged Players) Nameplate ESP Off");
            }
        }

        public override void OnLateUpdate()
        {

            if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.O))
            {
                if (GameObject.Find("Canvas_MainMenu(Clone)/Container/MMParent/Modal_MM_Keyboard").gameObject.GetComponent<GraphicRaycaster>().enabled != true)
                {
                    overlay = !overlay;
                    NameplateOverlay(overlay);
                    if (p.Count != 0)
                    {
                        try
                        {
                            foreach (var player in p)
                            {
                                NameplateESP(player);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        void UpdateTags()
        {

            using (WebClient wc = new WebClient())
            {
                s_rawTags = wc.DownloadString("https://raw.githubusercontent.com/Fewdys/FewTags/main/FewTags.json");
                s_tags = JsonConvert.DeserializeObject<Json._Tags>(s_rawTags);
            }
            Main.Log.Msg(System.ConsoleColor.Yellow, "Fetching Tags (If L + F Was Pressed This Could Potentially Cause Some Lag)");
        }

        /*
        private static void OnJoin(IntPtr _instance, IntPtr _user, IntPtr _methodInfo)
        {
            s_userJoined(_instance, _user, _methodInfo);
            var vrcPlayer = UnhollowerSupport.Il2CppObjectPtrToIl2CppObject<VRC.Player>(_user);
            //Chatbox.UpdateMyChatBoxOnJoin(vrcPlayer);
            if (!s_rawTags.Contains(vrcPlayer.field_Private_APIUser_0.id)) return;
            PlateHandler(vrcPlayer, overlay);
        }*/

        private static Plate s_plate { get; set; }
        private static PlateStatic platestatic { get; set; }
        private static Json.Tags[]? s_tagsArr { get; set; }

        public static void NameplateESP(VRC.Player player)
        {
            if (player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5 != null)
            {
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5.transform.FindChild("Trust Text").gameObject.GetComponent<NameplateTextMeshProUGUI>().isOverlay = overlay;
            }
        }

        public static void PlateHandler(VRC.Player vrcPlayer, bool overlay)
        {
            try
            {
                platestatic = new PlateStatic(vrcPlayer, overlay);
                s_tagsArr = s_tags.records.Where(x => x.UserID == vrcPlayer.field_Private_APIUser_0.id).ToArray();
                for (int i = 0; i < s_tagsArr.Length; i++)
                {
                    if (!s_tagsArr[i].Active) continue;
                    s_stringInstance = s_tagsArr[i].Size == null ? "" : s_tagsArr[i].Size;
                    for (int g = 0; g < s_tagsArr[i].Tag.Length; g++)
                    {
                        s_plate = new Plate(vrcPlayer, NameplateStatsLoaded || SnaxyTagsLoaded ? -158.75f - (g * 28f) : -128.75f - (g * 28f), overlay);
                        if (s_tagsArr[i].TextActive)
                        {
                            s_plate.Text.text += $"{s_tagsArr[i].Tag[g]}";
                            s_plate.Text.enabled = true;
                            s_plate.Text.gameObject.SetActive(true);
                            s_plate.Text.gameObject.transform.parent.gameObject.SetActive(true);
                            s_plate.Text.isOverlay = overlay;
                        }
                        if (!s_tagsArr[i].TextActive)
                        {
                            s_plate.Text.enabled = false;
                            s_plate.Text.gameObject.SetActive(false);
                            s_plate.Text.gameObject.transform.parent.gameObject.SetActive(false);
                        }
                    }
                    platestatic.TextID.text = $"<color=#ffffff>[</color><color=#808080>{s_tagsArr[i].id}</color><color=#ffffff>]";
                    platestatic.TextID.isOverlay = overlay;
                    if (s_tagsArr[i].Malicious)
                    {
                        platestatic.TextM.text += $"</color><color=#ff0000>Malicious User</color>";
                        platestatic.TextM.isOverlay = overlay;
                    }
                    if (!s_tagsArr[i].Malicious)
                    {
                        platestatic.TextM.text += $"</color><b><color=#ff0000>-</color> <color=#ff7f00>F</color><color=#ffff00>e</color><color=#80ff00>w</color><color=#00ff00>T</color><color=#00ff80>a</color><color=#00ffff>g</color><color=#0000ff>s</color> <color=#8b00ff>-</color><color=#ffffff></b>";
                        platestatic.TextM.isOverlay = overlay;
                    }
                    if (s_tagsArr[i].BigTextActive)
                    {
                        platestatic.TextBP.text += $"{s_stringInstance}{s_tagsArr[i].PlateBigText}</size>";
                        platestatic.TextBP.enabled = true;
                        platestatic.TextBP.gameObject.SetActive(true);
                        platestatic.TextBP.gameObject.transform.parent.gameObject.SetActive(true);
                        platestatic.TextBP.isOverlay = overlay;
                    }
                    if (!s_tagsArr[i].BigTextActive)
                    {
                        platestatic.TextBP.enabled = false;
                        platestatic.TextBP.gameObject.SetActive(false);
                        platestatic.TextBP.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            catch { }
        }
    }
}
