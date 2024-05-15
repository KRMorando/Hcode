using System;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace Hcode
{
    public partial class ShortCutWindow : Window
    {
        //인자로 받아온 윈도우와 선택된 언어
        private Window window;
        private string selectLanguage;

        //Hcode 폴더 경로 불러오기
        static string folderPath = "C:/Hcode";
        static string projectPath = folderPath + "/Project";

        public ShortCutWindow(Window window, string Language)
        {
            InitializeComponent();

            this.window = window;
            this.selectLanguage = Language;
            SelectedLabel.Content = Language;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            //프로젝트명 가져오기
            string fileName = FileName_TextBox.Text;
            DirectoryInfo newProjectInfoPath = new DirectoryInfo(projectPath + "/" + fileName);

            //해당 프로젝트 폴더 유무 체크 후 없을시 생성
            if (!newProjectInfoPath.Exists)
                newProjectInfoPath.Create();

            //Window를 연다. !! 지금은 CWindow를 객체로 생성하는데 Java와 Python을 구현할땐 바꿔야한다 !!
            Window newWindow = new CWindow(selectLanguage, FileName_TextBox.Text);
            newWindow.Show();
            window.Close();
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FileName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!okButton.IsEnabled && FileName_TextBox.Text.Length > 0)
                okButton.IsEnabled = true;
            else if (okButton.IsEnabled && FileName_TextBox.Text.Length <= 0)
                okButton.IsEnabled = false;
        }
    }
}