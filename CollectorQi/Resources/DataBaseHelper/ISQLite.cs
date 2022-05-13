using System;
using SQLite;


namespace CollectorQi.Resources.DataBaseHelper
{
    public interface ISQLite
    {
        SQLiteAsyncConnection GetAsyncConnection();
        SQLiteConnection GetConnection();

    }
}
