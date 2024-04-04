﻿using System.IO;
using System.Windows;
//11차 커밋

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
