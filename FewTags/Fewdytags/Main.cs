using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MelonLoader;
using System.Net;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using VRC.SDKBase;

//Thanks To Edward7 For Helping Me Redo This

namespace FewTags
{
    public class Main : MelonMod
    {
        private static Json.Tags s_tags { get; set; }
        private static string s_rawTags { get; set; }

        private static MethodInfo s_joinMethod { get; set; }
        public static bool SnaxyTagsLoaded { get; private set; }
        public static bool ProPlatesLoaded { get; private set; }
        public static bool AbyssClientLoaded { get; private set; }
        public static bool AstrayLoaded { get; private set; }
        public static bool ErrorClientLoaded { get; private set; }
        internal static float Position { get; set; }
        internal static float Position2 { get; set; }
        internal static float Position3 { get; set; }
        internal static float PositionMalicious { get; set; }
        internal static float PositionBigText { get; set; }
        private static string s_stringInstance { get; set; }


        private delegate IntPtr userJoined(IntPtr _instance, IntPtr _user, IntPtr _methodinfo);
        private static userJoined s_userJoined;


        public override void OnApplicationStart()
        {
            SnaxyTagsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "SnaxyTagsV3"); //For When He Updates It To Be V3
            SnaxyTagsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "SnaxyTagsV3.dll"); //This Is Here Bc Who Fkn Knows With Null - He Likes To Meme
            SnaxyTagsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "SnaxyTagsV2");
            ProPlatesLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "ProPlates");
            AbyssClientLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "AbyssLoader");
            ErrorClientLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "ErrorClient");
            MelonLogger.Msg(ConsoleColor.Green, "Started FewTags");


            using (WebClient wc = new WebClient())
            {
                s_rawTags = wc.DownloadString("https://raw.githubusercontent.com/Fewdys/tags/main/FewTagsv2.json");
                s_tags = JsonConvert.DeserializeObject<Json.Tags>(s_rawTags);
            }
            NativeHook();

            //Checks For Other Mods (Positions For A Fixed ProPlates and Snaxy Aren't Updated - Abyss Positions Might Not Be Updated Now Due To It Being C++)

            //If Snaxy, ProPlates, Abyss and Astray are Loaded
            if (FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded)
            {
                PositionMalicious = -143f;
                Position = -171f;
                Position2 = -199f;
                Position3 = -227f;
                PositionBigText = -283f;

            }
            //If ProPlates and Abyss are Loaded
            else if (!FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & !FewTags.Main.AstrayLoaded)
            {
                PositionMalicious = -114.1f;
                Position = -142.1f;
                Position2 = -170.1f;
                Position3 = -198.1f;
                PositionBigText = -254.1f;
            }
            //If Snaxy and Abyss are Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & !FewTags.Main.AstrayLoaded)
            {
                PositionMalicious = -114.1f;
                Position = -142.1f;
                Position2 = -170.1f;
                Position3 = -198.1f;
                PositionBigText = -254.1f;
            }
            //If Snaxy and ProPlates are Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.AstrayLoaded)
            {
                PositionMalicious = -114.1f;
                Position = -142.1f;
                Position2 = -170.1f;
                Position3 = -198.1f;
                PositionBigText = -254.1f;
            }
            //If Nothing Is Loaded
            else if (!FewTags.Main.ProPlatesLoaded & !FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.AstrayLoaded)
            {
                PositionMalicious = -70.95f;
                Position = -97.95f;
                Position2 = -124.95f;
                Position3 = -152.95f;
                PositionBigText = -218.95f;
            }
            //If Abyss Is Loaded
            else if (!FewTags.Main.ProPlatesLoaded & !FewTags.Main.SnaxyTagsLoaded & FewTags.Main.AbyssClientLoaded & !FewTags.Main.AstrayLoaded)
            {
                PositionMalicious = -114.1f;
                Position = -142.1f;
                Position2 = -170.1f;
                Position3 = -198.1f;
                PositionBigText = -254.1f;
            }
            //If ProPlates Is Loaded
            else if (FewTags.Main.ProPlatesLoaded & !FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.AstrayLoaded)
            {
                PositionMalicious = -87.75f;
                Position = -115.75f;
                Position2 = -143.75f;
                Position3 = -171.75f;
                PositionBigText = -227.75f;
            }
            //If Snaxy Is Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.ProPlatesLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.AstrayLoaded)
            {
                PositionMalicious = -90f;
                Position = -118f;
                Position2 = -146f;
                Position3 = -174f;
                PositionBigText = -230f;
            }
        }

        private unsafe void NativeHook()
        {
            var methodInfos = typeof(MonoBehaviourPrivateAc1AcOb2AcInStHa2Unique).GetMethods().First(x => x.Name == "Method_Public_Void_MonoBehaviourPublicAPOb_v_pObBo_UBoVRObUnique_0"); //MonoBehaviourPrivateAc1AcOb2AcInStHa2Unique - NetworkManager (Contains NetworkManager lol) //Method_Public_Void_MonoBehaviourPublicAPOb_v_pObBo_UBoVRObUnique_0 - Join Method - Often Changes (Can Get By Hooking NetworkManager)

            var methodPointer = *(IntPtr*)(IntPtr)UnhollowerBaseLib.UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(methodInfos).GetValue(null);
            MelonUtils.NativeHookAttach((IntPtr)(&methodPointer), typeof(FewTags.Main).GetMethod(nameof(OnJoin), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
            s_userJoined = Marshal.GetDelegateForFunctionPointer<userJoined>(methodPointer);
        }



        private static void OnJoin(IntPtr _instance, IntPtr _user, IntPtr _methodInfo)
        {
            s_userJoined(_instance, _user, _methodInfo);
            var vrcPlayer = UnhollowerSupport.Il2CppObjectPtrToIl2CppObject<MonoBehaviourPublicAPOb_v_pObBo_UBoVRObUnique>(_user);
            if (!s_rawTags.Contains(vrcPlayer.field_Private_APIUser_0.id)) return;
            PlateHandler(vrcPlayer);
            //MelonLogger.Msg("Test");
            //MelonLogger.Msg(vrcPlayer.field_Private_APIUser_0.displayName + " Has Joined");
        }

        private static Plate s_plate { get; set; }
        private static Json.Tag[] s_tagsArr { get; set; }


        private static void PlateHandler(MonoBehaviourPublicAPOb_v_pObBo_UBoVRObUnique vrcPlayer) //MonoBehaviourPublicAPOb_v_pObBo_UBoVRObUnique - Refrence For Updating (Contains id, username, displayname, avatarid ect *vrcplayer*)
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

                    }
                    if (s_tagsArr[i].TextActive)
                    {
                        s_plate.Text.text += $"<color=#ffffff>[</color><color=#808080>{s_tagsArr[i].id}</color><color=#ffffff>] - </color>{s_tagsArr[i].PlateText}";
                        s_plate.Text.enabled = true;
                        s_plate.Text.gameObject.SetActive(true);
                        s_plate.Text.gameObject.transform.parent.gameObject.SetActive(true);
                    }
                    if (s_tagsArr[i].Malicious)
                    {
                        s_plate.Text4.text += $"<color=#ff0000>Malicious User</color>";
                    }
                    if (!s_tagsArr[i].Malicious)
                    {
                        s_plate.Text4.text += $"<b><color=#ff0000>-</color> <color=#ff7f00>F</color><color=#ffff00>e</color><color=#80ff00>w</color><color=#00ff00>T</color><color=#00ff80>a</color><color=#00ffff>g</color><color=#0000ff>s</color> <color=#8b00ff>-</color><color=#ffffff></b>";
                    }
                    if (s_tagsArr[i].Text2Active)
                    {
                        s_plate.Text2.text += $"{s_tagsArr[i].PlateText2}";
                        s_plate.Text2.enabled = true;
                        s_plate.Text2.gameObject.SetActive(true);
                        s_plate.Text2.gameObject.transform.parent.gameObject.SetActive(true);
                    }
                    if (s_tagsArr[i].Text3Active)
                    {
                        s_plate.Text3.text += $"{s_tagsArr[i].PlateText3}";
                        s_plate.Text3.enabled = true;
                        s_plate.Text3.gameObject.SetActive(true);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(true);
                    }
                    if (s_tagsArr[i].BigTextActive)
                    {
                        s_plate.Text5.text += $"{s_stringInstance}{s_tagsArr[i].PlateBigText}</size>";
                        s_plate.Text5.enabled = true;
                        s_plate.Text5.gameObject.SetActive(true);
                        s_plate.Text5.gameObject.transform.parent.gameObject.SetActive(true);
                    }
                    else if (!s_tagsArr[i].Text2Active & !s_tagsArr[i].Text3Active & !s_tagsArr[i].BigTextActive)
                    {
                        s_plate.Text2.enabled = false;
                        s_plate.Text2.gameObject.SetActive(false);
                        s_plate.Text2.gameObject.transform.parent.gameObject.SetActive(false);
                        s_plate.Text3.enabled = false;
                        s_plate.Text3.gameObject.SetActive(false);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(false);
                        s_plate.Text5.enabled = false;
                        s_plate.Text5.gameObject.SetActive(false);
                        s_plate.Text5.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    else if (!s_tagsArr[i].Text2Active & !s_tagsArr[i].Text3Active)
                    {
                        s_plate.Text2.enabled = false;
                        s_plate.Text2.gameObject.SetActive(false);
                        s_plate.Text2.gameObject.transform.parent.gameObject.SetActive(false);
                        s_plate.Text3.enabled = false;
                        s_plate.Text3.gameObject.SetActive(false);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    else if (!s_tagsArr[i].Text3Active)
                    {
                        s_plate.Text3.text += $"{s_tagsArr[i].PlateText3}";
                        s_plate.Text3.enabled = false;
                        s_plate.Text3.gameObject.SetActive(false);
                        s_plate.Text3.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    else if (!s_tagsArr[i].Text2Active)
                    {
                        s_plate.Text2.text += $"{s_tagsArr[i].PlateText2}";
                        s_plate.Text2.enabled = false;
                        s_plate.Text2.gameObject.SetActive(false);
                        s_plate.Text2.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    else if (!s_tagsArr[i].BigTextActive)
                    {
                        s_plate.Text5.text += $"{s_stringInstance}{s_tagsArr[i].PlateBigText}";
                        s_plate.Text5.enabled = false;
                        s_plate.Text5.gameObject.SetActive(false);
                        s_plate.Text5.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            catch { }
        }
    }
}
