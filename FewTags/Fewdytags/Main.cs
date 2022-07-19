using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using System.Net;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnhollowerBaseLib;

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
        public static bool FewModLoaded { get; private set; }
        public static bool ErrorClientLoaded { get; private set; }
        internal static float Position { get; set; }
        private static string s_stringInstance { get; set; }

        private delegate IntPtr userJoined(IntPtr _instance, IntPtr _user, IntPtr _methodinfo);
        private static userJoined s_userJoined;


        public override void OnApplicationStart()
        {
            SnaxyTagsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "SnaxyTagsV3"); //For When He Updates It To Be V3
            SnaxyTagsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "SnaxyTagsV3.dll"); //This Is Here Bc Who Fkn Knows With Null
            SnaxyTagsLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "SnaxyTagsV2");
            ProPlatesLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "ProPlates");
            AbyssClientLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "AbyssLoader");
            ErrorClientLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "ErrorClient");
            FewModLoaded = MelonHandler.Mods.Any(m => m.Info.Name == "FewMod");
            MelonLogger.Msg(ConsoleColor.Green, "Started FewTags");

            using (WebClient wc = new WebClient())
            {
                s_rawTags = wc.DownloadString("https://raw.githubusercontent.com/Fewdys/tags/main/NamePlatesv2.json");
                s_tags = JsonConvert.DeserializeObject<Json.Tags>(s_rawTags);
            }
            NativeHook();
            //Checks For Other Mods

            //If Snaxy, ProPlates, Abyss and FewMod are Loaded
            if (FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -143f;
            }
            //If ProPlates and Abyss are Loaded
            else if (!FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & !FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If ProPlates and FewMod are Loaded
            else if (!FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & !FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If ProPlates, FewMod and Snaxy are Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & !FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -143f;
            }
            //If Abyss, FewMod and Snaxy are Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If ProPlates, FewMod and Abyss are Loaded
            else if (!FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -143f;
            }
            //If Snaxy and Abyss are Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & !FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If FewMod and Abyss are Loaded
            else if (!FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.ProPlatesLoaded & FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -143f;
            }
            //If FewMod and Snaxy are Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.ProPlatesLoaded & !FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If Snaxy and ProPlates are Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & FewTags.Main.ProPlatesLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If Nothing Is Loaded
            else if (!FewTags.Main.ProPlatesLoaded & !FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.FewModLoaded)
            {
                Position = -57.75f;
            }
            //If Abyss Is Loaded
            else if (!FewTags.Main.ProPlatesLoaded & !FewTags.Main.SnaxyTagsLoaded & FewTags.Main.AbyssClientLoaded & !FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If ProPlates Is Loaded
            else if (FewTags.Main.ProPlatesLoaded & !FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.FewModLoaded)
            {
                Position = -87.75f;
            }
            //If FewMod Is Loaded
            else if (!FewTags.Main.ProPlatesLoaded & !FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.AbyssClientLoaded & FewTags.Main.FewModLoaded)
            {
                Position = -114.1f;
            }
            //If Snaxy Is Loaded
            else if (FewTags.Main.SnaxyTagsLoaded & !FewTags.Main.ProPlatesLoaded & !FewTags.Main.AbyssClientLoaded & !FewTags.Main.FewModLoaded)
            {
                Position = -90f;
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

                    if (mt[j].ReadAsObject().ToString().Contains("OnPlayerJoin"))
                    {
                        var methodPointer = *(IntPtr*)(IntPtr)UnhollowerBaseLib.UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(methodInfos[i]).GetValue(null);
                        MelonUtils.NativeHookAttach((IntPtr)(&methodPointer), typeof(FewTags.Main).GetMethod(nameof(OnJoin), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
                        s_userJoined = Marshal.GetDelegateForFunctionPointer<userJoined>(methodPointer);
                    }


                }
            }
        }


        private static void OnWorldChange()
        {

        }

        private static void OnJoin(IntPtr _instance, IntPtr _user, IntPtr _methodInfo)
        {
            s_userJoined(_instance, _user, _methodInfo);
            var vrcPlayer = UnhollowerSupport.Il2CppObjectPtrToIl2CppObject<VRC.Player>(_user);
            if (!s_rawTags.Contains(vrcPlayer.field_Private_APIUser_0.id)) return;
            PlateHandler(vrcPlayer);
        }

        private static Plate s_plate { get; set; }
        private static Json.Tag[] s_tagsArr { get; set; }

        private static void PlateHandler(VRC.Player vrcPlayer)
        {
            s_plate = new Plate(vrcPlayer);
            Task.Run(new Action(() => {
                s_tagsArr = s_tags.records.Where(x => x.UserID == vrcPlayer.field_Private_APIUser_0.id).ToArray();
                for (int i = 0; i < s_tagsArr.Length; i++)
                {
                    if (!s_tagsArr[i].Active) continue;
                    s_stringInstance = s_tagsArr[i].Size == null ? "" : s_tagsArr[i].Size;
                    s_plate.Text.text += $"{s_stringInstance}<color=#ffffff>[</color><color=#808080>{s_tagsArr[i].id}</color><color=#ffffff>] - </color>{s_tagsArr[i].Text}<color=#ffffff></size> ";
                }

            }));

        }
    }
}
