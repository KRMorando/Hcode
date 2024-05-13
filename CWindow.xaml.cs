using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Media;

namespace Hcode
{
    public partial class CWindow : Window
    {
        // Hcode 폴더 경로 불러오는곳
        private static string folderPath = "C:/Hcode";
        private static string testPath = folderPath + "/test";
        private static DirectoryInfo directoryInfoPath = new DirectoryInfo(testPath);

        private List<string> fileNames = new List<string>();
        private List<TextBox> textBoxes = new List<TextBox>();
        private List<Button> fileButtons = new List<Button>();

        private string nowFile;
        private TextBox nowTextBox;
        private Button nowButton;

        private bool isFirst = true;

        public CWindow(string selectLanguage, string fileName)
        {
            this.InitializeComponent();

            fileNames.Add(fileName);
            textBoxes.Add(CodeTextBox);
            fileButtons.Add(fileButton0);

            nowFile = fileName;
            nowTextBox = CodeTextBox;
            nowButton = fileButton0;

            fileButton0.Content = fileName;

            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.MinHeight = 900;
            this.MaxHeight = SystemParameters.WorkArea.Height;

            this.MouseLeftButtonDown += new MouseButtonEventHandler(MainWindow_MouseLeftButtonDown);
            // CodeTextBox의 TextChanged 이벤트에 CodeTextBox_TextChanged 메서드를 연결
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            // 사용자가 입력한 파일명을 가져옵니다.
            string fileName = nowFile;

            // 파일명이 null이거나 빈 문자열인지 확인합니다.
            if (string.IsNullOrEmpty(fileName))
            {
                OutputTextBox.Text = "에러: 파일명을 입력해주세요.";
                return;
            }

            // 파일 내용을 가져옵니다.
            string sourceCode = nowTextBox.Text;

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
                String fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                bool isChecked = false;
                for (int i = 0; i < fileNames.Count; i++)
                {
                    if (fileNames[i].Equals(fileName))
                    {
                        DisabledComponent();

                        nowFile = fileNames[i];
                        nowTextBox = textBoxes[i];
                        nowButton = fileButtons[i];

                        EnabledComponent();
                        isChecked = true;
                        break;
                    }
                }
                if (!isChecked)
                {
                    Button newButton = new Button();
                    newButton.Content = fileName;
                    newButton.Margin = new Thickness(0, 0, 2, 0);
                    newButton.Width = 100;
                    newButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E3E"));
                    newButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC7C5C5"));
                    newButton.Foreground = Brushes.White;
                    newButton.BorderThickness = new Thickness(0, 2, 0, 0);
                    newButton.Click += FileButton_Click;

                    TextBox newTextBox = new TextBox();
                    newTextBox.TextWrapping = TextWrapping.Wrap;
                    newTextBox.AcceptsReturn = true;
                    newTextBox.Background = null;
                    newTextBox.BorderBrush = null;
                    newTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    newTextBox.Padding = new Thickness(5);
                    newTextBox.Foreground = Brushes.White;
                    newTextBox.FontSize = 20;
                    newTextBox.TextChanged += CodeTextBox_TextChanged;
                    newTextBox.Text = File.ReadAllText(openFileDialog.FileName);
                    
                    buttonPanel.Children.Add(newButton);
                    TextBoxBorder.Child = newTextBox;

                    DisabledComponent();

                    nowFile = fileName;
                    nowTextBox = newTextBox;
                    nowButton = newButton;

                    fileNames.Add(nowFile);
                    textBoxes.Add(newTextBox);
                    fileButtons.Add(newButton);
                }
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
                int caretIndex2 = textBox.CaretIndex;
                string text = textBox.Text;

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
                ShadowBorder.Margin = new Thickness(5);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowState = WindowState.Maximized;
                SizeButton.Content = "2";
                ShadowBorder.Margin = new Thickness(0);
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string fileName = clickedButton.Content.ToString();
            for (int i = 0; i < fileNames.Count; i++)
            {
                if (fileNames[i].Equals(fileName))
                {
                    DisabledComponent();

                    nowFile = fileNames[i];
                    nowTextBox = textBoxes[i];
                    nowButton = fileButtons[i];

                    EnabledComponent();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 파일 이름.c 경로
            string cFile = testPath + "/" + nowFile + ".c";
            string sourceCode = nowTextBox.Text;


            // .c 파일에 소스 코드 작성
            File.WriteAllText(cFile, sourceCode);
        }


        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // 파일명이 null이거나 빈 문자열인지 확인합니다.
            if (string.IsNullOrEmpty(nowFile))
            {
                MessageBox.Show("파일명을 입력하세요.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 파일 내용을 가져옵니다.
            string code = nowTextBox.Text;

            try
            {
                // 파일을 저장할 경로를 지정합니다.
                string filePath = Path.Combine(testPath, $"{nowFile}.c");

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

        private void OnClickCButton(object sender, RoutedEventArgs e)
        {
            Window newWindow = new ShortCutWindow(this, "C");
            newWindow.Show();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Window HelpWindow = new HelpWindow();
            HelpWindow.Show();
        }


       

        private void DisabledComponent()
        {
            nowTextBox.Visibility = Visibility.Collapsed;
            nowButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3E3E3E"));
        }

        private void EnabledComponent()
        {
            nowTextBox.Visibility = Visibility.Visible;
            nowButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC7C5C5"));

            TextBoxBorder.Child = nowTextBox;
        }


        // 클랭-포맷을 활용한 들여쓰기
        private void IndentButton_Click(object sender, RoutedEventArgs e)
        {
            string code = nowTextBox.Text;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\LLVM\bin\clang-format.exe",
                Arguments = "-style=LLVM",
                /*
                  Google 스타일: "-style=google"
                  LLVM 스타일: "-style=llvm"
                  GNU 스타일: "-style=gnu"
                  Chromium 스타일: "-style=chromium"
                  file: "-style=file"
                  custom: "-style={key: value, key: value, ...}"
                  fallback: "-style=fallback"
                  none: "-style=none"
                  */
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
                    using (StreamWriter sw = process.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            // 코드를 입력 스트림으로 전달
                            sw.WriteLine(code);
                            sw.Flush();
                        }
                    }

                    // clang-format에서 처리된 코드를 읽어옵니다.
                    string formattedCode = process.StandardOutput.ReadToEnd();

                    // clang-format 프로세스 종료 대기
                    process.WaitForExit();

                    // 들여쓴 코드를 텍스트 상자에 설정합니다.
                    nowTextBox.Text = formattedCode;
                }
            }
        }

        private void CodeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                // 현재 커서 위치를 가져옵니다.
                int caretIndex = CodeTextBox.CaretIndex;

                // 현재 커서 위치에 탭 문자를 삽입합니다.
                CodeTextBox.Text = CodeTextBox.Text.Insert(caretIndex, "\t");

                // 삽입 후 커서 위치를 조정합니다.
                CodeTextBox.CaretIndex = caretIndex + 1;

                // Tab 키 입력 이벤트를 처리하고 더 이상 전파되지 않도록 합니다.
                e.Handled = true;
            }
        }





        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}