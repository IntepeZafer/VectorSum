using Game.Tiles;
using UnityEngine;

namespace Game.Grid
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int Coordinates {get; private set;} // Hücrenin Izgarada Ki Koordinatlarını Tutar(Örn : 0 , 0 Veya 2 , 2)
        public Tile CurrentTile {get; set;}
        public bool IsOccupied => CurrentTile != null; // Taş Varsa Dolu Döner
        public void Initialize(Vector2Int coords) //  hücrenin ızgaradaki nerede olduğunu (x,y) söyler
        {
            Coordinates = coords;
        }
    }
}