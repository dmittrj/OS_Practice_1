using System;
using System.IO;

namespace OS_Practice_1
{
    class Program
    {
        static int unit = 0;
        static string[] units = new string[] {" Б ", " КБ", " МБ", " ГБ", " ТБ"};
        static int choosen_pos = 0;

        static DriveInfo[] drives;
        static string[] folders;
        static string[] files;


        static DirectoryInfo ddd;
        static int[] MAX_LENGTH = new int[8];
        static string path = "";

        static void About()
        {
            Console.WriteLine("Дисциплина: Операционные системы");
            Console.WriteLine("Практика №1");
            Console.WriteLine("Выполнил:\n\tстудент группы БББО-05-20\n\tБалабанов Дмитрий");
            for (int i = 0; i < 20; i++) Console.Write("=");
            Console.WriteLine();
        }

        static void GapAbove()
        {
            for (int i = 0; i < 3; i++)
                Console.WriteLine();
        }

        static void Gaps(int max, int cur)
        {
            for (int i = 0; i < max - cur + 2; i++)
                Console.Write(" ");
        }

        static void OS_DisksInfo()
        {
            drives = DriveInfo.GetDrives();
            MAX_LENGTH[0] = 3;
            MAX_LENGTH[1] = 3;
            MAX_LENGTH[2] = 6;
            MAX_LENGTH[3] = 11;
            MAX_LENGTH[4] = 10;
            MAX_LENGTH[5] = 9;
            foreach (DriveInfo item in drives)
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
        static void OS_GetFoldersAndFiles()
        {
            folders = Directory.GetDirectories(path);
            files = Directory.GetFiles(path);
            MAX_LENGTH[0] = 3;
            MAX_LENGTH[1] = 3;
            MAX_LENGTH[2] = 6;
            MAX_LENGTH[3] = 11;
            MAX_LENGTH[4] = 10;
            MAX_LENGTH[5] = 9;
            foreach (string item in folders)
            {
                DirectoryInfo temp = new DirectoryInfo(item);
                if (temp.Name.Length > MAX_LENGTH[0])
                    MAX_LENGTH[0] = temp.Name.Length;
            }
            foreach (string item in files)
            {
                if (item.Split('\\')[item.Split('\\').Length - 1].Length > MAX_LENGTH[0])
                    MAX_LENGTH[0] = item.Split('\\')[item.Split('\\').Length - 1].Length;
            }
        }

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
            for (int i = 0; i < drives.Length; i++)
            {
                if (i == (choosen_pos % drives.Length))
                    Console.Write("> ");
                else
                    Console.Write("  ");
                Console.Write(drives[i].Name);
                Gaps(MAX_LENGTH[0], drives[i].Name.Length);
                
                if (drives[i].IsReady)
                {
                    Console.Write(drives[i].VolumeLabel);
                    Gaps(MAX_LENGTH[1], drives[i].VolumeLabel.Length);
                    Console.Write(((long)(drives[i].TotalSize / Math.Pow(1024, unit))).ToString() + units[unit]);
                    Gaps(MAX_LENGTH[2], ((long)(drives[i].TotalSize / Math.Pow(1024, unit))).ToString().Length + 3);
                    Console.Write(((long)(drives[i].TotalFreeSpace / Math.Pow(1024, unit))).ToString() + units[unit]);
                    Gaps(MAX_LENGTH[3], ((long)(drives[i].TotalFreeSpace / Math.Pow(1024, unit))).ToString().Length + 3);
                    Console.Write(drives[i].DriveFormat);
                    Gaps(MAX_LENGTH[4], drives[i].DriveFormat.Length);
                    Console.Write(drives[i].DriveType);
                    Gaps(MAX_LENGTH[5], drives[i].DriveType.ToString().Length);
                } else
                {
                    Console.Write("устройство не готово");
                }
                Console.WriteLine();
            }
            Console.WriteLine("  ______________");
            Console.WriteLine("  Управление - стрелками");
            Console.WriteLine("  Ед. измерения: b, k, m, g, t");
            Console.WriteLine("  u - обновить, Enter - выбрать");
            //Console.WriteLine("  f - создать файл");
        }

        static void OS_DrawInside()
        {
            Console.Clear();
            Console.Write("\n\n ");
            Console.WriteLine(path);
            Console.Write("  Имя"); Gaps(MAX_LENGTH[0], 3);
            //Console.Write("Имя"); Gaps(MAX_LENGTH[1], 3);
            //Console.Write("Размер"); Gaps(MAX_LENGTH[2], 6);
            //Console.Write("Своб. место"); Gaps(MAX_LENGTH[3], 11);
            //Console.Write("Ф. система"); Gaps(MAX_LENGTH[4], 10);
            //Console.Write("Тип диска"); Gaps(MAX_LENGTH[5], 11);
            //Console.WriteLine();
            for (int i = 0; i < folders.Length; i++)
            {
                if (i == (choosen_pos % folders.Length))
                    Console.Write("> ");
                else
                    Console.Write("  ");
                Console.Write(folders[i].Split('\\')[folders[i].Split('\\').Length - 1]);
                Gaps(MAX_LENGTH[0], folders[i].Length);
                Console.WriteLine();
            }
            Console.WriteLine("  ______________");
            Console.WriteLine("  Управление - стрелками");
            Console.WriteLine("  Ед. измерения: b, k, m, g, t");
            Console.WriteLine("  u - обновить, Enter - выбрать");
            //Console.WriteLine("  f - создать файл");
        }

        static int Control()
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
                    OS_DisksInfo();
                    break;
                case ConsoleKey.Enter:
                    choosen_pos = 0;
                    if (path == "")
                        path = drives[choosen_pos].Name;
                    OS_GetFoldersAndFiles();
                    //OS_DrawInside();
                    break;
            }
            if (choosen_pos < 0) choosen_pos = 0;
            return 1;
        }

        static void Main(string[] args)
        {
            int cur_position = -1;
            OS_DisksInfo();
            do
            {
                if (path == "")
                    OS_DrawDisks();
                else
                    OS_DrawInside();
                cur_position = Control();
            }
            while (true);


            if (Console.ReadKey().Key == ConsoleKey.UpArrow)
            {
                Console.WriteLine("ВВерх");
            }
        }
    }
}
