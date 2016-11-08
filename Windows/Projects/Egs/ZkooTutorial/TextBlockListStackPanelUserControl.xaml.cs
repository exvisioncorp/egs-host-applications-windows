namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    public partial class TextBlockListStackPanelUserControl : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBlockListStackPanelUserControl), new PropertyMetadata("", new PropertyChangedCallback(TextChanged)));

        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
        private static void TextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var sender = (TextBlockListStackPanelUserControl)dependencyObject;
            var newText = (string)eventArgs.NewValue;
            sender.UpdateStackPanel(newText);
        }

        void UpdateStackPanel(string newText)
        {
            var strings = newText.Split('\n');
            thisStackPanel.Children.Clear();
            foreach (var item in strings)
            {
                // TODO: use reflection?
                var newTextBlock = new TextBlock();
                newTextBlock.FontFamily = _TemplateTextBlock.FontFamily;
                newTextBlock.FontSize = _TemplateTextBlock.FontSize;
                newTextBlock.FontStretch = _TemplateTextBlock.FontStretch;
                newTextBlock.Foreground = _TemplateTextBlock.Foreground;
                newTextBlock.TextWrapping = _TemplateTextBlock.TextWrapping;
                newTextBlock.Margin = _TemplateTextBlock.Margin;
                newTextBlock.LineHeight = _TemplateTextBlock.LineHeight;
                newTextBlock.LineStackingStrategy = _TemplateTextBlock.LineStackingStrategy;
                newTextBlock.Effect = (_TemplateTextBlock.Effect != null) ? _TemplateTextBlock.Effect.Clone() : null;
                newTextBlock.Text = item;
                thisStackPanel.Children.Add(newTextBlock);
            }
        }

        TextBlock _TemplateTextBlock;
        public TextBlock TemplateTextBlock
        {
            get { return _TemplateTextBlock; }
            set
            {
                _TemplateTextBlock = value;
                UpdateStackPanel(Text);
            }
        }

        public TextBlockListStackPanelUserControl()
        {
            InitializeComponent();
            _TemplateTextBlock = new TextBlock();
            Text = "";
        }
    }
}
