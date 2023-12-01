using System.Windows;

namespace ZoomBorder
{
    public interface IZoomBorder
    {
        void Reset();
        void Uniform();
        void ZoomOnRectangle(Rect rectangle);
    }
}