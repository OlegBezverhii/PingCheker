using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleApp1
{

    class Program
    {
        static CancellationTokenSource cts;

        static void Main(string[] args)
        {
            string NameOrAddress; //IP адрес
            string NameFile; //Имя файла
            string directory = AppDomain.CurrentDomain.BaseDirectory; //Директория запуска программы, 

            cts = new CancellationTokenSource();

            if (args.Length == 0)
            {  
                Console.WriteLine("Массив аргументов пуст. Введите нужные данные.");
                Console.WriteLine("Введите IP адрес:");
                NameOrAddress = Console.ReadLine();
                Console.WriteLine("Введите имя файла:");
                NameFile = Console.ReadLine();
            }
            else
            {
                if (args.Length > 2)
                {
                    Console.WriteLine("Задано много аргументов, проверьте синтаксис - IP_адрес название_файла");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    //Console.WriteLine("Используются следующие аргументы:");
                    NameOrAddress = args[0];
                    NameFile = args[1];
                    Console.WriteLine("IP адрес: "+ NameOrAddress);
                    Console.WriteLine("Имя файла: " + NameFile);
                    /*foreach (string arg in args)
                    {
                        Console.WriteLine(arg);
                    }*/
                }
            };


            NameFile = directory + NameFile; //полное имя файла
            //Console.WriteLine(NameFile);

            Console.WriteLine("Через 2 секунды начнется выполнение");
            Thread.Sleep(2000); //выдержка в две секунды и очищаем консоль
            Console.Clear();

            Console.WriteLine("Нажмите (Esc) для завершения потока пинга");
            Console.WriteLine("Пингуем ");

            var task = new Task(() => Pinging(NameOrAddress, NameFile), cts.Token); //создаем поток для бесконечного цикла

            //var task = new Task(Pinging, cts.Token); 

            task.Start();

            while (!task.IsCompleted)
            {
                var keyInput = Console.ReadKey(true);

                if (keyInput.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    Console.WriteLine("Нажата клавиша ESC, завершаю поток пинга. Для завершения нажмите любую клавишу.");
                    cts.Cancel();
                }
            }


            Console.SetCursorPosition(0, 3);
            Console.WriteLine("Готово.");
            //Console.ReadKey();
        }

        public static void Pinging(string Address, string File)
        {
            while (!cts.IsCancellationRequested)
            {
                ProgressBar();
                if (PingHost(Address)) 
                {
                    //Console.WriteLine(Address + " пингуется");
                }
                else
                {
                    //Console.WriteLine(Address + "не пингуется");

                    DateTime date1 = new DateTime();
                    string datenow = DateTime.Now.ToString();
                    string text = Address + " " + datenow + " не пингуется";

                    //Console.WriteLine(text);

                    WriteFiles(File, text);

                    //Console.WriteLine(Address);
                    //Console.WriteLine(File);

                }
            }
        }

        public static void ProgressBar() //функция ProgressBar'a в консоли
        {
            Thread.Sleep(100);
            Console.SetCursorPosition(9, 1); // left - позиция столбца курсора, top - срока курсора
            Console.Write("|");
            Thread.Sleep(100);
            Console.SetCursorPosition(9, 1); // left - позиция столбца курсора, top - срока курсора
            Console.Write("/");
            Thread.Sleep(100);
            Console.SetCursorPosition(9, 1);
            Console.Write("-");
            Thread.Sleep(100);
            Console.SetCursorPosition(9, 1);
            Console.Write("\\");
            Thread.Sleep(100);
        }


        public static void WriteFiles(string writePath, string text)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(text);
                }

                Console.WriteLine("Запись выполнена");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public static bool PingHost(string nameOrAddress)
        {

            //WriteFiles(writePath, ""); //вызов функции записи в файл


            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException e)
            {
                // Discard PingExceptions and return false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }


    }
}
