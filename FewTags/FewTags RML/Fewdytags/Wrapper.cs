using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Elements.Menus;
using VRCFlatBuffers;

namespace FewTags
{
    static class PlayerWrapper
    {
        public static Dictionary<int, Player> PlayersActorID = new Dictionary<int, Player>();
        public static VRC.Player LocalPlayer() => VRC.Player.prop_Player_0;
        public static VRCPlayer LocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;
        public static float GetFrames(this VRC.Player player) => (player._playerNet.prop_Byte_0 != 0) ? Mathf.Floor(1000f / (float)player._playerNet.prop_Byte_0) : -1f;
        public static short GetPing(this VRC.Player player) => player._playerNet.field_Private_Int16_0;
        public static bool IsBot(this VRC.Player player) => player.GetPing() <= 0 && player.GetFrames() <= 0 || player.transform.position == Vector3.zero;
        public static VRCPlayerApi GetVRCPlayerApi(this VRC.Player Instance) => Instance?.prop_VRCPlayerApi_0;
        public static bool GetIsMaster(this VRC.Player Instance) => Instance.GetVRCPlayerApi().isMaster;
        public static int GetActorNumber(this VRC.Player player) => player.GetVRCPlayerApi() != null ? player.GetVRCPlayerApi().playerId : -1;
        public static bool IsFriendsWith(this APIUser apiUser) => APIUser.CurrentUser.friendIDs.Contains(apiUser.id);
        public static VRCPlayer GetLocalVRCPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;
        public static bool IsMe(this Player p) => p.name == GetLocalVRCPlayer().name;

    }
}
