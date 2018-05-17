using UnityEngine;
using System.Collections;

public class Enemy : Entity {

    public float sight;
    public bool restrictedToDiagonalMovementOnly = false;

    public Enemy(Vector2 _position, float _health, float _maxHealth, float _armor, float _sight)
    {
        position = _position;
        health = _health;
        armor = _armor;
        sight = _sight;
    }

    public Enemy(Vector2 _position)
    {
        position = _position;
    }

    public Vector2 moveTowardsPlace(Vector2 targetPos)
    {

        var sortedMoveToList = new System.Collections.Generic.SortedList<float, Vector2>();

        if ((targetPos - position).magnitude < sight)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 tempMove = new Vector2(x, y);

                    if (!GameMap.Instance.wallDictionary.ContainsKey(position + tempMove))                      //  xxx
                    {                                                                                           //  x_x  -> vybere nejbližsí možnost z okolí
                        float distance = (targetPos - (position + tempMove)).magnitude;                         //  xxx     -> teoreticky by se dalo něco ušetřit
                        if (!sortedMoveToList.ContainsKey(distance))                                            // Pokud v listu ještě není hodnota o stejné vzdálenosti
                        {
                            if (restrictedToDiagonalMovementOnly)                                               // Neozkoušeno!
                            {
                                if ((Mathf.Abs(x) + Mathf.Abs(y)) < 2)                                          // Jen pokud je |x| nebo |y| 
                                {
                                    sortedMoveToList.Add(distance, tempMove);
                                }
                            }
                            else
                            {
                                sortedMoveToList.Add(distance, tempMove);
                            }
                        }
                    }
                }
            }

            if (sortedMoveToList.Count > 0)         // Pokud je v sortedListu nějaká hodnota, pak ji vyber
            {
                return sortedMoveToList.Values[0];
            }
            else                                    // Pokud není, tak vrať Vector2.zero
            {
                return Vector2.zero;
            }
        }
        else
        {
            return Vector2.zero;
        }
    }

}
