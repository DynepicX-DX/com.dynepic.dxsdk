using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace DX
{
    public class HyperlinkScript : MonoBehaviour, IPointerClickHandler
    {
        public string url;
        TextMeshProUGUI textMeshPro;

        private void Start()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
                switch (linkInfo.GetLinkID())
                {
                    case "Hyperlink":
                        Application.OpenURL(url);
                        break;
                }
            }
        }
    }
}