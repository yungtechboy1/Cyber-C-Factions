using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Faction2
{
    public class DBConnection
    {
        private MySqlConnection MySqlConnection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnection()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "connectcsharptomysql";
            uid = "username";
            password = "password";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                               database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            MySqlConnection = new MySqlConnection(connectionString);
        }

        //open MySqlConnection to database
        private bool OpenConnection()
        {
            try
            {
                MySqlConnection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close MySqlConnection
        private bool CloseConnection()
        {
            try
            {
                MySqlConnection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        

        //Insert statement
        public void Insert(String query)
        {
            //open MySqlConnection
            if (OpenConnection() == true)
            {
                //create command and assign the query and MySqlConnection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, MySqlConnection);
        
                //Execute command
                cmd.ExecuteNonQuery();

                //close MySqlConnection
                this.CloseConnection();
            }
        }

        //Update statement
        public void Update(String query)
        {
            //Open MySqlConnection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the MySqlConnection using MySqlConnection
                cmd.MySqlConnection = MySqlConnection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close MySqlConnection
                this.CloseConnection();
            }
        }

        //Delete statement
        public void Delete(String query)
        {
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, MySqlConnection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }

        }

        //Select statement
        public List Select()
        {
            string query = "SELECT * FROM tableinfo";

            //Create a list to store the result
           List list = new List();

            //Open MySqlConnection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, MySqlConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
        
                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    List al = new List();
                    foreach (var v in dataReader)
                    {
                        al.Add(v);
                    }
                    list.Add(al);
                }

                //close Data Reader
                dataReader.Close();

                //close MySqlConnection
                CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return null;
            }
        }

        //Count statement
        public int Count(String query = "SELECT Count(*) FROM tableinfo")
        {
            int Count = -1;

            //Open MySqlConnection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, MySqlConnection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar()+"");
        
                //close MySqlConnection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }
    }
}