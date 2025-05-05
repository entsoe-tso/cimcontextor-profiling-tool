using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using System.Threading;
/*************************************************************************\
***************************************************************************
* Product CimContextor       Company : Zamiren (Joinville-le-Pont France) *
*                                      Entsoe (Bruxelles Belgique)        *
***************************************************************************
***************************************************************************                     
* Version : 2.9.4                                         *  october 2019 *
*                                                                         *
***************************************************************************
*                                                                         *
*       Credit to:  Sebastien Maligue-Clausse                             *
*                   Andre Maizener   andre.maizener@zamiren.fr            *
*                   Jean-Luc Sanson  jean-luc.sanson@noos.fr              *
*                                                                         *
*       Contact: +33148854006                                             *
*                                                                         *
***************************************************************************

**************************************************************************/
namespace CimContextor.utilitaires
{
   public  class Ta
           {
        public static TextWriter timelog; //  the report file
        public static Dictionary<string, Stopwatch> dicSW = new Dictionary<string, Stopwatch>();// donne les stopwatchs lances
        readonly Stopwatch stopw = new Stopwatch();
        long elapst = 0;
        string code = "";
        TimeSpan ts = new TimeSpan(1);
        //Démarrage du chrono 
      public  Ta()
        {
            if (!Main.testperf) return;
            if (timelog == null)//am aug 2018
            {
                // string filepath = "";
                //  filepath = System.IO.Path.GetDirectoryName(Main.Repo.ConnectionString);
                // if (!File.Exists(filepath + "\\" + "CimContextor-Config.xml"))
                // {
                //    filepath = Environment.GetEnvironmentVariable("APPDATA");// am march  2017
                // }
                string filepath = Utilities.FileManager.GetParentDirPath(); // ABA20201020
                timelog = new StreamWriter(filepath + "\\timelog.log");
            }
        stopw = new Stopwatch(); 
        }

       /// <summary>
       /// cree un stopwatch avec code  en demarrant tout de suite le decompte
       /// </summary>
       /// <param name="code"></param>
      public Ta(string code)
      {
          if (!Main.testperf) return;
          if (timelog == null)//am aug 2018
          {
              // string filepath = Environment.GetEnvironmentVariable("APPDATA");// am march  2
              string filepath = Utilities.FileManager.GetParentDirPath(); // ABA20201020
              timelog = new StreamWriter(filepath + "\\timelog.log");
          }
          stopw = new Stopwatch();
          this.code = code;
          this.elapst = 0;
          stopw.Start();
          timelog.WriteLine("TimeAnalysis Start with code=" + code + " elapsedTime=0");
      }

    public   void  start(string code)
       {
         if (!Main.testperf) return;
         this.code = code;
         this.elapst = 0;
         stopw.Start();
         timelog.WriteLine("TimeAnalysis Start with code=" + code + " elapsedTime=0");
         //ts = new TimeSpan(1000);

       }
    public void start(StackFrame fr)
    {
        if (!Main.testperf) return;
        this.code = fr.GetMethod().ToString() + " dans " + Path.GetFileName(fr.GetFileName());
        this.elapst = 0;
        timelog.WriteLine("TimeAnalysis Start for" + code + " elapsedTime=0");
        stopw.Start();
        ts = new TimeSpan(1000);

    }
    public void restart(string code)
    {
        if (!Main.testperf) return;
        this.code = code;
        timelog.WriteLine(" Restart with code=" + code + " elapsedTime =" + stopw.ElapsedMilliseconds.ToString());
        stopw.Reset();
        start(this.code);

    }
    public void restart(StackFrame fr)
    {
        if (!Main.testperf) return;
        this.code=fr.GetMethod().ToString() + " dans " + Path.GetFileName(fr.GetFileName());
        timelog.WriteLine("TimeAnalysis reStart for" + code + " elapsedTime =" + stopw.ElapsedMilliseconds.ToString());
        stopw.Reset();
        start(new StackFrame(0, true));

    }
     public   long stop()
        {
            if (!Main.testperf) return 0;
            stopw.Stop();
            //On récupère la durée écoulé, en millisecondes 
            long milliseconds = stopw.ElapsedMilliseconds;
            elapst = milliseconds;
              timelog.WriteLine("stop elapse time for " + code +"=" + milliseconds.ToString() +" ms");
            return milliseconds;
        }
   /// <summary>
   /// gives the elapsedtime at a given step since last start or restart
   /// update elapst with current elapsedtime
   /// </summary>
   /// <param name="step"></param>
   /// <returns></returns>
   ///
     /// <summary>
     /// gives the difference between elapst and current  elapsedtime at a given step
     /// update elapst with current elapsedtime
     /// <param name="step"></param>
     /// <returns></returns>
     ///
    public   long elapsetime(string step)
        {
         if (!Main.testperf) return 0;
         long ret=stopw.ElapsedMilliseconds;
         elapst = ret;
         timelog.WriteLine("elapsetime for " + step + "=" + ret.ToString() + " ms");
         return ret; 
        }
    public long diffelaps(string step)
    {
        if (!Main.testperf) return 0;
        long ret = stopw.ElapsedMilliseconds-elapst;
        timelog.WriteLine("diffelaps for " + step + "=" + ret.ToString() + " ms");
        elapst = stopw.ElapsedMilliseconds;
        return ret;
    }
       public void reset()
    {
        if (!Main.testperf) return ;
        long milliseconds = stopw.ElapsedMilliseconds;
           timelog.WriteLine( " reset  processing time for " + code + "=" + milliseconds.ToString() + " ms");
        
        stopw.Reset();
    }
       public void quit()
       {
           if (!Main.testperf) return ;
           long milliseconds = stopw.ElapsedMilliseconds;
           timelog.WriteLine(" quit  processing time for editconnectors RDFS =" + milliseconds.ToString() + " ms");
           timelog.Flush();
           timelog.Close();
           timelog = null;
       }
       public bool isRunning()
       {
           return stopw.IsRunning;
       }
       public ArrayList startnew(string step)
       {         
          ArrayList ret=new ArrayList();
           timelog.WriteLine(" startnew  processing time for " + step + " =0 ms");
           ret[0] = Stopwatch.StartNew();
           ret[1] = step;
           dicSW[step]=(Stopwatch)ret[0];
           return ret;
       }
     public void stopnew(string step)
       {
           long milliseconds = dicSW[step].ElapsedMilliseconds;
           timelog.WriteLine(" stopnew  processing time for " + step + " = " +  milliseconds.ToString() +" ms");
           dicSW[step].Stop();
       }

}
}
