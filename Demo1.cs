//#define __DEMO_INSERT_MODIFY_DELETE__
//#define  __SHOW_INPUT__
using System;
//using System.Data;

using System.Data.OleDb;   //use the OleDB data interface

using System.Threading; // Sleep



namespace ConAppADONET_StressTest
{
    public class Demo1
    {
        static string strDatabasePath = @"\\XPS8700\Database\";
        static string strProvider = "Provider=Microsoft.Jet.OLEDB.4.0; ";

       // static string strDatabaseFileName1 = "assess.mdb";  //This one does NOT include Table: "TableCounter"
        static string strDatabaseFileName2 = "assess2.mdb";  //includes Table: "TableCounter"

        static string strDataSource = "Data Source=" + strDatabasePath + strDatabaseFileName2;

        static string strConnect = strProvider + strDataSource;

        //Error Counters
        static int Modify_FailedCount = 0;
        static int ModifyParameterized_FailureCount = 0;
        static int Connect_FailureCount = 0;
        static  int Delete_FailureCount = 0;
        static int iRetryCount_Max = 0;
        static int iRetryTotalCount = 0;

        public static void Summary_FailureCounts()
        {
            Console.WriteLine("Modify_FailedCount={0}", Modify_FailedCount);
            Console.WriteLine("ModifyParameterized_FailureCount={0}", ModifyParameterized_FailureCount);
            Console.WriteLine("Connect_FailureCount={0}", Connect_FailureCount);
            Console.WriteLine("Delete_FailureCount={0}", Delete_FailureCount);
            Console.WriteLine("iRetryCount_Max={0}", iRetryCount_Max);
            Console.WriteLine("iRetryTotalCount={0}", iRetryTotalCount);
        }
        //
        // int MyID -- my ID
        // int MaxID -- max number of simultaneous consoleApp processes
        // int CountMax - max number of iterations
        //
        //            Demo1.Start(ID, MaxID, CountMax);

        public static void Start(int MyID, int MaxID, int CountMax)
        {
            //Fields: Row_ID,  Count1, Count2, Count3, Count4, Count5, Text1, Text2
            int Row_ID;
            int Count = 1;
            int Count1, Count2, Count3, Count4, Count5, Count6, Count7, Count8, Count9, Count10 ;


            String Text1 = "Some Text: #1";
            String Text2 = "Some Text: #2";


            System.Console.WriteLine("START: Demo1.Start()");
            //Logger.TraceMessage("Tracing --> START: Demo1.Start()");

            String strCountField = String.Format("Count{0}", MyID);

            Console.WriteLine("strConnect=" + strConnect);

            while (Count < CountMax)
            {
                //simulate random user requests: minDelay =100 millisec, maxDelay=1000 millisec; uniform distribution
                random_delay(100, 1000); 

                //create the connection object
                OleDbConnection myConnection = new OleDbConnection(strConnect);

                String strQueryString = "SELECT * FROM TableCounter WHERE Row_ID=42";

                // Create the Command and Parameter objects.
                OleDbCommand command = new OleDbCommand(strQueryString, myConnection);

                // Step 1: Open the connection
                bool try_connect = true;
                int connectCount = 0;
                while (try_connect)
                {
                    connectCount += 1;
                    try
                    {
                        // System.Console.WriteLine("try to open the myConnection");
                        oleDbConnection_Open_WithRetry(myConnection); // <--- encapsulate with a retry because Connection.Open()  can fail

                        //System.Console.WriteLine("myConnection.Open()--> Success");
                        try_connect = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("connectCount=" + connectCount.ToString());
                        Console.WriteLine("Demo1.myConnection.Open() Failed: e=" + e.ToString());
                        Connect_FailureCount += 1;
                        Console.WriteLine("Connect_FailureCount=" + Connect_FailureCount.ToString());
                    }
                }// while (try_connect)

                //STEP 2: Fetch Data
                //Select the query text:  from TableCounter,  one row only where Row_ID=42

                OleDbDataReader myReader;
                try
                {
                    myReader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    //Logger.TraceMessage("Yikes! CONN Exception in Demo1.Start()");
                    Console.WriteLine("Yikes! CONN Exception in Demo1.Start()");
                    Console.WriteLine("Demo1.myCommand.ExecuteReader()  Failed: ex=" + ex.ToString());
                    Console.WriteLine("terminating early.");
                    Console.WriteLine("Pause before exit....");
                    Console.ReadKey();
                    return;
                }

                //STEP 3: Access the table, we expect only one record based upon the query
                if (myReader.Read() == false)
                {
                    Console.WriteLine("No records found.  exiting....");
                    Console.WriteLine("Pause before exit....");
                    Console.ReadKey();
                    return;
                }

                //Console.WriteLine("a record was read.");

                //Console.WriteLine("Access the current record content;  all five counter fields");
                Row_ID = Convert.ToInt32(myReader[0].ToString());  // TBD:  Watch out, there is no logic here to handle any conversion errors!!!
                Count1 = Convert.ToInt32(myReader[1].ToString());
                Count2 = Convert.ToInt32(myReader[2].ToString());
                Count3 = Convert.ToInt32(myReader[3].ToString());
                Count4 = Convert.ToInt32(myReader[4].ToString());
                Count5 = Convert.ToInt32(myReader[5].ToString());
                Count6 = Convert.ToInt32(myReader[6].ToString());
                Count7 = Convert.ToInt32(myReader[7].ToString());
                Count8 = Convert.ToInt32(myReader[8].ToString());
                Count9 = Convert.ToInt32(myReader[9].ToString());
                Count10 = Convert.ToInt32(myReader[10].ToString());
                Text1 = myReader[6].ToString();
                Text2 = myReader[7].ToString();
                //                                             {index[,alignment][:formatString]}
#if __SHOW_INPUT__
                Console.WriteLine(String.Format("Input:{0,5}, {1,5}, {2,5}, {3,5},{4,5},{5,5}, {6,5}, {7,5}, {8,5},{9,5}",                   
                Count1, Count2, Count3, Count4, Count5, Count6,Count7, Count8, Count9,Count10    ));
#endif 
                //Console.WriteLine("close the reader before trying the update");
                myReader.Close();

                //modify the counter field associated with this process ID
                Count += 1;
                string strCount = Count.ToString();
                string strRow_ID =  42.ToString();
    
                string strModifySQL = "Update TableCounter " + "SET " + strCountField + "=" + strCount + " where Row_ID=" + strRow_ID;
                ModifyRow(strConnect,  strModifySQL);

                //Console.WriteLine("Rows Modified=" + iRows.ToString());
                //Console.WriteLine("DONE:modify the database content");

                myConnection.Close();
            }// while (Count < intCountMax)

#if __DEMO_INSERT_MODIFY_DELETE__
            Demo_Insert_Delete();
#endif


            Summary_FailureCounts();

            //Logger.TraceMessage("Tracing --> DONE: Demo1.Start()");
            System.Console.WriteLine("DONE: Demo1.Start()");
        }//void Start()


        //---------------------------------------------------------------------------------------------------------
        //ModifyRow
        //---------------------------------------------------------------------------------------------------------
        public static void ModifyRow(string connectionString, string strModifySQL)
        {
            bool retry = true;

            while (retry)
            {
                using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
                {
                    OleDbCommand oleDbCommand = new OleDbCommand(strModifySQL);
                    oleDbCommand.Connection = oleDbConnection;
                    OleDbTransaction transaction = null;    //Define the transaction reference variable

                    try
                    {
                        oleDbConnection_Open_WithRetry(oleDbConnection);  //<--- encapsulate with a retry because Connection.Open()  can fail

                        // Start a local transaction 
                        transaction = oleDbConnection.BeginTransaction();  //create and initializet the transaction object
                        oleDbCommand.Transaction = transaction;             //associate the transaction object with the command object

                        oleDbCommand.ExecuteNonQuery();                    //Finally, execute the actual query, with the transaction 

                        // Commit the transaction.
                        transaction.Commit();                                        //no exceptions, so commit the transaction

                        oleDbConnection.Close();
                        retry = false;
                    }
                    catch (Exception ex) //get the error
                    {
                        // Roll back the transaction.
                        transaction.Rollback();                                    //we got an exception, so rollback the transaction

                        Modify_FailedCount += 1;
                        Console.WriteLine("ModifyRow failed!  Modify_FailedCount={0} \n", Modify_FailedCount);
                        //Console.WriteLine("Exception ex=" + ex.ToString());
                        //Console.WriteLine("Exception ex.GetType=" + ex.GetType().ToString());                
                    }
                }//using
            }//while
        }//ModifyRow

        //insert a random delay, milliseconds
        public static void random_delay(int iMinDelay_milliseconds, int iMaxDelay_milliseconds)
        {
            Random r = new Random();
            int iWait = r.Next(iMinDelay_milliseconds, iMaxDelay_milliseconds);
            Console.WriteLine("iWait=" + iWait.ToString());
            Thread.Sleep(iWait);
        }

        public static void oleDbConnection_Open_WithRetry(OleDbConnection oleDbConnection)
        {
            bool retry = true;
            int iRetryCount = 0;

            while (retry)
            {
                try
                {
                    oleDbConnection.Open();
                    //no exception, so keep going
                    retry = false;
                }
                catch (Exception ex) //get the error
                {
                    iRetryCount += 1;
                    iRetryTotalCount += 1;

                    Console.WriteLine("oleDbConnection_Open_WithRetry failed!  iRetryCount={0}, iRetryCount_Max={1}\n", iRetryCount, iRetryCount_Max);
                    //Console.WriteLine("Exception ex=" + ex.ToString());
                    //Console.WriteLine("Exception ex.GetType=" + ex.GetType().ToString());
                    if (iRetryCount > iRetryCount_Max)
                    {
                        iRetryCount_Max = iRetryCount;
                        Console.WriteLine("iRetryCount_Max={0}", iRetryCount_Max);
                    }
                    random_delay(100, 1000);
                }
            }//while (retry)
        } //oleDbConnection_Open_WithRetry

        //-------------------------------------------------------------------------------------------------
        //Demo_Insert_Delete
        //        Insert 20 rows
        //        pause to allow inspection of the database table, TableCounter
        //        and then Delete 10 of the new rows
        //-------------------------------------------------------------------------------------------------
        public static void Demo_Insert_Delete()
        {
            Demo_InsertRow();
            Console.WriteLine("pausing after Demo_InsertRow");
            Console.ReadKey();

            ModifyRowsParameterized( );
            Console.WriteLine("pausing after ModifyRowsParameterized");
            Console.ReadKey();

            Demo_DeletetRow();
            Console.WriteLine("pausing after Demo_DeletetRow");
            Console.ReadKey();
        }
        static int iBase = 500;

        //-------------------------------------------------------------------------------------------------
        //Demo_InsertRow
        //-------------------------------------------------------------------------------------------------
        public static void Demo_InsertRow()
        {
            Console.WriteLine("START: Demo_InsertRow");

            for (int i=0; i<20; i++)
            {
                //add row (i)
                int iRow_ID = iBase + i;
                String strInsertSQL = String.Format("Insert into TableCounter (Row_ID, Count1, Count2) values ({0},{1},{2})",     iRow_ID, i+1, i+2);

                InsertRow(strConnect, strInsertSQL);
            }
            Console.WriteLine("START: Demo_InsertRow");
        }

        //---------------------------------------------------------------------------------------------------------
        //InsertRow
        static int InsertFailedCount = 0;
        //---------------------------------------------------------------------------------------------------------
        public static void InsertRow(string connectionString, string strInsertSQL)
        {
            bool retry = true;
            Console.WriteLine("START: InsertRow");
            while (retry)
            {
                using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
                {
                    OleDbCommand oleDbCommand = new OleDbCommand(strInsertSQL);
                    oleDbCommand.Connection = oleDbConnection;

                    try
                    {
                        oleDbConnection.Open();
                        oleDbCommand.ExecuteNonQuery();
                        oleDbConnection.Close();
                        retry = false;
                    }
                    catch (Exception ex) //get the error
                    {
                        InsertFailedCount += 1;
                        Console.WriteLine("Insert failed!  InsertFailedCount={0} \n", InsertFailedCount);
                        //Console.WriteLine("Exception ex=" + ex.ToString());
                        //Console.WriteLine("Exception ex.GetType=" + ex.GetType().ToString());
                        Util.pause();
                    }
                }//using
            }//while
            Console.WriteLine("DONE:InsertRow ");
        }//InsertRow


        //---------------------------------------------------------------------------------------------------------
        //ModifyRowParameterized
        // https://msdn.microsoft.com/en-us/library/system.data.oledb.oledbcommand.parameters(v=vs.110).aspx
        //---------------------------------------------------------------------------------------------------------
        public static void ModifyRowsParameterized()
        {

            for (int i = 0; i < 20; i++)
            {
                int iRow_ID = iBase + i;
                int j = 100 + i;
 
 
   
                bool retry = true;
                Console.WriteLine("START: ModifyRowParameterized");
                while (retry)
                {
                    using (OleDbConnection oleDbConnection = new OleDbConnection(strConnect))
                    {
                        //Create the SQL Query having parameterized "?" placeholders associated with named fields.
                        string strModifySQL = String.Format("UPDATE TableCounter SET Count3 = ?  WHERE Row_ID = ?");

                        //Construct the command with parameters SQL
                        OleDbCommand oleDbCommand = new OleDbCommand(strModifySQL);

#region  Add SQL Query Parameters to the command object
                        //associate parameter values with the placeholders, associating Named Parameters with values
                        OleDbParameter oPar1 = new OleDbParameter("Count3", j);
                        OleDbParameter oPar2 = new OleDbParameter("Row_ID", iRow_ID);

                        OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter("Count3", j), new OleDbParameter("Row_ID", iRow_ID) };            

                        for (int iPar = 0; iPar < parameters.Length; iPar++)
                        {
                            oleDbCommand.Parameters.Add(parameters[iPar]);
                        }
#endregion   Add SQL Query Parameters to the command object

                        oleDbCommand.Connection = oleDbConnection;

                        try
                        {
                            oleDbConnection.Open();
                            oleDbCommand.ExecuteNonQuery();
                            oleDbConnection.Close();
                            retry = false;
                        }
                        catch (Exception ex) //get the error
                        {
                            ModifyParameterized_FailureCount += 1;
                            Console.WriteLine("ModifyRowsParameterized failed!  ModifyParameterized_FailureCount={0} \n", ModifyParameterized_FailureCount);
                            //Console.WriteLine("Exception ex=" + ex.ToString());
                            //Console.WriteLine("Exception ex.GetType=" + ex.GetType().ToString());
                            Util.pause();
                        }
                    }//using
                }//while


            }//for(int i
            Console.WriteLine("DONE:ModifyRowParameterized ");
        }//ModifyRowParameterized




        //-------------------------------------------------------------------------------------------------
        //Demo_DeletetRow
        //-------------------------------------------------------------------------------------------------

        public static void Demo_DeletetRow()
        {
            Console.WriteLine("START: Demo_DeletetRow");

            for (int i = 0; i < 10; i++)
            {
                //delete row (iBase+i)
                int iRow_ID = iBase + i;
                string strDeleteSQL = String.Format("Delete from TableCounter where Row_ID = {0}",  iRow_ID.ToString());
                DeleteRow(strConnect, strDeleteSQL);
            }
            Console.WriteLine("START: Demo_DeletetRow");
        }

        //---------------------------------------------------------------------------------------------------------
        //DeleteRow

        //---------------------------------------------------------------------------------------------------------
        public static void DeleteRow(string connectionString, string strDeleteSQL)
        {
            bool retry = true;

            while (retry)
            {
                using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
                {
                    OleDbCommand oleDbCommand = new OleDbCommand(strDeleteSQL);
                    oleDbCommand.Connection = oleDbConnection;

                    try
                    {
                        oleDbConnection.Open();
                        oleDbCommand.ExecuteNonQuery();
                        oleDbConnection.Close();
                        retry = false;
                    }
                    catch (Exception ex) //get the error
                    {
                        Delete_FailureCount += 1;
                        Console.WriteLine("Delete failed!  Delete_FailureCount={0} \n", Delete_FailureCount);
                       // Console.WriteLine("Exception Ex={0}", ex.ToString());
                    }
                }//using
            }//while
        }//DeleteRow

    }//class Demo1
}//namespace
