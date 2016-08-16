
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConAppADONET_StressTest
{

    class eQuit : Exception { };

    class Program
    {
   
       public static void Main(string[] args)
        {
            System.Console.WriteLine("START: Main()");
            DateTime StartTime_Now = DateTime.Now;

            General.Initialize();




            foreach (String sArg in args)
            {
                Console.WriteLine("arg=" + sArg);
            }

            if (args.Length != 3)
            {
                Console.WriteLine("wrong number of parameters:  Count =" + args.Length.ToString());
                return;
            }

            int ID = Convert.ToInt32(args[0]);
            int MaxID = Convert.ToInt32(args[1]);
            int CountMax = Convert.ToInt32(args[2]);

            Console.WriteLine("ID=" + ID.ToString());
            Console.WriteLine("MaxID=" + MaxID.ToString());
            Console.WriteLine("CountMax=" + CountMax.ToString());
  

            Demo1.Start(ID, MaxID, CountMax);
  
            DateTime EndTime_Now = DateTime.Now;
            TimeSpan dt = EndTime_Now.Subtract(StartTime_Now);
            Console.WriteLine(dt.ToString());
            Util.pause();

            System.Console.WriteLine("-----------------------------------");

            //Demo1.InsertRecords();


            System.Console.WriteLine("DONE: Main()");

            Util.pause();
        }//void Main()

    }//class Program

}//namespace