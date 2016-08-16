using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConAppADONET_StressTest
{
    public class Util
    {
        public static bool RecordSetIsEmpty(ADODB.Recordset rst)
        {
            return (rst.BOF & rst.EOF);  // record set is empty if both BOF and EOF are true simultaneously
        }

        public static void pause()
        {
            Console.WriteLine("pausing...");
            Console.ReadKey();
        }//void pause()

    }//class Util
}//namespace
