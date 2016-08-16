using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using ADODB;

namespace ConAppADONET_StressTest
{
    public class General
    {

        public static ADODB.Connection CONN = null; // global DB variables
        public static ADODB.Recordset RS = null;

        public static string gConnStr = String.Empty; // global DB Connect String
        public static string gApplicationPath = String.Empty;
        public static string gDatabasePath = String.Empty;


        public static void Initialize()
        {
            Console.WriteLine("START: General.Initialize()");

            // database connection path
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            gApplicationPath = Path.GetDirectoryName(assemblyLocation);

            gDatabasePath = gApplicationPath + "\\assess.mdb";
            gConnStr = "Provider = Microsoft.Jet.OLEDB.4.0;Data Source = " + gDatabasePath;

            Console.WriteLine("START: General.Initialize()");
        }
    }
}
