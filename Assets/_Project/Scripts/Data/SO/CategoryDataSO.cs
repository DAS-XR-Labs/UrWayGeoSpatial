using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Enumeration defining various categories for locations.
    /// </summary>
    public enum Category
    {
        All = -1,
        Restaurant = 22,
        Trail = 6,
        Hotel = 17,
        Bar_Grill = 16,
        CoffeShop = 0,
        Mine = 27,
        Mountain = 5,
        Store = 20,
        Swimming = 14,
        TouristAttraction = 10,
        Historical = 32,
        Park = 4,
        Waterfall = 2,
        PublicRestroom = 1,
        City = 3,
        
        //Reserved for Scavenger Hunt mode
        ScavengerHunt = 101,
    }
    
    /// <summary>
    /// Represents information about a category.
    /// </summary>
    [System.Serializable]
    public class CategoryInfo
    {
        /// <summary>
        /// The name of the category.
        /// </summary>
        [FormerlySerializedAs("name")] public string Name;

        /// <summary>
        /// The type of category.
        /// </summary>
        [FormerlySerializedAs("categoryType")] public Category CategoryType;

        /// <summary>
        /// The icon associated with the category.
        /// </summary>
        [FormerlySerializedAs("icon")]
        [TableColumnWidth(57, Resizable = false)]
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite Icon;
    }
    
    /// <summary>
    /// ScriptableObject class containing category information.
    /// </summary>
    [CreateAssetMenu(fileName = "CategoryData", menuName = "Scriptable Objects/Category Data")]
    public class CategoryDataSO : ScriptableObject
    {
       // private bool isFirstCallCompleted = false;
        
        [FormerlySerializedAs("categories")]
        [SerializeField]
        [TableList(ShowIndexLabels = true)]
        private List<CategoryInfo>
            m_categories = new List<CategoryInfo>(); // List of CategoryInfo objects representing different categories.

        [FormerlySerializedAs("enableScavengerHunt")]
        [Space(10)]
        [SerializeField] private bool m_enableScavengerHunt;
        [FormerlySerializedAs("scavengerHuntName")]
        [ShowIf("m_enableScavengerHunt")]
        [SerializeField] private string m_scavengerHuntName = "ScavengerHunt";
        [FormerlySerializedAs("scavengerHuntType")]
        [ReadOnly]
        [ShowIf("m_enableScavengerHunt")]
        [SerializeField] private Category m_scavengerHuntType = Category.TouristAttraction;
        [FormerlySerializedAs("scavengerHuntIcon")]
        [ShowIf("m_enableScavengerHunt")]
        [TableColumnWidth(57, Resizable = false)]
        [SerializeField] private Sprite m_scavengerHuntIcon;


        public List<CategoryInfo> Categories {
            get
            {
                if (m_enableScavengerHunt)
                {
                    CategoryInfo _item = m_categories.Find(x => x.CategoryType == Category.ScavengerHunt);
                    if (_item == null)
                    {
                        _item = new CategoryInfo();
                        _item.Name = m_scavengerHuntName;
                        _item.CategoryType = m_scavengerHuntType;
                        _item.Icon = m_scavengerHuntIcon;
                        m_categories.Add(_item);
                    }
                }
                return m_categories;
            }
        }
    }
}