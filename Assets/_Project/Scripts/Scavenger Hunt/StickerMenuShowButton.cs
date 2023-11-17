using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Represents a button to show a sticker menu and display the number of stickers.
    /// </summary>
    public class StickerMenuShowButton : MonoBehaviour
    {
        [FormerlySerializedAs("numberOfStickers")] [SerializeField] private TMP_Text m_numberOfStickers;

        /// <summary>
        /// Updates the displayed information about the number of stickers.
        /// </summary>
        /// <param name="currentN">The current number of stickers.</param>
        /// <param name="allN">The total number of stickers.</param>
        public void UpdateInfo(int currentN, int allN)
        {
            m_numberOfStickers.text = currentN + "/" + allN;
        }
    }
}