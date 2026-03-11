using UnityEngine;

namespace Lifehandled.Presentation.Pure2D
{
    [CreateAssetMenu(menuName = "Lifehandled/Pure2D/Typography Theme", fileName = "Pure2DTypographyTheme")]
    public class Pure2DTypographyTheme : ScriptableObject
    {
        [Header("Recommended families: Inter / Poppins / Nunito")]
        public Font headingFont;

        [Header("Recommended families: Inter / Source Sans")]
        public Font bodyFont;

        [Header("Optional scrapbook accent")]
        public Font noteFont;

        public int headingSize = 26;
        public int bodySize = 18;
        public int noteSize = 17;
    }
}
