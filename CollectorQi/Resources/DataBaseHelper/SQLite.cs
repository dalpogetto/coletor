using System.IO;
using Xamarin.Forms;
using CollectorQi.Resources.DataBaseHelper;
using System;
using SQLite;

[assembly: Dependency(typeof(SQLiteAndroid))]

namespace CollectorQi.Resources.DataBaseHelper
{
    public class  SQLiteAndroid : ISQLite
    {
        static SQLiteAsyncConnection sqliteConnect { get; set; }
        static DateTime dtSqlConnect { get; set; }
        public SQLiteAndroid()
        {

        }

        public SQLiteAsyncConnection GetAsyncConnection()
        {
            try
            {
                TimeSpan difference = DateTime.Now - dtSqlConnect;

                /* Mantem banco de dados conectado por 2 minutos */

                if (difference >= TimeSpan.FromSeconds(120) && sqliteConnect != null)
                {
                    sqliteConnect.CloseAsync();
                    sqliteConnect = null;
                }

                if (sqliteConnect == null)
                {
                    const string fileName = "CollectorQualiIT.db";

                    var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var path = Path.Combine(documentsPath, fileName);
                    var conPath = new SQLiteConnectionString(path, false, "");
                    var connection = new SQLiteAsyncConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);

                    sqliteConnect = connection;
                    dtSqlConnect = DateTime.Now;
                }
            }
            catch (SQLiteException)
            {
                sqliteConnect = null;
                GetAsyncConnection();
            }
            catch (Exception)
            {
                sqliteConnect = null;
                GetAsyncConnection();
            }

            return sqliteConnect;
        }

        public SQLiteConnection GetConnection()
        {

            const string fileName = "CollectorQualiIT.db";

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, fileName);

            //var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var conPath = new SQLiteConnectionString(path, false, "");

            //var cwLock = new SQLiteConnectionWithLock(conPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
            var connection = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
            //var cwLock = new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(path, true));
            //var connection = new SQLiteAsyncConnection(() => cwLock);

            return connection;
        }
    }
}
