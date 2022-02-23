using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace OS_Practice_1
{
    class Train
    {
        public string FirstStation { get; set; }
        public DateTime Departure { get; set; }
        public string LastStation { get; set; }
        public DateTime Arrival { get; set; }
        public int TrainNumber { get; set; }
        public bool WeekdaysOnly { get; set; }
        public string[] Stations { get; set; }
        public Train(string firstStation, DateTime departure, string lastStation, DateTime arrival, int trainNumber, bool weekdaysOnly, string[] stations)
        {
            FirstStation = firstStation;
            Departure = departure;
            LastStation = lastStation;
            Arrival = arrival;
            TrainNumber = trainNumber;
            WeekdaysOnly = weekdaysOnly;
            Stations = stations;
        }

        public Train()
        {
            FirstStation = "Курский вокзал";
            Departure = DateTime.Parse("10:00");
            LastStation = "Петушки";
            Arrival = DateTime.Parse("12:00");
            TrainNumber = 6928;
            WeekdaysOnly = false;
            string[] stations = { "Карачарово", "Чухлинка" };
            Stations = stations;
        }

        public override string ToString()
        {
            string vs = "";
            if (Stations.Length > 0)
                vs = Stations[0];
            for (int i = 1; i < Stations.Length; i++)
            {
                vs += ", " + Stations[i];
            }
            return WeekdaysOnly ?
             "  Электропоезд №" + TrainNumber.ToString() +
                "\n   " + FirstStation + " -> " +
                LastStation + "\n   Отправление: " + Departure.ToString("t") +
                ", прибытие: " + Arrival.ToString("t") + " (будни)\n   Остановки:\n   " + vs
                :
            "  Электропоезд №" + TrainNumber.ToString() +
                "\n   " + FirstStation + " -> " +
                LastStation + "\n   Отправление: " + Departure.ToString("t") +
                ", прибытие: " + Arrival.ToString("t") + " (ежедневно)\n   Остановки:\n   " + vs;
        }
    }


    class Program
    {
        const int INFOTEXT_HEIGHT = 13;
        enum OS_objs
        {
            OS_folder = 1,
            OS_file = 2,
            OS_archive = 3,
            OS_archfolder
        }

        enum OS_serialize
        {
            OS_json, OS_xml
        }

        static int unit = 0;
        static readonly string[] units = new string[] { " Б ", " КБ", " МБ", " ГБ", " ТБ" };
        static readonly char[] forbidden = new char[] { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' };
        static int choosen_pos = 0;
        static int pages = 1;

        static DriveInfo[] OS_Drives;
        static string[] OS_Folders;
        static string[] OS_Files;


        static readonly Stack OS_Positions = new Stack();
        static readonly int[] MAX_LENGTH = new int[8];
        static string OS_Current_Path = "";

        /// <summary>
        /// This function add spaces to make your CUR-letter word MAX-letter
        /// </summary>
        /// <param name="max">Length to be reached</param>
        /// <param name="cur">Length of your word</param>
        static void Gaps(int max, int cur)
        {
            for (int i = 0; i < max - cur + 2; i++)
                Console.Write(" ");
        }

        /// <summary>
        /// This function makes your number between MIN and MAX
        /// </summary>
        /// <param name="number_to_normal">What number do you want to normalize</param>
        /// <param name="min">Minimum value of your number</param>
        /// <param name="max">Maximum value of your number</param>
        static void OS_Normali(ref int number_to_normal, int min, int max)
        {
            if (number_to_normal < min)
                number_to_normal = min;
            else if (number_to_normal > max)
                number_to_normal = max;
        }

        /// <summary>
        /// This function gets info about disks that are in your computer
        /// </summary>
        static void OS_DisksInfo()
        {
            OS_Drives = DriveInfo.GetDrives();
            MAX_LENGTH[0] = 3;
            MAX_LENGTH[1] = 3;
            MAX_LENGTH[2] = 6;
            MAX_LENGTH[3] = 11;
            MAX_LENGTH[4] = 10;
            MAX_LENGTH[5] = 9;
            foreach (DriveInfo item in OS_Drives)
            {
                if (MAX_LENGTH[0] < item.Name.Length) MAX_LENGTH[0] = item.Name.Length;
                if (item.IsReady) if (MAX_LENGTH[1] < item.VolumeLabel.ToString().Length)
                        MAX_LENGTH[1] = item.VolumeLabel.ToString().Length;
                if (item.IsReady) if (MAX_LENGTH[2] < ((long)(item.TotalSize / Math.Pow(1024, unit))).ToString().Length + 3)
                        MAX_LENGTH[2] = ((long)(item.TotalSize / Math.Pow(1024, unit))).ToString().Length + 3;
                if (item.IsReady) if (MAX_LENGTH[3] < ((long)(item.TotalFreeSpace / Math.Pow(1024, unit))).ToString().Length + 3)
                        MAX_LENGTH[3] = ((long)(item.TotalFreeSpace / Math.Pow(1024, unit))).ToString().Length + 3;
                if (item.IsReady) if (MAX_LENGTH[4] < item.DriveFormat.ToString().Length)
                        MAX_LENGTH[4] = item.DriveFormat.ToString().Length;
                if (item.IsReady) if (MAX_LENGTH[5] < item.DriveType.ToString().Length)
                        MAX_LENGTH[5] = item.DriveType.ToString().Length;
            }
        }

        /// <summary>
        /// This function says whether current item folder or file
        /// </summary>
        /// <returns>TRUE if current object is folder, FALSE if it is file</returns>
        static bool OS_isFolder()
        {
            return choosen_pos < OS_Folders.Length;
        }

        /// <summary>
        /// This function says whether your item folder or file
        /// </summary>
        /// <param name="pos">position of your file</param>
        /// <returns>TRUE if your object is folder, FALSE if it is file</returns>
        static bool OS_isFolder(int pos)
        {
            return pos < OS_Folders.Length;
        }

        /// <summary>
        /// Give you size of the file with certain init
        /// </summary>
        /// <param name="size">Size of your file in bytes</param>
        /// <returns>Size of file in current unit</returns>
        static string OS_GetFileSize(long size)
        {
            long size_to_unit = size / (long)Math.Pow(1024, unit);
            return size_to_unit.ToString() + units[unit];
        }

        static bool OS_SwitchUnits()
        {
            ConsoleKey pressed = Console.ReadKey().Key;
            switch (pressed)
            {
                case ConsoleKey.B:
                    unit = 0;
                    OS_DrawFoldersAndFiles(true);
                    break;
                case ConsoleKey.K:
                    unit = 1;
                    OS_DrawFoldersAndFiles(true);
                    break;
                case ConsoleKey.M:
                    unit = 2;
                    OS_DrawFoldersAndFiles(true);
                    break;
                case ConsoleKey.G:
                    unit = 3;
                    OS_DrawFoldersAndFiles(true);
                    break;
                case ConsoleKey.T:
                    unit = 4;
                    OS_DrawFoldersAndFiles(true);
                    break;
                default:
                    OS_DrawFoldersAndFiles(false);
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets info about folders and files into current path
        /// </summary>
        static void OS_FoldersAndFilesInfo()
        {
            OS_Folders = Directory.GetDirectories(OS_Current_Path);
            OS_Files = Directory.GetFiles(OS_Current_Path);
            pages = (int)Math.Ceiling(((float)(OS_Folders.Length + OS_Files.Length) / (float)(Console.WindowHeight - INFOTEXT_HEIGHT)));

        }

        /// <summary>
        /// This function print fullscreen navigation-allow table with your disks
        /// </summary>
        static void OS_DrawDisks()
        {
            Console.Clear();
            Console.WriteLine("\n\n Логические диски\n");
            Console.Write("  Том"); Gaps(MAX_LENGTH[0], 3);
            Console.Write("Имя"); Gaps(MAX_LENGTH[1], 3);
            Console.Write("Размер"); Gaps(MAX_LENGTH[2], 6);
            Console.Write("Своб. место"); Gaps(MAX_LENGTH[3], 11);
            Console.Write("Ф. система"); Gaps(MAX_LENGTH[4], 10);
            Console.Write("Тип диска"); Gaps(MAX_LENGTH[5], 11);
            Console.WriteLine();

            OS_Normali(ref choosen_pos, 0, OS_Drives.Length - 1);

            for (int i = 0; i < OS_Drives.Length; i++)
            {
                if (i == choosen_pos)
                    Console.Write("> ");
                else
                    Console.Write("  ");
                Console.Write(OS_Drives[i].Name);
                Gaps(MAX_LENGTH[0], OS_Drives[i].Name.Length);

                if (OS_Drives[i].IsReady)
                {
                    Console.Write(OS_Drives[i].VolumeLabel);
                    Gaps(MAX_LENGTH[1], OS_Drives[i].VolumeLabel.Length);
                    Console.Write(OS_GetFileSize(OS_Drives[i].TotalSize));
                    Gaps(MAX_LENGTH[2], OS_GetFileSize(OS_Drives[i].TotalSize).Length);
                    Console.Write(OS_GetFileSize(OS_Drives[i].TotalFreeSpace));
                    Gaps(MAX_LENGTH[3], OS_GetFileSize(OS_Drives[i].TotalFreeSpace).Length);
                    Console.Write(OS_Drives[i].DriveFormat);
                    Gaps(MAX_LENGTH[4], OS_Drives[i].DriveFormat.Length);
                    Console.Write(OS_Drives[i].DriveType);
                    Gaps(MAX_LENGTH[5], OS_Drives[i].DriveType.ToString().Length);
                }
                else
                {
                    Console.Write("устройство не готово");
                }
                Console.WriteLine();
            }
            Console.WriteLine("  ______________");
            Console.WriteLine("  Управление - стрелками");
            Console.WriteLine("  Ед. измерения: b, k, m, g, t");
            Console.WriteLine("  u - обновить, Enter - выбрать");
        }

        static void OS_CreateJson(List<Train> trains, FileStream file)
        {
            string studentJson = JsonSerializer.Serialize<List<Train>>(trains);
            byte[] array = System.Text.Encoding.Default.GetBytes(studentJson);
            file.Write(array);
            file.Close();
        }

        static void OS_CreateXml(List<Train> trains, FileStream file)
        {
            XDocument xDoc = new XDocument();
            XElement timetable = new XElement("Trains");
            int index = 0;
            foreach (Train item in trains)
            {
                XElement train = new XElement("Train_" + index.ToString());
                XElement firstStation = new XElement("firstStation", item.FirstStation);
                XElement departure = new XElement("departure", item.Departure);
                XElement lastStation = new XElement("lastStation", item.LastStation);
                XElement arrival = new XElement("arrival", item.Arrival);
                XElement weekdaysOnly = new XElement("weekdaysOnly", item.WeekdaysOnly);
                XElement trainNumber = new XElement("trainNumber", item.TrainNumber);
                XElement stations = new XElement("stations");
                int station_index = 0;
                foreach (string stat in item.Stations)
                {
                    XElement station = new XElement("station_" + station_index.ToString(), stat);
                    stations.Add(station);
                    station_index++;
                }

                index++;
                train.Add(firstStation);
                train.Add(departure);
                train.Add(lastStation);
                train.Add(arrival);
                train.Add(weekdaysOnly);
                train.Add(trainNumber);
                train.Add(stations);
                timetable.Add(train);
            }
            xDoc.Add(timetable);
            xDoc.Save(file);
            file.Close();
        }


        /// <summary>
        /// Draws a dialog screen with creating folder/file interface
        /// </summary>
        /// <param name="t">Type of object to be create</param>
        /// <returns>Name of folder/file or empty string if user refused</returns>
        static string OS_DrawNewObject(OS_objs t)
        {
            string name;
            bool repeating;
            do
            {
                repeating = false;
                Console.Clear();
                switch (t)
                {
                    case OS_objs.OS_folder:
                        Console.WriteLine("\n\n  Создать папку в директории");
                        Console.WriteLine("  " + OS_Current_Path);
                        Console.Write("\n  Введите имя папки > ");
                        break;
                    case OS_objs.OS_file:
                        Console.WriteLine("\n\n  Создать файл в директории");
                        Console.WriteLine("  " + OS_Current_Path);
                        Console.Write("\n  Введите имя файла > ");
                        break;
                    case OS_objs.OS_archive:
                        Console.WriteLine("\n\n  Создать архив");
                        Console.WriteLine("  " + OS_Current_Path);
                        Console.Write("\n  Введите имя архива > ");
                        break;
                    case OS_objs.OS_archfolder:
                        Console.WriteLine("\n\n  Распаковать архив в директорию");
                        Console.WriteLine("  " + OS_Current_Path);
                        Console.Write("\n  Введите имя папки (нажмите Enter для распаковки в исходную папку) > ");
                        break;
                    default:
                        break;
                }
                name = Console.ReadLine();
                if (t == OS_objs.OS_folder)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(OS_Current_Path + "\\" + name);
                    if (directoryInfo.Exists)
                    {
                        for (int i = 0; i < 22; i++)
                            Console.Write(" ");
                        for (int i = 0; i < name.Length; i++)
                            Console.Write("~");
                        Console.WriteLine();
                        Console.WriteLine("Ошибка: такая папка уже есть в этой директории");
                        Console.WriteLine("Повторить ввод? y - да/n - нет > ");
                        ConsoleKey key = Console.ReadKey().Key;
                        if (key == ConsoleKey.Y) repeating = true;
                        else return "";
                    }
                    else
                    {
                        foreach (char letter in forbidden)
                        {
                            if (name.Contains(letter))
                            {
                                int forbidden_index = name.IndexOf(letter);
                                for (int i = 0; i < 22 + forbidden_index; i++)
                                    Console.Write(" ");
                                Console.Write("~");
                                Console.WriteLine();
                                Console.WriteLine("Ошибка: недопустимый символ");
                                Console.WriteLine("Повторить ввод? y - да/n - нет > ");
                                ConsoleKey key = Console.ReadKey().Key;
                                if (key == ConsoleKey.Y) repeating = true;
                                else return "";
                            }
                        }
                    }
                }
                else
                {
                    FileInfo directoryInfo = new FileInfo(OS_Current_Path + "\\" + name);
                    if (directoryInfo.Exists)
                    {
                        for (int i = 0; i < 22; i++)
                            Console.Write(" ");
                        for (int i = 0; i < name.Length; i++)
                            Console.Write("~");
                        Console.WriteLine();
                        Console.WriteLine("Ошибка: такой файл уже есть в этой директории");
                        Console.WriteLine("Повторить ввод? y - да/n - нет > ");
                        ConsoleKey key = Console.ReadKey().Key;
                        if (key == ConsoleKey.Y) repeating = true;
                        else return "";
                    }
                    else
                    {
                        foreach (char letter in forbidden)
                        {
                            if (name.Contains(letter))
                            {
                                int forbidden_index = name.IndexOf(letter);
                                for (int i = 0; i < 22 + forbidden_index; i++)
                                    Console.Write(" ");
                                Console.Write("~");
                                Console.WriteLine();
                                Console.WriteLine("Ошибка: недопустимый символ");
                                Console.WriteLine("Повторить ввод? y - да/n - нет > ");
                                ConsoleKey key = Console.ReadKey().Key;
                                if (key == ConsoleKey.Y) repeating = true;
                                else return "";
                            }
                        }
                    }
                }
            }
            while (repeating);
            return OS_Current_Path + "\\" + name;
        }

        /// <summary>
        /// Asks user whether he really wants to delete folder/file into current path
        /// </summary>
        /// <param name="t">Object to be deleted</param>
        /// <returns>TRUE if user wants to delete folder/file, FALSE if he doesn't</returns>
        static bool OS_DrawDeleteObject(OS_objs t)
        {
            do
            {
                Console.Clear();
                Console.Write("\n\n Удалить ");
                switch (t)
                {
                    case OS_objs.OS_folder:
                        Console.WriteLine("папку");
                        Console.WriteLine("  " + OS_Folders[choosen_pos]);
                        Console.Write("\n  Вы действительно хотите удалить эту папку? > ");
                        break;
                    case OS_objs.OS_file:
                        Console.WriteLine("файл");
                        Console.WriteLine("  " + OS_Files[choosen_pos - OS_Folders.Length]);
                        Console.Write("\n  Вы действительно хотите удалить этот файл? > ");
                        break;
                }
                Console.WriteLine("y - да/n - нет > ");
                ConsoleKey key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.N:
                        return false;
                    case ConsoleKey.Y:
                        return true;
                    default:
                        break;
                }
            }
            while (true);
        }

        /// <summary>
        /// Opens file into current path as string
        /// </summary>
        static void OS_OpenAsString()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n  " + OS_Files[choosen_pos - OS_Folders.Length]);
            Console.WriteLine("  Текст файла:\n");
            using (FileStream fstream = File.OpenRead(OS_Files[choosen_pos - OS_Folders.Length]))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine("______\n");
                Console.WriteLine(textFromFile);
            }
            Console.WriteLine("______\n");
            Console.WriteLine("  Нажмите любую клавишу, чтобы вернуться к файлам");
            Console.ReadKey();
        }

        static void OS_OpenAsJson()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n  " + OS_Files[choosen_pos - OS_Folders.Length]);

            List<Train> trains = new List<Train>();
            using (FileStream fstream = File.OpenRead(OS_Files[choosen_pos - OS_Folders.Length]))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine("______\n");
                try
                {
                    trains = JsonSerializer.Deserialize<List<Train>>(textFromFile);
                }
                catch (Exception)
                {
                    Console.WriteLine("  Извините, не удалось распаковать файл :(");
                    Console.WriteLine("  Возможные причины:\n   - Файл не в формате JSON\n   - Файл был изменён извне с нарушением кодировки");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("  Файл распакован.\n");
                foreach (Train train in trains)
                {
                    Console.WriteLine(train.ToString());
                }

            }
            Console.WriteLine("______\n");
            Console.WriteLine("  Нажмите любую клавишу, чтобы вернуться к файлам");
            Console.ReadKey();
        }

        static void OS_OpenAsXml()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n  " + OS_Files[choosen_pos - OS_Folders.Length]);

            List<Train> trains = new List<Train>();
            Console.WriteLine("______\n");
            try
            {
                int index = 0;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(OS_Files[choosen_pos - OS_Folders.Length]);
                XmlElement xRoot = xDoc.DocumentElement;
                while (xRoot["Train_" + index.ToString()] != null)
                {
                    XmlElement xTimetable = xRoot["Train_" + index++.ToString()];

                    Train train = new Train();
                    train.FirstStation = xTimetable["firstStation"].InnerText;
                    train.Departure = DateTime.Parse(xTimetable["departure"].InnerText);
                    train.LastStation = xTimetable["lastStation"].InnerText;
                    train.Arrival = DateTime.Parse(xTimetable["arrival"].InnerText);
                    train.WeekdaysOnly = bool.Parse(xTimetable["weekdaysOnly"].InnerText);
                    train.TrainNumber = Int32.Parse(xTimetable["trainNumber"].InnerText);
                    List<string> stations = new List<string>();
                    int station_index = 0;
                    XmlElement xStations = xTimetable["stations"];
                    while (xStations["station_" + station_index.ToString()] != null)
                    {
                        stations.Add(xStations["station_" + station_index++.ToString()].InnerText);
                    }
                    train.Stations = stations.ToArray();
                    trains.Add(train);
                }
                Console.WriteLine("  Файл распакован.\n");
                foreach (Train train in trains)
                {
                    Console.WriteLine(train.ToString());
                }
            }
            catch (Exception)
            {
                Console.WriteLine("  Извините, не удалось распаковать файл :(");
                Console.WriteLine("  Возможные причины:\n   - Файл не в формате XML\n   - Файл был изменён извне с нарушением кодировки");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("______\n");
            Console.WriteLine("  Нажмите любую клавишу, чтобы вернуться к файлам");
            Console.ReadKey();
        }

        static void OS_DrawOpenMode()
        {
            Console.Clear();
            Console.WriteLine("\n\n  В каком режиме открыть файл?");
            Console.WriteLine("  " + OS_Files[choosen_pos - OS_Folders.Length]);
            Console.WriteLine("\n  s - строка\n  j - JSON\n  x - XML\n  z - распаковать архив\n\n  > ");
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.J:
                    OS_OpenAsJson();
                    break;
                case ConsoleKey.X:
                    OS_OpenAsXml();
                    break;
                case ConsoleKey.S:
                    OS_OpenAsString();
                    break;
                case ConsoleKey.Z:
                    OS_Unpack(OS_Files[choosen_pos - OS_Folders.Length]);
                    break;
                default:
                    break;
            }
        }

        static void OS_CreateFolder()
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(OS_DrawNewObject(OS_objs.OS_folder));
                if (directoryInfo.Name != "")
                    directoryInfo.Create();
            }
            catch { }

        }


        static void OS_DeleteObject()
        {
            OS_objs objs;
            if (OS_isFolder()) objs = OS_objs.OS_folder;
            else objs = OS_objs.OS_file;
            if (OS_DrawDeleteObject(objs))
            {
                if (objs == OS_objs.OS_folder)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(OS_Folders[choosen_pos]);
                    directoryInfo.Delete(true);
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(OS_Files[choosen_pos - OS_Folders.Length]);
                    fileInfo.Delete();
                }
            }

        }

        static void OS_CreateFile()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(OS_DrawNewObject(OS_objs.OS_file));
                if (fileInfo.Name == "") return;
                FileStream file = fileInfo.Create();
                Console.Write("  Введите текст файла > ");
                string text = Console.ReadLine();
                byte[] array = System.Text.Encoding.Default.GetBytes(text);
                file.Write(array);
                file.Close();
            }
            catch { }

        }

        /// <summary>
        /// Create Json-file with user's data
        /// </summary>
        static void OS_CreateSerialize(OS_serialize t)
        {
            List<Train> trains = new List<Train>();
            try
            {
                FileInfo fileInfo = new FileInfo(OS_DrawNewObject(OS_objs.OS_file));
                FileStream file = fileInfo.Create();
                do
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n  В этом режиме вы можете создать объект с расписанием пригородных поездов");
                    Console.WriteLine("  Сейчас поездов: " + trains.Count.ToString());
                    Console.Write("  Создать новый поезд? (y - да/n - нет) > ");
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.N) break;
                    Console.Write("\n  Введите станцию отправления > ");
                    string firstStation = Console.ReadLine();
                    Console.Write("  Введите время отправления (HH:MM) > ");
                m1:
                    DateTime departure;
                    try
                    {
                        departure = DateTime.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.Write("  Введите время отправления (HH:MM) > ");
                        goto m1;
                    }
                    Console.Write("  Введите станцию назначения > ");
                    string lastStation = Console.ReadLine();
                    Console.Write("  Введите время прибытия (HH:MM) > ");
                m2:
                    DateTime arrival;
                    try
                    {
                        arrival = DateTime.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.Write("  Введите время прибытия (HH:MM) > ");
                        goto m2;
                    }
                    Console.Write("  Введите номер поезда > ");
                m3:
                    int trainNumber;
                    try
                    {
                        trainNumber = Int32.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        Console.Write("  Введите номер поезда > ");
                        goto m3;
                    }
                m4:
                    Console.Write("  Данный поезд ходит только по будням? (y - да/n - нет) > ");
                    bool weekdaysOnly;
                    ConsoleKey wdo = Console.ReadKey().Key;
                    switch (wdo)
                    {
                        case ConsoleKey.Y:
                            weekdaysOnly = true;
                            break;
                        case ConsoleKey.N:
                            weekdaysOnly = false;
                            break;
                        default:
                            goto m4;
                    }
                    Console.WriteLine("\n  Вводите станции, разделяя их переносом строки [Enter], /end для выхода");
                    List<string> stations = new List<string>();
                    do
                    {
                        Console.Write("  [" + (stations.Count + 1).ToString() + "]: ");
                        stations.Add(Console.ReadLine());
                        if (stations[^1] == "/end")
                        {
                            stations.RemoveAt(stations.Count - 1);
                            break;
                        }
                    } while (true);
                    string[] st = stations.ToArray();
                    trains.Add(new Train(firstStation, departure, lastStation, arrival, trainNumber, weekdaysOnly, st));
                } while (true);
                switch (t)
                {
                    case OS_serialize.OS_json:
                        OS_CreateJson(trains, file);
                        break;
                    case OS_serialize.OS_xml:
                        OS_CreateXml(trains, file);
                        break;
                    default:
                        break;
                }

            }
            catch { }
        }

        static void OS_Decompress(string out_file, string in_file)
        {
            using (FileStream sourceStream = new FileStream(out_file, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(in_file))
                {
                    using (GZipStream gZip = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        gZip.CopyTo(targetStream);
                    }
                }
            }
        }


        static void OS_Unpack(string zipFile)
        {
            string targetFolder = OS_DrawNewObject(OS_objs.OS_file);
            try
            {
                ZipFile.ExtractToDirectory(zipFile, targetFolder);
            }
            catch
            {
                try
                {
                    OS_Decompress(zipFile, targetFolder);
                }
                catch
                {
                    Console.WriteLine("  Извините, не удалось распаковать архив :(");
                    Console.WriteLine("  Возможные причины:\n   - Файл не является архивом\n   - Файл был изменён извне\n   - Файл архивирован сторонним архиватором");
                    Console.ReadKey();
                    return;
                }

            }
            Console.WriteLine("  Удалить исходный архив? (y - да/n - нет) > ");
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.Y:
                    FileInfo fileInfo = new FileInfo(OS_Files[choosen_pos - OS_Folders.Length]);
                    fileInfo.Delete();
                    break;
                case ConsoleKey.N:
                default:
                    break;
            }
        }

        static void OS_Compress(string in_file, string out_file)
        {
            using (FileStream sourceStream = new FileStream(in_file, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(out_file))
                {
                    using (GZipStream gZip = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(gZip);
                    }
                }
            }
        }


        static void OS_CreateZip(OS_objs source, string name_source)
        {
            string zipFile = OS_DrawNewObject(OS_objs.OS_archive);
            string[] ext = name_source.Split('.');
            if (ext.Length > 1) zipFile += "." + ext[1] + ".zip";
            switch (source)
            {
                case OS_objs.OS_folder:
                    ZipFile.CreateFromDirectory(name_source, zipFile);
                    break;
                case OS_objs.OS_file:
                    OS_Compress(name_source, zipFile);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This function print fullscreen navigation-allow table with your folders and files
        /// </summary>
        /// <param name="info">TRUE if you want to see info about selected folder or file in the footer</param>
        static void OS_DrawFoldersAndFiles(bool info)
        {
            Console.Clear();
            Console.Write("\n\n ");
            Console.WriteLine(OS_Current_Path);
            Console.Write("\n  Имя");
            Console.WriteLine();
            pages = (int)Math.Ceiling(((float)(OS_Folders.Length + OS_Files.Length) / (float)(Console.WindowHeight - INFOTEXT_HEIGHT)));
            if (OS_Folders.Length + OS_Files.Length == 0)
            {
                Console.WriteLine("   Эта папка пуста");
            }
            else
            {
                OS_Normali(ref choosen_pos, 0, OS_Folders.Length + OS_Files.Length - 1);
                int cur_page = choosen_pos / (Console.WindowHeight - INFOTEXT_HEIGHT);
                for (int i = cur_page * (Console.WindowHeight - INFOTEXT_HEIGHT); i < (cur_page + 1) * (Console.WindowHeight - INFOTEXT_HEIGHT); i++)
                {

                    if (OS_isFolder(i))
                    {
                        DirectoryInfo temp = new DirectoryInfo(OS_Folders[i]);
                        if (i == choosen_pos)
                            Console.Write("> ");
                        else
                            Console.Write("  ");
                        Console.Write(temp.Name);
                        Console.WriteLine();

                    }
                    else if (i < OS_Folders.Length + OS_Files.Length)
                    {
                        FileInfo temp = new FileInfo(OS_Files[i - OS_Folders.Length]);
                        if (i == choosen_pos)
                            Console.Write("> ");
                        else
                            Console.Write("  ");
                        if (temp.Extension.Length > 0)
                            Console.WriteLine(temp.Name + " (файл " + temp.Extension[1..].ToUpper() + ")");
                        else
                            Console.WriteLine(temp.Name + " (файл)");

                    }
                }
                if (pages > 1)
                    Console.WriteLine("  ..... стр " + (cur_page + 1) + " из " + pages + " .....");
            }
            Console.WriteLine("  ________________");
            if (info)
            {
                do
                {
                    if (OS_isFolder())
                    {
                        DirectoryInfo temp = new DirectoryInfo(OS_Folders[choosen_pos]);
                        Console.WriteLine("  " + temp.Name + ". Тип: папка с файлами");
                        Console.WriteLine("  Создан: " + temp.CreationTime.ToString("g") + ", посл. изменение: " + temp.LastWriteTime.ToString("g"));
                        Console.WriteLine("  Посл. доступ: " + temp.LastAccessTime.ToString("g"));
                    }
                    else
                    {
                        FileInfo temp = new FileInfo(OS_Files[choosen_pos - OS_Folders.Length]);
                        if (temp.Extension.Length > 0)
                            Console.WriteLine("  " + temp.Name + ". Тип: файл \"" + temp.Extension[1..].ToUpper() + "\"");
                        else
                            Console.WriteLine("  " + temp.Name + ". Тип: файл");
                        Console.WriteLine("  Размер: " + OS_GetFileSize(temp.Length));
                        Console.WriteLine("  Создан: " + temp.CreationTime.ToString("g") + ", посл. изменение: " + temp.LastWriteTime.ToString("g"));
                        Console.WriteLine("  Посл. доступ: " + temp.LastAccessTime.ToString("g"));
                    }
                    Console.WriteLine("  Нажмите любую клавишу, чтобы вернуться");
                }
                while (OS_SwitchUnits());
            }
            else
            {
                Console.WriteLine("  Управление - стрелками");
                Console.WriteLine("  u - обновить  Enter - выбрать  Del - удалить  i - инфо");
                Console.WriteLine("  создать.. d - папку  f - файл  j - JSON  x - XML");
                Console.Write("  z - архивировать ");
                if (OS_isFolder()) Console.WriteLine("папку");
                else Console.WriteLine("файл");
                Console.WriteLine("  Backspace - назад  q - выйти");
            }
        }

        /// <summary>
        /// Request key pressing to do certain action
        /// </summary>
        /// <returns>TRUE if user doesn't want to exit</returns>
        static bool Control()
        {
            ConsoleKey pressed = Console.ReadKey().Key;
            switch (pressed)
            {
                case ConsoleKey.UpArrow:
                    choosen_pos--;
                    break;
                case ConsoleKey.DownArrow:
                    choosen_pos++;
                    break;
                case ConsoleKey.RightArrow:
                    choosen_pos += (Console.WindowHeight - 13);
                    break;
                case ConsoleKey.LeftArrow:
                    choosen_pos -= (Console.WindowHeight - 13);
                    break;
                case ConsoleKey.B:
                    unit = 0;
                    OS_DisksInfo();
                    break;
                case ConsoleKey.K:
                    unit = 1;
                    OS_DisksInfo();
                    break;
                case ConsoleKey.M:
                    unit = 2;
                    OS_DisksInfo();
                    break;
                case ConsoleKey.G:
                    unit = 3;
                    OS_DisksInfo();
                    break;
                case ConsoleKey.T:
                    unit = 4;
                    OS_DisksInfo();
                    break;
                case ConsoleKey.U:
                    if (OS_Current_Path == "")
                        OS_DisksInfo();
                    else
                        OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.D:
                    if (OS_Current_Path != "")
                    {
                        OS_CreateFolder();
                        OS_FoldersAndFilesInfo();
                    }
                    break;
                case ConsoleKey.Delete:
                    if (OS_Current_Path != "")
                    {
                        OS_DeleteObject();
                        choosen_pos = 0;
                        OS_FoldersAndFilesInfo();
                    }
                    break;
                case ConsoleKey.I:
                    if (OS_Current_Path != "")
                        OS_DrawFoldersAndFiles(true);
                    break;
                case ConsoleKey.F:
                    if (OS_Current_Path != "")
                    {
                        OS_CreateFile();
                        OS_FoldersAndFilesInfo();
                    }
                    break;
                case ConsoleKey.J:
                    if (OS_Current_Path != "")
                    {
                        OS_CreateSerialize(OS_serialize.OS_json);
                        OS_FoldersAndFilesInfo();
                    }
                    break;
                case ConsoleKey.X:
                    if (OS_Current_Path != "")
                    {
                        OS_CreateSerialize(OS_serialize.OS_xml);
                        OS_FoldersAndFilesInfo();
                    }
                    break;
                case ConsoleKey.Z:
                    if (OS_isFolder())
                        OS_CreateZip(OS_objs.OS_folder, OS_Folders[choosen_pos]);
                    else
                        OS_CreateZip(OS_objs.OS_file, OS_Files[choosen_pos - OS_Folders.Length]);
                    OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.Q:
                    return false;
                case ConsoleKey.Backspace:
                    if (OS_Current_Path == "") return true;
                    if (Directory.GetParent(OS_Current_Path) == null)
                        OS_Current_Path = "";
                    else
                        OS_Current_Path = Directory.GetParent(OS_Current_Path).ToString();
                    if (OS_Current_Path == "")
                        OS_DisksInfo();
                    else
                        OS_FoldersAndFilesInfo();
                    choosen_pos = (int)OS_Positions.Pop();
                    break;
                case ConsoleKey.Enter:
                    if (OS_Current_Path == "")
                    {
                        if (!OS_Drives[choosen_pos].IsReady) return true;
                        OS_Current_Path = OS_Drives[choosen_pos].Name;
                        OS_Positions.Push(choosen_pos);
                        choosen_pos = 0;
                    }
                    else if (OS_isFolder())
                    {
                        OS_Current_Path = OS_Folders[choosen_pos];
                        OS_Positions.Push(choosen_pos);
                        choosen_pos = 0;
                    }
                    else
                        OS_DrawOpenMode();
                    OS_FoldersAndFilesInfo();
                    break;
            }
            return true;
        }

        static void Main(string[] args)
        {
            OS_DisksInfo();
            do
            {
                if (OS_Current_Path == "")
                    OS_DrawDisks();
                else
                    OS_DrawFoldersAndFiles(false);
            }
            while (Control());
        }
    }
}
