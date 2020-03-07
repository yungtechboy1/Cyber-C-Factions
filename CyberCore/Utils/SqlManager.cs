using System;
using System.Data;
using log4net;
using MiNET.Net;
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
                return cc.ExecuteReader();
                
            }
            catch (Exception e)
            {
                Log.Error("SQL ERROR E333: \n" + e);
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
                return true;
                
            }
            catch (Exception e)
            {
                Log.Error("SQL ERROR E334: \n" + e);
            }

            return false;
            
            // throw new NotImplementedException();
        }
    }
}