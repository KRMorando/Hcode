using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Input;

namespace Hcode
{
    public partial class CWindow : Window
    {
        // 파일 이름
        private string fileName;
        // Hcode 폴더 경로 불러오는곳
        private static string folderPath = "C:/Hcode";
        private static string testPath = folderPath + "/test";
        private static DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);

        public CWindow(string selectLanguage, string fileName)
        {
            this.fileName = fileName;

            this.InitializeComponent();

            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.MinHeight = 900;
            this.MaxHeight = SystemParameters.WorkArea.Height;

            this.MouseLeftButtonDown += new MouseButtonEventHandler(MainWindow_MouseLeftButtonDown);
            // CodeTextBox의 TextChanged 이벤트에 CodeTextBox_TextChanged 메서드를 연결
            CodeTextBox.TextChanged += CodeTextBox_TextChanged;
            FileName_Label.Content = fileName;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            ;
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            // 사용자가 입력한 파일명을 가져옵니다.
            string fileName = FileName_Label.Content.ToString();

            // 파일명이 null이거나 빈 문자열인지 확인합니다.
            if (string.IsNullOrEmpty(fileName))
            {
                OutputTextBox.Text = "에러: 파일명을 입력해주세요.";
                return;
            }

            // 파일 내용을 가져옵니다.
            string sourceCode = CodeTextBox.Text;

            // 파일 경로
            string cFile = Path.Combine(testPath, $"{fileName}.c");

            try
            {
                // 코드를 .c 파일로 저장합니다.
                File.WriteAllText(cFile, sourceCode);

                // Define input values
                string input = "5"; // Example input value

                // 컴파일된 프로그램이 저장될 .exe 파일 경로
                string exeFile = Path.Combine(testPath, $"{fileName}.exe");

                // 컴파일러 실행
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\LLVM\bin\clang.exe",
                    Arguments = $"-target x86_64-pc-windows-msvc {cFile} -o \"{exeFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        // Pass input to the process
                        using (StreamWriter sw = process.StandardInput)
                        {
                            if (sw.BaseStream.CanWrite)
                            {
                                // Write input to the standard input stream
                                sw.WriteLine(input);
                                sw.Flush();
                            }
                        }

                        // 컴파일러에서 오류 메시지 가져오기
                        StringBuilder error = new StringBuilder();
                        process.ErrorDataReceived += (s, args) => error.AppendLine(args.Data);
                        process.BeginErrorReadLine();

                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            // 컴파일이 성공 → 컴파일된 프로그램 실행
                            ProcessStartInfo psi2 = new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/k echo off & echo \"{testPath}\" && \"{exeFile}\"",
                                UseShellExecute = false,
                                CreateNoWindow = false
                            };

                            Process.Start(psi2);
                        }
                        else
                        {
                            // 컴파일 에러 발생 시 OutputTextBox에 에러 메시지 출력
                            OutputTextBox.Text = "컴파일 에러";
                            OutputTextBox.AppendText("\n" + error.ToString());
                        }
                    }
                }
            } catch (Exception ex)
            {
                // 파일 저장 중 오류가 발생한 경우 오류 메시지를 표시합니다.
                OutputTextBox.Text = $"파일 저장 중 오류가 발생했습니다: {ex.Message}";
            }
        }



        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            // 파일을 열기 위한 OpenFileDialog 호출
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // 선택된 파일의 파일명 및 내용 표시
                FileName_Label.Content = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
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

                // 새로 추가된 문자가 괄호나 따옴표인지 확인합니다.
                if (addedText == "{" || addedText == "(" || addedText == "[" || addedText == "'" || addedText == "\"")
                {
                    // 현재 커서 위치를 저장합니다.
                    int caretIndex = textBox.CaretIndex;
                    // 추가된 문자가 여는 괄호나 따옴표일 경우에는 그에 해당하는 닫는 괄호나 따옴표를 추가하고 커서를 원래 위치로 되돌립니다.
                    string closingBracket = addedText == "{" ? "}" : (addedText == "(" ? ")" : (addedText == "[" ? "]" : (addedText == "'" ? "'" : "\"")));
                    if (offset + addedLength < textBox.Text.Length && textBox.Text[offset + addedLength] == closingBracket[0])
                    {
                        // 이미 같은 종류의 괄호가 입력된 경우에는 추가하지 않습니다.
                        textBox.CaretIndex = caretIndex;
                    }
                    else
                    {
                        // 다음에 추가될 문자열을 계산합니다.
                        string nextChar = (offset + addedLength < textBox.Text.Length) ? textBox.Text[offset + addedLength].ToString() : "";
                        // 만약 다음 문자가 같은 종류의 괄호나 따옴표이면 추가하지 않습니다.
                        if (closingBracket == nextChar)
                        {
                            textBox.CaretIndex = caretIndex;
                        }
                        else
                        {
                            // 아니면 추가합니다.
                            textBox.Text = textBox.Text.Insert(offset + addedLength, closingBracket);
                            textBox.CaretIndex = caretIndex;
                        }
                    }
                }
            }
        }


        private void ToMiniButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.ResizeMode = ResizeMode.CanResize;
                this.WindowState = WindowState.Normal;
                SizeButton.Content = "1";
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowState = WindowState.Maximized;
                SizeButton.Content = "2";
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string sourceCode = CodeTextBox.Text;

            // 파일 이름.c 경로
            string cFile = testPath + "/" + FileName_Label.Content.ToString() + ".c";
            Console.WriteLine("파일 이름: " + FileName_Label.Content.ToString());

            // .c 파일에 소스 코드 작성
            File.WriteAllText(cFile, sourceCode);
        }


        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // 사용자가 입력한 파일명을 가져옵니다.
            string fileName = FileName_Label.Content.ToString();

            // 파일명이 null이거나 빈 문자열인지 확인합니다.
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("파일명을 입력하세요.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 파일 내용을 가져옵니다.
            string code = CodeTextBox.Text;

            try
            {
                // 파일을 저장할 경로를 지정합니다.
                string filePath = Path.Combine(testPath, $"{fileName}.c");

                // 파일을 저장합니다.
                File.WriteAllText(filePath, code);

                // 성공적으로 저장되었다는 메시지를 표시합니다.
                MessageBox.Show("파일이 성공적으로 저장되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ex)
            {
                // 파일 저장 중 오류가 발생한 경우 오류 메시지를 표시합니다.
                MessageBox.Show($"파일 저장 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnClickCButton(object sender, RoutedEventArgs e)
        {
            Window newWindow = new ShortCutWindow(this, "C");
            newWindow.Show();
        }
    }
}