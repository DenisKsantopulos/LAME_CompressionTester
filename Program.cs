using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LAME_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string lameVersionsDir = "lame_versions";
            string wavFilesDir = "wav_files";
            string reportFile = "report.txt";

            string[] lameVersions = new string[] { "3.100", "3.99" };

            string[] wavFiles = new string[] { "testcase.wav", "testcase2.wav" };

            string[] encodingSettings = new string[] { "-b 128", "-b 192", "-b 256", "-b 320", "-V 2" };

            using (StreamWriter writer = File.CreateText(reportFile))
            {
                writer.WriteLine("LAME Regression Test Report");
                writer.WriteLine("----------------------------");
                writer.WriteLine($"Test Environment: {lameVersionsDir} and {wavFilesDir}");
                writer.WriteLine();
            }

            foreach (string lameVersion in lameVersions)
            {
                foreach (string wavFile in wavFiles)
                {
                    foreach (string encodingSetting in encodingSettings)
                    {
                        string lameExePath = Path.Combine(lameVersionsDir, lameVersion, "lame.exe");
                        string wavFilePath = Path.Combine(wavFilesDir, wavFile);
                        string mp3FilePath = Path.Combine("output", $"{wavFile}_{lameVersion}_{encodingSetting}.mp3");
                        string arguments = $"{wavFilePath} {encodingSetting} {mp3FilePath}";

                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = lameExePath;
                            process.StartInfo.Arguments = arguments;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;

                            process.OutputDataReceived += (sender, data) => Console.WriteLine(data.Data);
                            process.ErrorDataReceived += (sender, data) => Console.WriteLine(data.Data);

                            process.Start();
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            Stopwatch stopwatch = Stopwatch.StartNew();
                            process.WaitForExit();
                            stopwatch.Stop();

                            using (StreamWriter writer = File.AppendText(reportFile))
                            {
                                writer.WriteLine($"Test Result: {wavFile} with {lameVersion} and {encodingSetting}");
                                writer.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
                                writer.WriteLine($"MP3 File: {mp3FilePath}");
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }

            foreach (string wavFile in wavFiles)
            {
                foreach (string encodingSetting in encodingSettings)
                {
                    string mp3File1 = Path.Combine("output", $"{wavFile}_3.100_{encodingSetting}.mp3");
                    string mp3File2 = Path.Combine("output", $"{wavFile}_3.99_{encodingSetting}.mp3");

                    if (File.Exists(mp3File1) && File.Exists(mp3File2))
                    {
                        byte[] mp3File1Bytes = File.ReadAllBytes(mp3File1);
                        byte[] mp3File2Bytes = File.ReadAllBytes(mp3File2);

                        if (!mp3File1Bytes.SequenceEqual(mp3File2Bytes))
                        {
                            using (StreamWriter writer = File.AppendText(reportFile))
                            {
                                writer.WriteLine($"Binary Difference Detected: {wavFile} with {encodingSetting}");
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
        }
    }
}