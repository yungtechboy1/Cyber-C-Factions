using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using log4net;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Utils;
using MySql.Data.MySqlClient;

namespace CyberCore.Utils
{
    public class SqlManager
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(SqlManager));
        public String Host { get; set; }
        public int Port { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Database { get; set; }


        public CyberCoreMain CCM { get; }

        private MySqlConnection MSC { get; set; } = null;
        private bool Active = false;
        public string ConnectionString;

        public SqlManager(CyberCoreMain ccm)
        {
            CCM = ccm;
            Host = CCM.MasterConfig.GetProperty("Host", null);
            Username = CCM.MasterConfig.GetProperty("Username", null);
            Password = CCM.MasterConfig.GetProperty("Password", null);
            Database = CCM.MasterConfig.GetProperty("db-Server", null);
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
                    if (Active)
                    {
                        Log.Info("MySQL MySqlConnection to" + Host + " was successful!");
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Error!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", e);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

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
        public List<Dictionary<string, object>> executeSelect(String query)
        {
            return executeSelecta(query).Result;
        }
 public async Task<List<Dictionary<string, object>>> executeSelecta(String query)
        {
            List<Dictionary<String, Object>> data = new List<Dictionary<String, Object>>();
            List<String> cols = new List<string>();
            DataTable schema = null;


            Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1" + ConnectionString);
            using (var con = new MySqlConnection(ConnectionString))
            {
                await con.OpenAsync();
                Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.1");
                using (var schemaCommand = new MySqlCommand(query, con))
                {
                    Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.2");
                    using (var reader = await schemaCommand.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    {
                        Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.3");
                        schema = reader.GetSchemaTable();
                        Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 1.4");
                    }
                }

                Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 2");

                foreach (DataRow col in schema.Rows)
                {
                    cols.Add(col.Field<String>("ColumnName"));
                }


                Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 3");

                using (var schemaCommand = new MySqlCommand(query, con))
                {
                    using (var reader = await schemaCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // int i = 0;
                            Dictionary<String, Object> aa = new Dictionary<string, object>();
                            foreach (var co in cols)
                            {
                                aa.Add(co, reader[co]);
                                // i++;
                            }
                            
                            data.Add(aa);
                        }
                    }
                }
            }

            Log.Info("DDDDDDDDDDDDDDDPASSSSSSSSS 4");
            return data;
        }

        public bool Insert(string q, string c, byte[] b)
        {
            using (MySqlConnection con = GetMySqlConnection())
            {
                string query = q;
                using (MySqlCommand cmd = new MySqlCommand(query))
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
            MySqlConnection c = GetMySqlConnection();
            if (c == null)
            {
                Log.Error("Attempt to Insert @ " + Host + " while connection was Invalid for Query:\n " + q);
                return false;
            }

            try
            {
                MySqlCommand cc = new MySqlCommand {Connection = c, CommandText = q};
                cc.ExecuteNonQuery();
                c.Close();
                return true;
            }
            catch (Exception e)
            {
                Log.Error("SQL ERROR E334: \n" + e);
            }

            c.Close();
            return false;

            // throw new NotImplementedException();
        }
    }
}