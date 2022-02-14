using System;
using System.Collections;
using System.IO;

namespace OS_Practice_1
{
    class Program
    {
        const int INFOTEXT_HEIGHT = 13;
        enum OS_objs
        {
            OS_folder = 1,
            OS_file = 2
        }

        static int unit = 0;
        static string[] units = new string[] {" Б ", " КБ", " МБ", " ГБ", " ТБ"};
        static int choosen_pos = 0;
        static int pages = 1;

        static DriveInfo[] drives;
        static string[] folders;
        static string[] files;


        static DirectoryInfo ddd;
        static Stack poses = new Stack();
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
        static void OS_FoldersAndFilesInfo()
        {
            folders = Directory.GetDirectories(path);
            files = Directory.GetFiles(path);
            //MAX_LENGTH[0] = 3;
            //MAX_LENGTH[1] = 3;
            //MAX_LENGTH[2] = 6;
            //MAX_LENGTH[3] = 11;
            //MAX_LENGTH[4] = 10;
            //MAX_LENGTH[5] = 9;
            pages = (int)Math.Ceiling(((float)(folders.Length + files.Length) / (float)(Console.WindowHeight - INFOTEXT_HEIGHT)));
            
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

        static string OS_DrawNewFolder()
        {
            string name;
            bool repeating = false;
            do
            {
                repeating = false;
                Console.Clear();
                Console.WriteLine("\n\n Создать папку в директории");
                Console.WriteLine(path);
                Console.Write("\n  Введите имя папки > ");
                name = Console.ReadLine();
                DirectoryInfo directoryInfo = new DirectoryInfo(path + "\\" + name);
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
                    else repeating = false;
                }
            }
            while (repeating);
            return path + "\\" + name;
        }

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
                        Console.WriteLine("  " + folders[choosen_pos]);
                        Console.Write("\n  Вы действительно хотите удалить эту папку? > ");
                        break;
                    case OS_objs.OS_file:
                        Console.WriteLine("файл");
                        Console.WriteLine("  " + files[choosen_pos - folders.Length]);
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
                        //Console.WriteLine("Повторите ввод.\ny - да/n - нет > ");
                        break;
                }
            }
            while (true);
        }

        static void OS_CreateFolder()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(OS_DrawNewFolder());
            directoryInfo.Create();
        }

        static void OS_DeleteObject()
        {
            OS_objs objs;
            if (choosen_pos < folders.Length) objs = OS_objs.OS_folder;
            else objs = OS_objs.OS_file;
            if (OS_DrawDeleteObject(objs))
            {
                if (objs == OS_objs.OS_folder) {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folders[choosen_pos]);
                    directoryInfo.Delete(true);
                } 
                else
                {
                    FileInfo fileInfo = new FileInfo(path + "\\" + files[choosen_pos - folders.Length]);
                    fileInfo.Delete();
                }
            }
            
        }

        static void OS_DrawFoldersAndFiles()
        {
            Console.Clear();
            Console.Write("\n\n ");
            Console.WriteLine(path);
            Console.Write("\n  Имя");
            //Console.Write("Имя"); Gaps(MAX_LENGTH[1], 3);
            //Console.Write("Размер"); Gaps(MAX_LENGTH[2], 6);
            //Console.Write("Своб. место"); Gaps(MAX_LENGTH[3], 11);
            //Console.Write("Ф. система"); Gaps(MAX_LENGTH[4], 10);
            //Console.Write("Тип диска"); Gaps(MAX_LENGTH[5], 11);
            Console.WriteLine();
            if (folders.Length + files.Length == 0)
            {
                Console.WriteLine("  Эта папка пуста");
            }
            else
            {
                choosen_pos %= (folders.Length + files.Length);
                if (choosen_pos < 0) choosen_pos = 0;
                int cur_page = choosen_pos / (Console.WindowHeight - INFOTEXT_HEIGHT);
                for (int i = cur_page * (Console.WindowHeight - INFOTEXT_HEIGHT); i < (cur_page + 1) * (Console.WindowHeight - INFOTEXT_HEIGHT); i++)
                {
                    if (i < folders.Length)
                    {
                        DirectoryInfo temp = new DirectoryInfo(folders[i]);

                        if (i == (choosen_pos % (folders.Length + files.Length)))
                            Console.Write("> ");
                        else
                            Console.Write("  ");
                        Console.Write(temp.Name);
                        //Gaps(MAX_LENGTH[0], temp.Name.Length);
                        Console.WriteLine();
                    }
                    else if (i < (folders.Length + files.Length))
                    {
                        FileInfo temp = new FileInfo(files[i - folders.Length]);

                        if (i == (choosen_pos % (folders.Length + files.Length)))
                            Console.Write("> ");
                        else
                            Console.Write("  ");
                        Console.WriteLine(temp.Name + " (файл " + temp.Extension + ")");
                        //Gaps(MAX_LENGTH[0], temp.Name.Length);
                        //Console.WriteLine();
                    }
                }
                Console.WriteLine("  .... стр " + (cur_page + 1) + " из " + pages + " ....");
            }
            Console.WriteLine("  ______________");
            Console.WriteLine("  Управление - стрелками");
            Console.WriteLine("  Ед. измерения: b, k, m, g, t");
            Console.WriteLine("  u - обновить  Enter - выбрать  Del - удалить  i - инфо");
            Console.WriteLine("  создать.. d - папку  f - файл  j - JSON  x - XML  z - архив");
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
                case ConsoleKey.RightArrow:
                    choosen_pos += (Console.WindowHeight - 13);
                    if (choosen_pos >= folders.Length + files.Length) choosen_pos = folders.Length + files.Length - 1;
                    break;
                case ConsoleKey.LeftArrow:
                    choosen_pos -= (Console.WindowHeight - 13);
                    if (choosen_pos < 0) choosen_pos = 0;
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
                case ConsoleKey.D:
                    OS_CreateFolder();
                    OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.Delete:
                    OS_DeleteObject();
                    choosen_pos = 0;
                    OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.Backspace:
                    if (Directory.GetParent(path) == null)
                        path = "";
                    else
                        path = Directory.GetParent(path).ToString();
                    if (path == "")
                        OS_DisksInfo();
                    else
                        OS_FoldersAndFilesInfo();
                    choosen_pos = (int)poses.Pop();
                    break;
                case ConsoleKey.Enter:
                    if (path == "")
                        path = drives[choosen_pos].Name;
                    else if (choosen_pos < folders.Length) 
                        path = folders[choosen_pos];
                    poses.Push(choosen_pos);
                    choosen_pos = 0;
                    OS_FoldersAndFilesInfo();
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
                    OS_DrawFoldersAndFiles();
                cur_position = Control();
            }
            while (true);
        }
    }
}
