using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;


namespace ProjectOneFinal
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public void RequestAccount(string user, string pass, string firstName, string lastName, string type)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            string sqlSelect = "insert into USERREQUEST (userName, userPassword, firstName, lastName, employeeType, employeeApproved) " +
                "values(@userNameValue, @userPasswordValue, @firstNameValue, @lastNameValue, @employeeTypeValue, 0);";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@userNameValue", HttpUtility.UrlDecode(user));
            sqlCommand.Parameters.AddWithValue("@userPasswordValue", HttpUtility.UrlDecode(pass));
            sqlCommand.Parameters.AddWithValue("@firstNameValue", HttpUtility.UrlDecode(firstName));
            sqlCommand.Parameters.AddWithValue("@lastNameValue", HttpUtility.UrlDecode(lastName));
            sqlCommand.Parameters.AddWithValue("@employeeTypeValue", HttpUtility.UrlDecode(type));

            sqlCommand.Connection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Connection.Close();

        }

        [WebMethod(EnableSession = true)]
        public Account[] GetAccountRequests()
        {
            DataTable sqlDt = new DataTable("accounts");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;

            string sqlSelect = "SELECT userName, userPassword, firstName, lastName, employeeType, employeeApproved FROM userrequest WHERE employeeApproved = 0 ORDER BY lastName desc;";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);

            List<Account> accountRequests = new List<Account>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                accountRequests.Add(new Account
                {
                    userName = sqlDt.Rows[i]["userName"].ToString(),
                    firstName = sqlDt.Rows[i]["firstName"].ToString(),
                    lastName = sqlDt.Rows[i]["lastName"].ToString(),
                    employeeType = Convert.ToInt32(sqlDt.Rows[i]["employeeType"]),
                    employeeApproved = Convert.ToInt32(sqlDt.Rows[i]["employeeApproved"])

                });
            }

            return accountRequests.ToArray();

        }

        //EXAMPLE OF A SELECT, AND RETURNING "COMPLEX" DATA TYPES
        [WebMethod(EnableSession = true)]
        public Account[] GetAccounts()
        {
            DataTable sqlDt = new DataTable("accounts");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            string sqlSelect = "SELECT userName, userPassword, firstName, lastName, employeeType, employeeApproved FROM userrequest u, employee e WHERE e.employeeUserName = u.userName ORDER BY u.lastName;";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            sqlDa.Fill(sqlDt);

            List<Account> accounts = new List<Account>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {

                accounts.Add(new Account
                {
                    userName = sqlDt.Rows[i]["userName"].ToString(),
                    firstName = sqlDt.Rows[i]["firstName"].ToString(),
                    lastName = sqlDt.Rows[i]["lastName"].ToString(),
                    employeeType = Convert.ToInt32(sqlDt.Rows[i]["employeeType"]),
                    employeeApproved = Convert.ToInt32(sqlDt.Rows[i]["employeeApproved"])

                });

            }
            //convert the list of accounts to an array and return!
            return accounts.ToArray();

        }

        //EXAMPLE OF A SELECT, AND RETURNING "COMPLEX" DATA TYPES
        [WebMethod(EnableSession = true)]
        public Account[] GetTime()
        {
            DataTable sqlDt = new DataTable("accounts");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            string sqlSelect = "SELECT firstName, lastName, userName, SUM(HOUR(timeOut-timeIn)) AS 'hoursWorked' FROM employee e, timecard t, userrequest u WHERE e.employeeUserName = t.employeeUserName AND e.employeeUserName = u.userName;";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            sqlDa.Fill(sqlDt);

            List<Account> accounts = new List<Account>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {

                accounts.Add(new Account
                {
                    userName = sqlDt.Rows[i]["userName"].ToString(),
                    firstName = sqlDt.Rows[i]["firstName"].ToString(),
                    lastName = sqlDt.Rows[i]["lastName"].ToString(),
                    hoursWorked = Convert.ToInt32(sqlDt.Rows[i]["hoursWorked"])

                });

            }
            //convert the list of accounts to an array and return!
            return accounts.ToArray();

        }


        //EXAMPLE OF AN UPDATE QUERY
        [WebMethod(EnableSession = true)]
        public void ActivateAccount(string userName)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            //this is a simple update, with parameters to pass in values
            string sqlSelect = "UPDATE USERREQUEST SET employeeApproved=1 WHERE userName=@userNameValue; INSERT INTO `employee` (`employeeUserName`, `managerUserName`) VALUES (@userNameValue, 'admin');";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@userNameValue", HttpUtility.UrlDecode(userName));

            sqlConnection.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }

        //EXAMPLE OF A DELETE QUERY
        [WebMethod(EnableSession = true)]
        public void RejectAccount(string userName)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            string sqlSelect = "DELETE FROM USERREQUEST WHERE userName=@userNameValue";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@userNameValue", HttpUtility.UrlDecode(userName));

            sqlConnection.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            sqlConnection.Close();
        }
        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public bool LogOnEmployee(string uid, string pass)
        {
            //we return this flag to tell them if they logged in or not
            bool success = false;

            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            string sqlSelect = "SELECT userName, employeeType FROM userRequest WHERE userName=@idValue and userPassword=@passValue and employeeType=0";

            //set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            //set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

            //a data adapter acts like a bridge between our command object and 
            //the data we are trying to get back and put in a table object
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            //here's the table we want to fill with the results from our query
            DataTable sqlDt = new DataTable();
            //here we go filling it!
            sqlDa.Fill(sqlDt);
            //check to see if any rows were returned.  If they were, it means it's 
            //a legit account
            if (sqlDt.Rows.Count > 0)
            {
                //if we found an account, store the id and admin status in the session
                //so we can check those values later on other method calls to see if they 
                //are 1) logged in at all, and 2) and admin or not
                Session["id"] = sqlDt.Rows[0]["userName"];
                Session["admin"] = sqlDt.Rows[0]["employeeType"];
                success = true;
            }
            //return the result!
            return success;
        }

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public bool LogOnManager(string uid, string pass)
        {
            //we return this flag to tell them if they logged in or not
            bool success = false;

            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            string sqlSelect = "SELECT userName, employeeType FROM userRequest WHERE userName=@idValue and userPassword=@passValue and employeeType=1";

            //set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            //set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

            //a data adapter acts like a bridge between our command object and 
            //the data we are trying to get back and put in a table object
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            //here's the table we want to fill with the results from our query
            DataTable sqlDt = new DataTable();
            //here we go filling it!
            sqlDa.Fill(sqlDt);
            //check to see if any rows were returned.  If they were, it means it's 
            //a legit account
            if (sqlDt.Rows.Count > 0)
            {
                //if we found an account, store the id and admin status in the session
                //so we can check those values later on other method calls to see if they 
                //are 1) logged in at all, and 2) and admin or not
                Session["id"] = sqlDt.Rows[0]["userName"];
                Session["admin"] = sqlDt.Rows[0]["employeeType"];
                success = true;
            }
            //return the result!
            return success;
        }

        //Send insert statement to database when user clicks Clock In button in landing.html
        [WebMethod(EnableSession = true)]
        public void SendClockIn(string employeeUserName, string timeIn, string timeOut)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            string insert = "insert into TIMECARD (employeeUserName, managerUserName, timeIn, timeOut) " +
                "VALUES(@employeeUserNameValue, 'admin', @timeInValue, @timeOutValue);";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(insert, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@employeeUserNameValue", HttpUtility.UrlDecode(employeeUserName));
            sqlCommand.Parameters.AddWithValue("@timeInValue", HttpUtility.UrlDecode(timeIn));
            sqlCommand.Parameters.AddWithValue("@timeOutValue", HttpUtility.UrlDecode(timeOut));


            sqlCommand.Connection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Connection.Close();
        }

        //Send update statement to database when user clicks Clock Out button in landing.html
        [WebMethod(EnableSession = true)]
        public void SendClockOut(string userName, string timeOut)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;
            string insert = $"UPDATE timecard SET timeOut = @timeOutValue WHERE employeeUserName = @employeeUserNameValue AND timeOut = '0000-00-00 00:00:00';";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(insert, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@employeeUserNameValue", HttpUtility.UrlDecode(userName));
            sqlCommand.Parameters.AddWithValue("@timeOutValue", HttpUtility.UrlDecode(timeOut));


            sqlCommand.Connection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Connection.Close();
        }

        [WebMethod(EnableSession = true)]
        public History[] GetHistory(string userName)
        {
            DataTable sqlDt = new DataTable("accounts");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["Work"].ConnectionString;

            string sqlSelect = "select employeeUserName, timeIn, timeOut From aasu_work.timecard where employeeUserName = @employeeUserNameValue;";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@employeeUserNameValue", HttpUtility.UrlDecode(userName));

            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);

            List<History> userHistory = new List<History>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                userHistory.Add(new History
                {
                    employeeUserName = sqlDt.Rows[i]["employeeUsername"].ToString(),
                    timeIn = sqlDt.Rows[i]["timeIn"].ToString(),
                    timeOut = sqlDt.Rows[i]["timeOut"].ToString()
                });
            }

            return userHistory.ToArray();

        }

    }






}
