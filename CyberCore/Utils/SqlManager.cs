using System;
using System.Collections.Generic;
using System.Data;
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

        public SqlManager(CyberCoreMain ccm)
        {
            CCM = ccm;
            Host = CCM.MasterConfig.GetProperty("Host", null);
            Username = CCM.MasterConfig.GetProperty("Username", null);
            Password = CCM.MasterConfig.GetProperty("Password", null);
            Database = CCM.MasterConfig.GetProperty("Database", null);
            Port = CCM.MasterConfig.GetProperty("Port", 3360);
            String cs = "SERVER=" + Host + ";" + "DATABASE=" +
                        Database + ";" + "UID=" + Username + ";" + "PASSWORD=" + Password + ";";
            try
            {
                MSC = new MySqlConnection(cs);
                MSC.Open();
                CheckMSQLState();
                if (Active)
                {
                    Log.Info("MySQL MySqlConnection to" + Host + " was successful!");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public MySqlConnection GetMySqlConnection()
        {
            CheckMSQLState();
            if (Active)
            {
                return MSC;
            }

            return null;
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

        public MySqlDataReader Query(String q)
        {
            MySqlConnection c = GetMySqlConnection();
            if (c == null)
            {
                Log.Error("Attempt to Query @ " + Host + " while connection was Invalid for Query:\n " + q);
                return null;
            }

            try
            {
                MySqlCommand cc = new MySqlCommand(q, c);
                c.Close();
                return cc.ExecuteReader();
            }
            catch (Exception e)
            {
                Log.Error("SQL ERROR E333: \n" + e);
            }

            c.Close();
            return null;
        }

        public List<Dictionary<String, Object>> executeSelect(String query)
        {
            List<Dictionary<String, Object>> data = new List<Dictionary<String, Object>>();
            List<String> cols = new List<string>();
            DataTable schema = null;


            using (var con = GetMySqlConnection())
            {
                using (var schemaCommand = new MySql.Data.MySqlClient.MySqlCommand(query, con))
                {
                    con.Open();
                    using (var reader = schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        schema = reader.GetSchemaTable();
                    }

                    con.Close();
                }
            }

            foreach (DataRow col in schema.Rows)
            {
                cols.Add(col.Field<String>("ColumnName"));
            }

            var a = Query(query);
            if (a != null)
            {
                while (a.Read())
                {
                    int i = 0;
                    Dictionary<String, Object> aa = new Dictionary<string, object>();
                    foreach (var co in cols)
                    {
                        aa.Add(cols[i], a[cols[i]]);
                    }

                    data.Add(aa);
                }

                a.Close();
                return data;
            }
            else
            {
                Log.Error($"Error Executing Select for query '{query}'");
            }

            return null;
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