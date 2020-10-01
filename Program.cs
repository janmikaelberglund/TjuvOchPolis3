using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

namespace TjuvOchPolis3
{
    class Program
    {
        // Settings
        public static int number_of_policemen = 10;
        public static int number_of_thieves = 20;
        public static int number_of_upstandingCitizens = 30;
        public static int pauseTimePerCycle = 0;
        public static int pauseAtEvent = 2000;
        // 

        public static List<Prison> prison = new List<Prison>();

        static void Main(string[] args)
        {

            Console.WriteLine("Om programmet körs för snabbt kan du ändra steghasigheten, rek. 0-200ms");
            pauseTimePerCycle = int.Parse(Console.ReadLine());

            Console.WindowHeight = 45;
            Console.WindowWidth = 120;

            int[] mapSize = { 25, 100 }; // X-axel, Y-axel

            List<Person> citizens = new List<Person>();
            AddCitizens(citizens, mapSize);

            Console.Clear();
            Console.WriteLine("Tjuv och polis:");

            while (programIsrunning)
            {
                Console.SetCursorPosition(0, 1);
                PrintToConsole(mapSize, citizens);
                Thread.Sleep(pauseTimePerCycle);
                Movement(mapSize, citizens);
                Meeting(citizens);
            }
        }



        private static void Meeting(List<Person> citizens) //Kontrollerar om en tjuv står på samma plats som en medborgare eller polis och rånar/arresterar.
        {
            foreach (Person person in citizens)
            {
                if (person is Thief)
                {
                    IsFreePerson(person.Xposition, person.Yposition, citizens);
                    if (thief && upstandingCitizen)
                    {
                        if ((citizens[temporaryIndexUpstandingCitizen].Possessions[0] 
                             + citizens[temporaryIndexUpstandingCitizen].Possessions[1] 
                             + citizens[temporaryIndexUpstandingCitizen].Possessions[2] 
                             + citizens[temporaryIndexUpstandingCitizen].Possessions[3]) 
                             > 0)
                        {
                            int rngsteal = rng.Next(0, 3);
                            citizens[temporaryIndexUpstandingCitizen].Possessions[rngsteal] --;
                            ((Thief)person).Swag[rngsteal]++;
                            thief_upstandingCitizen = true;
                            robberies++;
                        }
                        else
                        {
                            attemptedRobberies++;
                        }
                    }

                    if (thief && police)
                    {
                        if ((((Thief)person).Swag[0] + ((Thief)person).Swag[1]
                            + ((Thief)person).Swag[2] + ((Thief)person).Swag[3]) > 0)
                        {
                            citizens[temporaryIndexPolice].Evidence[0] += ((Thief)person).Swag[0];//Lägger tjuvens stöldgods i polisens inventory
                            citizens[temporaryIndexPolice].Evidence[1] += ((Thief)person).Swag[1];
                            citizens[temporaryIndexPolice].Evidence[2] += ((Thief)person).Swag[2];
                            citizens[temporaryIndexPolice].Evidence[3] += ((Thief)person).Swag[3];
                            ((Thief)person).Swag[0] = 0; //tömmer tjuvens inventory
                            ((Thief)person).Swag[1] = 0;
                            ((Thief)person).Swag[2] = 0;
                            ((Thief)person).Swag[3] = 0;
                            thief_police = true;
                            arrests++;
                            ((Thief)person).Prisoner = true;
                            prison.Add(new Prison(DateTime.Now)); //Sätter tjuven i fängelse-listen vid tidpunkten polisen haffar denne.
                        }
                    }
                }
            }
        }



        private static void Movement(int[] mapSize, List<Person> citizens) //Flyttar varje person ett steg i sin redan bestämda riktning
                                                                           //och ser till att de hamnar på andra sidan staden om de går utanför kanten.
        {
            thief_upstandingCitizen = false;
            thief_police = false;

            foreach (Person person in citizens)
            {
                if (person is Thief && person.Prisoner && !person.Prisoner)
                {

                }
                person.Yposition += person.Ydirection;
                person.Xposition += person.Xdirection;

                if (person.Yposition == -1)
                {
                    person.Yposition = mapSize[1] - 1;
                }
                else if (person.Yposition == mapSize[1])
                {
                    person.Yposition = 0;
                }
                if (person.Xposition == -1)
                {
                    person.Xposition = mapSize[0] - 1;
                }
                else if (person.Xposition == mapSize[0])
                {
                    person.Xposition = 0;
                }
            }
        }

        private static void AddCitizens(List<Person> citizens, int[] mapSize) //Skapar alla tjuvar/poliser/vanliga medborgare och ger dem en position på kartan.
        {
            for (int i = 0; i < number_of_upstandingCitizens; i++)
            {
                NewDirection();
                citizens.Add(new UpstandingCitizen(rng.Next(1, mapSize[0]), rng.Next(1, mapSize[1]), xdirection, ydirection));
            }
            for (int i = 0; i < number_of_thieves; i++)
            {
                NewDirection();
                citizens.Add(new Thief(rng.Next(1, mapSize[0]), rng.Next(1, mapSize[1]), xdirection, ydirection));
            }
            for (int i = 0; i < number_of_policemen; i++)
            {
                NewDirection();
                citizens.Add(new Police(rng.Next(1, mapSize[0]), rng.Next(1, mapSize[1]), xdirection, ydirection));
            }
        }



        private static void NewDirection() //Skapar en riktning samt ser till att riktningen inte är stillastående dvs 0,0.
        {
            xdirection = rng.Next(-1, 1 + 1);
            ydirection = rng.Next(-1, 1 + 1);
            while (xdirection == 0 && ydirection == 0)
            {
                xdirection = rng.Next(-1, 1 + 1);
                ydirection = rng.Next(-1, 1 + 1);
            }
        }


        private static void PrintToConsole(int[] mapSize, List<Person> citizens) //Ritar ut det som syns i konsollen.
        {
            for (int x = 0; x < mapSize[0]; x++) //Ritar ut kartan och varje person.
            {
                for (int y = 0; y < mapSize[1]; y++)
                {
                    IsFreePerson(x, y, citizens);
                    if ((upstandingCitizen && thief) || (police && thief))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("X");
                    }
                    else if (upstandingCitizen)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("M");
                    }
                    else if (thief)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.Write("T");
                    }
                    else if (police)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("P");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }


            Console.WriteLine($"Antal rån:           {robberies}");
            Console.WriteLine($"Antal rånförsök:     {attemptedRobberies}");
            Console.WriteLine($"Antal arresteringar: {arrests}");
            Console.WriteLine();

            // Skriver ut alla som sitter i fängelset och hur längde de har setat inne.
            Console.WriteLine("Fängelse: "); 
            foreach (var thief in prison)
            {
                Console.WriteLine($"Denna fånge har setat {ShowSecondsServed(DateTime.Now.Second, ((Prison)thief).TimeServed.Second)} sekunder i fängelset.");
            }

            // Skriver ut om en tjuv stöter på någon.
            if (thief_police || thief_upstandingCitizen) 
            {
                Console.SetCursorPosition(0, 40);
                if (thief_upstandingCitizen)
                {
                    Console.WriteLine("Tjuv har rånat en medborgare");
                }
                if (thief_police)
                {
                    Console.WriteLine("Polis har fångat en tjuv");
                }
                Thread.Sleep(pauseAtEvent);
                Console.SetCursorPosition(0, 40);
                Console.WriteLine("                                                             ");
                Console.WriteLine("                                                             ");
                Console.WriteLine("                                                             ");

                //Skriver om en fånge frisläpps och rensar fängelselistan
                if (Prison())
                {
                    Console.SetCursorPosition(0, 39);
                    Console.WriteLine("En fånge har släppts från fängelset.");
                    Thread.Sleep(pauseAtEvent);
                    Console.SetCursorPosition(0, 31);
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                    Console.WriteLine("                                                             ");
                }
            }
        }

        private static void IsFreePerson(int x, int y, List<Person> citizens) //Kontrollerar om och vilken typ av person som finns på den aktuella platsen: x, y
                                                                              //Samt om personen är en fri person eller sitter i fängelset.
        {
            thief = false;
            upstandingCitizen = false;
            police = false;

            foreach (Person person in citizens)
            {
                if (person is UpstandingCitizen)
                {
                    if (x == ((UpstandingCitizen)person).Xposition && y == ((UpstandingCitizen)person).Yposition)
                    {
                        temporaryIndexUpstandingCitizen = citizens.IndexOf(person);
                        upstandingCitizen = true;
                    }
                }
                if (person is Thief && !person.Prisoner)
                {
                    if (x == ((Thief)person).Xposition && y == ((Thief)person).Yposition)
                    {
                        thief = true;
                    }
                }
                if (person is Police)
                {
                    if (x == ((Police)person).Xposition && y == ((Police)person).Yposition)
                    {
                        temporaryIndexPolice = citizens.IndexOf(person);
                        police = true;
                    }
                }
            }
        }

        private static bool Prison() //Räknar fångarnas fängelsetid samt tar bort fångar från fängelset och returnar true om någon frisläpps
        {
            bool releaseFromPrison = false;
            int[] prisonersToBeRelased = new int[10];

            int i = 0;
            foreach (var thief in prison)
            {
                if (((Prison)thief).TimeToServe <= DateTime.Now)
                {
                    releaseFromPrison = true;
                    prisonersToBeRelased[i] = prison.IndexOf(thief) + 1;
                    i++;
                }
            }
            foreach (var prisoner in prisonersToBeRelased)
            {
                if (prisoner != 0)
                {
                    prison.RemoveAt(prisoner - 1);
                }
            }
            if (releaseFromPrison)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int ShowSecondsServed(int timeNow, int timeServed) //Räknar hur länge någon setat i fängelset.
        {
            if ((timeNow - timeServed) > -1)
            {
                return timeNow - timeServed;
            }
            else
            {
                return timeNow + 60 - timeServed;
            }
        }


        public static bool programIsrunning = true;
        public static int ydirection = 0;
        public static int xdirection = 0;
        public static Random rng = new Random();
        public static bool thief = false;
        public static bool upstandingCitizen = false;
        public static bool police = false;
        public static bool thief_upstandingCitizen = false;
        public static bool thief_police = false;
        public static int arrests;
        public static int robberies;
        public static int temporaryIndexUpstandingCitizen;
        public static int temporaryIndexPolice;
        public static int attemptedRobberies;
    }
}
