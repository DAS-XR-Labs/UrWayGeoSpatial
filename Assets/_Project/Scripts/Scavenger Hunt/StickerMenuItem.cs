using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Represents a menu item displaying a sticker with a title.
    /// </summary>
    public class StickerMenuItem : MonoBehaviour
    {
        [FormerlySerializedAs("image")] [SerializeField] private Image m_image;
        [FormerlySerializedAs("title")] [SerializeField] private TMP_Text m_title;
        
        /// <summary>
        /// Initializes the menu item with the specified text and icon.
        /// </summary>
        /// <param name="text">The text/title to display.</param>
        /// <param name="icon">The icon/sticker to display.</param>
        public void Init(string text, Sprite icon)
        {
            m_title.text = text;
            m_image.sprite = icon;
        }
    }
}