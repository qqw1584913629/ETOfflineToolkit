using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MH
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Any Localization/Auto Localization")]
    public class AutoLocalization : MonoBehaviour
    {
        [SerializeField] public string m_Key = string.Empty;
        private void OnEnable()
        {
            LocalizationSingleton.Instance.RegisterAutoLocalization(this);
            ShowText();
        }

        private void OnDisable()
        {
            LocalizationSingleton.Instance.UnRegisterAutoLocalization(this);
        }

        public void ShowText()
        {
            if (m_Key.Equals(string.Empty))
            {
                //只换font
                if (TryGetComponent(out TextMeshProUGUI textMeshProUGUI2))
                {
                    var fontAsset = LocalizationSingleton.Instance.GetFont(Init.Instance.Root);
                    textMeshProUGUI2.font = fontAsset;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                    return;
                }

                if (TryGetComponent(out TextMeshPro textMeshPro2))
                {
                    var fontAsset = LocalizationSingleton.Instance.GetFont(Init.Instance.Root);
                    textMeshProUGUI2.font = fontAsset;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                    return;
                }
                return;
            }

            string path = LocalizationSingleton.Instance.GetString(m_Key);

            if (TryGetComponent(out Text text))
            {
                text.text = path;
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                return;
            }

            if (TryGetComponent(out TextMeshProUGUI textMeshProUGUI))
            {
                var fontAsset = LocalizationSingleton.Instance.GetFont(Init.Instance.Root);
                textMeshProUGUI.font = fontAsset;
                textMeshProUGUI.text = path;
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                return;
            }

            if (TryGetComponent(out TextMeshPro textMeshPro))
            {
                var fontAsset = LocalizationSingleton.Instance.GetFont(Init.Instance.Root);
                textMeshProUGUI.font = fontAsset;
                textMeshPro.text = path;
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                return;
            }
        }
    }
}