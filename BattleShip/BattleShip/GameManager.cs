using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip
{
    public class GameManager
    {
        // Στόλος του Παίκτη και του Αντιπάλου
        private List<Ship> playerFleet;
        private List<Ship> opponentFleet;

        // Χρονικά δεδομένα για τις προσπάθειες
        private HashSet<(int Row, int Col)> playerAttempts;
        private HashSet<(int Row, int Col)> opponentAttempts;

        public int PlayerWins { get; private set; }
        public int OpponentWins { get; private set; }

        private Random random;

        public GameManager()
        {
            playerFleet = new List<Ship>();
            opponentFleet = new List<Ship>();

            playerAttempts = new HashSet<(int, int)>();
            opponentAttempts = new HashSet<(int, int)>();

            random = new Random();

            // Αρχικοποίηση στόλων
            InitializeFleets();
        }

        // Αρχικοποίηση στόλων (πλοία του παίκτη και του αντιπάλου)
        private void InitializeFleets()
        {
            // Παράδειγμα τοποθέτησης πλοίων στον στόλο του παίκτη
            playerFleet.Add(new Ship("AircraftCarrier", 5));
            playerFleet.Add(new Ship("Destroyer", 4));
            playerFleet.Add(new Ship("Battleship", 3));
            playerFleet.Add(new Ship("Submarine", 2));

            // Τοποθέτηση πλοίων στον στόλο του αντιπάλου
            opponentFleet.Add(new Ship("AircraftCarrier", 5));
            opponentFleet.Add(new Ship("Destroyer", 4));
            opponentFleet.Add(new Ship("Battleship", 3));
            opponentFleet.Add(new Ship("Submarine", 2));

            // Τυχαία τοποθέτηση των πλοίων
            PlaceShips(playerFleet);
            PlaceShips(opponentFleet);
        }

        // Μέθοδος για να πάρεις τον στόλο του παίκτη
        public List<Ship> GetPlayerFleet()
        {
            return playerFleet;
        }

        // Μέθοδος για να πάρεις τον στόλο του αντιπάλου
        public List<Ship> GetOpponentFleet()
        {
            return opponentFleet;
        }

        // Τυχαία τοποθέτηση πλοίων στον πίνακα (σε μια τυχαία θέση)
        private void PlaceShips(List<Ship> fleet)
        {
            foreach (var ship in fleet)
            {
                bool placed = false;
                while (!placed)
                {
                    bool isHorizontal = random.Next(2) == 0;
                    int startRow = random.Next(0, 10 - (isHorizontal ? 0 : ship.Size));
                    int startCol = random.Next(0, 10 - (isHorizontal ? ship.Size : 0));

                    bool canPlace = true;

                    for (int i = 0; i < ship.Size; i++)
                    {
                        int checkRow = startRow + (isHorizontal ? 0 : i);
                        int checkCol = startCol + (isHorizontal ? i : 0);

                        if (fleet.Any(s => s.Positions.Contains((checkRow, checkCol))))
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace)
                    {
                        for (int i = 0; i < ship.Size; i++)
                        {
                            int row = startRow + (isHorizontal ? 0 : i);
                            int col = startCol + (isHorizontal ? i : 0);
                            ship.AddPosition(row, col);
                        }
                        placed = true;
                    }
                }
            }
        }

        // Μέθοδος επίθεσης του παίκτη
        public string PlayerAttack(int row, int col)
        {
            if (playerAttempts.Contains((row, col)))
                return "Έχεις ήδη επιτεθεί σε αυτή τη θέση!";

            playerAttempts.Add((row, col));

            Ship hitShip = opponentFleet.FirstOrDefault(ship => ship.Positions.Contains((row, col)));
            if (hitShip != null)
            {
                hitShip.Hit(row, col);
                if (hitShip.IsSunk)
                    return $"Βυθίστηκε το {hitShip.Name}!";
                return "Χτύπησες πλοίο!";
            }

            return "Αστόχησες!";
        }

        // Μέθοδος επίθεσης του αντιπάλου
        public (int Row, int Col, string Message) OpponentAttack()
        {
            int row, col;
            do
            {
                row = random.Next(0, 10);
                col = random.Next(0, 10);
            } while (opponentAttempts.Contains((row, col)));

            opponentAttempts.Add((row, col));

            Ship hitShip = playerFleet.FirstOrDefault(ship => ship.Positions.Contains((row, col)));
            if (hitShip != null)
            {
                hitShip.Hit(row, col);
                if (hitShip.IsSunk)
                    return (row, col, $"Βυθίστηκε το {hitShip.Name} σου!");
                return (row, col, "Χτύπημα!");
            }

            return (row, col, "Αστόχησε!");
        }

        // Μέθοδος ελέγχου νίκης
        public bool CheckVictory(bool isPlayer)
        {
            if (isPlayer)
            {
                if (opponentFleet.All(ship => ship.IsSunk))
                {
                    PlayerWins++;
                    return true; // Παίκτης κερδίζει
                }
            }
            else
            {
                if (playerFleet.All(ship => ship.IsSunk))
                {
                    OpponentWins++;
                    return true; // Υπολογιστής κερδίζει
                }
            }

            return false; // Το παιχνίδι συνεχίζεται
        }
    }

}


