### Консольное приложение (.NET Framework) на языке C#,  которое проводит тест двух версий LAME и формирует отчет о тестировании.
Отчет включает следующие сведения: скорость обработки (ms) и наличие разницы в кодировании между разными версиями LAME (без подробностей - просто факт бинарного различия файлов).

- Исходные wav файлы хранятся в bin/Debug/wav_files
- Файлы разных версий LAME хранятся в bin/Debug/lame_versions/3.100 и bin/Debug/lame_versions/3.99
- Полученные в ходе тестирования файлы в формате mp3 записываются в bin/Debug/output
- отчет находится в bin/Debug/отчет.txt
- bat-файл для запуска программы: bin/Debug/file.bat

## EN
### Console application (.NET Framework) in C#, which tests two versions of LAME and generates a test report.
The report includes the following information: processing speed (ms) and the presence of differences in encoding between different versions of LAME (without details - just the fact of binary file differences).

- The original wav files are stored in bin/Debug/wav_files
- The files of different versions of LAME are stored in bin/Debug/lame_versions/3.100 and bin/Debug/lame_versions/3.99
- The files obtained during testing in mp3 format are written to bin/Debug/output
- The report is located in bin/Debug/отчет.txt
- A bat file for running the program: bin/Debug/file.bat
