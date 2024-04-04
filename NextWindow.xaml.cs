using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
//코드 테스트


namespace Hcode
{
    public partial class NextWindow : Window
    {
        public NextWindow()
        {
            InitializeComponent();
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            // Hcode 폴더 경로 불러오기
            string folderPath = "C:/Hcode";
            string testPath = folderPath + "/test";

            string FileName = TitleBox.Text;
            string sourceCode = CodeTextBox.Text;

            DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);
            string cFile = Path.ChangeExtension(testPath + "/" + FileName, ".c");
            string exeFile = testPath + "/" + FileName + ".exe";

            // 폴더 유/무 체크
            if (!directoryInfoPath.Exists)
                directoryInfoPath.Create();

            File.WriteAllText(cFile, sourceCode);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\LLVM\bin\clang.exe",
                Arguments = $"-target x86_64-pc-windows-msvc {cFile} -o \"{exeFile}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };


            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {

                    StringBuilder output = new StringBuilder();
                    StringBuilder error = new StringBuilder();
                    process.OutputDataReceived += (s, args) => output.AppendLine(args.Data);
                    process.ErrorDataReceived += (s, args) => error.AppendLine(args.Data);
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();


                    OutputTextBox.Text = output.ToString();
                    ErrorTextBox.Text = error.ToString();

                    //if (process.ExitCode == 0)
                    //{
                    //    File.WriteAllText("Result.txt", output.ToString());
                    //    Process.Start($"{tempFile}.exe");
                    //}
                }
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                CodeTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
    }
}
