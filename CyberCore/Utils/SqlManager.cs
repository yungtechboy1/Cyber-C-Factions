using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CyberCore.Manager.Forms;
using log4net;
using MySql.Data.MySqlClient;

namespace CyberCore.Utils
{
    public class SqlManager
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(SqlManager));
        private bool Active;
        public string ConnectionString;

        public SqlManager(CyberCoreMain ccm, String key = "")
        {
            if(key.Length != 0)key +="-";
            CCM = ccm;
            Host = CCM.MasterConfig.GetProperty(key+"Host", null);
            Username = CCM.MasterConfig.GetProperty(key+"Username", null);
            Password = CCM.MasterConfig.GetProperty(key+"Password", null);
            Database = CCM.MasterConfig.GetProperty(key+"db-Server", null);
            Port = CCM.MasterConfig.GetProperty("Port", 3360);
            ConnectionString = $"SERVER={Host};port={Port};DATABASE={Database};user id={Username};PASSWORD={Password};";
            try
            {
                // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using var connection = new MySqlConnection(ConnectionString);
                MSC = connection;
                try
                {
                    MSC.Open();
                    CheckMSQLState();
                    // executeSelect("SELECT *");
                    if (Active) Log.Info("MySQL MySqlConnection to" + Host + " was successful!");
                }
                catch (Exception e)
                {
                    Log.Error("Error!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!aaa:"+Host, e);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }


        public CyberCoreMain CCM { get; }

        private MySqlConnection MSC { get; }

        public MySqlConnection GetMySqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public void CheckMSQLState()
        {
            if (MSC != null)
            {
                if (MSC.State == ConnectionState.Open)
                {
                    Active = true;
                    return;
                }

                Log.Warn("MySQL is Connected but is in state: " + MSC.State);
                Log.Warn("Please look at ConnectionState.cs for more info!");
            }

            Active = false;
        }

        // public async  List<Dictionary<string, object>> Query(String q)
        // {
        //     using (var conn = new MySqlConnection(ConnectionString))
        //     {
        //         await conn.OpenAsync();
        //
        //         // Retrieve all rows
        //         var cmd = new MySqlCommand(q, conn);
        //
        //         var reader = await cmd.ExecuteReaderAsync();
        //
        //         while (await reader.ReadAsync())
        //             Console.WriteLine(reader.GetString(0) + "|||||" + reader.GetString(1));
        //         return reader;
        //
        //
        //         // using (var cmd = new MySqlCommand(q, conn))
        //         //               {
        //         //                   using (var reader = await cmd.ExecuteReaderAsync())
        //         //                   {
        //         //                       while (await reader.ReadAsync())
        //         //                           Console.WriteLine(reader.GetString(0)+"|||||"+reader.GetString(1));
        //         //                       return reader;
        //         //                   }
        //         //               }
        //
        //         Log.Error("Attempt to Query @ " + Host + " while connection was Invalid for Query:\n " + q);
        //         return null;
        //     }
        // }
        public List<Dictionary<string, object>> executeSelect(string query)
        {
            return executeSelecta(query);
        }

        public List<Dictionary<string, object>> executeSelecta(string query)
        {
            var startt = DateTime.Now.Ticks;
            var data = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            DataTable schema = null;


            // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1" + ConnectionString);
            using (var con = new MySqlConnection(ConnectionString))
            {
                con.Open();
                // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.1");
                using (var schemaCommand = new MySqlCommand(query, con))
                {
                    // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.2");
                    using (var reader = schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.3");
                        schema = reader.GetSchemaTable();
                        // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.4");
                    }
                }

                // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 2");

                foreach (DataRow col in schema.Rows) cols.Add(col.Field<string>("ColumnName"));


                // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 3");

                using (var schemaCommand = new MySqlCommand(query, con))
                {
                    using (var reader = schemaCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // int i = 0;
                            var aa = new Dictionary<string, object>();
                            foreach (var co in cols)
                                if (reader.GetOrdinal(co) != -1)
                                    aa.Add(co, reader[co]);
                            // i++;

                            data.Add(aa);
                        }
                    }
                }
            }
            var stop = DateTime.Now.Ticks;
            var final = stop - startt;
            // CyberCoreMain.Log.Info($"IT TOOK {final/500000} OR {final/10000000} Secs OR {final/100000} OR {final/1000} orr {final/10} TICKS TO EXECUTE SQL COMMAND : "+query);
            // Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 4");
            return data;
        }

        public bool Insert(string q, string c, byte[] b)
        {
            using (var con = GetMySqlConnection())
            {
                var query = q;
                using (var cmd = new MySqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue(c, b);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return true;
                }
            }

            return false;
        }

        public bool Insert(string q)
        {
            return Inserta(q).Result;
        }

        public async Task<bool> Inserta(string q)
        {
            using (var c = GetMySqlConnection())
            {
                await c.OpenAsync();
                if (c == null)
                {
                    Log.Error("Attempt to Insert @ " + Host + " while connection was Invalid for Query:\n " + q);
                    return false;
                }


                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = c;
                    cmd.CommandText = q;
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }

                return false;

                // throw new NotImplementedException();
            }
        }
    }
}