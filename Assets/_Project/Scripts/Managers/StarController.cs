using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Manages the star rating UI.
    /// </summary>
    public class StarController : MonoBehaviour
    {
        [FormerlySerializedAs("emptyStar")] public Sprite EmptyStar; // Sprite for an empty star
        [FormerlySerializedAs("fullStar")] public Sprite FullStar; // Sprite for a full star
        [FormerlySerializedAs("halfstar")] public Sprite Halfstar; // Sprite for a half star

        [FormerlySerializedAs("images")] public List<Image> Images = new List<Image>(); // List of Image components representing stars


        [FormerlySerializedAs("testRating")] public float TestRating; // Test rating value for testing purposes
        [FormerlySerializedAs("test")] public bool Test; // Flag for testing the script

        /// <summary>
        /// Called every frame.
        /// Updates stars based on testRating if test is true. Used for testing the script.
        /// </summary
        private void Update()
        {
            if (Test)
            {
                Test = false;
                UpdateStars(TestRating);
            }
        }


        /// <summary>
        /// Updates the star images based on the provided rating.
        /// </summary>
        /// <param name="rating">The rating to update stars for.</param>
        public void UpdateStars(float rating)
        {
            foreach (Image image in Images)
            {
                image.sprite = EmptyStar;
            }

            int _iRating = Mathf.FloorToInt(rating);

            for (int i = 0; i < _iRating; i++)
            {
                Images[i].sprite = FullStar;
            }

            if (rating > _iRating) Images[_iRating].sprite = Halfstar;
        }
    }
}