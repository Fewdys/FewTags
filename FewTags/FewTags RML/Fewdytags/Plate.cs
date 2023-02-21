using BestHTTP.SecureProtocol.Org.BouncyCastle.Pkix;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRC.SDKBase;

//Code Is Still Aids But It Is What It Is Bc I Don't Feel Like Redoing The Json's

namespace FewTags
{
    internal class Plate
    {
        public TMPro.TextMeshProUGUI Text5 { get; set; }
        public TMPro.TextMeshProUGUI Text4 { get; set; }
        public TMPro.TextMeshProUGUI Text3 { get; set; }
        public TMPro.TextMeshProUGUI Text2 { get; set; }
        public TMPro.TextMeshProUGUI Text { get; set; }
        public static GameObject _gameObject5 { get; set; }
        public static GameObject _gameObject4 { get; set; }
        public static GameObject _gameObject3 { get; set; }
        public static GameObject _gameObject2 { get; set; }
        public static GameObject _gameObject { get; set; }
        private RectTransform[] _rectTransforms5 { get; set; }
        private RectTransform[] _rectTransforms4 { get; set; }
        private RectTransform[] _rectTransforms3 { get; set; }
        private RectTransform[] _rectTransforms2 { get; set; }
        private RectTransform[] _rectTransforms { get; set; }
        public bool overlay;

        ~Plate() { _rectTransforms = null; _gameObject = null; }
        public Plate(VRC.Player __0)
        {
            //Main Plate
            _gameObject = GameObject.Instantiate(__0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5, __0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_0.transform).gameObject;
            _gameObject.name = "FewTagsPlate";
            Text = _gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>().First(x => x.name == "Trust Text");
            _rectTransforms = _gameObject.GetComponentsInChildren<RectTransform>().Where(x => x.name != "Trust Text" && x.name != "FewTagsPlate").ToArray();
            for (int i = 0; i < _rectTransforms.Length; i++)
            {
                try
                {
                    Object.DestroyImmediate(_rectTransforms[i].gameObject);
                }
                catch { }
            }
            _gameObject.transform.localPosition = new Vector3(0, Main.Position, 0);
            _gameObject.SetActive(true);
            //Text.isOverlay = overlay;
            Text.text = "";

            //Plate 2
            _gameObject2 = GameObject.Instantiate(__0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5, __0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_0.transform).gameObject;
            _gameObject2.name = "FewTagsPlate2";
            Text2 = _gameObject2.GetComponentsInChildren<TMPro.TextMeshProUGUI>().First(x => x.name == "Trust Text");
            _rectTransforms2 = _gameObject2.GetComponentsInChildren<RectTransform>().Where(x => x.name != "Trust Text" && x.name != "FewTagsPlate2").ToArray();
            for (int i = 0; i < _rectTransforms2.Length; i++)
            {
                try
                {
                    Object.DestroyImmediate(_rectTransforms2[i].gameObject);
                }
                catch { }
            }
            _gameObject2.transform.localPosition = new Vector3(0, Main.Position2, 0);
            _gameObject2.SetActive(true);
            //Text2.isOverlay = overlay;
            Text2.text = "";

            //Plate3
            _gameObject3 = GameObject.Instantiate(__0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5, __0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_0.transform).gameObject;
            _gameObject3.name = "FewTagsPlate3";
            Text3 = _gameObject3.GetComponentsInChildren<TMPro.TextMeshProUGUI>().First(x => x.name == "Trust Text");
            _rectTransforms3 = _gameObject3.GetComponentsInChildren<RectTransform>().Where(x => x.name != "Trust Text" && x.name != "FewTagsPlate3").ToArray();
            for (int i = 0; i < _rectTransforms3.Length; i++)
            {
                try
                {
                    Object.DestroyImmediate(_rectTransforms3[i].gameObject);
                }
                catch { }
            }
            _gameObject3.transform.localPosition = new Vector3(0, Main.Position3, 0);
            _gameObject3.SetActive(true);
            //Text3.isOverlay = overlay;
            Text3.text = "";

            //Plate4
            _gameObject4 = GameObject.Instantiate(__0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5, __0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_0.transform).gameObject; ;
            _gameObject4.name = "FewTags";
            Text4 = _gameObject4.GetComponentsInChildren<TMPro.TextMeshProUGUI>().First(x => x.name == "Trust Text");
            _rectTransforms4 = _gameObject4.GetComponentsInChildren<RectTransform>().Where(x => x.name != "Trust Text" && x.name != "FewTags").ToArray();
            for (int i = 0; i < _rectTransforms4.Length; i++)
            {
                try
                {
                    Object.DestroyImmediate(_rectTransforms4[i].gameObject);
                }
                catch { }
            }
            _gameObject4.transform.localPosition = new Vector3(0, Main.PositionMalicious, 0);
            _gameObject4.SetActive(true);
            //Text4.isOverlay = overlay;
            Text4.text = "";

            //BigPlate
            _gameObject5 = GameObject.Instantiate(__0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5, __0._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_0.transform).gameObject;
            _gameObject5.name = "FewTagsBigPlate";
            Text5 = _gameObject5.GetComponentsInChildren<TMPro.TextMeshProUGUI>().First(x => x.name == "Trust Text");
            _rectTransforms5 = _gameObject5.GetComponentsInChildren<RectTransform>().Where(x => x.name != "Trust Text" && x.name != "FewTagsBigPlate").ToArray();
            for (int i = 0; i < _rectTransforms5.Length; i++)
            {
                try
                {
                    Object.DestroyImmediate(_rectTransforms5[i].gameObject);
                }
                catch { }
            }
            _gameObject5.transform.localPosition = new Vector3(0, Main.PositionBigText, 0);
            _gameObject5.transform.GetComponent<ImageThreeSlice>().color = new Color(1, 1, 1, 0f);
            _gameObject5.SetActive(true);
            //Text5.isOverlay = overlay;
            Text5.text = "";
        }
    }
}