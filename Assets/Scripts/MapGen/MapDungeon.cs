using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MapDungeon
{

    public int width = 0;
    public int height = 0;
    public int[,] mapArray;

    public Vector2 entrance;
    public Vector2 exit;

    static int floorID =        0;
    static int wallID =         1;
    static int corridorID =     2;
    static int entranceID =     332;
    static int exitID =         333;

    Vector2 roomMinSize;
    Vector2 roomMaxSize;
    int numOfRooms;

    List<Room> roomList;

    System.Random rnd = new System.Random();

    // Entrance = 332
    // Exit = 333
    
    // 0 = floor
    // 1 = wall
    // 2 = corridor

    public MapDungeon(int width, int height, int numOfRooms, Vector2 roomMinSize, Vector2 roomMaxSize)
    {
        this.width = width;
        this.height = height;
        this.roomMaxSize = roomMaxSize;
        this.roomMinSize = roomMinSize;
        this.numOfRooms = numOfRooms;

        mapArray = new int[width, height];
        roomList = new List<Room>();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                mapArray[x, y] = wallID;
            }
        }

        InitialiseMap();
        createEntranceAndExit();
    }

    void InitialiseMap()
    {
        fillRoomList();
        convertIntoTilemap(roomList);
        createCorridors();

    }

    void fillRoomList()
    {
        int totalTries = 0;
        int tries = 0;

        while (roomList.Count < numOfRooms)
        {
            tries++;
            totalTries++;
            Vector2 randomSize = new Vector2(rnd.Next((int)roomMinSize.x, (int)roomMaxSize.x + 1), rnd.Next((int)roomMinSize.y, (int)roomMaxSize.y + 1));
            Vector2 randomPos = new Vector2(rnd.Next(1, width - (int)randomSize.x - 2), rnd.Next(1, height - (int)randomSize.y - 2));

            Room tempRoom = new Room(randomPos, randomSize);            // vygeneruj random room


            if (tempRoom.doesOverlapWithAnyRoom(roomList) == false)     // Zkontroluj, jestli neoverlapuje s jinou místností v listu
            {
                roomList.Add(tempRoom);
            }

            if (tries > numOfRooms * 10)                                // Pokud bych generoval moc dlouho, radši začnu odznova
            {
                tries = 0;
                roomList.Clear();
            }
        }

        Debug.Log("MapDungeon: All " + numOfRooms +" rooms generated in total of " + totalTries + " tries.");
    }

    void convertIntoTilemap(List<Room> rList)
    {
        foreach (Room r in rList)                                    // zkonvertuj do int[] GameMap
        {
            for (int x = (int)r.start.x; x < ((int)r.start.x + (int)r.size.x); x++)
            {
                for (int y = (int)r.start.y; y < ((int)r.start.y + (int)r.size.y); y++)
                {
                    mapArray[x, y] = floorID;
                }
            }
        }
    }

    void createCorridors()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].isConnected)
            {
                int j = rnd.Next(1, roomList.Count);
                connectTwoRooms(roomList[i], roomList[(i + j) % roomList.Count]);
            }
        }

        if (roomList.Count > 4)
        {
            // connect few random corridors (4)
            for (int i = 0; i < 4; i++)
            {
                int first = UnityEngine.Random.Range(0, roomList.Count);
                int second = first;

                while (second == first)
                {
                    second = UnityEngine.Random.Range(0, roomList.Count);
                }

                connectTwoRooms(roomList[first], roomList[second]);
            }
        }
    }

    void connectTwoRooms(Room r1, Room r2)
    {

        int x = (int)r1.roomCenter.x;
        int y = (int)r1.roomCenter.y;

        while (x != (int)r2.roomCenter.x)
        {
            if (mapArray[x, y] != floorID)
            {
                mapArray[x, y] = corridorID;
            }

            x += x < (int)r2.roomCenter.x ? 1 : -1;
        }

        while (y != (int)r2.roomCenter.y)
        {
            if (mapArray[x, y] != floorID)
            {
                mapArray[x, y] = corridorID;
            }

            y += y < (int)r2.roomCenter.y ? 1 : -1;
        }

        r1.isConnected = true;
        r2.isConnected = true;

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
            int randomX = rnd.Next(2, width - 2);
            int randomY = rnd.Next(2, height - 2);
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

        mapArray[(int)exit.x, (int)exit.y] = exitID;


    }

    Boolean checkIfConnected(Vector2 entranceVector, Vector2 exitVector, int[,] array)  // Využij "flooding" algoritmu
    {
        int f = 1024;                                       // tuto hodnotu budu používat pro flooding

        int[,] tempArray = new int[width, height];          // vytvoř array o stejné velikosti jako mapArray
        Array.Copy(mapArray, tempArray, mapArray.Length);   // kdybych dal jednoduše =, nedojde ke zkopírování, ale jen se předá reference!

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
                            {                               // !!!!!! kdyby krajní políčka nebyly zeď, dojde k chybě
                                if (tempArray[x + xx, y + yy] == floorID || tempArray[x + xx, y + yy] == corridorID)     // Pokud je hledaný pixel floor
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

