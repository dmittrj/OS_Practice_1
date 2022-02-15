using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace OS_Practice_1
{
    class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string University { get; set; }
        public short Course { get; set; }

        public Student(string name, int age, string university, short course)
        {
            Name = name;
            Age = age;
            University = university;
            Course = course;
        }

        public Student()
        {
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

        static int unit = 0;
        static string[] units = new string[] {" Б ", " КБ", " МБ", " ГБ", " ТБ"};
        static int choosen_pos = 0;
        static int pages = 1;

        static DriveInfo[] drives;
        static string[] folders;
        static string[] files;


        static Stack poses = new Stack();
        static int[] MAX_LENGTH = new int[8];
        static string path = "";

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

        static void OS_CreateJson(Student student, FileStream file)
        {
            string studentJson = JsonSerializer.Serialize<Student>(student);
            byte[] array = System.Text.Encoding.Default.GetBytes(studentJson);
            file.Write(array);
            file.Close();
        }

        static void OS_CreateXml(Student student, FileStream file)
        {
            XDocument xDoc = new XDocument();
            XElement stud = new XElement("Student");
            XElement name = new XElement("name", student.Name);
            XElement age = new XElement("age", student.Age);
            XElement uni = new XElement("university", student.University);
            XElement course = new XElement("course", student.Course);
            stud.Add(name);
            stud.Add(age);
            stud.Add(uni); 
            stud.Add(course);
            xDoc.Add(stud);
            xDoc.Save(file);
            file.Close();
        }



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
                        Console.WriteLine("\n\n Создать папку в директории");
                        Console.WriteLine(path);
                        Console.Write("\n  Введите имя папки > ");
                        break;
                    case OS_objs.OS_file:
                        Console.WriteLine("\n\n Создать файл в директории");
                        Console.WriteLine(path);
                        Console.Write("\n  Введите имя файла > ");
                        break;
                    case OS_objs.OS_archive:
                        Console.WriteLine("\n\n Создать архив");
                        Console.WriteLine(path);
                        Console.Write("\n  Введите имя архива > ");
                        break;
                    case OS_objs.OS_archfolder:
                        Console.WriteLine("\n\n Распаковать архив в директорию");
                        Console.WriteLine(path);
                        Console.Write("\n  Введите имя папки (нажмите Enter для распаковки в исходную папку) > ");
                        break;
                    default:
                        break;
                }
                name = Console.ReadLine();
                //if (t == OS_objs.OS_archive)
                //    name += ".zip";
                if (t == OS_objs.OS_folder)
                {
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
                else
                {
                    FileInfo directoryInfo = new FileInfo(path + "\\" + name);
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
                        else repeating = false;
                    }
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

        static void OS_OpenAsString()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n  " + files[choosen_pos - folders.Length]);
            Console.WriteLine("  Текст файла:\n");
            using (FileStream fstream = File.OpenRead(files[choosen_pos - folders.Length]))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine("______\n");
                Console.WriteLine(textFromFile);
                Console.WriteLine("______");  
            }
            Console.WriteLine("\n\n  Что вы хотите делать далее?");
            Console.WriteLine("  Backspace - вернуться к файлам  Ins - дописать в конец");
            Console.WriteLine("  r - переписать  c - очистить");
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.Backspace:
                    return;
                case ConsoleKey.Insert:
                    break;
            }
        }

        static Student Os_EditSer(string name, int age, string university, short course)
        {
            int num;
            do
            {
                Console.Clear();
                Console.WriteLine("\n\n\n  Редактирование файла\n  " + files[choosen_pos - folders.Length]);

                Console.WriteLine("  1. Имя студента > " + name);
                Console.WriteLine("  2. Возраст студента > " + age.ToString());
                Console.WriteLine("  3. Университет > " + university);
                Console.WriteLine("  4. Курс > " + course.ToString());
                Console.WriteLine("  0 - завершить ");
                Console.Write("\n  Какой пункт вы хотите изменить? > ");
                try
                {
                    num = Int32.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    continue;
                }
                switch (num)
                {
                    case 0:
                        return new Student(name, age, university, course);
                    case 1:
                        Console.Write("  Введите новое имя студента > ");
                        name = Console.ReadLine();
                        break;
                    case 2:
                        Console.Write("  Введите новое значение возраста > ");
                        age = Int32.Parse(Console.ReadLine());
                        break;
                    case 3:
                        Console.Write("  Введите новое название университета > ");
                        university = Console.ReadLine();
                        break;
                    case 4:
                        Console.Write("  Введите новое значение курса > ");
                        course = Int16.Parse(Console.ReadLine());
                        break;
                }
            } while (true);
        }

        static void OS_OpenAsJson()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n  " + files[choosen_pos - folders.Length]);
            
            Student student;
            using (FileStream fstream = File.OpenRead(files[choosen_pos - folders.Length]))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine("______\n");
                try
                {
                    student = JsonSerializer.Deserialize<Student>(textFromFile);
                }
                catch (Exception)
                {
                    Console.WriteLine("Извините, не удалось распаковать файл");
                    Console.ReadKey();
                    return;
                    //throw;
                }
                Console.WriteLine("  Файл распакован.\n");
                Console.WriteLine("      Имя студента > " + student.Name);
                Console.WriteLine("  Возраст студента > " + student.Age.ToString());
                Console.WriteLine("       Университет > " + student.University);
                Console.WriteLine("              Курс > " + student.Course.ToString());
                Console.WriteLine("______");
            }
            Console.WriteLine("\n\n  Что вы хотите делать далее?");
            Console.WriteLine("  Backspace - вернуться к файлам  e - редактировать");
            Console.WriteLine("  r - переписать  c - очистить");
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.Backspace:
                    return;
                case ConsoleKey.E:
                    OS_CreateJson(Os_EditSer(student.Name, student.Age, student.University, student.Course), new FileStream(files[choosen_pos - folders.Length], FileMode.OpenOrCreate));
                    break;
            }
        }

        static void OS_OpenAsXml()
        {
            Console.Clear();
            Console.WriteLine("\n\n\n  " + files[choosen_pos - folders.Length]);

            Student student = new Student();
            //using (FileStream fstream = File.OpenRead(files[choosen_pos - folders.Length]))
            //{
                //byte[] array = new byte[fstream.Length];
                //fstream.Read(array, 0, array.Length);
                //string textFromFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine("______\n");
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(files[choosen_pos - folders.Length]);
                    XmlElement xRoot = xDoc.DocumentElement;

                    student.Name = xRoot["name"].InnerText;
                    student.Age = Int32.Parse(xRoot["age"].InnerText);
                    student.University = xRoot["university"].InnerText;
                    student.Course = Int16.Parse(xRoot["course"].InnerText);
            }
                catch (Exception)
                {
                    Console.WriteLine("Извините, не удалось распаковать файл");
                    Console.ReadKey();
                    return;
                    //throw;
                }
                Console.WriteLine("  Файл распакован.\n");
                Console.WriteLine("      Имя студента > " + student.Name);
                Console.WriteLine("  Возраст студента > " + student.Age.ToString());
                Console.WriteLine("       Университет > " + student.University);
                Console.WriteLine("              Курс > " + student.Course.ToString());
                Console.WriteLine("______");
            //}
            Console.WriteLine("\n\n  Что вы хотите делать далее?");
            Console.WriteLine("  Backspace - вернуться к файлам  e - редактировать");
            Console.WriteLine("  r - переписать  c - очистить");
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.Backspace:
                    return;
                case ConsoleKey.E:
                    OS_CreateXml(Os_EditSer(student.Name, student.Age, student.University, student.Course), new FileStream(files[choosen_pos - folders.Length], FileMode.OpenOrCreate));
                    break;
            }
        }

        static void OS_DrawOpenMode()
        {
            Console.Clear();
            Console.WriteLine("\n\n  В каком режиме открыть файл?");
            Console.WriteLine("  " + files[choosen_pos - folders.Length]);
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
                    OS_Unpack(files[choosen_pos - folders.Length]);
                    break;
                default:
                    break;
            }
        }

        static void OS_CreateFolder()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(OS_DrawNewObject(OS_objs.OS_folder));
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
                    FileInfo fileInfo = new FileInfo(files[choosen_pos - folders.Length]);
                    fileInfo.Delete();
                }
            }
            
        }

        static void OS_CreateFile()
        {
            FileInfo fileInfo = new FileInfo(OS_DrawNewObject(OS_objs.OS_file));
            FileStream file = fileInfo.Create();
            Console.Write("  Введите текст файла > ");
            string text = Console.ReadLine();
            byte[] array = System.Text.Encoding.Default.GetBytes(text);
            file.Write(array);
            file.Close();
        }

        
        static void OS_CreateJson()
        {
            FileInfo fileInfo = new FileInfo(OS_DrawNewObject(OS_objs.OS_file));
            FileStream file = fileInfo.Create();
            Console.Write("  В этом режиме вы можете создать объект студента, указав его данные\n\n  Введите имя студента > ");
            string name = Console.ReadLine();
            Console.Write("  Введите возраст студента > ");
            int age = Int32.Parse(Console.ReadLine());
            Console.Write("  Введите название университета > ");
            string university = Console.ReadLine();
            Console.Write("  Введите курс, на котором учится студент > ");
            short course = Int16.Parse(Console.ReadLine());
            Student student = new Student(name, age, university, course);
            //byte[] array = System.Text.Encoding.Default.GetBytes(text);
            //string studentJson = JsonSerializer.Serialize<Student>(student);
            //byte[] array = System.Text.Encoding.Default.GetBytes(studentJson);
            //file.Write(array);
            //file.Close();
            OS_CreateJson(student, file);
        }

        static void OS_CreateXml()
        {
            FileInfo fileInfo = new FileInfo(OS_DrawNewObject(OS_objs.OS_file));
            FileStream file = fileInfo.Create();
            Console.Write("  В этом режиме вы можете создать объект студента, указав его данные\n\n  Введите имя студента > ");
            string name = Console.ReadLine();
            Console.Write("  Введите возраст студента > ");
            int age = Int32.Parse(Console.ReadLine());
            Console.Write("  Введите название университета > ");
            string university = Console.ReadLine();
            Console.Write("  Введите курс, на котором учится студент > ");
            short course = Int16.Parse(Console.ReadLine());
            Student student = new Student(name, age, university, course);
            //byte[] array = System.Text.Encoding.Default.GetBytes(text);
            //string studentJson = JsonSerializer.Serialize<Student>(student);
            //byte[] array = System.Text.Encoding.Default.GetBytes(studentJson);
            //file.Write(array);
            //file.Close();
            OS_CreateXml(student, file);
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
                    Console.WriteLine("Извините, не удалось распаковать архив");
                    Console.ReadKey();
                    return;
                }
                
            }
            Console.WriteLine("  Удалить исходный архив? (y - да/n - нет) > ");
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.Y:
                    FileInfo fileInfo = new FileInfo(files[choosen_pos - folders.Length]);
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
            //FileStream file = fileInfo.Create();
            //Console.Write("  В этом режиме вы можете создать объект студента, указав его данные\n\n  Введите имя студента > ");
            //string name = Console.ReadLine();
            //Console.Write("  Введите возраст студента > ");
            //int age = Int32.Parse(Console.ReadLine());
            //Console.Write("  Введите название университета > ");
            //string university = Console.ReadLine();
            //Console.Write("  Введите курс, на котором учится студент > ");
            //short course = Int16.Parse(Console.ReadLine());
            //Student student = new Student(name, age, university, course);
            ////byte[] array = System.Text.Encoding.Default.GetBytes(text);
            ////string studentJson = JsonSerializer.Serialize<Student>(student);
            ////byte[] array = System.Text.Encoding.Default.GetBytes(studentJson);
            ////file.Write(array);
            ////file.Close();
            //OS_CreateXml(student, file);
        }


        static void OS_DrawFoldersAndFiles(bool info)
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
            if (info)
            {
                if (choosen_pos < folders.Length)
                {
                    DirectoryInfo temp = new DirectoryInfo(folders[choosen_pos]);
                    //Console.WriteLine("  Размер: " + temp.Attributes + " байт");
                    Console.WriteLine("  Создан: " + temp.CreationTime.ToString("g") + ", посл. изменение: " + temp.LastWriteTime.ToString("g"));
                    Console.WriteLine("  Нажмите любую клавишу, чтобы вернуться");
                } else
                {
                    FileInfo temp = new FileInfo(files[choosen_pos - folders.Length]);
                    Console.WriteLine("  Размер: " + temp.Length + " байт");
                    Console.WriteLine("  Создан: " + temp.CreationTime.ToString("g") + ", посл. изменение: " + temp.LastWriteTime.ToString("g"));
                    Console.WriteLine("  Нажмите любую клавишу, чтобы вернуться");
                }
            }
            else
            {
                Console.WriteLine("  Управление - стрелками");
                //Console.WriteLine("  Ед. измерения: b, k, m, g, t");
                Console.WriteLine("  u - обновить  Enter - выбрать  Del - удалить  i - инфо");
                Console.WriteLine("  создать.. d - папку  f - файл  j - JSON  x - XML");
                Console.Write("  z - архивировать ");
                if (choosen_pos < folders.Length) Console.WriteLine("папку");
                else Console.WriteLine("файл");
                Console.WriteLine("  q - выйти");
            }
        }

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
                    OS_FoldersAndFilesInfo();
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
                case ConsoleKey.I:
                    OS_DrawFoldersAndFiles(true);
                    Console.ReadKey();
                    break;
                case ConsoleKey.F:
                    OS_CreateFile();
                    OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.J:
                    OS_CreateJson();
                    OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.X:
                    OS_CreateXml();
                    OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.Z:
                    if (choosen_pos < folders.Length)
                        OS_CreateZip(OS_objs.OS_folder, folders[choosen_pos]);
                    else
                        OS_CreateZip(OS_objs.OS_file, files[choosen_pos - folders.Length]);
                    OS_FoldersAndFilesInfo();
                    break;
                case ConsoleKey.Q:
                    return false;
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
                    {
                        path = drives[choosen_pos].Name;
                        poses.Push(choosen_pos);
                        choosen_pos = 0;
                    }
                    else if (choosen_pos < folders.Length)
                    {
                        path = folders[choosen_pos];
                        poses.Push(choosen_pos);
                        choosen_pos = 0;
                    }
                    else
                        OS_DrawOpenMode();
                    OS_FoldersAndFilesInfo();
                    //OS_DrawInside();
                    break;
            }
            if (choosen_pos < 0) choosen_pos = 0;
            return true;
        }

        static void Main(string[] args)
        {
            OS_DisksInfo();
            do
            {
                if (path == "")
                    OS_DrawDisks();
                else
                    OS_DrawFoldersAndFiles(false);
            }
            while (Control());
        }
    }
}
