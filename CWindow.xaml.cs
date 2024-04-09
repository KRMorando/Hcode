using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;

namespace Hcode
{
    public partial class CWindow : Window
    {
        public CWindow()
        {
            InitializeComponent();
            
            // CodeTextBox의 TextChanged 이벤트에 CodeTextBox_TextChanged 메서드를 연결
            CodeTextBox.TextChanged += CodeTextBox_TextChanged;
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            // Hcode 폴더 경로 불러오는 곳
            string folderPath = "C:/Hcode";
            string testPath = folderPath + "/test";
            DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);

            // 파일 이름을 TitleBox에서 가져옴
            string FileName = TitleBox.Text;

            // !!!!! stdio.h 나중에 삭제 유무 결정할 것 !!!!!
            string sourceCode = "#include <stdio.h>\n" + CodeTextBox.Text;

            // 소스 코드를 .c 파일로 저장
            string cFile = Path.ChangeExtension(testPath + "/" + FileName, ".c");

            // 컴파일된 프로그램이 저장될 .exe 파일 경로
            string exeFile = testPath + "/" + FileName + ".exe";

            // .c 파일에 소스 코드 작성
            File.WriteAllText(cFile, sourceCode);

            // 컴파일러 실행
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
                    StringBuilder error = new StringBuilder();
                    process.ErrorDataReceived += (s, args) => error.AppendLine(args.Data);
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        // 컴파일이 성공 → 프로그램 실행
                        ProcessStartInfo psi2 = new ProcessStartInfo
                        {
                            FileName = exeFile,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using (Process exeProcess = Process.Start(psi2))
                        {
                            if (exeProcess != null)
                            {
                                // 프로그램 실행 결과를 OutputTextBox에 출력
                                string output = exeProcess.StandardOutput.ReadToEnd();
                                OutputTextBox.Text = output;
                            }
                            else
                            {
                                OutputTextBox.Text = "프로그램 실행 실패";
                            }
                        }
                    }
                    else
                    {
                        // 컴파일 에러 발생 시 OutputTextBox에 에러 메시지 출력
                        OutputTextBox.Text = "컴파일 에러";
                    }

                    // 에러 메시지를 ErrorTextBox에 출력
                    ErrorTextBox.Text = error.ToString();
                }
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            // 파일을 열기 위한 OpenFileDialog 호출
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // 선택된 파일의 내용을 CodeTextBox에 표시
                CodeTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            foreach (TextChange change in e.Changes)
            {
                int offset = change.Offset;
                int addedLength = change.AddedLength;
                string addedText = textBox.Text.Substring(offset, addedLength);

                // 새로운 괄호가 추가될 때 이전에 있는 괄호 앞에 커서가 위치하도록 함
                if (addedText == "{" || addedText == "(" || addedText == "[" || addedText == "'" || addedText == "\"")
                {
                    int caretIndex = textBox.CaretIndex;
                    string closingBracket = addedText == "{" ? "}" : (addedText == "(" ? ")" : (addedText == "[" ? "]" : (addedText == "'" ? "'" : "\"")));
                    textBox.Text = textBox.Text.Insert(offset + addedLength, closingBracket);
                    textBox.CaretIndex = caretIndex;
                }
            }
        }
    }
}