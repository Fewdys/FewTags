using System;
using UnityEngine;
using System.Linq;
namespace Fewdytags
{
    internal class Plate
    {
        public TMPro.TextMeshProUGUI Text { get; set; }
        private GameObject _gameObject { get; set; }
        private RectTransform[] _rectTransforms { get; set; }

        ~Plate() { _rectTransforms = null; _gameObject = null; }
        public Plate(VRC.Player player)
        {
            _gameObject = GameObject.Instantiate(player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_5, player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_GameObject_0.transform).gameObject;
            _gameObject.name = "Fewdy's Plate";
            _rectTransforms = _gameObject.GetComponentsInChildren<RectTransform>().Where(x => x.name != "Trust Text" && x.name != "Fewdy's Plate").ToArray();
            for (int i = 0; i < _rectTransforms.Length; i++)
            {
                try
                {
                    GameObject.DestroyImmediate(_rectTransforms[i].gameObject);
                }
                catch { }
            }
            _gameObject.SetActive(true);
            _gameObject.transform.localPosition = new Vector3(0, -60, 0);
            Text = _gameObject.transform.Find("Trust Text").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            Text.text = "";
        }

    }
}
