using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZoomBorder
{
    public partial class ZoomBorder : Border, IZoomBorder
    {
        private FrameworkElement _child;
        private Point _origin;
        private Point _start;
        private double _childWidth => _child.Width;
        private double _childHeight => _child.Height;
        private bool _isChildInitialized => _child != null && !double.IsNaN(_childWidth) && !double.IsNaN(_childHeight);

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (value != null && value != Child)
                    Initialize((FrameworkElement)value);

                base.Child = value;
            }
        }

        public ZoomBorder()
        {
            Background = new SolidColorBrush(Colors.LightGray);

            MouseWheel += OnMouseWheel;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;

            SizeChanged += ZoomBorder_SizeChanged;
        }

        public void Reset()
        {
            if (!_isChildInitialized)
                return;

            var tt = GetTranslateTransform(_child);
            var st = GetScaleTransform(_child);

            st.SetScale(1, this);

            tt.SetX(AnchorOrigin ? GetMaxX(ActualWidth) : 0, this);
            tt.SetY(AnchorOrigin ? GetMaxY(ActualHeight, st.ScaleY) : 0, this);
        }

        public void Uniform()
        {
            if (!_isChildInitialized)
                return;

            var propWidth = ActualWidth / _childWidth;
            var propHeight = ActualHeight / _childHeight;

            var zoom = Math.Min(propWidth, propHeight);

            var st = GetScaleTransform(_child);
            var tt = GetTranslateTransform(_child);

            Point relative = new(_childWidth / 2, _childHeight / 2);

            st.SetScale(zoom, this);

            if (AnchorOrigin)
            {
                tt.SetX(GetMaxX(ActualWidth), this);
                tt.SetY(GetMaxY(ActualHeight, st.ScaleY), this);
                return;
            }

            tt.SetX(relative.X - relative.X * st.ScaleX, this);
            tt.SetY(relative.Y - relative.Y * st.ScaleY, this);

            if (propWidth < 1 && propHeight >= 1)
            {
                tt.SetX(0, this);
                return;
            }

            if (propHeight < 1 && propWidth >= 1)
            {
                tt.SetY(0, this);
                return;
            }

            if (propWidth < 1 && propHeight < 1)
            {
                if (propWidth > propHeight)
                {
                    tt.SetX(ActualWidth / 2 - _childWidth * st.ScaleX / 2, this);
                    tt.SetY(0,this);
                }
                else
                {
                    tt.SetX(0, this);
                    tt.SetY(ActualHeight / 2 - _childHeight * st.ScaleY / 2, this);
                }
            }
        }

        public void ZoomOnRectangle(Rect rectangle)
        {
            if (!_isChildInitialized)
                return;

            var newRectangle = FixRectanglePoints(rectangle);

            var propWidth = ActualWidth / newRectangle.Width;
            var propHeight = ActualHeight / newRectangle.Height;

            var newZoom = Math.Min(propWidth, propHeight);

            var st = GetScaleTransform(_child);
            var tt = GetTranslateTransform(_child);

            st.SetScale(newZoom, this);

            if (AnchorOrigin)
            {
                var maxX = GetMaxX(ActualWidth);
                var candidateX = maxX - newRectangle.X * newZoom;

                var maxY = GetMaxY(ActualHeight, newZoom);
                var candidateY = maxY + rectangle.Y * newZoom;

                if (candidateX > maxX)
                    candidateX = maxX;

                if (candidateY < maxY)
                    candidateY = maxY;

                tt.SetX(candidateX, this);
                tt.SetY(candidateY, this);

                return;
            }


            if (propHeight < propWidth)
            {
                double xTo0;

                if (ActualWidth > _childWidth)
                    xTo0 = newRectangle.X * newZoom + (ActualWidth - _childWidth) / 2;
                else
                    xTo0 = newRectangle.X * newZoom;

                tt.SetX(-xTo0 + (ActualWidth / 2 - newRectangle.Width * newZoom / 2), this);

                if (ActualHeight < _childHeight)
                    tt.SetY(-(newRectangle.Y * newZoom), this);
                else
                    tt.SetY(-(newRectangle.Y * newZoom + (ActualHeight - _childHeight) / 2), this);
            }
            else
            {
                double yTo0;

                if (ActualHeight > _childHeight)
                    yTo0 = newRectangle.Y * newZoom + (ActualHeight - _childHeight) / 2;
                else
                    yTo0 = newRectangle.Y * newZoom;

                tt.SetY(-yTo0 + (ActualHeight / 2 - newRectangle.Height * newZoom / 2),this);

                if (ActualWidth < _childWidth)
                    tt.SetX(-(newRectangle.X * newZoom), this);
                else
                    tt.SetX(-(newRectangle.X * newZoom + (ActualWidth - _childWidth) / 2), this);
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_isChildInitialized)
                return;

            var st = GetScaleTransform(_child);
            var tt = GetTranslateTransform(_child);

            var candidateZoom = e.Delta > 0 ? st.ScaleX + ZoomFactor : st.ScaleX - ZoomFactor;

            if (candidateZoom < MinScale)
                candidateZoom = MinScale;

            if (candidateZoom > MaxScale)
                candidateZoom = MaxScale;

            var relative = e.GetPosition(_child);

            if (_child.LayoutTransform is ScaleTransform scaleTransform)
            {
                if (scaleTransform.ScaleY == -1)
                    relative.Y = _child.Height - relative.Y;
                if (scaleTransform.ScaleX == -1)
                    relative.X = _child.Width - relative.X;
            }

            var absoluteX = relative.X * st.ScaleX + tt.X;
            var absoluteY = relative.Y * st.ScaleY + tt.Y;

            st.SetScale(candidateZoom, this);

            var candidateX = absoluteX - relative.X * st.ScaleX;
            var candidateY = absoluteY - relative.Y * st.ScaleY;

            CheckTranslationLimits(candidateX, candidateY, st.ScaleX, out var x, out var y);

            tt.SetX(x, this);
            tt.SetY(y, this);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isChildInitialized || !_child.IsMouseCaptured)
                return;

            var tt = GetTranslateTransform(_child);
            var st = GetScaleTransform(_child);
            Vector v = _start - e.GetPosition(this);

            var candidateX = _origin.X - v.X;
            var candidateY = _origin.Y - v.Y;

            CheckTranslationLimits(candidateX, candidateY, st.ScaleX, out var x, out var y);

            tt.SetX(x, this);
            tt.SetY(y, this);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isChildInitialized)
                return;

            var tt = GetTranslateTransform(_child);
            _start = e.GetPosition(this);
            _origin = new Point(tt.X, tt.Y);
            Cursor = Cursors.Hand;
            _child.CaptureMouse();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isChildInitialized)
                return;

            _child.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
        }

        private void Initialize(FrameworkElement child)
        {
            _child = child;

            if (_child == null)
                return;

            _child.SizeChanged += Child_SizeChanged;

            TransformGroup group = new();
            ScaleTransform st = new();
            group.Children.Add(st);
            TranslateTransform tt = new();
            group.Children.Add(tt);
            _child.RenderTransform = group;
            _child.RenderTransformOrigin = new Point(0.0, 0.0);
        }

        private void Child_SizeChanged(object sender, SizeChangedEventArgs e) => Reset();

        private void ZoomBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isChildInitialized)
                return;

            if (AnchorOrigin)
            {
                var tt = GetTranslateTransform(_child);
                var st = GetScaleTransform(_child);

                var delta = tt.X - GetMaxX(e.PreviousSize.Width);
                var newTranslation = GetMaxX(ActualWidth);
                tt.SetX(newTranslation + delta, this);

                delta = e.PreviousSize.Height != 0 ? tt.Y - GetMaxY(e.PreviousSize.Height, st.ScaleY) : 0;
                newTranslation = GetMaxY(ActualHeight, st.ScaleY);
                tt.SetY(newTranslation + delta, this);
            }
        }

        private void CheckTranslationLimits(double candidateX, double candidateY, double scale, out double x, out double y)
        {
            x = candidateX;
            y = candidateY;

            if (AnchorOrigin)
            {
                var maxX = GetMaxX(ActualWidth);

                if (candidateX > maxX)
                    x = maxX;

                var maxY = GetMaxY(ActualHeight, scale);

                if (candidateY < maxY)
                    y = maxY;
            }
        }

        private double GetMaxX(double actualWidth)
        {
            if (actualWidth < _childWidth)
                return 0;

            return -(actualWidth - _childWidth) / 2;
        }

        private double GetMaxY(double actualHeight, double scale)
        {
            if (actualHeight < _childHeight)
                return -(_childHeight * scale - actualHeight);

            return actualHeight / 2 - (_childHeight * scale - _childHeight / 2);
        }

        private Rect FixRectanglePoints(Rect rectangle)
        {
            var y = rectangle.Y;
            var x = rectangle.X;

            if (_child.LayoutTransform is ScaleTransform scaleTransform)
            {
                if (scaleTransform.ScaleY == -1)
                    y = _child.Height - y - rectangle.Height;
                if (scaleTransform.ScaleX == -1)
                    x = _child.Width - x - rectangle.Width;
            }

            return new Rect(x, y, rectangle.Width, rectangle.Height);
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }
    }
}