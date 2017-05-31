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
    using System.Windows.Shapes;

    public partial class DialogOnTutorialExitingWindow : Window
    {
        public DialogOnTutorialExitingWindow()
        {
            InitializeComponent();

            this.Title = Properties.Resources.ZkooTutorialModel_TutorialWindowTitle;
            TutorialExitingWindowTextBlock.Text = "You can start Tutorial from Settings > Basic." + Environment.NewLine + Environment.NewLine;
            TutorialExitingWindowTextBlock.Text += "Settings menu can be opened by tapping on Camera View or right-clicking " + ApplicationCommonSettings.HostApplicationName + " icon on the notification area.";

            var msg1 = Properties.Resources.ZkooTutorialModel_YouCanStartTutorialFromSettingsBasic + Environment.NewLine;
            msg1 += Environment.NewLine;
            msg1 += Properties.Resources.ZkooTutorialModel_SettingsMenuCanBeOpenedBy + Environment.NewLine;
            TutorialExitingWindowTextBlock.Text = msg1;

            TutorialExitingWindowCheckBox.Content = Properties.Resources.ZkooTutorialModel_StartTutorialAgainWhenZkooApplicationIsLaunched;

            CloseButton.Click += delegate { this.Close(); };
        }
    }
}
