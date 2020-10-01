﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;

namespace TheifAndPolice
{
    public class City
    {
        private int Height { get; set; } //no work with accessors.
        private int Width { get; set; }
        public List<Person> PeopleInCity { get; set; }
        public string Event { get; set; }
        public bool Collision { get; set; }
        public int TotalArrests { get; set; }
        public int TotalStolen { get; set; }
        public static List<Person> Prison { get; } = new List<Person>();


        public City(int height, int width) //when a city is generated entered height n width gets "saved" into a property. And all people gets created.
        {
            Height = height;
            Width = width;
            PeopleInCity = Person.CreatePeople(height, width);
            RunCity();
        }
        public void RunCity() //controls the loop that pauses and moves people.
        {
            
            while (true)
            {
                Console.CursorVisible = false;
                DrawCitySize();
                Console.Clear();
                foreach (var p in PeopleInCity)
                {
                    p.Move(Height, Width);
                }


            }
        } 
        public void CheckCollision() //if a collision gets detected the sim pauses for 2sec
        {
            Collision = false;
            GetCollisions();

            if (Collision)
            {
                Console.WriteLine(Event);
                Thread.Sleep(2000);
            }
            else
            {
                Thread.Sleep(300);
            }
        }
        public void GetCollisions() //two loops that compares if "different" people meet. the interaction between thief n police and thief and citizen only matters
        {
            Event = ""; //string that becomes empty beefore any comparison is made, and then adds on depending of what happends.
            Random random = new Random();
           

            foreach (Person person in PeopleInCity)
            { 

                foreach (var personType in PeopleInCity)
                {
                    if (personType.GetType().Name == "Thief" && personType.InPrison!=true && person.GetType().Name =="Citizen")
                    {
                        if (personType.PositionX==person.PositionX && personType.PositionY==person.PositionY)
                        {

                            if (person.Inventory.Count>0)
                            {
                                int randItem = random.Next(0, person.Inventory.Count);
                                person.Inventory.RemoveAt(randItem);
                                TotalStolen++;
                                personType.Inventory.Add((Item)randItem);
                                Event += "Thief robs citizen of : " + (Item)randItem + "!" + Environment.NewLine;
                            }
                            else
                            {
                                Event += "Thief tried to rob citizen but found nothing of value..." + Environment.NewLine;
                            }

                            Collision = true;
                        }
                    }
                    if (personType.GetType().Name == "Thief" && personType.InPrison!=true && person.GetType().Name == "Police")
                    {
                        string builder = "";
                        if (personType.PositionX == person.PositionX && personType.PositionY == person.PositionY)
                        {
                            TotalArrests++;
                            if (personType.Inventory.Count > 0)
                            {
                                while (personType.Inventory.Count > 0)
                                {
                                    int randDDRY = random.Next(0, personType.Inventory.Count);
                                    personType.Inventory.RemoveAt(randDDRY);
                                    person.Inventory.Add((Item)randDDRY);
                                    builder += " " + ((Item)randDDRY) + " "; //DO 'don't repeat code'
                                }
                                Event += "Police arrests thief and confiscates:" + builder + ".";
                            }
                            else
                            {
                                Event += "Police arrests thief.";
                            }
                            Event += " Thief gets sent of to jail!" + Environment.NewLine;
                            //send personType to jail.
                            Collision = true;
                            HandlePrison(personType);
                        }
                    }
                    
                }
             
            }
           
        }
    

        public void DrawCitySize()
        {

            char[,] array2d = new char[Height, Width];
            Console.WriteLine("Number of thieves in prison: " + Prison.Count);

            Console.WriteLine($"Total item stolen: {TotalStolen}".ToUpper());
            Console.WriteLine($"Number of arrests: {TotalArrests}".ToUpper());
            for (int y = 0; y < array2d.GetLength(0); y++) 
            {
                for (int x = 0; x < array2d.GetLength(1); x++) 
                {


                    foreach (var p in PeopleInCity)
                    {
                        if (p is Thief && p.InPrison != true && p.PositionX == x && p.PositionY == y)
                        {
                            if (array2d[y, x] == '\0')
                            {
                                array2d[y, x] = p.Symbol;
                            }
                            else array2d[y, x] = 'X';

                        }
                        if (p is Police && p.PositionX == x && p.PositionY == y)
                        {
                            if (array2d[y, x] == '\0')
                            {
                                array2d[y, x] = p.Symbol;
                            }
                            else array2d[y, x] = 'X';
                        }
                        if (p is Citizen && p.PositionX == x && p.PositionY == y)
                        {
                            if (array2d[y, x] == '\0')
                            {
                                array2d[y, x] = p.Symbol;
                            }
                            else array2d[y, x] = 'X';

                        }

                    }
                    if (x == 0 || y == 0 || x == array2d.GetLength(1) - 1 || y == array2d.GetLength(0) - 1)
                    {
                        array2d[y, x] = '*';
                        Console.Write(array2d[y, x]);
                    }

                    else
                    {

                        Console.Write(array2d[y, x]);

                    }

                }
                Console.WriteLine();
            }
            CheckCollision();


        }
        public void HandlePrison(Person arrested) //no timefunctions added yet
        {
            arrested.InPrison = true;
            Prison.Add(arrested);
        }
    }
}
