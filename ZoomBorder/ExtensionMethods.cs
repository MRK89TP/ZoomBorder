using System.Windows.Media;

namespace ZoomBorder
{
    public static class ExtensionMethods
    {
        public static void SetX(this TranslateTransform tt, double value, ZoomBorder zoomBorder)
        {
            tt.X = value;
            zoomBorder.TranslationX = value;
        }

        public static void SetY(this TranslateTransform tt, double value, ZoomBorder zoomBorder)
        {
            tt.Y = value;
            zoomBorder.TranslationY = value;
        }

        public static void SetScale(this ScaleTransform st, double value, ZoomBorder zoomBorder)
        {
            st.ScaleX = value;
            st.ScaleY = value;
            zoomBorder.Scale = value;
        }
    }
}
