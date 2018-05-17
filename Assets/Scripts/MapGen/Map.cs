using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Map //: MonoBehaviour
{

    public int width = 0;
    public int height = 0;
    public int[,] mapArray;
    public int generationChanceinPercent = 50;

    public Vector2 entrance;
    public Vector2 exit;

    static int floorID =        0;
    static int wallID =         1;
    static int exitID =         333;


    // Entrance = 332
    // Exit = 333
    
    // 0 = floor
    // 1 = wall

    public Map(int width, int height, int generationChanceInPercent, int numberOfIterations)
    {
        this.width = width;
        this.height = height;
        this.generationChanceinPercent = generationChanceInPercent;
        mapArray = new int[width, height];
        

        InitialiseMap();
        IterateMap(numberOfIterations);
        createEntranceAndExit();
    }

    void InitialiseMap()
    {
        System.Random rnd = new System.Random(System.DateTime.Now.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || y == height - 1 || x == width - 1)
                {
                    mapArray[x, y] = 1;
                }
                else
                {
                    if (rnd.Next(0, 100) < generationChanceinPercent)
                    {
                        mapArray[x, y] = 1;
                    }
                    else
                    {
                        mapArray[x, y] = 0;
                    }
                }
            }
        }
    }

    void IterateMap(int numberOfIterations)
    {
        for (int i = 0; i < numberOfIterations; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    int neighbourWallTiles = countNeighbours(x, y);

                    if (neighbourWallTiles > 4)
                        mapArray[x, y] = wallID;
                    else if (neighbourWallTiles < 4)
                        mapArray[x, y] = floorID;
                }
            }
        }
    }

    int countNeighbours(int xPos, int yPos)             // Spočítej okolní políčka
    {
        int neighbourCount = 0;
        for (int x = xPos - 1; x <= xPos + 1; x++)
        {
            for (int y = yPos - 1; y <= yPos + 1; y++)
            {
                if (x < 0 || y < 0 || y > height - 1 || x > width - 1)
                {
                    neighbourCount++;
                }
                else
                {
                    if (!(x == xPos && y == yPos))
                    {
                        if (mapArray[x, y] == 1)
                        {
                            neighbourCount++;
                        }
                    }
                }
            }
        }
        return neighbourCount;
    }


    void createEntranceAndExit()
    {
        System.Random rnd = new System.Random(DateTime.Now.GetHashCode());


        entrance = new Vector2();                           // vygeneruj vchod
        bool entranceGenerated = false;
        while (entranceGenerated == false)
        {
            int randomX = rnd.Next(2, width-2);
            int randomY = rnd.Next(2, height-2);
            if (mapArray[randomX, randomY] == 0)            // pokud je na náhodných souřadnicích podlaha
            {
                entranceGenerated = true;
                entrance = new Vector2(randomX, randomY);   // pak přiřaď do vektoru náhodnou pozici
            }
        }

        exit = new Vector2();                               // vygeneruj exit
        bool exitGenerated = false;
        while (exitGenerated == false || exit == entrance)  // dělej dokud je prázdný || pokud je exit == entrance
        {
            int randomX = rnd.Next(2, width-2);
            int randomY = rnd.Next(2, height-2);
            if (mapArray[randomX, randomY] == 0)            // pokud je na náhodných souřadnicích podlaha
            {
                exitGenerated = true;
                exit = new Vector2(randomX, randomY);       // pak přiřaď do vektoru náhodnou pozici
            }
        }

        Debug.Log("Map: Checking if exit and entrance are connected.");
        if (checkIfConnected(entrance, exit, mapArray) == false)
        {
            Debug.Log("Map: Nope, they are not conected.");
            createEntranceAndExit();                        // !!rekurzivní volání sama sebe může skončit smyčkou
        }

        //mapArray[(int)entrance.x, (int)(entrance.y)] = entranceID;
        mapArray[(int)exit.x, (int)exit.y] = exitID;


    }

    // Využiji "flooding" algoritmu
    Boolean checkIfConnected(Vector2 entranceVector, Vector2 exitVector, int[,] array)
    {
        int f = 1024;                                       // tuto hodnotu budu používat pro flooding

        int[,] tempArray = new int[width, height];          // vytvoř array o stejné velikosti jako mapArray
        Array.Copy(mapArray, tempArray, mapArray.Length);   // kdybych dal jednoduše =, nedojde ke zkopírování, ale jen se předá reference

        tempArray[(int)entrance.x, (int)entrance.y] = f;    // na místo vchodu vlož f

        int counter = 0;                                    // počítadlo kroků
        int maxCounter = width * height;                    // opakuj maximálně width*height
        while (tempArray[(int)exitVector.x, (int)exitVector.y] != f && counter < maxCounter)    // prováděj dokud není zaplavený i východ && dokud je counter < maxCounter
        {
            counter++;                                      // iteruj counter
            for (int x = 1; x < (width-1); x++)
            {
                for (int y = 1; y < (height-1); y++)
                {
                    if (tempArray[x, y] == f)               // pokud je F s požadovanou hodnotou
                    {
                        for (int yy = -1; yy <= 1; yy++)    // xx a yy jsou okolní souřadnice 
                        {
                            for (int xx = -1; xx <= 1; xx++)
                            {                                           // !!!!!! kdyby krajní políčka nebyly zeď, dojde k chybě
                                if (tempArray[x + xx, y + yy] == 0)     // Pokud je hledaný pixel floor
                                {           
                                    tempArray[x + xx, y + yy] = f;      // zatop políčka
                                }
                            }
                        }

                    }
                }
            }
        }

        if (counter < maxCounter - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

