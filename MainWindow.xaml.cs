using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// 9차 커밋

namespace Hcode {

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Hcode 폴더 경로 불러오기
            string folderPath = "C:/Hcode";
            string testPath = folderPath + "/test";

            DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);

            // 폴더 유/무 체크
            if (!directoryInfoPath.Exists)
                directoryInfoPath.Create();

            InitializeComponent();
        }

        private void OnClickCButton(object sender, RoutedEventArgs e)
        {
            Window newWindow = new CWindow();
            newWindow.Show();
            this.Close();
        }
        private void OnClickJButton(object sender, RoutedEventArgs e) {
            Window newWindow = new JavaWindow();
            newWindow.Show();
            this.Close();
        }
        private void OnClickPButton(object sender, RoutedEventArgs e) {
            Window newWindow = new PythonWindow();
            newWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
