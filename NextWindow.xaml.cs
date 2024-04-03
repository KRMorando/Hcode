﻿using System;
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
            string sourceCode = CodeTextBox.Text;


            sourceCode = "#include <stdio.h>\n" + sourceCode;


            string tempFile = Path.GetTempFileName();
            string cTempFile = Path.ChangeExtension(tempFile, ".c");
            File.WriteAllText(cTempFile, sourceCode);


            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @"C:\MinGW\bin\gcc.exe",
                Arguments = $"-mconsole {cTempFile} -o {tempFile}.exe",
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


                    if (process.ExitCode == 0)
                    {
                        File.WriteAllText("Result.txt", output.ToString());
                        Process.Start($"{tempFile}.exe");
                    }
                }
            }


            File.Delete(tempFile);
            File.Delete($"{tempFile}.exe");
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