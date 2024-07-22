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
            // Директории для хранения версий LAME и тестовых файлов
            string lameVersionsDir = "lame_versions";
            string wavFilesDir = "wav_files";
            string reportFile = "отчет.txt";

            // Массив версий LAME для тестирования
            string[] lameVersions = new string[] { "3.100", "3.99" };

            // Массив тестовых файлов
            string[] wavFiles = new string[] { "test_file1.wav", "test_file2.wav" };

            // Создание папки output, если не создана
            string outputDir = "output";
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Установка настроек для тестирования
            string[] encodingSettings = new string[]
                {
                    "-b 96",  // Low bitrate
                    "-b 192", // Medium bitrate
                    "-b 320", // High bitrate
                    "-V 0 --vbr-new", // Variable bitrate with highest quality
                    "-V 2 --abr 128" // Average bitrate with medium quality
                };

            // Создание отчета о тестировании
            using (StreamWriter writer = File.CreateText(reportFile))
            {
                writer.WriteLine("Отчет о тестировании разных версий LAME");
                writer.WriteLine("----------------------------");
                writer.WriteLine($"Версии LAME: {lameVersions[0]} и {lameVersions[1]}");
                writer.WriteLine($"Тестовые файлы: {wavFiles[0]} и {wavFiles[1]}");
                writer.WriteLine();
            }

            // Цикл по всем версиям LAME
            foreach (string lameVersion in lameVersions)
            {
                // Цикл по всем тестовым файлам
                foreach (string wavFile in wavFiles)
                {
                    // Цикл по всем настройкам кодирования
                    foreach (string encodingSetting in encodingSettings)
                    {
                        // Путь к исполняемому файлу LAME
                        string lameExePath = Path.Combine(lameVersionsDir, lameVersion, "lame.exe");
                        // Путь к тестовому файлу
                        string wavFilePath = Path.Combine(wavFilesDir, wavFile);
                        // Аргументы для запуска LAME
                        string arguments = $"{wavFilePath} {encodingSetting}";
                        // Имя файла вывода
                        string outputFile = Path.Combine(outputDir, $"{wavFile}_{lameVersion}_{encodingSetting.Replace(" ", "_")}.mp3");

                        // Запуск процесса LAME
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = lameExePath;
                            process.StartInfo.Arguments = $"{wavFilePath} {encodingSetting} {outputFile}";

                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;

                            process.OutputDataReceived += (sender, data) => Console.WriteLine(data.Data);
                            process.ErrorDataReceived += (sender, data) => Console.WriteLine(data.Data);

                            process.Start();
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            // Таймер для измерения времени выполнения
                            Stopwatch stopwatch = Stopwatch.StartNew();
                            process.WaitForExit();
                            stopwatch.Stop();

                            // Добавление результатов тестирования в отчет
                            using (StreamWriter writer = File.AppendText(reportFile))
                            {
                                writer.WriteLine($"Результат испытаний: {wavFile} версия LAME {lameVersion} и {encodingSetting}");
                                writer.WriteLine($"Время исполнения(ms): {stopwatch.ElapsedMilliseconds} ms");
                                writer.WriteLine();

                                // Проверка на бинарные различия между файлами, закодированными разными версиями LAME
                                foreach (string otherEncodingSetting in encodingSettings.Where(es => es != encodingSetting))
                                {
                                    string otherOutputFile = Path.Combine(outputDir, $"{wavFile}_{lameVersion}_{otherEncodingSetting.Replace(" ", "_")}.mp3");
                                    if (File.Exists(otherOutputFile))
                                    {
                                        bool filesAreDifferent = !File.ReadAllBytes(outputFile).SequenceEqual(File.ReadAllBytes(otherOutputFile));
                                        writer.WriteLine($"Бинарное различие с {otherEncodingSetting}: {filesAreDifferent}");
                                    }
                                }

                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
        }
    }
}