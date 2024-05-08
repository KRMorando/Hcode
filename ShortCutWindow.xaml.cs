using System.IO;
using System.Windows;

namespace Hcode
{
    public partial class ShortCutWindow : Window
    {
        //인자로 받아온 MainWindow와 선택된 언어
        private Window mainWindow;
        private string selectLanguage;

        // Hcode 폴더 경로 불러오는 곳
        private static string folderPath = "C:/Hcode";
        private static string testPath = folderPath + "/test";
        private static DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);

        public ShortCutWindow(Window mainWindow, string Language)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.selectLanguage = Language;
            SelectedLabel.Content = Language;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            //Window를 연다. !! 지금은 CWindow를 객체로 생성하는데 Java와 Python을 구현할땐 바꿔야한다 !!
            Window window = new CWindow(selectLanguage, FileName_TextBox.Text);
            mainWindow.Close();
            this.Close();
            window.Show();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FileName_TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!okButton.IsEnabled && FileName_TextBox.Text.Length > 0)
                okButton.IsEnabled = true;
            else if (okButton.IsEnabled && FileName_TextBox.Text.Length <= 0)
                okButton.IsEnabled = false;
        }
    }
}