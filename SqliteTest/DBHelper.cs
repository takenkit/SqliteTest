using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SqliteTest
{
    /// <summary>
    /// データベースヘルパークラス
    /// </summary>
    public static class DBHelper
    {
        public static void DisplayAllUsers()
        {
            Console.WriteLine("ユーザー一覧:");

            var query = "SELECT * FROM User";
            var dt = ExecuteRead(query, null);

            foreach (DataRow dr in dt.Rows)
            {
                Console.WriteLine($"ID:{dr["Id"]} {dr["FirstName"]} {dr["LastName"]}");
            }
        }

        public static int ExecuteWrite(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;

            // setup the connection to the database
            using (var con = new SQLiteConnection("Data Source=test.db"))
            {
                con.Open();

                // open a new command
                using (var cmd = new SQLiteCommand(query, con))
                {
                    // set the arguments giben in the query
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }

                    // execute the query and get the number of row affected
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }

                return numberOfRowsAffected;
            }
        }

        public static DataTable ExecuteRead(string query, Dictionary<string, object> args)
        {
            if (string.IsNullOrEmpty(query.Trim()))
            {
                return null;
            }

            using (var con = new SQLiteConnection("Data Source=test.db"))
            {
                using (var cmd = new SQLiteCommand(query, con))
                {
                    if (args != null)
                    {

                        foreach (KeyValuePair<string, object> entry in args)
                        {
                            cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                        }
                    }

                    var da = new SQLiteDataAdapter(cmd);

                    var dt = new DataTable();
                    da.Fill(dt);

                    da.Dispose();
                    return dt;
                }
            }
        }

        public static void CreateTable()
        {
            using (var con = new SQLiteConnection("Data Source=test.db"))
            {
                con.Open();
                try
                {
                    const string query = "CREATE TABLE IF NOT EXISTS User(" +
                                            "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                            "FirstName TEXT NOT NULL," +
                                            "LastName TEXT NOT NULL)";

                    using var com = new SQLiteCommand(query, con);
                    com.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public static void DeleteTable()
        {
            using (var con = new SQLiteConnection("Data Source=test.db"))
            {
                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DROP TABLE IF EXISTS Users";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int AddUser(User user)
        {
            const string query = "INSERT INTO User(FirstName, LastName) VALUES(@firstName, @lastName)";

            // here we are setting the parameter values that will be actually
            // replaced in the query in Execute methd
            var args = new Dictionary<string, object>
            {
                { "@firstName", user.FirstName },
                { "@lastName", user.LastName }
            };

            return ExecuteWrite(query, args);

        }

        public static int EditUser(User user)
        {
            const string query = "UPDATE User SET FirstName = @firstName, LastName = @lastName WHERE Id = @id";

            // here we are setting the parameter values that will beactually
            // replaced in the query in Execute method
            var args = new Dictionary<string, object>
            {
                { "@id", user.Id },
                { "@firstName", user.FirstName },
                { "lastName", user.LastName }
            };

            return ExecuteWrite(query, args);
        }

        public static int DeleteUser(User user)
        {
            const string query = "Delete from User WHERE Id = @id";

            // here we are setting the parameter values that will be actually
            // replaced in the query in Execute method
            var args = new Dictionary<string, object>
            {
                { "@id", user.Id }
            };

            return ExecuteWrite(query, args);
        }

        public static User GetUserById(int id)
        {
            var query = "SELECT * FROM User WHERE Id = @id";

            var args = new Dictionary<string, object>
            {
                { "@id", id }
            };

            DataTable dt = ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            var user = new User
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                FirstName = Convert.ToString(dt.Rows[0]["FirstName"]),
                LastName = Convert.ToString(dt.Rows[0]["LastName"])
            };

            return user;
        }
    }
}
