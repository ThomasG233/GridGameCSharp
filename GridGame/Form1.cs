using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GridGame
{
    public partial class Form1 : Form
    {
        /* have put this here as it may come in use later.
        *  idea was hold all needed variables for the gameplay.
        */
        class GameSettings
        {
            private int difficulty;
            private bool[,] board1 = new bool[9, 9];
            private bool[,] board2 = new bool[9, 9];
            private bool player1Turn = true;
            private bool ai = false;
            private int score1 = 0, score2 = 0;

            public void setDifficulty(int diff)
            {
                difficulty = diff;
            }
            public void setAI(bool AI)
            {
                ai = AI;
            }
            public void setBoard1(bool[,] board1)
            {
                this.board1 = board1;
            }
            public void setBoard2(bool[,] board2)
            {
                this.board2 = board2;
            }
            public void setPlayer1Turn(bool turn)
            {
                player1Turn = turn;
            }

            public void setScore1(int score)
            {
                score1 = score;
            }

            public void setScore2(int score)
            {
                score2 = score;
            }
            public int getDifficulty()
            {
                return difficulty;
            }
            public bool getAI()
            {
                return ai;
            }
            public bool[,] getBoard1()
            {
                return board1;
            }
            public bool[,] getBoard2()
            {
                return board2;
            }
            public bool getPlayer1Turn()
            {
                return player1Turn;
            }
            public bool getBoard1CellState(int x, int y)
            {
                return board1[x, y];
            }
            public bool getBoard2CellState(int x, int y)
            {
                return board2[x, y];
            }
            public void setBoard1CellState(int x, int y, bool state)
            {
                board1[x, y] = state;
            }
            public void setBoard2CellState(int x, int y, bool state)
            {
                board2[x, y] = state;
            }
            public void clearBoard1()
            {
                for (int x = 0; x > board1.GetLength(0); x++)
                {
                    for (int y = 0; y > board1.GetLength(1); y++)
                    {
                        board1[x, y] = false;
                    }
                }
            }
            public void clearBoard2()
            {
                for (int x = 0; x > board2.GetLength(0); x++)
                {
                    for (int y = 0; y > board2.GetLength(1); y++)
                    {
                        board2[x, y] = false;
                    }
                }
            }
            public int getScore1()
            {
                return score1;
            }

            public int getScore2()
            {
                return score2;
            }
        }

        Label lblPlayerTurn = new Label();
        Label lblScore1 = new Label();
        Label lblScore2 = new Label();
        bool sound = true;
        Font menuFont = new Font("Times New Roman", 18.0f);
        /* Had to make the grids for the selection screen global variables, so that the necessary checks can be performed.
         * i.e. when you confirm your ship placements, isSelectionValid() must be called.
         * you can't really use the button grid as a parameter into the method as they're already placed
         * BtnPlayer2Grid represents the CPU, but in case we do 2 player, i've made it public too.
        */
        Button[,] BtnPlayer1Grid;
        Button[,] BtnPlayer2Grid;
        GameSettings gameSettings = new GameSettings();
        public Form1()
        {
            InitializeComponent();
            menuStrip();
            initMenu();
            soundButton(sound);
            
        }

        // needed to access code easily on opening the files
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        // Ship Selection Screen visuals.
        private void initShipSelect()
        {
            BtnPlayer1Grid = new Button[9, 9];
            BtnPlayer2Grid = new Button[9, 9];

            Button BtnConfirm = new Button();
            Label LblInstructions = new Label();
            // still need to add these to the side of the grid.
            Label[] LblGridTop = new Label[9];
            Label[] LblGridSide = new Label[9];

            LblInstructions.Text = "Please select your ship locations:";
            LblInstructions.SetBounds((int)(this.ClientSize.Width / 3.55), 15, 400, 35);
            LblInstructions.TextAlign = ContentAlignment.TopCenter;
            LblInstructions.Font = menuFont;

            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i' };

            BtnConfirm.SetBounds((int)(this.ClientSize.Width / 2) - 60, (int)(this.ClientSize.Height / 1.25), 100, 50);
            BtnConfirm.Text = "Confirm";
            BtnConfirm.Font = menuFont;
            BtnConfirm.Click += new EventHandler(BtnConfirmEvent_Click);

            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {

                LblGridTop[x] = new Label();
                LblGridTop[x].Text = Convert.ToString(letters[x]);
                LblGridTop[x].TextAlign = ContentAlignment.MiddleLeft;
                LblGridTop[x].SetBounds((int)((this.ClientSize.Width / 3.33333) + (40 * x)) + 15, ((int)(this.ClientSize.Height / 9)) - 15, 10, 15);

                LblGridSide[x] = new Label();
                LblGridSide[x].SetBounds((int)(this.ClientSize.Width / 3.33333) - 15, ((int)(this.ClientSize.Height / 9)) + (40 * x) + 14, 10, 15);
                LblGridSide[x].Text = Convert.ToString(x);
                LblGridSide[x].TextAlign = ContentAlignment.MiddleLeft;

                Controls.Add(LblGridTop[x]);
                Controls.Add(LblGridSide[x]);
                for (int y = 0; y < BtnPlayer1Grid.GetLength(1); y++)
                {
                    // Create all buttons on the grid.
                    BtnPlayer1Grid[x, y] = new Button();
                    BtnPlayer2Grid[x, y] = new Button();
                    BtnPlayer1Grid[x, y].SetBounds((int)(this.ClientSize.Width / 3.33333) + (40 * x), ((int)(this.ClientSize.Height / 9)) + (40 * y), 40, 40);
                    BtnPlayer1Grid[x, y].BackColor = Color.PowderBlue;
                    BtnPlayer2Grid[x, y].BackColor = Color.PowderBlue;

                    BtnPlayer1Grid[x, y].Click += new EventHandler(this.BtnPlayer1GridEvent_Click);

                    Controls.Add(BtnPlayer1Grid[x, y]);
                }
            }
            Controls.Add(LblInstructions);
            Controls.Add(BtnConfirm);
        }
        // Change colour of grid button when clicked.
        // still need to add further checks
        void BtnPlayer1GridEvent_Click(object sender, EventArgs e)
        {
            // Checks to see if the ship has already been placed in cell.
            if (((Button)sender).BackColor != Color.Gray)
            {  
                // Places ship within cell of grid.
                ((Button)sender).BackColor = Color.Gray;
            }
            else
            {
                // Unselects the cell in the grid where ship was.
                ((Button)sender).BackColor = Color.PowderBlue;
            }
        }
        //when the player picks a spot to target on the opponents side
        //this will check if there is a ship there or not
        void BtnTargetSelectionEvent_Click(object sender, EventArgs e)
        {
            //using button name to associate it with the x and y values of it in the players bool board
            String name = ((Button)sender).Name;
            char pVal = name[1];
            char xVal = name[2];
            char yVal = name[3];
            int p = int.Parse(name[1].ToString());
            int x = int.Parse(name[2].ToString());
            int y = int.Parse(name[3].ToString());

            if (gameSettings.getAI() == false)
            {
                // Take turns targeting ships (player vs player)
                if (gameSettings.getPlayer1Turn() == true)
                {
                    if (p == 1)
                    {
                        MessageBox.Show("Can't Target Your Own Side.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (gameSettings.getBoard2CellState(x, y) == true)
                        {
                            ((Button)sender).BackColor = Color.Red;
                            gameSettings.setScore1(gameSettings.getScore1() + 1);
                            lblScore1.Text = Convert.ToString(gameSettings.getScore1());
                        }
                        else
                            ((Button)sender).BackColor = Color.White;

                        gameSettings.setPlayer1Turn(false);
                        lblPlayerTurn.Text = "Player 2's Turn";
                    }
                }
                else
                {
                    if (p == 2)
                    {
                        MessageBox.Show("Can't Target Your Own Side.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (gameSettings.getBoard1CellState(x,y) == true)
                        {
                            ((Button)sender).BackColor = Color.Red;
                            gameSettings.setScore2(gameSettings.getScore2() + 1);
                            lblScore2.Text = Convert.ToString(gameSettings.getScore2());
                        }
                        else
                            ((Button)sender).BackColor = Color.White;

                        gameSettings.setPlayer1Turn(true);
                        lblPlayerTurn.Text = "Player 1's Turn";
                    }
                }
            }
            else
            {
                if (gameSettings.getPlayer1Turn() == true)
                {
                    if (p == 1)
                    {
                        MessageBox.Show("Can't Target Your Own Side.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (gameSettings.getBoard2CellState(x, y) == true)
                            ((Button)sender).BackColor = Color.Red;
                        else
                            ((Button)sender).BackColor = Color.White;

                        gameSettings.setPlayer1Turn(true);
                    }
                }
                else
                {
                    // Write code for ai's turn depending on difficulty
                }
            }
        }

        // Confirm the grid placements.
        void BtnConfirmEvent_Click(object sender, EventArgs e)
        {
            // Check if the selections made are valid.
            if (isSelectionValid())
            {
                for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
                {
                    for (int y = 0; y < BtnPlayer1Grid.GetLength(1); y++)
                    {
                        // If the cell has been selected by the user
                        if (BtnPlayer1Grid[x, y].BackColor == Color.Gray)
                        {
                            // need to program to transfer over to gameSettings object
                        }
                    }
                }
                clearForm();
                // Clear array so it's not taking up memory.
                //BtnPlayer1Grid = null;
                playGame();
            }
            else
            {
                MessageBox.Show("Invalid amount of ship placements.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Check the player's selections to see if it's valid.
        bool isSelectionValid()
        {
            int numberOfShipGrids = 0;
            // Count all of the number of grid cells selected.
            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {
                for (int y = 0; y < BtnPlayer1Grid.GetLength(0); y++)
                {
                    if (BtnPlayer1Grid[x, y].BackColor == Color.Gray)
                    {
                        numberOfShipGrids++;
                    }
                }
            }
            // Minimum number of cells selected needs to be 17.
            if (numberOfShipGrids != 17)
            {
                MessageBox.Show("Invalid amount of ship placements.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            /* An efficient way to do this, rather than the large amount of code for the checks.
             * Check FindFive() for comments on how it works
             * */
            if (!(findShipInSelection(5) && findShipInSelection(4) && findShipInSelection(3) && findShipInSelection(3) && findShipInSelection(2)))
            {
                gameSettings.clearBoard1();
                return false;
            }

            return true;
        }

        // Find the ship on the selection screen and see if the placement is valid.
        bool findShipInSelection(int amountToFind)
        {
            // HORIZONTAL CHECKING
            // Defines the search space to look for the ships.
            for (int x = 0; x < gameSettings.getBoard1().GetLength(0) - (amountToFind - 1); x++)
            {
                for (int y = 0; y < gameSettings.getBoard1().GetLength(1); y++)
                {
                    // Set this to 0 for indexing.
                    int val = 0;
                    // Check to see if there exists a ship pattern.
                    while (val < amountToFind)
                    {
                        if (BtnPlayer1Grid[x + val, y].BackColor == Color.Gray)
                        {
                            val++;
                        }
                        else
                        {
                            // forcibly break out of loop.
                            val = 6;
                        }
                    }
                    // If a ship has been found.
                    if (val == amountToFind)
                    {
                        val = 0;
                        bool found = false;
                        // Check to see if this selection encapsulates another.
                        // i.e. 4 cells can exist within a 5 cell ship, so this must be checked..
                        while (val < amountToFind)
                        {
                            if (gameSettings.getBoard1CellState(x + val, y))
                            {
                                found = true;
                                val = 6;
                            }
                            else
                            {
                                val++;
                            }
                        }
                        // If the ship doesn't overlap with others in the grid.
                        if (!found)
                        {
                            // Add ship into gameSettings board.
                            for (int i = 0; i < amountToFind; i++)
                            {
                                gameSettings.setBoard1CellState(x + i, y, true);
                            }
                            // MessageBox.Show("Found a ship of " + amountToFind + ", added.", "Added successfully");
                            return true;
                        }
                        else
                        {
                            // MessageBox.Show("Found a ship of " + amountToFind + " but found previously, not added.", "Not added");
                        }
                    }
                }
            }
            // VERTICAL CHECKING
            // Defines the search space to look for the ships.
            for (int x = 0; x < gameSettings.getBoard1().GetLength(0); x++)
            {
                for (int y = 0; y < gameSettings.getBoard1().GetLength(1) - (amountToFind - 1); y++)
                {
                    // Set this to 0 for indexing.
                    int val = 0;
                    // Checks to see if a ship pattern exists on the grid.
                    while (val < amountToFind)
                    {
                        if (BtnPlayer1Grid[x, y + val].BackColor == Color.Gray)
                        {
                            val++;
                        }
                        else
                        {
                            // forcibly break out of loop.
                            val = 6;
                        }
                    }
                    // If a ship has been found.
                    if (val == amountToFind)
                    {
                        val = 0;
                        bool found = false;
                        // Check to see if this selection encapsulates another.
                        // i.e. 4 cells can exist within a 5 cell ship, so this must be checked.
                        while (val < amountToFind)
                        {
                            if (gameSettings.getBoard1CellState(x, y + val))
                            {
                                found = true;
                                val = 6;
                            }
                            val++;
                        }
                        // If the ship doesn't overlap with others in the grid.
                        if (!found)
                        {
                            // Add into gameSettings board.
                            for (int i = 0; i < amountToFind; i++)
                            {
                                gameSettings.setBoard1CellState(x, y + i, true);
                            }
                            // MessageBox.Show("Found a ship of " + amountToFind + ", added.", "Added successfully");
                            return true;
                        }
                        else
                        {
                            // MessageBox.Show("Found a ship of " + amountToFind + " but found previously, not added.", "Not added");
                        }
                    }
                }
            }
            MessageBox.Show("Could not find a ship of size " + amountToFind, "Cannot add.");
            return false;
        }
        // Display the main menu
        private void initMenu()
        {
            Button btnStart = new Button();
            Button btnRules = new Button();
            
            btnStart.SetBounds(this.ClientSize.Width / 2 - 50, this.ClientSize.Height / 2, 100, 50);
            btnRules.SetBounds(this.ClientSize.Width / 2 - 50, this.ClientSize.Height / 2 + 75, 100, 50);

            btnStart.Font = menuFont;
            btnRules.Font = menuFont;
            btnStart.Text = "Start";
            btnRules.Text = "Rules";

            PictureBox boat = new PictureBox();
            boat.ImageLocation = "../../Battleship.png";
            boat.Size = new Size(200, 200);
            boat.Location = new Point(this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2 - 225);

            btnStart.Click += new EventHandler(this.btnStartEvent_Click);
            btnRules.Click += new EventHandler(this.btnRulesEvent_Click);

            Controls.Add(btnStart);
            Controls.Add(btnRules);
            Controls.Add(boat);
        }

        // Display the difficulty menu
        private void difficultyMenu()
        {
            Button[] btns = new Button[3];
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i] = new Button();
                btns[i].Font = menuFont;
            }

            btns[0].SetBounds(this.ClientSize.Width / 2 - 180, this.ClientSize.Height / 2 - 50, 100, 50);
            btns[1].SetBounds(this.ClientSize.Width / 2 - 60, this.ClientSize.Height / 2 - 50, 100, 50);
            btns[2].SetBounds(this.ClientSize.Width / 2 + 60, this.ClientSize.Height / 2 - 50, 100, 50);
            btns[0].Text = "Easy";
            btns[1].Text = "Normal";
            btns[2].Text = "Hard";

            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].Click += new EventHandler(this.btnGameStartEvent_Click); // after clicking difficulty, set game
                Controls.Add(btns[i]);
            }
        }

        // Clears the form and re-initialises with the menu strip intact
        // (Clearance of form allows for switches between main menu to game screen)
        private void clearForm()
        {
            this.Controls.Clear();
            this.InitializeComponent();
            menuStrip();
            soundButton(sound);
        }

        // sound button, responsible for toggling of in-game music/sounds
        private void soundButton(bool sound)
        {
            Button btnSound = new Button();

            Image soundImg = Image.FromFile("../../megaphone.png");
            btnSound.BackgroundImage = soundImg;
            btnSound.BackgroundImageLayout = ImageLayout.Stretch;
            btnSound.SetBounds(25, 20, 50, 50);
            btnSound.Click += new EventHandler(this.btnSoundEvent_Click);

            btnSound.FlatStyle = FlatStyle.Flat;
            btnSound.FlatAppearance.BorderSize = 1;
            if (sound)
            {
                btnSound.FlatAppearance.BorderColor = Color.Blue;
            }
            else
            {
                btnSound.FlatAppearance.BorderColor = Color.Red;
            }
            
            Controls.Add(btnSound);
        }

        // sound toggle click event
        void btnSoundEvent_Click(object sender, EventArgs e)
        {
            if (!sound)
            {
                ((Button)sender).FlatAppearance.BorderColor = Color.Blue;
                sound = true;
                // toggle sound on
            }
            else if (sound)
            {
                ((Button)sender).FlatAppearance.BorderColor = Color.Red;
                sound = false;
                // toggle sound off
            }
        }

        // Creates the main menu strip in the program
        private void menuStrip()
        {
            MainMenu strip = new MainMenu();
            MenuItem options = strip.MenuItems.Add("&Options");
            options.MenuItems.Add(new MenuItem("&Main Menu", stripMainMenuEvent_Click));
            options.MenuItems.Add(new MenuItem("&Toggle Music On/Off"));
            options.MenuItems.Add(new MenuItem("&Exit", stripMainMenuExitEvent_Click));
            this.Menu = strip;
            MenuItem rules = strip.MenuItems.Add("&Rules", btnRulesEvent_Click);
            this.Menu = strip;
            strip.GetForm().BackColor = Color.LightCyan;
        }
        
        void btnGameStartEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            // Loads the ship selection screen, prior to a game starting.
            initShipSelect();
            // insert game code
            // currently all difficulties direct to this event

        }

        void playGame()
        {
            // Check if it is in player vs player mode or player vs AI mode

            // layout buttons for the game

            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {

                for (int y = 0; y < BtnPlayer1Grid.GetLength(1); y++)
                {
                    // Create all buttons on the grid.
                    BtnPlayer1Grid[x, y].SetBounds(((this.ClientSize.Width / 2) - 50) - (31 * y), 101 + (31 * x), 25, 25);
                    BtnPlayer2Grid[x, y].SetBounds(((this.ClientSize.Width / 2) + 50) + (31 * y), 349 - (31 * x), 25, 25);

                    BtnPlayer1Grid[x, y].Click -= BtnPlayer1GridEvent_Click;
                    BtnPlayer1Grid[x, y].Click += new EventHandler(this.BtnTargetSelectionEvent_Click);
                    BtnPlayer2Grid[x, y].Click += new EventHandler(this.BtnTargetSelectionEvent_Click);

                    BtnPlayer1Grid[x, y].Name = "P1" + Convert.ToString(x) + Convert.ToString(y);
                    BtnPlayer2Grid[x, y].Name = "P2" + Convert.ToString(x) + Convert.ToString(y);

                    lblScore1.SetBounds(((this.ClientSize.Width / 2) - 49), 50, 25, 25);
                    lblScore2.SetBounds(((this.ClientSize.Width / 2) + 50), 50, 25, 25);
                    lblScore1.ForeColor = Color.Blue;
                    lblScore2.ForeColor = Color.Red;
                    lblScore1.Font = menuFont;
                    lblScore2.Font = menuFont;
                    lblScore1.Text = Convert.ToString(gameSettings.getScore1());
                    lblScore2.Text = Convert.ToString(gameSettings.getScore2());

                    lblPlayerTurn.SetBounds((this.ClientSize.Width / 2) - 67, 400, 250, 25);
                    lblPlayerTurn.Font = menuFont;
                    lblPlayerTurn.Text = "Player 1's Turn";

                    Label lblP1 = new Label();
                    Label lblP2 = new Label();

                    lblP1.SetBounds(((this.ClientSize.Width / 2) - 100), (this.ClientSize.Height / 2) - 223, 50, 25);
                    lblP2.SetBounds(((this.ClientSize.Width / 2) + 94), (this.ClientSize.Height / 2) - 223, 50, 25);
                    lblP1.Font = menuFont;
                    lblP2.Font = menuFont;
                    lblP1.Text = "P1";
                    lblP2.Text = "P2";
                    lblP1.ForeColor = Color.Blue;
                    lblP2.ForeColor = Color.Red;

                    Controls.Add(BtnPlayer1Grid[x, y]);
                    Controls.Add(BtnPlayer2Grid[x, y]);
                    Controls.Add(lblScore1);
                    Controls.Add(lblScore2);
                    Controls.Add(lblPlayerTurn);
                    Controls.Add(lblP1);
                    Controls.Add(lblP2);
                }
            }
        }

        // Start button click event, leads directly to difficulty menu
        void btnStartEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            difficultyMenu();
        }

        void btnRulesEvent_Click(object sender, EventArgs e)
        {
            string msg1 = "The object of Battleship is to try and sink all of the other player's before they sink all of your ships. All of the other player's ships are somewhere on his/her board.  You try and hit them by clicking one of the squares on the board.  The other player will also try to hit your ships in turns.  Neither you nor the other player can see the other's board so you must try to guess where they are.";
            string msg2 = "Each player places the 5 ships somewhere on their board.  The ships can only be placed vertically or horizontally. Diagonal placement is not allowed. No part of a ship may hang off the edge of the board.  Ships may not overlap each other.  No ships may be placed on another ship. \r\n\r\nOnce the guessing begins, the players may not move the ships.\r\n\r\nThe 5 ships are:  Carrier (occupies 5 spaces), Battleship (4), Cruiser (3), Submarine (3), and Destroyer (2).";
            string msg3 = "Player's take turns guessing by clicking on the grid squares. Upon guessing, a red X will be marked for hit, and a white O for miss. For example, if you click F6 and your opponent does not have any ship located at F6, that grid square will be marked with a white O. \r\n\r\nWhen all of the squares that one your ships occupies have been hit, the ship will be sunk. As soon as all of one player's ships have been sunk, the game ends.";
            string caption = "Rules";
            MessageBox.Show(msg1, caption);
            MessageBox.Show(msg2, caption);
            MessageBox.Show(msg3, caption);
        }

        // Strip "Main Menu" event click
        void stripMainMenuEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            initMenu();
        }

        // Strip Main Menu Exit button, closes program.
        void stripMainMenuExitEvent_Click(object sender, EventArgs e)
        {
            Close();
        }

        void CheckWin()
        {

        }
    }
}

