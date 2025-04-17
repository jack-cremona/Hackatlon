using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace hackatlon
{
    /*
     * CONSIDERAZIONI
     * 1) la distanza massima andrebbe parametrizzata e chiesta all'inizio                  ESEGUITO
     * 2) il calcolo del pedaggio lo faccio in un metodo che ritorna un double              ESEGUITO
     * 3) parametrizzare tutti i tempi di attesa con costanti ma senza chiederle            ESEGUITO
     */
    internal class Program
    {
        static string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileDiLog.txt");
        static Random r = new Random();
        static Mutex Libero = new Mutex();
        static int numMacchineEntrate = 0;
        static int limiteMacchinePassate;   //numero massimo di macchine
        static int MaxKm;   //kilometri massimi

        //si possono chiedere
        #region Costanti di attesa      
        static int Telepass = 3000;
        static int Contanti = 5000;
        static int MezziPesanti = 8000;
        #endregion

        #region Costanti per pagamenti
        static double PrezzoBase = 0.50;
        #endregion

        static void Main(string[] args)
        {
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            List<Thread> Caselli = new List<Thread>();
            Console.Write("Quanti caselli vuoi inserire? ");
            int numCaselli;
            int.TryParse(Console.ReadLine(), out numCaselli);

            Console.Write("Quante macchine vuoi fare passare? ");
            int.TryParse(Console.ReadLine(), out limiteMacchinePassate);

            Console.Write("quanti km si possono fare al massimo? ");
            int.TryParse(Console.ReadLine(), out MaxKm);

            for (int i = 0; i < numCaselli; i++)
            {
                int index = i;
                Caselli.Add(new Thread(() => Casello(index)));
            }

            foreach (Thread thread in Caselli)
            {
                thread.Start();
            }
        }

        static double CalcoloPedaggio(int numKm, int TipoVeicolo)
        {
            return PrezzoBase + (PrezzoBase * TipoVeicolo) + (PrezzoBase * (numKm / 10));
        }

        //lo switch con 3 condizioni ci mette la metà del tempo --> https://code-maze.com/csharp-comparing-performance-of-the-switch-and-if-else-statements/
        static int ReturnTempo(int tipoVeicolo)
        {
            switch (tipoVeicolo)
            {
                case 1:
                    return Contanti;
                case 2:
                    return Telepass;
                case 3:
                    return MezziPesanti;
                default:
                    return 0;
            }
        }

        static void Scrittore(string s)
        {
            Libero.WaitOne();
            StreamWriter scrittore = new StreamWriter("FileDiLog.txt", true);
            scrittore.WriteLine(s);
            scrittore.Close();
            Libero.ReleaseMutex();
        }

        static void Casello(int numeroCasello)
        {
            Stopwatch cronometro = new Stopwatch();
            while (true)
            {
                if (numMacchineEntrate >= limiteMacchinePassate)
                    return;
                cronometro.Reset();
                cronometro.Start();
                Libero.WaitOne();
                numMacchineEntrate++;
                Libero.ReleaseMutex();

                int tipoVeicolo = r.Next(1, 4);
                int tipoPagamento;

                if (tipoVeicolo == 3)
                    tipoPagamento = 3;
                else
                    tipoPagamento = r.Next(1, 3);
                int kmPercorsi = r.Next(10, MaxKm);
                string s = string.Format("ORE {1} --> veicolo entrato nel casello {0}", numeroCasello, DateTime.Now.ToString("h:mm:ss.ff t"));
                Console.WriteLine(s);
                Scrittore(s);

                //pagamento

                Thread.Sleep(ReturnTempo(tipoPagamento));
                string st = string.Format("CASELLO {0} ORE {4} --> tipo di veicolo: {1} km percorsi {2} prezzo pagato {3} euro", numeroCasello, tipoVeicolo, kmPercorsi, CalcoloPedaggio(tipoVeicolo, kmPercorsi), DateTime.Now.ToString("h:mm:ss.ff t"));
                Console.WriteLine(st);
                Scrittore(st);

                cronometro.Stop();
                string tempo = cronometro.ElapsedMilliseconds.ToString();

                //STAMPA FINE

                string stringaFinale = string.Format("CASELLO {0} ORE {1} --> il veicolo ha liberato il casello. il suo tempo è stato di {2} millisecondi", numeroCasello, DateTime.Now.ToString("h:mm:ss.ff t"), tempo);
                Console.WriteLine(stringaFinale);
                Scrittore(stringaFinale);
            }
        }
    }
}