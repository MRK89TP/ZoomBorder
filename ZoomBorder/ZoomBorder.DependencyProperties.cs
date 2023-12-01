using System;
using System.Windows;

namespace ZoomBorder
{
    public partial class ZoomBorder
    {
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(nameof(IsZoomEnabled), typeof(bool), typeof(ZoomBorder), new FrameworkPropertyMetadata(true, OnIsZoomEnabledChanged));
        public static readonly DependencyProperty MinScaleProperty = DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(ZoomBorder), new FrameworkPropertyMetadata(0.1, OnMinScaleChanged));
        public static readonly DependencyProperty MaxScaleProperty = DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(ZoomBorder), new FrameworkPropertyMetadata(7d, OnMaxScaleChanged));
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(nameof(ZoomFactor), typeof(double), typeof(ZoomBorder), new FrameworkPropertyMetadata(0.1, OnZoomFactorChanged));
        public static readonly DependencyProperty AnchorOriginProperty = DependencyProperty.Register(nameof(AnchorOrigin), typeof(bool), typeof(ZoomBorder), new FrameworkPropertyMetadata(false, OnAnchorOriginChanged));
        public static readonly DependencyProperty TranslationXProperty = DependencyProperty.Register(nameof(TranslationX), typeof(double), typeof(ZoomBorder), new FrameworkPropertyMetadata(0d, OnTranslationXChanged));
        public static readonly DependencyProperty TranslationYProperty = DependencyProperty.Register(nameof(TranslationY), typeof(double), typeof(ZoomBorder), new FrameworkPropertyMetadata(0d, OnTranslationYChanged));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(ZoomBorder), new FrameworkPropertyMetadata(1d, OnScaleChanged));

        public bool IsZoomEnabled
        {
            get => (bool)GetValue(IsZoomEnabledProperty);
            set => SetValue(IsZoomEnabledProperty, value);
        }

        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }

        public double MaxScale
        {
            get => (double)GetValue(MaxScaleProperty);
            set => SetValue(MaxScaleProperty, value);
        }

        public double ZoomFactor
        {
            get => (double)GetValue(ZoomFactorProperty);
            set => SetValue(ZoomFactorProperty, value);
        }

        public bool AnchorOrigin
        {
            get => (bool)GetValue(AnchorOriginProperty);
            set => SetValue(AnchorOriginProperty, value);
        }

        public double TranslationX
        {
            get => (double)GetValue(TranslationXProperty);
            set => SetValue(TranslationXProperty, value);
        }

        public double TranslationY
        {
            get => (double)GetValue(TranslationYProperty);
            set => SetValue(TranslationYProperty, value);
        }

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        private static void OnMinScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not double value || d is not ZoomBorder instance)
                return;

            if (value <= 0)
                throw new Exception($"{nameof(MinScale)} non può essere negativo");
        }

        private static void OnMaxScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not double value || d is not ZoomBorder instance)
                return;

            if (value <= 0)
                throw new Exception($"{nameof(MaxScale)} non può essere negativo");
        }

        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not double value || d is not ZoomBorder instance)
                return;

            if (value <= 0)
                throw new Exception($"{nameof(ZoomFactor)} non può essere negativo");
        }
        private static void OnTranslationXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not double value || d is not ZoomBorder instance)
                return;
        }
        private static void OnTranslationYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not double value || d is not ZoomBorder instance)
                return;
        }


        private static void OnIsZoomEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not bool value || d is not ZoomBorder instance)
                return;

            if (value)
            {
                instance.MouseWheel += instance.OnMouseWheel;
                instance.MouseLeftButtonDown += instance.OnMouseLeftButtonDown;
                instance.MouseLeftButtonUp += instance.OnMouseLeftButtonUp;
                instance.MouseMove += instance.OnMouseMove;
            }
            else
            {
                instance.MouseWheel -= instance.OnMouseWheel;
                instance.MouseLeftButtonDown -= instance.OnMouseLeftButtonDown;
                instance.MouseLeftButtonUp -= instance.OnMouseLeftButtonUp;
                instance.MouseMove -= instance.OnMouseMove;
            }
        }

        private static void OnAnchorOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not bool value || d is not ZoomBorder instance)
                return;
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not bool value || d is not ZoomBorder instance)
                return;
        }
    }
}
