using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Room {

        public Vector2 start;
        public Vector2 size;

        public bool isConnected = false;

        public Room(Vector2 _spawnPos, Vector2 _size)
        {
            start = _spawnPos;
            size = _size;
        }

        public bool checkIfOverlap(Room r1, Room r2)
        {
            // Zde může být něco špatně, nejsem si jistý nastavenou šířkou zdí
            if (((Enumerable.Range((int)r1.start.x - 1, (int)r1.start.x + (int)r1.size.x + 1).Contains((int)r2.start.x)) || (Enumerable.Range((int)r1.start.x - 1, (int)r1.start.x + (int)r1.size.x + 1).Contains((int)r2.start.x + (int)r2.size.x)))
            && (((Enumerable.Range((int)r1.start.y - 1, (int)r1.start.y + (int)r1.size.y + 1).Contains((int)r2.start.y)) || (Enumerable.Range((int)r1.start.y - 1, (int)r1.start.y + (int)r1.size.y + 1).Contains((int)r2.start.y + (int)r2.size.y)))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Vector2 roomCenter
        {
            get
            {
                return new Vector2(start.x + Mathf.RoundToInt(size.x / 2), start.y + Mathf.RoundToInt(size.y / 2));
            }
            set
            {
                Debug.Log("MapDungeon: Setting a center of a room makes no sense.");
            }
        }

        public bool doesOverlapWithAnyRoom(List<Room> roomList)
        {
            bool doesOverlap = false;
            foreach (Room r in roomList)
            {
                if (r.checkIfOverlap(r, this) == true)
                {
                    doesOverlap = true;
                    break;
                }
            }
            return doesOverlap;
        }
}

