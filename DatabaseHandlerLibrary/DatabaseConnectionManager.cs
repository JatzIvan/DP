using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace RoadSectionHandler
{
    public class DatabaseConnectionManager {

        private static DatabaseConnectionManager Instance;

        private string ConnectionString { get; set; }

        public static DatabaseConnectionManager getInstance(string ConnectionString = "")
        {
            if (Instance == null)
            {
                Instance = new DatabaseConnectionManager(ConnectionString);
            }

            return Instance;
        }

        private DatabaseConnectionManager(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /**
         * connectionString - Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase
         * connections should get disposed of after usage
         */
        public async Task<NpgsqlConnection> GetConnection(string connectionString)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            return connection;
        }

        public async Task<NpgsqlConnection> GetConnection()
        {
            return await GetConnection(this.ConnectionString);
        }

        public async Task<List<T>> RetrieveData<T>(string tableName) where T : IDatabaseModel<T>
        {
            return await RetrieveData<T>(tableName, await GetConnection());
        }

        public async Task<List<T>> RetrieveData<T>(string tableName, NpgsqlConnection connection) where T : IDatabaseModel<T>
        {
            await connection.OpenAsync();

            List<T> data = new List<T>();
            T objTemp = (T)Activator.CreateInstance(typeof(T), new object[] {});
            await using (var command = new NpgsqlCommand("SELECT * FROM (@p1)"))
            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    data.Add(objTemp.MapReaderToObject(reader));
                }

                return data;
            }

            return default;
        }

        public async Task<List<T>> RetrieveData<T>(NpgsqlCommand strCommand, NpgsqlConnection connection) where T : IDatabaseModel<T>
        {
            await connection.OpenAsync();

            List<T> data = new List<T>();
            T objTemp = (T)Activator.CreateInstance(typeof(T), new object[] { });
            await using (var command = strCommand)
            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    data.Add(objTemp.MapReaderToObject(reader));
                }

                return data;
            }

            return default;
        }

        public static T GetPropertyValue<T>(object obj, string propName) { 
            return (T)obj.GetType().GetProperty(propName).GetValue(obj, null); 
        }

        public async void InsertData<T>(List<T> values, string tableName) where T : IDatabaseModel<T>
        {
            InsertData<T>(values, tableName, await GetConnection());
        }

        public async void InsertData<T>(List<T> values, string tableName ,NpgsqlConnection connection) where T : IDatabaseModel<T>
        {
            await connection.OpenAsync();

            List<T> data = new List<T>();
            //T objTemp = (T)Activator.CreateInstance(typeof(T), new object[] { });

            

            MemberInfo[] members = typeof(T).GetMembers();
            List<string> flds = new List<string>();
            foreach(MemberInfo member in members)
            {
                if (member.MemberType.Equals(MemberTypes.Property))
                {
                    flds.Add(member.Name);
                }
            }

            List<string> translatedValues = new List<string>();

            foreach(T value in values)
            {
                List<object> valProps = new List<object>();
                foreach(string fld in flds)
                {
                    object propVal = GetPropertyValue<object>(value, fld);
                    if(propVal != null)
                    {
                        valProps.Add("'" + propVal + "'");
                    }
                    else
                    {
                        valProps.Add(propVal);
                    }
                }
                translatedValues.Add("(" + String.Join(",", valProps) + ")");
            }

            await using (var command = new NpgsqlCommand("INSERT INTO " + tableName + "(" + String.Join(",", flds) + ") VALUES " + String.Join(",", translatedValues)))
            await command.ExecuteNonQueryAsync();

        }

    }
}
