using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShip
{
    public partial class Form1 : Form
    {
        // Δημιουργία του GameManager
        private GameManager gameManager = new GameManager();

        public Form1()
        {
            InitializeComponent();
        }z

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ρύθμιση μεγέθους για τα κελιά των πινάκων
            AdjustCellSizes(playerBoard, 50); // Όρισε μέγεθος κελιών σε 50x50 pixels
            AdjustCellSizes(opponentBoard, 50);

            // Εμφάνιση πλοίων
            DisplayShips(playerBoard, gameManager.GetPlayerFleet());
            DisplayShips(opponentBoard, gameManager.GetOpponentFleet(), true);

            // Ενεργοποίηση ενός εμφανές χρώμα για να διακρίνεται το TableLayoutPanel
            playerBoard.BackColor = Color.LightGray;
            opponentBoard.BackColor = Color.LightGray;

            // Δημιουργία layout για την οθόνη
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Αριστερά
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); // Κέντρο
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Δεξιά

            layout.Controls.Add(panel2, 0, 0);
            layout.Controls.Add(panel1, 1, 0);
            layout.Controls.Add(panel3, 2, 0);

            this.Controls.Add(layout);
        }

        // Μέθοδος για να εμφανίσεις τα πλοία στον πίνακα
        private void DisplayShips(TableLayoutPanel board, List<Ship> ships, bool hidden = false)
        {
            foreach (var ship in ships)
            {
                foreach (var position in ship.Positions)
                {
                    PictureBox shipPart = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        BackColor = hidden ? Color.LightGray : Color.Blue,
                        Tag = position // Αποθήκευση των συντεταγμένων στη θέση του PictureBox
                    };

                    // Εγκατάσταση χειριστή για το κλικ
                    shipPart.MouseClick += (sender, e) =>
                    {
                        PictureBox clickedBox = sender as PictureBox;
                        var clickedPosition = (ValueTuple<int, int>)clickedBox.Tag; // Ανάκτηση των συντεταγμένων
                        int row = clickedPosition.Item1;
                        int col = clickedPosition.Item2;

                        // Έλεγχος για το αν είναι η θέση του αντιπάλου
                        string result = gameManager.PlayerAttack(row, col);

                        // Δημιουργία ετικέτας για το "Χ" ή την παύλα "-"
                        Label attackLabel = new Label
                        {
                            AutoSize = true,
                            Font = new Font("Arial", 16),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Dock = DockStyle.Fill
                        };

                        // Ενημέρωση του κειμένου και του χρώματος
                        if (result.Contains("Αστόχησες"))
                        {
                            attackLabel.Text = "-";
                            attackLabel.ForeColor = Color.Green; // Πράσινη παύλα
                        }
                        else
                        {
                            attackLabel.Text = "X";
                            attackLabel.ForeColor = Color.Red; // Κόκκινο "Χ"
                        }

                        // Αντικαθιστούμε το PictureBox με το Label πάνω στο κελί
                        clickedBox.Controls.Add(attackLabel);

                        // Ενημέρωση για το αποτέλεσμα της επίθεσης του παίκτη
                        MessageBox.Show(result);

                        // Αν ο παίκτης κερδίσει, εμφανίζεται το μήνυμα
                        if (gameManager.CheckVictory(true))
                        {
                            MessageBox.Show("Κέρδισες!");
                        }
                        else
                        {
                            // Αν ο αντίπαλος κερδίσει
                            var (opRow, opCol, opResult) = gameManager.OpponentAttack();
                            MessageBox.Show($"Ο αντίπαλος επιτέθηκε στη θέση ({opRow}, {opCol}): {opResult}");

                            // Δημιουργία ετικέτας για την επίθεση του αντιπάλου
                            var opponentLabel = new Label
                            {
                                AutoSize = true,
                                Font = new Font("Arial", 16),
                                TextAlign = ContentAlignment.MiddleCenter,
                                Dock = DockStyle.Fill
                            };

                            // Ενημέρωση του κειμένου και του χρώματος για την επίθεση του αντιπάλου
                            if (opResult.Contains("Αστόχησε"))
                            {
                                opponentLabel.Text = "-";
                                opponentLabel.ForeColor = Color.Green; // Πράσινη παύλα για τον αντίπαλο
                            }
                            else
                            {
                                opponentLabel.Text = "X";
                                opponentLabel.ForeColor = Color.Red; // Κόκκινο "Χ" για τον αντίπαλο
                            }

                            // Ενημέρωση του πίνακα του αντιπάλου
                            var opponentBox = opponentBoard.GetControlFromPosition(opCol, opRow) as PictureBox;
                            if (opponentBox != null)
                            {
                                opponentBox.Controls.Add(opponentLabel);
                            }

                            if (gameManager.CheckVictory(false))
                            {
                                MessageBox.Show("Ο αντίπαλος κέρδισε!");
                            }
                        }
                    };

                    board.Controls.Add(shipPart, position.Col, position.Row);
                }
            }
        }






        // Ρύθμιση του μεγέθους των κελιών
        private void AdjustCellSizes(TableLayoutPanel table, int cellSize)
        {
            // Ρύθμιση ίσου μεγέθους για κάθε γραμμή
            foreach (RowStyle style in table.RowStyles)
            {
                style.SizeType = SizeType.Absolute;
                style.Height = cellSize;
            }

            // Ρύθμιση ίσου μεγέθους για κάθε στήλη
            foreach (ColumnStyle style in table.ColumnStyles)
            {
                style.SizeType = SizeType.Absolute;
                style.Width = cellSize;
            }
        }

        // Μέθοδος για την επαναφορά του παιχνιδιού
        private void btnRestart_Click(object sender, EventArgs e)
        {
            gameManager = new GameManager(); // Επαναφορά του GameManager και των δεδομένων
            Form1_Load(sender, e); // Επαναφόρτωση της φόρμας για να ξαναρχίσει το παιχνίδι
        }

        // Μέθοδος για την επίθεση του παίκτη μέσω κλικ στον πίνακα του αντιπάλου
        private void opponentBoard_MouseClick(object sender, MouseEventArgs e)
        {
            // Υπολογισμός της θέσης στον πίνακα του αντιπάλου
            int row = e.Y / 50;
            int col = e.X / 50;

            // Κλήση της μεθόδου επίθεσης του παίκτη
            string result = gameManager.PlayerAttack(row, col);

            // Εμφάνιση του αποτελέσματος της επίθεσης
            MessageBox.Show(result);

            // Ελέγχουμε αν ο παίκτης έχει νικήσει
            if (gameManager.CheckVictory(true))
            {
                MessageBox.Show("Κέρδισες!");
                // Εμφάνιση της κίνησης του αντιπάλου (με βάση την τυχαία επίθεση)
                var (opponentRow, opponentCol, opponentResult) = gameManager.OpponentAttack();
                MessageBox.Show($"Ο αντίπαλος επιτέθηκε στη θέση ({opponentRow}, {opponentCol}): {opponentResult}");

                // Ελέγχουμε αν ο αντίπαλος έχει νικήσει
                if (gameManager.CheckVictory(false))
                {
                    MessageBox.Show("Ο αντίπαλος κέρδισε!");
                }
            }
            else
            {
                // Αν ο παίκτης δεν έχει νικήσει, η σειρά του αντιπάλου
                var (opponentRow, opponentCol, opponentResult) = gameManager.OpponentAttack();
                MessageBox.Show($"Ο αντίπαλος επιτέθηκε στη θέση ({opponentRow}, {opponentCol}): {opponentResult}");

                // Ελέγχουμε αν ο αντίπαλος έχει νικήσει
                if (gameManager.CheckVictory(false))
                {
                    MessageBox.Show("Ο αντίπαλος κέρδισε!");
                }
            }
        }
    }

}

