using System;
using System.Threading;
using System.Threading.Tasks;
using static SqliteTest.DBHelper;

namespace SqliteTest
{
    /// <summary>
    /// コマンドライン操作でユーザーの表示・追加・編集・削除を行う
    /// </summary>
    class Program
    {
        static KeyboardEventLoop eventLoop;
        
        public static void Main(string[] args)
        {
            CreateTable();

            DisplayHelp();

            var cts = new CancellationTokenSource();
            eventLoop = new KeyboardEventLoop(code => OnKeyDown(code, cts));

            Task.Run(() => eventLoop.Start(cts.Token)).Wait();
        }

        private static void OnKeyDown(string line, CancellationTokenSource cts)
        {
            string eventCode = line.Trim();// 先頭コマンドを取得
            switch (eventCode)
            {
                case "add":
                    AddUser();
                    break;
                case "edit":
                    EditUser();
                    break;
                case "del":
                    DeleteUser();
                    break;
                case "disp":
                    DisplayUser();
                    break;
                case "help":
                    DisplayHelp();
                    break;
                case "list":
                    // ユーザー一覧を表示する
                    DBHelper.DisplayAllUsers();
                    break;
                case "quit":
                    cts.Cancel();
                    break;
                default:
                    DisplayHelp();
                    break;
            }
        }
        /// <summary>
        /// ヘルプを表示する
        /// </summary>
        private static void DisplayHelp()
        {
            Console.Write(
                "使い方\n" +
                "add    : ユーザーを追加します。\n" +
                "edit   : ユーザーを編集します。\n" +
                "del    : ユーザーを削除します。\n" +
                "disp   : ユーザーの情報を表示します。\n" +
                "help   : ヘルプを表示します\n" +
                "list   : ユーザーの一覧を表示します。\n" +
                "quit   : プログラムを終了します。\n");
        }

        /// <summary>
        /// ユーザーを追加する
        /// </summary>
        private static void AddUser()
        {
            // FirstNameを指定する
            Console.Write("FirstName>");
            var firstName = Console.ReadLine();

            // LastNameを指定する
            Console.Write("LastName>");
            var lastName = Console.ReadLine();

            var user = new User()
            {
                FirstName = firstName,
                LastName = lastName,
            };

            // データベースへ追加
            int ret = DBHelper.AddUser(user);

            Console.WriteLine($"ユーザーを追加しました({firstName} {lastName})");
        }

        /// <summary>
        /// ユーザーを編集する
        /// </summary>
        private static void EditUser()
        {
            // Idを指定する
            Console.Write("Id>");
            var id = int.Parse(Console.ReadLine());

            // ユーザーを取得する
            var user = DBHelper.GetUserById(id);

            if (user != null)
            {
                // ユーザーを表示する
                Console.WriteLine($"ユーザー名r={user.FirstName} {user.LastName}");
                // FirstNameを指定する
                Console.Write("FirstName>");
                var firstName = Console.ReadLine();

                // LastNameを指定する
                Console.Write("LastName>");
                var lastName = Console.ReadLine();

                user.FirstName = firstName;
                user.LastName = lastName;

                // 編集を反映する
                int ret = DBHelper.EditUser(user);

                Console.WriteLine($"ユーザーを編集しました({firstName} {lastName})");
            }
            else
            {
                Console.WriteLine("ユーザーが存在しませんでした");
            }
        }

        /// <summary>
        /// ユーザーを削除する
        /// </summary>
        private static void DeleteUser()
        {
            // Idを指定する
            Console.Write("Id>");
            var id = int.Parse(Console.ReadLine());

            // ユーザーを取得する
            var user = DBHelper.GetUserById(id);

            if (user != null)
            {
                // ユーザーを表示する
                Console.WriteLine($"ユーザー名={user.FirstName} {user.LastName}");
                Console.Write("本当に削除しますか(y/n)？");
                string yn = Console.ReadLine();

                if (yn.ToLower().Equals("y"))
                {
                    DBHelper.DeleteUser(user);
                    Console.WriteLine("ユーザーを削除しました");
                }
            }
            else
            {
                Console.WriteLine("ユーザーが存在しませんでした");
            }
            
        }

        /// <summary>
        /// ユーザーを表示する
        /// </summary>
        private static void DisplayUser()
        {
            // Idを指定する
            Console.Write("Id>");
            var id = int.Parse(Console.ReadLine());

            // ユーザーを取得する
            var user = DBHelper.GetUserById(id);

            if (user != null)
            {
                // ユーザーを表示する
                Console.WriteLine($"ユーザー名={user.FirstName} {user.LastName}");
            }
            else
            {
                Console.WriteLine("ユーザーが存在しませんでした");
            }
        }
    }
}
