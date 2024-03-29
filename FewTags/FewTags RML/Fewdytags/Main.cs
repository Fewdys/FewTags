﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MelonLoader;
using System.Net;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using VRC.SDKBase;
using VRC;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine.UI;
using VRC.Core;
using static VRC.SDKBase.VRC_SpecialLayer;

namespace FewTags
{
    public class Main : MelonMod
    {
        private static Json.Tags s_tags { get; set; }
        public static string s_rawTags { get; set; }

        private static MethodInfo s_joinMethod { get; set; }
        public static bool SnaxyTagsLoaded { get; private set; }
        public static bool AbyssClientLoaded { get; private set; }
        public static bool NameplateStatsLoaded { get; private set; }
        public static bool VanixClientLoaded { get; private set; }
        internal static float Position { get; set; }
        internal static float Position2 { get; set; }
        internal static float Position3 { get; set; }
        internal static float PositionMalicious { get; set; }
        internal static float PositionBigText { get; set; }
        private static string s_stringInstance { get; set; }
        public static bool overlay;

        private delegate IntPtr userJoined(IntPtr _instance, IntPtr _user, IntPtr _methodinfo);
        private static userJoined s_userJoined;

        public override void OnApplicationLateStart()
        {
            new WaitForSeconds(3f);
            NativeHook();
            //Task.Run(() => { OnPlayer.InitPatches(); });
            UpdateTags();
        }

        public override void OnApplicationStart()
        {
            //SnaxyTagsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "SnaxyTagsV2");
            //ProPlatesLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "ProPlates");
            //AbyssClientLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "AbyssLoader");
            NameplateStatsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "NameplateStats");
            VanixClientLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "Vanix Client");
            MelonLogger.Msg(ConsoleColor.Green, "Started FewTags");
            MelonLogger.Msg(ConsoleColor.DarkCyan, "To ReFetch Tags Press L + F (Rejoin Required)");
            overlay = false;
            MelonLogger.Msg(ConsoleColor.Green, "Finished Fetching Tags (This Message Doesn't Appear When Tags Are ReFetched)");
            MelonLogger.Msg(ConsoleColor.Green, "Tagged Players - Nameplate ESP On: RightShift + O (Rejoin Required)");
            MelonLogger.Msg(ConsoleColor.Red, "Tagged Players - Nameplate ESP Off: RightShift + P (Rejoin Required)");

            //Checks For Other Mods (Positions For A Fixed ProPlates and Snaxy Aren't Updated - Abyss Positions Might Not Be Updated Now Due To It Being C++)
            //If Nothing Is Loaded
            if (!FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.VanixClientLoaded & !FewTags.Main.NameplateStatsLoaded)
            {
                PositionMalicious = -70.95f;
                Position = -97.95f;
                Position2 = -124.95f;
                Position3 = -152.95f;
                PositionBigText = 182.25f;
            }
            //If Vanix Client Is Loaded
            if (FewTags.Main.VanixClientLoaded)
            {
                PositionMalicious = -70.95f;
                Position = -97.95f;
                Position2 = -124.95f;
                Position3 = -152.95f;
                PositionBigText = 202f;
            }
            //If NameplateStats is Loaded
            else if (FewTags.Main.NameplateStatsLoaded)
            {
                PositionMalicious = -98.25f;
                Position = -126.25f;
                Position2 = -154.25f;
                Position3 = -182.25f;
                PositionBigText = 182.25f;
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F) & Input.GetKey(KeyCode.L))
            {
                UpdateTags();
            }
        }

        public override void OnLateUpdate()
        {
            if (Input.GetKey(KeyCode.RightShift) & Input.GetKeyDown(KeyCode.O))
            {
                try
                {
                    if (s_plate.Text.isActiveAndEnabled)
                    {
                        overlay = true;
                        s_plate.Text.isOverlay = true;
                        MelonLogger.Msg(ConsoleColor.Green, "(Tagged Players) Nameplate ESP On");
                    }
                }
                catch { }
            }

            if (Input.GetKey(KeyCode.RightShift) & Input.GetKeyDown(KeyCode.P))
            {
                try
                {
                    if (s_plate.Text.isActiveAndEnabled)
                    {
                        overlay = false;
                        s_plate.Text.isOverlay = false;
                        MelonLogger.Msg(ConsoleColor.Red, "(Tagged Players) Nameplate ESP Off");
                    }
                }
                catch { }
            }
        }

        private unsafe void NativeHook()
        {
            var methodInfos = typeof(NetworkManager).GetMethods().Where(x => x.Name.StartsWith("Method_Public_Void_Player_")).ToArray();

            for (int i = 0; i < methodInfos.Length; i++)
            {
                var mt = UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(methodInfos[i]).ToArray();
                for (int j = 0; j < mt.Length; j++)
                {
                    if (mt[j].Type != UnhollowerRuntimeLib.XrefScans.XrefType.Global) continue;

                    if (mt[j].ReadAsObject().ToString().Contains("OnPlayer"))
                    {
                        var methodPointer = *(IntPtr*)(IntPtr)UnhollowerBaseLib.UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(methodInfos[i]).GetValue(null);
                        MelonUtils.NativeHookAttach((IntPtr)(&methodPointer), typeof(FewTags.Main).GetMethod(nameof(OnJoin), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
                        s_userJoined = Marshal.GetDelegateForFunctionPointer<userJoined>(methodPointer);
                    }
                }
            }
        }

        void UpdateTags()
        {

            using (WebClient wc = new WebClient())
            {
                s_rawTags = wc.DownloadString("https://raw.githubusercontent.com/Fewdys/tags/main/FewTagsv2.json");
                s_tags = JsonConvert.DeserializeObject<Json.Tags>(s_rawTags);
            }
            MelonLogger.Msg(ConsoleColor.Yellow, "Fetching Tags (If L + F Was Pressed This Could Potentially Cause Some Lag)");
        }

        private static void OnJoin(IntPtr _instance, IntPtr _user, IntPtr _methodInfo)
        {
            s_userJoined(_instance, _user, _methodInfo);
            var vrcPlayer = UnhollowerSupport.Il2CppObjectPtrToIl2CppObject<VRC.Player>(_user);
            UpdateMyChatBoxOnJoin(vrcPlayer);
            if (!s_rawTags.Contains(vrcPlayer.field_Private_APIUser_0.id)) return;
            PlateHandler(vrcPlayer, overlay);
        }

        private static Plate s_plate { get; set; }
        private static Json.Tag[] s_tagsArr { get; set; }


        public static void PlateHandler(VRC.Player vrcPlayer, bool overlay)
        {
            try
            {
                s_plate = new Plate(vrcPlayer);
                s_tagsArr = s_tags.records.Where(x => x.UserID == vrcPlayer.field_Private_APIUser_0.id).ToArray();
                for (int i = 0; i < s_tagsArr.Length; i++)
                {
                    if (!s_tagsArr[i].Active) continue;
                    s_stringInstance = s_tagsArr[i].Size == null ? "" : s_tagsArr[i].Size;
                    if (!s_tagsArr[i].TextActive)
                    {
                        s_plate.Text.enabled = false;
                        s_plate.Text.gameObject.SetActive(false);
                        s_plate.Text.gameObject.transform.parent.gameObject.SetActive(false);
                        s_plate.Text2.enabled = false;
                        s_plate.Text2.gameObject.SetActive(false);
                        s_plate.Text2.gameObject.transform.parent.gameObject.SetActive(false);
                        s_plate.Text3.enabled = false;
                        s_plate.Text3.gameObject.SetActive(false);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    if (s_tagsArr[i].TextActive)
                    {
                        s_plate.Text.text += $"<color=#ffffff>[</color><color=#808080>{s_tagsArr[i].id}</color><color=#ffffff>] - </color>{s_tagsArr[i].PlateText}";
                        s_plate.Text.enabled = true;
                        s_plate.Text.gameObject.SetActive(true);
                        s_plate.Text.gameObject.transform.parent.gameObject.SetActive(true);
                        s_plate.Text.isOverlay = overlay;
                    }
                    if (s_tagsArr[i].Malicious)
                    {
                        s_plate.Text4.text += $"<color=#ff0000>Malicious User</color>";
                        s_plate.Text4.isOverlay = overlay;
                    }
                    if (!s_tagsArr[i].Malicious)
                    {
                        s_plate.Text4.text += $"<b><color=#ff0000>-</color> <color=#ff7f00>F</color><color=#ffff00>e</color><color=#80ff00>w</color><color=#00ff00>T</color><color=#00ff80>a</color><color=#00ffff>g</color><color=#0000ff>s</color> <color=#8b00ff>-</color><color=#ffffff></b>";
                        s_plate.Text4.isOverlay = overlay;
                    }
                    if (s_tagsArr[i].Text2Active)
                    {
                        s_plate.Text2.text += $"{s_tagsArr[i].PlateText2}";
                        s_plate.Text2.enabled = true;
                        s_plate.Text2.gameObject.SetActive(true);
                        s_plate.Text2.gameObject.transform.parent.gameObject.SetActive(true);
                        s_plate.Text2.isOverlay = overlay;
                    }
                    if (s_tagsArr[i].Text3Active)
                    {
                        s_plate.Text3.text += $"{s_tagsArr[i].PlateText3}";
                        s_plate.Text3.enabled = true;
                        s_plate.Text3.gameObject.SetActive(true);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(true);
                        s_plate.Text3.isOverlay = overlay;
                    }
                    if (s_tagsArr[i].BigTextActive)
                    {
                        s_plate.Text5.text += $"{s_stringInstance}{s_tagsArr[i].PlateBigText}</size>";
                        s_plate.Text5.enabled = true;
                        s_plate.Text5.gameObject.SetActive(true);
                        s_plate.Text5.gameObject.transform.parent.gameObject.SetActive(true);
                        s_plate.Text5.isOverlay = overlay;
                    }
                    if (!s_tagsArr[i].Text2Active)
                    {
                        s_plate.Text2.enabled = false;
                        s_plate.Text2.gameObject.SetActive(false);
                        s_plate.Text2.gameObject.transform.parent.gameObject.SetActive(false);
                        s_plate.Text3.enabled = false;
                        s_plate.Text3.gameObject.SetActive(false);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    if (!s_tagsArr[i].Text3Active)
                    {
                        s_plate.Text3.enabled = false;
                        s_plate.Text3.gameObject.SetActive(false);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    if (!s_tagsArr[i].BigTextActive)
                    {
                        s_plate.Text5.enabled = false;
                        s_plate.Text5.gameObject.SetActive(false);
                        s_plate.Text5.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            catch { }
        }

        public static void UpdateMyChatBoxOnJoin(Player vrcPlayer)
        {
            try
            {
                /*if (vrcPlayer.field_Private_APIUser_0.isFriend)
                {
                    vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubble/Canvas/ChatText").gameObject.transform.gameObject.GetComponent<TMPro.TextMeshProUGUI>().color = Color.cyan;
                    vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubble/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color = new Color(0, 0, 0, 0.29f);
                    vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubbleMirror/Canvas/ChatText").gameObject.transform.gameObject.GetComponent<TMPro.TextMeshProUGUI>().color = Color.cyan;
                    vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubbleMirror/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color = new Color(0, 0, 0, 0.29f);
                }*/
                if (vrcPlayer.IsMe() == true)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubble/Canvas/ChatText").gameObject.transform.gameObject.GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                    vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubble/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color = new Color(0, 0, 0, 0.29f);
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubbleMirror/Canvas/ChatText").gameObject.transform.gameObject.GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                    vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubbleMirror/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color = new Color(0, 0, 0, 0.29f);
                }
                else
                {
                    if (vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubble/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color != new Color(0, 0, 0, 0.29f) || vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubbleMirror/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color != new Color(0, 0, 0, 0.29f))
                    {
                        vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubble/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color = new Color(0, 0, 0, 0.29f);
                        vrcPlayer._vrcplayer.field_Public_GameObject_0.gameObject.transform.FindChild("ChatBubbleMirror/Canvas").gameObject.transform.gameObject.GetComponent<Graphic>().color = new Color(0, 0, 0, 0.29f);
                    }
                }
            }
            catch { }
        }
    }
}
