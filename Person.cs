using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Sockets;

namespace TjuvOchPolis3
{
    public abstract class Person
    {
        public int Xposition { get; set; }
        public int Yposition { get; set; }
        public int Xdirection { get; set; }
        public int Ydirection { get; set; }

        public static int Wallet { get; set; }
        public static int Keys { get; set; }
        public static int Money { get; set; }
        public static int CellPhone { get; set; }

        public int[] Possessions = {Wallet, Keys, Money, CellPhone };
        public int[] Swag = { Wallet, Keys, Money, CellPhone };
        public int[] Evidence = { Wallet, Keys, Money, CellPhone };

        public bool Prisoner { get; set; }
        public int TimeServed { get; set; }
    }

    public class UpstandingCitizen : Person
    {
        public UpstandingCitizen(int xposition, int yposition, int xdirection, int ydirection)
        {
            Xposition = xposition;
            Yposition = yposition;
            Xdirection = xdirection;
            Ydirection = ydirection;
            Possessions[0] = 1;
            Possessions[1] = 1;
            Possessions[2] = 1;
            Possessions[3] = 1;

        }
    }

    public class Thief : Person
    {
        public Thief(int xposition, int yposition, int xdirection, int ydirection)
        {
            Xposition = xposition;
            Yposition = yposition;
            Xdirection = xdirection;
            Ydirection = ydirection;

            TimeServed = 0;
            Swag[0] = 0;
            Swag[1] = 0;
            Swag[2] = 0;
            Swag[3] = 0;
        }
    }

    public class Police : Person
    {
        public Police(int xposition, int yposition, int xdirection, int ydirection)
        {
            Xposition = xposition;
            Yposition = yposition;
            Xdirection = xdirection;
            Ydirection = ydirection;

            Evidence[0] = 0;
            Evidence[1] = 0;
            Evidence[2] = 0;
            Evidence[3] = 0;
        }
    }

    public class Inventory : Person
    {
        public Inventory(int wallet, int keys, int money, int cellphone)
        {
            Wallet = wallet;
            Keys = keys;
            Money = money;
            CellPhone = cellphone;
        }
    }
    
    public class Prison 
    {
        public DateTime TimeOfArrest { get; set; }
        public DateTime TimeServed { get; set; }
        public DateTime TimeToServe { get; set; }
        public Prison(DateTime datetime)
        {
            TimeOfArrest = datetime;
            TimeServed = TimeOfArrest;
            TimeToServe = TimeOfArrest.AddSeconds(30);
        }
    }
}
