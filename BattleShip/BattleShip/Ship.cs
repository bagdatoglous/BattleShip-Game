using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip
{
    public class Ship
    {
        public string Name { get; private set; } // Όνομα πλοίου
        public int Size { get; private set; } // Μέγεθος πλοίου (κελιά)
        public List<(int Row, int Col)> Positions { get; private set; } // Θέσεις του πλοίου στον πίνακα
        public bool IsSunk => Positions.Count == 0; // Αν βυθίστηκε το πλοίο

        public Ship(string name, int size)
        {
            Name = name;
            Size = size;
            Positions = new List<(int, int)>();
        }

        public void AddPosition(int row, int col)
        {
            Positions.Add((row, col));
        }

        public void Hit(int row, int col)
        {
            Positions.Remove((row, col));
        }
    }
}

