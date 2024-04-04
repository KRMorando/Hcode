using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Hcode {
    public partial class CWindow : Window {
        public CWindow() {
            InitializeComponent();
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e) {
            // Hcode 폴더 경로 불러오는곳
            string folderPath = "C:/Hcode";
            string testPath = folderPath + "/test";
            DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);

            string FileName = TitleBox.Text;
            string sourceCode = "#include <stdio.h>\n" + CodeTextBox.Text;

            string cFile = Path.ChangeExtension(testPath + "/" + FileName, ".c");
            string exeFile = testPath + "/" + FileName + ".exe";

            File.WriteAllText(cFile, sourceCode);

            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = @"C:\Program Files\LLVM\bin\clang.exe",
                Arguments = $"-target x86_64-pc-windows-msvc {cFile} -o \"{exeFile}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi)) {
                if (process != null) {
                    StringBuilder error = new StringBuilder();
                    process.ErrorDataReceived += (s, args) => error.AppendLine(args.Data);
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode == 0) {
                        // 프로그램 실행 후 결과를 output 변수에 저장
                        ProcessStartInfo psi2 = new ProcessStartInfo {
                            FileName = exeFile,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using (Process exeProcess = Process.Start(psi2)) {
                            if (exeProcess != null) {
                                string output = exeProcess.StandardOutput.ReadToEnd();
                                OutputTextBox.Text = output; // 출력결과 나오는데
                            } else {
                                OutputTextBox.Text = "Failed to start the program!";
                            }
                        }
                    } else {
                        OutputTextBox.Text = "Compilation failed!";
                    }

                    ErrorTextBox.Text = error.ToString(); // 여긴 에러임
                }
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                CodeTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
    }
}