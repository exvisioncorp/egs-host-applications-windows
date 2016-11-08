﻿namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// original code: http://astel-labs.net/blog/diary/2012/05/06-1.html
    /// </summary>
    [ContentProperty("Text")]
    class OutlinedText : FrameworkElement
    {
        private FormattedText FormattedText;
        private Geometry TextGeometry;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextInvalidated));
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));
        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));
        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(OutlinedText), new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnFormattedTextUpdated));
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(OutlinedText), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(OutlinedText), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(OutlinedText), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));
        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated)); public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));
        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));
        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(OutlinedText), new FrameworkPropertyMetadata(OnFormattedTextUpdated));

        public OutlinedText()
        {
            this.TextDecorations = new TextDecorationCollection();
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)this.GetValue(TextDecorationsProperty); }
            set { this.SetValue(TextDecorationsProperty, value); }
        }
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            this.EnsureGeometry();

            drawingContext.DrawGeometry(this.Fill, new Pen(this.Stroke, this.StrokeThickness), this.TextGeometry);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.EnsureFormattedText();

            this.FormattedText.MaxTextWidth = Math.Min(3579139, availableSize.Width);
            this.FormattedText.MaxTextHeight = availableSize.Height;

            return new Size(this.FormattedText.Width, this.FormattedText.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.EnsureFormattedText();

            this.FormattedText.MaxTextWidth = finalSize.Width;
            this.FormattedText.MaxTextHeight = finalSize.Height;

            this.TextGeometry = null;

            return finalSize;
        }

        private static void OnFormattedTextInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlinedText)dependencyObject;
            outlinedTextBlock.FormattedText = null;
            outlinedTextBlock.TextGeometry = null;

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private static void OnFormattedTextUpdated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var outlinedTextBlock = (OutlinedText)dependencyObject;
            outlinedTextBlock.UpdateFormattedText();
            outlinedTextBlock.TextGeometry = null;

            outlinedTextBlock.InvalidateMeasure();
            outlinedTextBlock.InvalidateVisual();
        }

        private void EnsureFormattedText()
        {
            if (this.FormattedText != null || this.Text == null)
                return;

            this.FormattedText = new FormattedText(
                this.Text,
                CultureInfo.CurrentUICulture,
                this.FlowDirection,
                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, FontStretches.Normal),
                this.FontSize,
                Brushes.Black);

            this.UpdateFormattedText();
        }

        private void UpdateFormattedText()
        {
            if (this.FormattedText == null)
                return;

            this.FormattedText.MaxLineCount = this.TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
            this.FormattedText.TextAlignment = this.TextAlignment;
            this.FormattedText.Trimming = this.TextTrimming;

            this.FormattedText.SetFontSize(this.FontSize);
            this.FormattedText.SetFontStyle(this.FontStyle);
            this.FormattedText.SetFontWeight(this.FontWeight);
            this.FormattedText.SetFontFamily(this.FontFamily);
            this.FormattedText.SetFontStretch(this.FontStretch);
            this.FormattedText.SetTextDecorations(this.TextDecorations);
        }

        private void EnsureGeometry()
        {
            if (this.TextGeometry != null)
                return;

            this.EnsureFormattedText();
            this.TextGeometry = this.FormattedText.BuildGeometry(new Point());
        }
    }
}