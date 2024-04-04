using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Hcode {
    /// <summary>
    /// PythonWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PythonWindow : Window {
        public PythonWindow() {
            InitializeComponent();
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e) {
            // Hcode 폴더 경로 불러오기
            string folderPath = "C:/Hcode";
            string testPath = folderPath + "/test";
            DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                CodeTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
    }
}
