// Team 4: Kayee Liu, Thomas Gourlay and Oliver Shearer

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
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Deployment.Application;

using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;
using System.IO;

namespace GridGame
{
    public partial class Form1 : Form
    {
        // Holds information pertaining to a player object.
        class Player
        {
            // Player's side of the board.
            private bool[,] board = new bool[9, 9];
            private int score = 0;

            // Reset the board to have no placements.
            public void clearBoard()
            {
                for (int x = 0; x < board.GetLength(0); x++)
                {
                    for (int y = 0; y < board.GetLength(1); y++)
                    {
                        board[x, y] = false;
                    }
                }
            }
            // Get a specific cell of the player's board.
            public bool getBoardCellState(int x, int y)
            {
                return board[x, y];
            }
            // Set a specific cell of the player's board.
            public void setBoardCellState(int x, int y, bool state)
            {
                board[x, y] = state;
            }
            // Set a player's score.
            public void setScore(int score)
            {
                this.score = score;
            }
            // Get a player's score.
            public int getScore()
            {
                return score;
            }
        }
        // Holds all details for the battleship game.
        class GameSettings
        {
            private Player[] player = new Player[2];                                // Player elements
            private bool player1Turn = true;                                        // Hold's the current player's turn.
            private bool ai = false;                                                // Indicator for AI mode.
            private int lastHitX, lastHitY, difficulty, totalTurns, boardLength;    // lastHitX & lastHitY used for AI intellect
            private string playerName;                                              // Used for the winner of a match in leaderboards.

            // Sets all of the initial values for the game upon load.
            public GameSettings()
            {
                difficulty = 0;
                player[0] = new Player();
                player[1] = new Player();
                player1Turn = true;
                ai = false;
                lastHitX = -1;
                lastHitY = -1;
                playerName = "";
                totalTurns = 0;
                boardLength = 9;
            }
            // Set a player's name
            public void setPlayerName(string name)
            {
                playerName = name;
            }
            // Get the game board length.
            public int getBoardLength()
            {
                return boardLength;
            }
            // Accesses a player's information.
            public Player getPlayer(int playerNumber)
            {
                return player[playerNumber];
            }
            // Increase counter for total turns by 1.
            public void incrementTurns()
            {
                totalTurns++;
            }
            // Reset turns after a match is reset.
            public void resetTurns()
            {
                totalTurns = 0;
            }
            // Sets the difficulty for an AI.
            public void setDifficulty(int diff)
            {
                difficulty = diff;
            }
            // Enable/Disable the AI.
            public void setAI(bool ai)
            {
                this.ai = ai;
            }
            // Set the current player's turn.
            public void setPlayer1Turn(bool turn)
            {
                player1Turn = turn;
            }
            // Get the total number of turns played so far.
            public int getTurns()
            {
                return totalTurns;
            }
            // Get the player/winner's name.
            public string getPlayerName()
            {
                return playerName;
            }
            // Checks the current difficulty setting.
            public int getDifficulty()
            {
                return difficulty;
            }
            // Checks if AI is enabled.
            public bool getAI()
            {
                return ai;
            }
            // Checks whose turn it is.
            public bool getPlayer1Turn()
            {
                return player1Turn;
            }
            // Removes all turns across both game boards.
            public void clearBoards()
            {
                player[0].clearBoard();
                player[1].clearBoard();
            }
            // Reset the scores of both players.
            public void resetScores()
            {
                player[0].setScore(0);
                player[1].setScore(0);
            }
            public int getLastHitX()
            {
                return lastHitX;
            }

            public int getLastHitY()
            {
                return lastHitY;
            }

            public void setLastHitX(int x)
            {
                lastHitX = x;
            }

            public void setLastHitY(int y)
            {
                lastHitY = y;
            }
        }

        // All elements that need to be public in order to be modified or receive data.
        Label lblPlayerTurn = new Label();
        TextBox scoreP1 = new TextBox();
        TextBox scoreP2 = new TextBox();
        TextBox pName = new TextBox();
        Button[,] BtnPlayer1Grid;
        Button[,] BtnPlayer2Grid;
        GameSettings gameSettings = new GameSettings();

        // Sound and music control for lines of code
        bool sound = true;
        SoundPlayer explosion = new SoundPlayer(@"../../explosion.wav");
        SoundPlayer music = new SoundPlayer(@"../../waves.wav");
        Font menuFont = new Font("Times New Roman", 18.0f);
        Image ocean = Image.FromFile(@"../../ocean.jpg");

        // Timer variables
        int count = 0;
        Label lblTimer = new Label();
        Timer t = new Timer();

        public Form1()
        {
            InitializeComponent();
            menuStrip();
            initMenu();
            // Adds sound button across each screen.
            soundButton(sound);
            t.Tick += new EventHandler(tickEvent);
            // Set background to an ocean.
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.BackgroundImage = ocean;
        }

        // needed to access code
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        // Ship Selection Screen visuals.
        private void initShipSelect()
        {
            // Declare grids and panels.
            Panel pnlBackGround = new Panel();
            BtnPlayer1Grid = new Button[9, 9];
            BtnPlayer2Grid = new Button[9, 9];
            Button BtnConfirm = new Button();
            // Labels for the grid.
            Label LblInstructions = new Label();
            Label[] LblGridTop = new Label[9];
            Label[] LblGridSide = new Label[9];

            pnlBackGround.BorderStyle = BorderStyle.FixedSingle;
            pnlBackGround.SetBounds((int)(this.ClientSize.Width / 4), 15, (this.ClientSize.Width / 2), this.ClientSize.Height - 30);
            pnlBackGround.BackColor = Color.LightBlue;
            // Change instruction text depending on whose grid is used
            if (gameSettings.getPlayer1Turn())
            {
                LblInstructions.ForeColor = Color.Blue;
                LblInstructions.Text = "(P1) Please select your ship locations:";
            }
            else
            {
                LblInstructions.ForeColor = Color.Red;
                LblInstructions.Text = "(P2) Please select your ship locations:";
            }
            LblInstructions.SetBounds((int)(this.ClientSize.Width / 3.45), 25, 400, 35);
            LblInstructions.TextAlign = ContentAlignment.TopCenter;
            LblInstructions.Font = new Font(menuFont, FontStyle.Bold);
            LblInstructions.BackColor = Color.LightBlue;

            // Letters used on side of grid to emulate real battleships.
            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

            BtnConfirm.SetBounds((int)(this.ClientSize.Width / 2) - 50, (int)(this.ClientSize.Height / 1.2), 100, 50);
            BtnConfirm.Text = "Confirm";
            BtnConfirm.Font = menuFont;
            BtnConfirm.Click += new EventHandler(BtnConfirmEvent_Click);

            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {
                // Set labels on top of the buttons
                LblGridTop[x] = new Label();
                LblGridTop[x].Text = Convert.ToString(letters[x]);
                LblGridTop[x].TextAlign = ContentAlignment.MiddleLeft;
                LblGridTop[x].Font = new Font(LblGridTop[x].Font, FontStyle.Bold);
                LblGridTop[x].BackColor = Color.LightBlue;
                LblGridTop[x].SetBounds((int)((this.ClientSize.Width / 3.25) + (40 * x)) + 15, ((int)(this.ClientSize.Height / 7.33333)) - 15, 12, 15);
                // Set labels along the side of the buttons.
                LblGridSide[x] = new Label();
                LblGridSide[x].SetBounds((int)(this.ClientSize.Width / 3.25) - 15, ((int)(this.ClientSize.Height / 7.33333)) + (40 * x) + 14, 10, 15);
                LblGridSide[x].Text = Convert.ToString(x + 1);
                LblGridSide[x].Font = new Font(LblGridSide[x].Font, FontStyle.Bold);
                LblGridSide[x].TextAlign = ContentAlignment.MiddleLeft;
                LblGridSide[x].BackColor = Color.LightBlue;
                // Add labels
                Controls.Add(LblGridTop[x]);
                Controls.Add(LblGridSide[x]);
                for (int y = 0; y < BtnPlayer1Grid.GetLength(1); y++)
                {
                    // Create all buttons on the grid.
                    BtnPlayer1Grid[x, y] = new Button();
                    BtnPlayer2Grid[x, y] = new Button();

                    BtnPlayer1Grid[x, y].SetBounds((int)(this.ClientSize.Width / 3.25) + (40 * x), ((int)(this.ClientSize.Height / 7)) + (40 * y), 40, 40);
                    BtnPlayer1Grid[x, y].BackColor = Color.PowderBlue;
                    
                    BtnPlayer2Grid[x, y].SetBounds((int)(this.ClientSize.Width / 3.25) + (40 * x), ((int)(this.ClientSize.Height / 7)) + (40 * y), 40, 40);
                    BtnPlayer2Grid[x, y].BackColor = Color.PowderBlue;

                    BtnPlayer1Grid[x, y].Click += new EventHandler(this.BtnPlayer1GridEvent_Click);
                    BtnPlayer2Grid[x, y].Click += new EventHandler(this.BtnPlayer1GridEvent_Click);

                    // Show the grid for the respective player.
                    if (gameSettings.getPlayer1Turn())
                        Controls.Add(BtnPlayer1Grid[x, y]);
                    else if (!gameSettings.getPlayer1Turn() && gameSettings.getDifficulty() == 3)
                    {
                        Controls.Add(BtnPlayer2Grid[x, y]);
                    }
                }
            }
            // Add in all elements.
            Controls.Add(pnlBackGround);
            Controls.Add(LblInstructions);
            Controls.Add(BtnConfirm);
            pnlBackGround.SendToBack();
        }
        // Randomise the AI's ships.
        void randomiseSelection()
        {
            // Clear the board in case there's anything left.
            gameSettings.getPlayer(1).clearBoard();
            Random random = new Random();

            int shipSize = 5;
            int cointoss;
            int rdmX;
            int rdmY;
            bool firstThirdAdded = false;
            // While there are still ships to add.
            while (shipSize > 1)
            {
                // Randomise a starting coordinate.
                rdmX = random.Next(0, 9);
                rdmY = random.Next(0, 9);
                cointoss = random.Next(2);
                // Ship will be placed horizontally
                if (cointoss == 0)
                {
                    bool validated = false;
                    while (!validated)
                    {
                        // Ship must be placed towards right side to avoid going out of bounds
                        if (rdmX + (shipSize - 1) < gameSettings.getBoardLength())
                        {
                            bool shipOverlaps = false;
                            // Checks if the placement will overlap with an existing ship.
                            for (int x = 0; x < shipSize; x++)
                            {
                                if (gameSettings.getPlayer(1).getBoardCellState(rdmX + x, rdmY))
                                {
                                    shipOverlaps = true;
                                    break;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                // Check the next row over to see if the ship can be placed there.
                                int y = rdmY + 1;
                                if (y > 8)
                                {
                                    y = 0;
                                }
                                // While a valid placement is yet to be found.
                                while (!foundValidPlacement && y != rdmY)
                                {
                                    // Reset y coordinate if it reaches the outer bounds.
                                    if (y > 8)
                                    {
                                        y = 0;
                                    }
                                    bool shipCanFit = true;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        // If the ship cannot fit in this spot.
                                        if (gameSettings.getPlayer(1).getBoardCellState(rdmX + x, y))
                                        {
                                            shipCanFit = false;
                                            break;
                                        }
                                    }
                                    if (shipCanFit)
                                    {
                                        foundValidPlacement = true;
                                    }
                                    // Search the next cell over.
                                    else
                                    {
                                        y++;
                                    }
                                }
                                // Ship can be placed here.
                                if (foundValidPlacement)
                                {
                                    // Set the new Y coordinate.
                                    rdmY = y;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        // Add this new ship onto the AI's grid.
                                        gameSettings.getPlayer(1).setBoardCellState(rdmX + x, rdmY, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    // Randomise another coordinate and run the checks again.
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            // If the ship doesn't overlap with anything first try.
                            else
                            {
                                // Add this ship straight into the grid.
                                for (int x = 0; x < shipSize; x++)
                                {
                                    gameSettings.getPlayer(1).setBoardCellState(rdmX + x, rdmY, true);
                                }
                                validated = true;
                            }
                        }
                        // Ship must be placed towards left side to avoid going out of bounds
                        else
                        {
                            bool shipOverlaps = false;
                            // Checks if the placement will overlap with an existing ship.
                            for (int x = 0; x < shipSize; x++)
                            {
                                // If the ship cannot fit in this spot.
                                if (gameSettings.getPlayer(1).getBoardCellState(rdmX - x, rdmY))
                                {
                                    shipOverlaps = true;
                                    break;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                // Check the next row over to see if the ship can be placed there.
                                int y = rdmY - 1;
                                if (y < 0)
                                {
                                    y = 8;
                                }
                                // While a valid placement is yet to be found.
                                while (!foundValidPlacement && y != rdmY)
                                {
                                    // Reset y coordinate if it reaches the outer bounds.
                                    if (y < 0)
                                    {
                                        y = 8;
                                    }
                                    bool shipCanFit = true;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        // If the ship cannot fit in this spot.
                                        if (gameSettings.getPlayer(1).getBoardCellState(rdmX - x, y))
                                        {
                                            shipCanFit = false;
                                            break;
                                        }
                                    }
                                    if (shipCanFit)
                                    {
                                        foundValidPlacement = true;
                                    }
                                    else
                                    {
                                        // Check the next row over.
                                        y++;
                                    }
                                }
                                if (foundValidPlacement)
                                {
                                    // Set the new Y coordinate.
                                    rdmY = y;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        // Add this ship into the board.
                                        gameSettings.getPlayer(1).setBoardCellState(rdmX - x, rdmY, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    // Randomise another coordinate and run the checks again.
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            // Ship doesn't overlap, ignore checks.
                            else
                            {
                                // Add ship to grid.
                                for (int x = 0; x < shipSize; x++)
                                {
                                    gameSettings.getPlayer(1).setBoardCellState(rdmX - x, rdmY, true);
                                }
                                validated = true;
                            }
                        }
                    }
                }
                // Ship will be placed vertically
                else
                {
                    // Ship can be placed towards the bottom without going out of bounds
                    bool shipOverlaps = false;
                    bool validated = false;
                    while (!validated)
                    {
                        if (rdmY + (shipSize - 1) < gameSettings.getBoardLength())
                        {
                            // Checks if the placement will overlap with an existing ship.
                            for (int y = 0; y < shipSize; y++)
                            {
                                // If the ship cannot fit in this spot.
                                if (gameSettings.getPlayer(1).getBoardCellState(rdmX, rdmY + y))
                                {
                                    shipOverlaps = true;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                // Check the next row over to see if the ship can be placed there.
                                int x = rdmX + 1;
                                if (x > 8)
                                {
                                    x = 0;
                                }
                                // While a valid placement is yet to be found.
                                while (!foundValidPlacement && x != rdmX)
                                {
                                    // Keep value within bounds.
                                    if (x > 8)
                                    {
                                        x = 0;
                                    }
                                    bool shipCanFit = true;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        // If the ship cannot fit in this spot.
                                        if (gameSettings.getPlayer(1).getBoardCellState(x, rdmY + y))
                                        {
                                            shipCanFit = false;
                                        }
                                    }
                                    if (shipCanFit)
                                    {
                                        foundValidPlacement = true;
                                    }
                                    else
                                    {
                                        // Check the next row over.
                                        x++;
                                    }
                                }
                                if (foundValidPlacement)
                                {
                                    // Set new X coordinate.
                                    rdmX = x;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        // Add this ship into the board.
                                        gameSettings.getPlayer(1).setBoardCellState(rdmX, rdmY + y, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    // Randomise a new starting location and run checks again.
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            else
                            {
                                // Ship doesn't overlap, add to board.
                                for (int y = 0; y < shipSize; y++)
                                {
                                    gameSettings.getPlayer(1).setBoardCellState(rdmX, rdmY + y, true);
                                }
                                validated = true;
                            }
                        }
                        else
                        {
                            // Ship can be placed towards the top without going out of bounds
                            // checks if the placement will overlap with an existing ship.
                            for (int y = 0; y < shipSize; y++)
                            {
                                if (gameSettings.getPlayer(1).getBoardCellState(rdmX, rdmY - y))
                                {
                                    shipOverlaps = true;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                // Check the next row over.
                                int x = rdmX - 1;
                                if (x < 0)
                                {
                                    x = 8;
                                }
                                // While a valid placement is yet to be found.
                                while (!foundValidPlacement && x != rdmX)
                                {
                                    // Keeps coordinate within bounds.
                                    if (x < 0)
                                    {
                                        x = 8;
                                    }
                                    bool shipCanFit = true;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        // If the ship overlaps with another in this coordinate.
                                        if (gameSettings.getPlayer(1).getBoardCellState(x, rdmY - y))
                                        {
                                            shipCanFit = false;
                                        }
                                    }
                                    if (shipCanFit)
                                    {
                                        foundValidPlacement = true;
                                    }
                                    else
                                    {
                                        // Run the check on the next row.
                                        x--;
                                    }
                                }
                                // Ship can go here.
                                if (foundValidPlacement)
                                {
                                    // Move ship to this X.
                                    rdmX = x;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        // Add ship to board.
                                        gameSettings.getPlayer(1).setBoardCellState(rdmX, rdmY - y, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    // Randomise a new coordinate and run check again.
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            else
                            {
                                for (int y = 0; y < shipSize; y++)
                                {
                                    gameSettings.getPlayer(1).setBoardCellState(rdmX, rdmY - y, true);
                                }
                                validated = true;
                            }
                        }
                    }
                }
                // Value doesn't decrement if both 3 ship-sizes haven't been placed.
                if (shipSize == 3 && !firstThirdAdded)
                {
                    firstThirdAdded = true;
                }
                // Add another ship.
                else
                {
                    shipSize--;
                }
            }
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
            int p = int.Parse(name[1].ToString());
            int x = int.Parse(name[2].ToString());
            int y = int.Parse(name[3].ToString());

            // for playMusic to ensure explosion and music does not interrupt each other
            Timer musicTimer = new Timer();
            musicTimer.Interval = 1250;

            // checks whose turn it is
            if (gameSettings.getPlayer1Turn() == true)
            {
                if (p == 1) // it is player 1's turn and if they select a button on their own side or a button on the enemy side that has already been chosen then through message box errors
                {
                    MessageBox.Show("You Can't Target Your Own Side.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (((Button)sender).BackColor == Color.Red || ((Button)sender).BackColor == Color.White)
                {
                    MessageBox.Show("You Can't Target A Spot You've Already Targeted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (gameSettings.getPlayer(1).getBoardCellState(x, y) == true) // if chosen button is a hit then turn the button red otherwise turn it white
                    {
                        ((Button)sender).BackColor = Color.Red;
                        gameSettings.getPlayer(0).setScore(gameSettings.getPlayer(0).getScore() + 1);
                        scoreP1.Text = Convert.ToString(gameSettings.getPlayer(0).getScore());
                        playMusic(false);
                        musicTimer.Tick += new EventHandler(playMusicEvent_Timer);
                        musicTimer.Start();
                        playSound();
                    }
                    else
                        ((Button)sender).BackColor = Color.White;

                    CheckWin();
                    counterSet();
                    gameSettings.incrementTurns();
                    gameSettings.setPlayer1Turn(false);
                    lblPlayerTurn.Text = "Player 2's Turn";
                    if (gameSettings.getAI())
                        AIPlacement();
                }
            }
            else // start of player 2's turn but if it is against an AI then this doesn't run as there is not real second player
            {
                if (gameSettings.getAI() == false)
                {
                    if (p == 2) //same errors thrown but for player 2
                    {
                        MessageBox.Show("You Can't Target Your Own Side.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (((Button)sender).BackColor == Color.Red || ((Button)sender).BackColor == Color.White)
                    {
                        MessageBox.Show("You Can't Target A Spot You've Already Targeted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (gameSettings.getPlayer(0).getBoardCellState(x, y) == true) //same as earlier
                        {
                            ((Button)sender).BackColor = Color.Red;
                            gameSettings.getPlayer(1).setScore(gameSettings.getPlayer(1).getScore() + 1);
                            scoreP2.Text = Convert.ToString(gameSettings.getPlayer(1).getScore());
                            playMusic(false);
                            musicTimer.Tick += new EventHandler(playMusicEvent_Timer);
                            musicTimer.Start();
                            playSound();
                        }
                        else
                            ((Button)sender).BackColor = Color.White;

                        CheckWin();
                        counterSet();
                        gameSettings.setPlayer1Turn(true);
                        lblPlayerTurn.Text = "Player 1's Turn";
                    }
                }
            }
        }

        // AI placement of the bombs
        void AIPlacement()
        {
            Timer musicTimer = new Timer();
            musicTimer.Interval = 1250;

            // code for ai selection of bomb
            Random rnd = new Random();
            bool guess = true;
            bool vertical = true;

            // depending on the difficulty, the chance of the ai making an educated guess increases as the game gets harder
            if (gameSettings.getLastHitX() != -1) //if the last guess was a hit then use that as the base for the next guess
            {
                int z = rnd.Next(4);
                if (gameSettings.getDifficulty() == 0 && (z == 3)) //25% chance of making an educated guess
                    guess = false;
                else if (gameSettings.getDifficulty() == 1 && (z == 2 || z == 3)) //50% chance of making an educated guess
                    guess = false;
                else if (gameSettings.getDifficulty() == 2) //Always makes an educated guess as its meant to be hard
                    guess = false;
            }

            //code to check the surrounding buttons of the last hit ship and chooses one based on the position of it
            //looks out for already hit ships using background color red and also checks if the button is white so that it goes in a different direction
            if (guess == false)
            {
                int tempX = -1;
                int tempY = -1;
                if (gameSettings.getLastHitX() == 0 && gameSettings.getLastHitY() == 0) // top left corner
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.White)
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.Red)
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0) // randomise whether the placement will be vertical or horizontal if no clear option 
                        vertical = false;
                }
                else if (gameSettings.getLastHitX() == 0 && gameSettings.getLastHitY() == 8) // bottom left corner
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.White)
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.Red)
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }
                else if (gameSettings.getLastHitX() == 8 && gameSettings.getLastHitY() == 0) // top right corner
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.White)
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.Red)
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }
                else if (gameSettings.getLastHitX() == 8 && gameSettings.getLastHitY() == 8) // bottom right corner
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.White)
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.Red)
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }
                else if (gameSettings.getLastHitX() == 0) // along the top edge
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.Red || (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.White && BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.White))
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.Red)
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }
                else if (gameSettings.getLastHitX() == 8) // along the bottom edge
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.Red || (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.White && BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.White))
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.Red)
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }
                else if (gameSettings.getLastHitY() == 0) // along the left edge
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.White)
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.Red || (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.White && BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.White))
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }
                else if (gameSettings.getLastHitY() == 8) // along the right edge
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.White)
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.Red || (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.White && BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.White))
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }
                else // anywhere in the middle of the board
                {
                    if (BtnPlayer1Grid[gameSettings.getLastHitX() - 1, gameSettings.getLastHitY()].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX() + 1, gameSettings.getLastHitY()].BackColor == Color.Red || (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.White && BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.White))
                        vertical = false;
                    else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - 1].BackColor == Color.Red || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + 1].BackColor == Color.Red)
                        vertical = true;
                    else
                        if (rnd.Next(2) == 0)
                        vertical = false;
                }

                int counter = 0;

                // depending on the previous code set, vertical will either be true or false indicating where to place the educated guess
                if (vertical == true)
                {

                    if (gameSettings.getLastHitY() == 0) // vertical placement only depends on the Y value so that is the one being checked and if 0 then it is at the top of the board
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + i].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + i].BackColor == Color.PowderBlue)
                            {
                                tempX = gameSettings.getLastHitX();
                                tempY = gameSettings.getLastHitY() + i;
                                counter++;
                                break;
                            }
                            else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + i].BackColor == Color.White)
                                break;
                        }
                    }
                    else if (gameSettings.getLastHitY() == 8) // checked if 8 then the last hit ship will be at the bottom of the board
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - i].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - i].BackColor == Color.PowderBlue)
                            {
                                tempX = gameSettings.getLastHitX();
                                tempY = gameSettings.getLastHitY() - i;
                                counter++;
                                break;
                            }
                            else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - i].BackColor == Color.White)
                                break;
                        }
                    }
                    else // if not at bottom or top of the board then check both directions (up and down) from the last hit ship and check if its already white or red so that it either keeps going if red or stops if white
                    {
                        for (int i = 1; i < 5; i++) // only for loops 4 times as the longest ship is 5 long so there is no point to keep going.
                        {
                            if (gameSettings.getLastHitY() + i < 9) // this checks downwards
                            {
                                if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + i].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + i].BackColor == Color.PowderBlue)
                                {
                                    tempX = gameSettings.getLastHitX();
                                    tempY = gameSettings.getLastHitY() + i;
                                    counter++;
                                    break;
                                }
                                else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() + i].BackColor == Color.White)
                                    break;
                            }
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            if (gameSettings.getLastHitY() - i > -1) // this checks upwards
                            {
                                if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - i].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - i].BackColor == Color.PowderBlue)
                                {
                                    tempX = gameSettings.getLastHitX();
                                    tempY = gameSettings.getLastHitY() - i;
                                    counter++;
                                    break;
                                }
                                else if (BtnPlayer1Grid[gameSettings.getLastHitX(), gameSettings.getLastHitY() - i].BackColor == Color.White)
                                    break;
                            }
                        }
                    }

                }
                else // this is the code for horizontal placement and does the same thing as vertical except with the x axis
                {
                    if (gameSettings.getLastHitX() == 0)
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            if (BtnPlayer1Grid[gameSettings.getLastHitX() + i, gameSettings.getLastHitY()].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX() + i, gameSettings.getLastHitY()].BackColor == Color.PowderBlue)
                            {
                                tempX = gameSettings.getLastHitX() + i;
                                tempY = gameSettings.getLastHitY();
                                counter++;
                                break;
                            }
                            else if (BtnPlayer1Grid[gameSettings.getLastHitX() + i, gameSettings.getLastHitY()].BackColor == Color.White)
                                break;
                        }
                    }
                    else if (gameSettings.getLastHitX() == 8)
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            if (BtnPlayer1Grid[gameSettings.getLastHitX() - i, gameSettings.getLastHitY()].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX() - i, gameSettings.getLastHitY()].BackColor == Color.PowderBlue)
                            {
                                tempX = gameSettings.getLastHitX() - i;
                                tempY = gameSettings.getLastHitY();
                                counter++;
                                break;
                            }
                            else if (BtnPlayer1Grid[gameSettings.getLastHitX() - i, gameSettings.getLastHitY()].BackColor == Color.White)
                                break;
                        }
                    }
                    else
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            if (gameSettings.getLastHitX() + i < 9)
                            {
                                if (BtnPlayer1Grid[gameSettings.getLastHitX() + i, gameSettings.getLastHitY()].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX() + i, gameSettings.getLastHitY()].BackColor == Color.PowderBlue)
                                {
                                    tempX = gameSettings.getLastHitX() + i;
                                    tempY = gameSettings.getLastHitY();
                                    counter++;
                                    break;
                                }
                                else if (BtnPlayer1Grid[gameSettings.getLastHitX() + i, gameSettings.getLastHitY()].BackColor == Color.White)
                                    break;
                            }
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            if (gameSettings.getLastHitX() - i > -1)
                            {
                                if (BtnPlayer1Grid[gameSettings.getLastHitX() - i, gameSettings.getLastHitY()].BackColor == Color.Gray || BtnPlayer1Grid[gameSettings.getLastHitX() - i, gameSettings.getLastHitY()].BackColor == Color.PowderBlue)
                                {
                                    tempX = gameSettings.getLastHitX() - i;
                                    tempY = gameSettings.getLastHitY();
                                    counter++;
                                    break;
                                }
                                else if (BtnPlayer1Grid[gameSettings.getLastHitX() - i, gameSettings.getLastHitY()].BackColor == Color.White)
                                    break;
                            }
                        }
                    }
                }

                // if counter is 0 after all checks then that means the ship mustve been destroyed meaning there was no logical guess other than a random one
                if (counter == 0)
                {
                    //resets the last hit co-ordinates as they are not needed anymore
                    gameSettings.setLastHitX(-1);
                    gameSettings.setLastHitY(-1);
                    guess = true; // set guess to true so that it does a random guess
                }
                else // if counter is not 0 then it will place the guess at the position that was calculated to be the best from the previous code section
                {
                    if (gameSettings.getPlayer(0).getBoardCellState(tempX, tempY) == true) //if that educated guess was a hit then turn the button red
                    {
                        BtnPlayer1Grid[tempX, tempY].BackColor = Color.Red;
                        gameSettings.getPlayer(1).setScore(gameSettings.getPlayer(1).getScore() + 1); // add one to score label
                        scoreP2.Text = Convert.ToString(gameSettings.getPlayer(1).getScore());
                        playMusic(false);
                        musicTimer.Tick += new EventHandler(playMusicEvent_Timer);
                        musicTimer.Start();
                        playSound();
                        gameSettings.setLastHitX(tempX);
                        gameSettings.setLastHitY(tempY);

                    }
                    else // if a miss then turn it white
                    {
                        BtnPlayer1Grid[tempX, tempY].BackColor = Color.White;
                    }
                    CheckWin();
                    counterSet();
                    gameSettings.setPlayer1Turn(true);
                    lblPlayerTurn.Text = "Player 1's Turn";
                }

            }

            // this runs when the randomiser didnt result in an educated guess or the educated guess found no good placement
            if (guess == true)
            {
                int xval = rnd.Next(9); // randomises x and y co-ordinates
                int yval = rnd.Next(9);
                while (BtnPlayer1Grid[xval, yval].BackColor == Color.Red || BtnPlayer1Grid[xval, yval].BackColor == Color.White) // keeps randomising until a button has been chosen that hasnt already been chosen before so no repeats happen
                {
                    xval = rnd.Next(9);
                    yval = rnd.Next(9);
                }

                if (gameSettings.getPlayer(0).getBoardCellState(xval, yval) == true) // do same as before, put guess at co-ordinates and if hit then button red, if miss then button white
                {
                    BtnPlayer1Grid[xval, yval].BackColor = Color.Red;
                    gameSettings.getPlayer(1).setScore(gameSettings.getPlayer(1).getScore() + 1);
                    scoreP2.Text = Convert.ToString(gameSettings.getPlayer(1).getScore());
                    playMusic(false);
                    musicTimer.Tick += new EventHandler(playMusicEvent_Timer);
                    musicTimer.Start();
                    playSound();
                    gameSettings.setLastHitX(xval);
                    gameSettings.setLastHitY(yval);

                }
                else
                {
                    BtnPlayer1Grid[xval, yval].BackColor = Color.White;
                }
                CheckWin();
                counterSet();
                gameSettings.setPlayer1Turn(true);
                lblPlayerTurn.Text = "Player 1's Turn";
            }
        }

        // Confirm the grid placements.
        void BtnConfirmEvent_Click(object sender, EventArgs e)
        {
            // Check if the selections made are valid.
            if (isSelectionValid())
            {
                bool startGame = false;
                clearForm();

                if (gameSettings.getDifficulty() == 3 && gameSettings.getPlayer1Turn())
                {
                    gameSettings.setPlayer1Turn(false);
                    initShipSelect();
                }
                else if (gameSettings.getDifficulty() == 3 && !gameSettings.getPlayer1Turn())
                {
                    gameSettings.setPlayer1Turn(true);
                    startGame = true;
                }
                // Match is against an AI.
                if (gameSettings.getDifficulty() != 3) 
                {
                    playGame();
                    randomiseSelection();
                }
                // player vs player
                else if (startGame)
                {
                    playGame();
                }
            }
        }

        // Check the player's selections to see if it's valid.
        bool isSelectionValid()
        {
            int numberOfShipGrids = 0;
            // Count all of the number of grid cells selected.
            if (gameSettings.getPlayer1Turn())
            {
                for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
                {
                    for (int y = 0; y < BtnPlayer1Grid.GetLength(0); y++)
                    {
                        Debug.WriteLine(Convert.ToString(x) + " " + Convert.ToString(y) + ": " + BtnPlayer1Grid[x, y].BackColor);
                        if (BtnPlayer1Grid[x, y].BackColor == Color.Gray)
                        {
                            numberOfShipGrids++;
                        }
                    }
                }
            }
            if (!gameSettings.getPlayer1Turn() && gameSettings.getDifficulty() == 3)
            {
                for (int x = 0; x < BtnPlayer2Grid.GetLength(0); x++)
                {
                    for (int y = 0; y < BtnPlayer2Grid.GetLength(0); y++)
                    {
                        Debug.WriteLine(Convert.ToString(x) + " " + Convert.ToString(y) + ": " + BtnPlayer2Grid[x, y].BackColor);
                        if (BtnPlayer2Grid[x, y].BackColor == Color.Gray)
                        {
                            numberOfShipGrids++;
                        }
                    }
                }
            }

            // Minimum number of cells selected needs to be 17.
            if (numberOfShipGrids != 17)
            {
                MessageBox.Show("Invalid amount of ship placed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            bool foundFirstThree = false;
            bool validPlacements = true;
            // Find all of the selected ships in the grid.
            for (int i = 5; i > 1; i--)
            {
                // If a ship can't be found.
                if (!findShipInSelection(i))
                {
                    validPlacements = false;
                    break;
                }
                // Checks for two ships.
                if (i == 3 && !foundFirstThree)
                {
                    i++;
                    foundFirstThree = true;
                }
            }
            // If the ship placements aren't valid
            if (!validPlacements)
            {
                MessageBox.Show("Your ship placements are not valid, please place your ships horizontally or vertically with at least 1 space between them. The valid ship sizes are: \n\nCarrier (5 boxes), \nBattleship (4 boxes) \nCruiser (3 boxes) \nSubmarine (3 boxes) \nDestroyer (2 boxes)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // Find the ship on the selection screen and see if the placement is valid.
        bool findShipInSelection(int amountToFind)
        {
            if (gameSettings.getPlayer1Turn())
            {
                // HORIZONTAL CHECKING
                // Defines the search space to look for the ships.
                for (int x = 0; x < gameSettings.getBoardLength() - (amountToFind - 1); x++)
                {
                    for (int y = 0; y < gameSettings.getBoardLength(); y++)
                    {
                        // Set this to 0 for indexing.
                        int val = 0;
                        // Check to see if there exists a ship pattern.
                        while (val != amountToFind)
                        {
                            if (y != 0 && y != 8)
                            {
                                // Checks to see if the spaces in, above and around this ship are vacant.
                                if (BtnPlayer1Grid[x + val, y].BackColor != Color.PowderBlue && BtnPlayer1Grid[x + val, y + 1].BackColor == Color.PowderBlue && BtnPlayer1Grid[x + val, y - 1].BackColor == Color.PowderBlue)
                                {
                                    // Checks space behind the ship
                                    if (val == 0 && x != 0)
                                    {
                                        
                                        if (BtnPlayer1Grid[x - 1, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    // Checks space in front of the ship
                                    if (x + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer1Grid[x + amountToFind, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else if (y == 0)
                            {
                                // Checks to see if the spaces in, and below this ship are vacant.
                                if (BtnPlayer1Grid[x + val, y].BackColor != Color.PowderBlue && BtnPlayer1Grid[x + val, y + 1].BackColor == Color.PowderBlue)
                                {
                                    // Checks space behind the ship
                                    if (val == 0 && x != 0)
                                    {
                                        if (BtnPlayer1Grid[x - 1, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    // Checks space in front of the ship
                                    if (x + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer1Grid[x + amountToFind, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else
                            {
                                // Checks to see if the spaces in, and above this ship are vacant.
                                if (BtnPlayer1Grid[x + val, y].BackColor != Color.PowderBlue && BtnPlayer1Grid[x + val, y - 1].BackColor == Color.PowderBlue)
                                {
                                    // Checks space behind the ship
                                    if (val == 0 && x != 0)
                                    {
                                        if (BtnPlayer1Grid[x - 1, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    // Checks space in front of the ship.
                                    if (x + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer1Grid[x + amountToFind, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                        }
                        // If a VALID ship has been found.
                        if (val == amountToFind)
                        {
                            val = 0;
                            bool found = false;
                            // Check to see if this selection encapsulates another.
                            // i.e. 4 cells can exist within a 5 cell ship, so this must be checked..
                            while (val < amountToFind)
                            {
                                if (gameSettings.getPlayer(0).getBoardCellState(x + val, y))
                                {
                                    found = true;
                                    break;
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
                                    gameSettings.getPlayer(0).setBoardCellState(x + i, y, true);
                                }
                                return true;
                            }
                        }
                    }
                }
                // VERTICAL CHECKING
                // Defines the search space to look for the ships.
                for (int x = 0; x < gameSettings.getBoardLength(); x++)
                {
                    for (int y = 0; y < gameSettings.getBoardLength() - (amountToFind - 1); y++)
                    {
                        // Set this to 0 for indexing.
                        int val = 0;
                        // Checks to see if a ship pattern exists on the grid.
                        while (val < amountToFind)
                        {
                            if (x != 0 && x != 8)
                            {
                                // If there's nothing in, above, or below the ship placement.
                                if (BtnPlayer1Grid[x, y + val].BackColor != Color.PowderBlue && BtnPlayer1Grid[x - 1, y + val].BackColor == Color.PowderBlue && BtnPlayer1Grid[x + 1, y + val].BackColor == Color.PowderBlue)
                                {
                                    // Checks space behind the ship.
                                    if (val == 0 && y != 0)
                                    {
                                        if (BtnPlayer1Grid[x, y - 1].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    // Checks space in front of the ship
                                    if (y + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer1Grid[x, y + amountToFind].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else if (x == 0)
                            {
                                // If there's nothing in, or below the ship placement.
                                if (BtnPlayer1Grid[x, y + val].BackColor != Color.PowderBlue && BtnPlayer1Grid[x + 1, y + val].BackColor == Color.PowderBlue)
                                {
                                    // Checks space behind the ship.
                                    if (val == 0 && y != 0)
                                    {
                                        if (BtnPlayer1Grid[x, y - 1].BackColor != Color.PowderBlue)
                                        {
                                            break;
                                        }
                                    }
                                    val++;
                                    // Checks space in front of the ship
                                    if (y + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer1Grid[x, y + amountToFind].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else
                            {
                                // If there's nothing in or above the ship placement.
                                if (BtnPlayer1Grid[x, y + val].BackColor != Color.PowderBlue && BtnPlayer1Grid[x - 1, y + val].BackColor == Color.PowderBlue)
                                {
                                    if (val == 0 && y != 0)
                                    {
                                        if (BtnPlayer1Grid[x, y - 1].BackColor != Color.PowderBlue)
                                        {
                                            break;
                                        }
                                    }
                                    val++;
                                    if (y + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer1Grid[x, y + amountToFind].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
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
                                if (gameSettings.getPlayer(0).getBoardCellState(x, y + val))
                                {
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    val++;
                                }
                            }
                            // If the ship doesn't overlap with others in the grid.
                            if (!found)
                            {
                                // Add into gameSettings board.
                                for (int i = 0; i < amountToFind; i++)
                                {
                                    gameSettings.getPlayer(0).setBoardCellState(x, y + i, true);
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                // HORIZONTAL CHECKING FOR PLAYER 2
                // Defines the search space to look for the ships.
                for (int x = 0; x < gameSettings.getBoardLength() - (amountToFind - 1); x++)
                {
                    for (int y = 0; y < gameSettings.getBoardLength(); y++)
                    {
                        // Set this to 0 for indexing.
                        int val = 0;
                        // Check to see if there exists a ship pattern.
                        while (val != amountToFind)
                        {
                            if (y != 0 && y != 8)
                            {
                                // Checks to see if the spaces in, above and around this ship are vacant.
                                if (BtnPlayer2Grid[x + val, y].BackColor != Color.PowderBlue && BtnPlayer2Grid[x + val, y + 1].BackColor == Color.PowderBlue && BtnPlayer2Grid[x + val, y - 1].BackColor == Color.PowderBlue)
                                {
                                    if (val == 0 && x != 0)
                                    {
                                        if (BtnPlayer2Grid[x - 1, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    if (x + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer2Grid[x + amountToFind, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else if (y == 0)
                            {
                                // Checks to see if the spaces in, and below this ship are vacant.
                                if (BtnPlayer2Grid[x + val, y].BackColor != Color.PowderBlue && BtnPlayer2Grid[x + val, y + 1].BackColor == Color.PowderBlue)
                                {
                                    if (val == 0 && x != 0)
                                    {
                                        if (BtnPlayer2Grid[x - 1, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    if (x + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer2Grid[x + amountToFind, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else
                            {
                                // Checks to see if the spaces in, and above this ship are vacant.
                                if (BtnPlayer2Grid[x + val, y].BackColor != Color.PowderBlue && BtnPlayer2Grid[x + val, y - 1].BackColor == Color.PowderBlue)
                                {
                                    if (val == 0 && x != 0)
                                    {
                                        if (BtnPlayer2Grid[x - 1, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    if (x + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer2Grid[x + amountToFind, y].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                        }
                        // If a VALID ship has been found.
                        if (val == amountToFind)
                        {
                            val = 0;
                            bool found = false;
                            // Check to see if this selection encapsulates another.
                            // i.e. 4 cells can exist within a 5 cell ship, so this must be checked..
                            while (val < amountToFind)
                            {
                                if (gameSettings.getPlayer(1).getBoardCellState(x + val, y))
                                {
                                    found = true;
                                    break;
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
                                    gameSettings.getPlayer(1).setBoardCellState(x + i, y, true);
                                }
                                return true;
                            }
                        }
                    }
                }
                // VERTICAL CHECKING
                // Defines the search space to look for the ships.
                for (int x = 0; x < gameSettings.getBoardLength(); x++)
                {
                    for (int y = 0; y < gameSettings.getBoardLength() - (amountToFind - 1); y++)
                    {
                        // Set this to 0 for indexing.
                        int val = 0;
                        // Checks to see if a ship pattern exists on the grid.
                        while (val < amountToFind)
                        {
                            if (x != 0 && x != 8)
                            {
                                if (BtnPlayer2Grid[x, y + val].BackColor != Color.PowderBlue && BtnPlayer2Grid[x - 1, y + val].BackColor == Color.PowderBlue && BtnPlayer2Grid[x + 1, y + val].BackColor == Color.PowderBlue)
                                {
                                    if (val == 0 && y != 0)
                                    {
                                        if (BtnPlayer2Grid[x, y - 1].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                    val++;
                                    if (y + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer2Grid[x, y + amountToFind].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else if (x == 0)
                            {
                                if (BtnPlayer2Grid[x, y + val].BackColor != Color.PowderBlue && BtnPlayer2Grid[x + 1, y + val].BackColor == Color.PowderBlue)
                                {
                                    if (val == 0 && y != 0)
                                    {
                                        if (BtnPlayer2Grid[x, y - 1].BackColor != Color.PowderBlue)
                                        {
                                            break;
                                        }
                                    }
                                    val++;
                                    if (y + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer2Grid[x, y + amountToFind].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
                            }
                            else
                            {
                                if (BtnPlayer2Grid[x, y + val].BackColor != Color.PowderBlue && BtnPlayer2Grid[x - 1, y + val].BackColor == Color.PowderBlue)
                                {
                                    if (val == 0 && y != 0)
                                    {
                                        if (BtnPlayer2Grid[x, y - 1].BackColor != Color.PowderBlue)
                                        {
                                            break;
                                        }
                                    }
                                    val++;
                                    if (y + amountToFind <= 8 && val == amountToFind)
                                    {
                                        if (BtnPlayer2Grid[x, y + amountToFind].BackColor != Color.PowderBlue)
                                        {
                                            val = 6;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // forcibly break out of loop.
                                    break;
                                }
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
                                if (gameSettings.getPlayer(1).getBoardCellState(x, y + val))
                                {
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    val++;
                                }
                            }
                            // If the ship doesn't overlap with others in the grid.
                            if (!found)
                            {
                                // Add into gameSettings board.
                                for (int i = 0; i < amountToFind; i++)
                                {
                                    gameSettings.getPlayer(1).setBoardCellState(x, y + i, true);
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            if (gameSettings.getPlayer1Turn())
            {
                gameSettings.getPlayer(0).clearBoard();
            }
            else
            {
                gameSettings.getPlayer(1).clearBoard();
            }
            MessageBox.Show("Your ship placements are not valid, please place your ships horizontally or vertically with at least 1 space between them. The valid ship sizes are: \n\nCarrier (5 boxes), \nBattleship (4 boxes) \nCruiser (3 boxes) \nSubmarine (3 boxes) \nDestroyer (2 boxes)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // Display the main menu
        private void initMenu()
        {
            // Reset this when loading the menu.
            gameSettings.setPlayer1Turn(true);
            Button btnStart = new Button();
            Button btnRules = new Button();
            Button btnScores = new Button();

            btnStart.SetBounds(this.ClientSize.Width / 2 - 75, this.ClientSize.Height / 2 + 60, 150, 50);
            btnRules.SetBounds(this.ClientSize.Width / 2 - 75, this.ClientSize.Height / 2 + 120, 150, 50);
            btnScores.SetBounds(this.ClientSize.Width / 2 - 75, this.ClientSize.Height / 2 + 180, 150, 50);

            btnStart.Font = menuFont;
            btnRules.Font = menuFont;
            btnScores.Font = menuFont;
            btnStart.Text = "Start";
            btnRules.Text = "Rules";
            btnScores.Text = "Highscores";
            // Adds graphic of battleship.
            PictureBox picBoat = new PictureBox();
            picBoat.ImageLocation = "../../Battleship.png";
            picBoat.Size = new Size(700, 400);
            picBoat.Location = new Point(this.ClientSize.Width / 2 - 240, this.ClientSize.Height / 2 - 230);
            picBoat.BackColor = Color.Transparent;
            // Event handlers for each button.
            btnStart.Click += new EventHandler(this.btnStartEvent_Click);
            btnRules.Click += new EventHandler(this.btnRulesEvent_Click);
            btnScores.Click += new EventHandler(this.btnHighscoresEvent_Click);
            // Add all elements.
            Controls.Add(btnStart);
            Controls.Add(btnRules);
            Controls.Add(btnScores);
            Controls.Add(picBoat);
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

            PictureBox picBoat = new PictureBox();
            picBoat.ImageLocation = "../../Battleship.png";
            picBoat.Size = new Size(700, 275);
            picBoat.Location = new Point(this.ClientSize.Width / 2 - 230, this.ClientSize.Height / 2 - 230);
            picBoat.BackColor = Color.Transparent;
            Controls.Add(picBoat);
            // 
            btns[0].SetBounds(this.ClientSize.Width / 2 - 160, this.ClientSize.Height / 2 + 50, 100, 50);
            btns[1].SetBounds(this.ClientSize.Width / 2 - 45, this.ClientSize.Height / 2 + 50, 100, 50);
            btns[2].SetBounds(this.ClientSize.Width / 2 + 70, this.ClientSize.Height / 2 + 50, 100, 50);
            btns[0].Text = "Easy";
            btns[1].Text = "Normal";
            btns[2].Text = "Hard";

            Button btnPlayerVsPlayer = new Button();
            btnPlayerVsPlayer.Font = menuFont;
            btnPlayerVsPlayer.SetBounds(this.ClientSize.Width / 2 - 98, this.ClientSize.Height / 2 + 125, 200, 50);
            btnPlayerVsPlayer.Text = "Player vs Player";

            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].Click += new EventHandler(this.btnGameStartEvent_Click); // after clicking difficulty, set game
                Controls.Add(btns[i]);
            }

            btnPlayerVsPlayer.Click += new EventHandler(this.btnGameStartEvent_Click);
            Controls.Add(btnPlayerVsPlayer);
        }

        // Clears the form and re-initialises with the menu strip intact
        // (Clearance of form allows for switches between main menu to game screen)
        private void clearForm()
        {
            // Remove all buttons.
            foreach (var btn in Controls.OfType<Button>().ToList())
            {
                if (btn.Name == "btnSound")
                {
                    continue;
                }

                Controls.Remove(btn);
            }
            // Remove all labels.
            foreach (var lbl in Controls.OfType<Label>().ToList())
            {
                Controls.Remove(lbl);
            }
            // Remove all panels.
            foreach (var pnl in Controls.OfType<Panel>().ToList())
            {
                Controls.Remove(pnl);
            }
            // Remove all text boxes.
            foreach (var tBox in Controls.OfType<TextBox>().ToList())
            {
                Controls.Remove(tBox);
            }
            // Remove all images.
            foreach (var image in Controls.OfType<PictureBox>().ToList())
            {
                Controls.Remove(image);
            }

            t.Enabled = false;

        }

        // Sound button, responsible for toggling of in-game music/sounds
        private void soundButton(bool sound)
        {
            Button btnSound = new Button();
            btnSound.Name = "btnSound";
            Image soundImg = Image.FromFile("../../megaphone.png"); //loading from specific file location
            btnSound.BackgroundImage = soundImg; //image for button
            btnSound.BackgroundImageLayout = ImageLayout.Stretch;
            btnSound.SetBounds(25, 20, 50, 50);
            btnSound.Click += new EventHandler(this.btnSoundEvent_Click);

            btnSound.FlatStyle = FlatStyle.Flat;
            btnSound.FlatAppearance.BorderSize = 1;
            if (sound)
            {
                btnSound.FlatAppearance.BorderColor = Color.Blue;
                music.Play(); //plays music ONLY if button state is true
            }
            else
            {
                btnSound.FlatAppearance.BorderColor = Color.Red;
            }

            Controls.Add(btnSound);
        }

        // Sound toggle click event
        void btnSoundEvent_Click(object sender, EventArgs e)
        {
            if (!sound)
            {
                ((Button)sender).FlatAppearance.BorderColor = Color.Blue;
                sound = true;
                music.Play();
                // Toggle sound on
            }
            else if (sound)
            {
                ((Button)sender).FlatAppearance.BorderColor = Color.Red;
                sound = false;
                music.Stop();
                // Toggle sound off
            }
        }

        // Creates the main menu strip in the program
        private void menuStrip()
        {
            MainMenu strip = new MainMenu();
            MenuItem options = strip.MenuItems.Add("&Options");
            options.MenuItems.Add(new MenuItem("&Main Menu", stripMainMenuEvent_Click));
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

            // Difficulty code handling, where 0 is easy, 1 is normal, 2 is hard, 3 is player vs player
            if (((Button)sender).Text == "Easy")
            {
                gameSettings.setDifficulty(0);
                gameSettings.setAI(true);
                initShipSelect();
            }
            else if (((Button)sender).Text == "Normal")
            {
                gameSettings.setDifficulty(1);
                gameSettings.setAI(true);
                initShipSelect();
            }
            else if (((Button)sender).Text == "Hard")
            {
                gameSettings.setDifficulty(2);
                gameSettings.setAI(true);
                initShipSelect();
            }
            else
            {
                gameSettings.setDifficulty(3);
                gameSettings.setAI(false);
                initShipSelect();
            }

        }

        void playGame()
        {
            // Check if it is in player vs player mode or player vs AI mode

            // layout buttons for the game
            gameSettings.resetScores();

            Panel pnlTop = new Panel();
            Panel pnlCentre = new Panel();
            Panel pnlBottom = new Panel();
            Panel pnlDivider = new Panel();

            pnlTop.SetBounds((this.ClientSize.Width / 5) - 35, 45, 665, 35);
            pnlTop.BackColor = Color.LightCyan;
            pnlTop.BorderStyle = BorderStyle.FixedSingle;

            pnlCentre.SetBounds((this.ClientSize.Width / 5) - 35, 80, 665, 315);
            pnlCentre.BackColor = Color.LightBlue;
            pnlCentre.BorderStyle = BorderStyle.FixedSingle;

            pnlBottom.SetBounds((this.ClientSize.Width / 5) - 35, 315 + 80, 665, 40);
            pnlBottom.BackColor = Color.LightCyan;
            pnlBottom.BorderStyle = BorderStyle.FixedSingle;

            pnlDivider.SetBounds((this.ClientSize.Width / 2) + 12, 80, 5, 315);
            pnlDivider.BackColor = Color.Gray;

            Controls.Add(pnlTop);
            Controls.Add(pnlCentre);
            Controls.Add(pnlBottom);
            Controls.Add(pnlDivider);

            Label[] LblTop1 = new Label[9];
            Label[] LblTop2 = new Label[9];
            Label[] LblSide1 = new Label[9];
            Label[] LblSide2 = new Label[9];

            // Letters for the side labels.
            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {
                // Add all labels around the boards.
                LblTop1[x] = new Label();
                LblTop1[x].Font = new Font(LblTop1[x].Font, FontStyle.Bold);
                LblTop1[x].TextAlign = ContentAlignment.MiddleLeft;
                LblTop1[x].SetBounds((this.ClientSize.Width / 2) - 25, 106 + (31 * x) + 6, 15, 15);
                LblTop1[x].Text = Convert.ToString(letters[x]);
                LblTop1[x].BackColor = Color.LightBlue;

                LblTop2[x] = new Label();
                LblTop2[x].Font = new Font(LblTop2[x].Font, FontStyle.Bold);
                LblTop2[x].TextAlign = ContentAlignment.MiddleRight;
                LblTop2[x].SetBounds((this.ClientSize.Width / 2) + 35, 106 + (31 * x) + 6, 15, 15);
                LblTop2[x].Text = Convert.ToString(letters[8 - x]);
                LblTop2[x].BackColor = Color.LightBlue;

                LblSide1[x] = new Label();
                LblSide1[x].Font = new Font(LblTop1[x].Font, FontStyle.Bold);
                LblSide1[x].TextAlign = ContentAlignment.MiddleLeft;
                LblSide1[x].SetBounds(((this.ClientSize.Width / 2) - 45) - (31 * x), 90, 15, 15);
                LblSide1[x].Text = Convert.ToString(x + 1);
                LblSide1[x].BackColor = Color.LightBlue;

                LblSide2[x] = new Label();
                LblSide2[x].Font = new Font(LblTop1[x].Font, FontStyle.Bold);
                LblSide2[x].TextAlign = ContentAlignment.MiddleLeft;
                LblSide2[x].SetBounds(((this.ClientSize.Width / 2) + 55) + (31 * x), 90, 15, 15);
                LblSide2[x].Text = Convert.ToString(x + 1);
                LblSide2[x].BackColor = Color.LightBlue;

                Controls.Add(LblTop1[x]);
                Controls.Add(LblTop2[x]);
                Controls.Add(LblSide1[x]);
                Controls.Add(LblSide2[x]);
                for (int y = 0; y < BtnPlayer1Grid.GetLength(1); y++)
                {
                    // Create all buttons on the grid.
                    BtnPlayer1Grid[x, y].SetBounds(((this.ClientSize.Width / 2) - 50) - (31 * y), 106 + (31 * x), 25, 25);
                    BtnPlayer2Grid[x, y].SetBounds(((this.ClientSize.Width / 2) + 50) + (31 * y), 353 - (31 * x), 25, 25);

                    BtnPlayer1Grid[x, y].Click -= BtnPlayer1GridEvent_Click;
                    BtnPlayer2Grid[x, y].Click -= BtnPlayer1GridEvent_Click;
                    if (gameSettings.getAI() == false)
                        BtnPlayer1Grid[x, y].Click += new EventHandler(this.BtnTargetSelectionEvent_Click);
                    BtnPlayer2Grid[x, y].Click += new EventHandler(this.BtnTargetSelectionEvent_Click);

                    BtnPlayer1Grid[x, y].Name = "P1" + Convert.ToString(x) + Convert.ToString(y);
                    BtnPlayer2Grid[x, y].Name = "P2" + Convert.ToString(x) + Convert.ToString(y);

                    BtnPlayer2Grid[x, y].BackColor = Color.PowderBlue;

                    Controls.Add(BtnPlayer1Grid[x, y]);
                    Controls.Add(BtnPlayer2Grid[x, y]);
                }
            }

            // timer code
            lblTimer.Font = menuFont;
            counterSet();
            //lblTimer.Text = Convert.ToString(count);
            lblTimer.SetBounds(((this.ClientSize.Width / 2) - 2), (this.ClientSize.Height / 2) - 223, 40, 25);
            Controls.Add(lblTimer);

            // labels for scoring and turn identification
            scoreP1.SetBounds(((this.ClientSize.Width / 2) - 50), 50, 27, 25);
            scoreP2.SetBounds(((this.ClientSize.Width / 2) + 50), 50, 27, 25);
            scoreP1.TextAlign = HorizontalAlignment.Center;
            scoreP2.TextAlign = HorizontalAlignment.Center;
            scoreP1.ForeColor = Color.Blue;
            scoreP2.ForeColor = Color.Red;
            scoreP1.Font = menuFont;
            scoreP2.Font = menuFont;
            scoreP1.Text = Convert.ToString(gameSettings.getPlayer(0).getScore());
            scoreP2.Text = Convert.ToString(gameSettings.getPlayer(1).getScore());
            scoreP1.ReadOnly = true;
            scoreP2.ReadOnly = true;
            scoreP1.BorderStyle = BorderStyle.None;
            scoreP2.BorderStyle = BorderStyle.None;
            scoreP1.BackColor = this.BackColor;
            scoreP2.BackColor = this.BackColor;

            lblPlayerTurn.SetBounds((this.ClientSize.Width / 2) - 67, 400, 250, 25);
            lblPlayerTurn.Font = menuFont;
            lblPlayerTurn.Text = "Player 1's Turn";

            // labels for each player's board identification
            Label lblP1 = new Label();
            Label lblP2 = new Label();
            lblP1.SetBounds(((this.ClientSize.Width / 2) - 100), (this.ClientSize.Height / 2) - 223, 50, 25);
            lblP2.SetBounds(((this.ClientSize.Width / 2) + 78), (this.ClientSize.Height / 2) - 223, 50, 25);
            lblP1.Font = menuFont;
            lblP2.Font = menuFont;
            lblP1.Text = "P1";
            lblP2.Text = "P2";
            lblP2.TextAlign = ContentAlignment.TopRight;
            lblP1.ForeColor = Color.Blue;
            lblP2.ForeColor = Color.Red;

            startTick();
            Controls.Add(lblP1);
            Controls.Add(lblP2);
            Controls.Add(scoreP1);
            Controls.Add(scoreP2);
            Controls.Add(lblPlayerTurn);

            pnlTop.SendToBack();
            pnlCentre.SendToBack();
            pnlBottom.SendToBack();
        }

        // starts timer
        private void startTick()
        {
            t.Interval = 1000;
            t.Enabled = true;
        }

        // occurs every 1s to update timer label and to check if timer runs out
        void tickEvent(object sender, EventArgs e)
        {
            t.Enabled = false;
            lblTimer.Text = Convert.ToString(count);
            count--;

            if (count == 0)
            {
                gameTimerEvent();
            }

            t.Enabled = true;
        }

        // switches player's turn if timer exceeded
        void gameTimerEvent()
        {
            t.Enabled = false;
            counterSet();
            MessageBox.Show("Timer exceeded! Moving to opponent's turn.", "Timer");
            if (gameSettings.getPlayer1Turn())
            {
                gameSettings.setPlayer1Turn(false);
                lblPlayerTurn.Text = "Player 2's Turn";
                if (gameSettings.getAI() == true)
                    AIPlacement();
            }
            else
            {
                gameSettings.setPlayer1Turn(true);
                lblPlayerTurn.Text = "Player 1's Turn";
            }
            t.Enabled = true;
        }

        // game timer (20s hard, 30s normal, 40s easy)
        // difficulty 0 = easy, 1 = normal, 2 = normal, 3 = player vs player
        private void counterSet()
        {
            if (gameSettings.getDifficulty() == 0)
            {
                count = 40;
            }
            else if (gameSettings.getDifficulty() == 1)
            {
                count = 30;
            }
            else if (gameSettings.getDifficulty() == 2)
            {
                count = 20;
            }
            else if (gameSettings.getDifficulty() == 3)
            {
                count = 30;
            }
        }

        // Start button click event, leads directly to difficulty menu
        void btnStartEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            difficultyMenu();
        }

        //Rules for the game displayed by 3 message boxes back-to-back-to-back
        void btnRulesEvent_Click(object sender, EventArgs e)
        {
            string msg1 = "The object of Battleship is to try and sink all of the other player's before they sink all of your ships. All of the other player's ships are somewhere on his/her board.  You try and hit them by clicking one of the squares on the board.  The other player will also try to hit your ships in turns.  Neither you nor the other player can see the other's board so you must try to guess where they are.";
            string msg2 = "Each player places the 5 ships somewhere on their board.  The ships can only be placed vertically or horizontally. Diagonal placement is not allowed. No part of a ship may hang off the edge of the board.  Ships may not overlap each other.  No ships may be placed on another ship. \r\n\r\nOnce the guessing begins, the players may not move the ships.\r\n\r\nThe 5 ships are:  Carrier (occupies 5 spaces), Battleship (4), Cruiser (3), Submarine (3), and Destroyer (2).";
            string msg3 = "Player's take turns guessing by clicking on the grid squares. Upon guessing, a red square will be marked for hit, and a white square for miss. For example, if you click F6 and your opponent does not have any ship located at F6, that grid square will be marked with a white square. \r\n\r\nWhen all of the squares that one your ships occupies have been hit, the ship will be sunk. As soon as all of one player's ships have been sunk, the game ends.";
            string caption = "Rules";
            MessageBox.Show(msg1, caption);
            MessageBox.Show(msg2, caption);
            MessageBox.Show(msg3, caption);
        }

        // Strip "Main Menu" event click
        void stripMainMenuEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            gameSettings.clearBoards();
            initMenu();
        }

        // Strip Main Menu Exit button, closes program.
        void stripMainMenuExitEvent_Click(object sender, EventArgs e)
        {
            Close();
        }

        //checks to see if either player has destroyed all 5 ships then runs entername() which runs into endgame()
        void CheckWin()
        {
            if (scoreP1.Text == Convert.ToString(17) || scoreP2.Text == Convert.ToString(17))
            {
                System.Threading.Thread.Sleep(1000);
                t.Enabled = false;
                enterName();
            }
        }

        // Is ran when someone has won the game and heads over to the new endgame screen which shows the user(s) who won and allows them to either play again or go back to menu
        void endgame()
        {
            clearForm();

            Panel pnlBack = new Panel();
            Label lblWinner = new Label();
            Button btnPlayAgain = new Button();
            Button btnMainMenu = new Button();
            Font titleFont = new Font("Times New Roman", 40.0f);

            pnlBack.SetBounds(this.ClientSize.Width / 2 - 300, 100, 600, 350);
            pnlBack.BackColor = Color.LightBlue;
            Controls.Add(pnlBack);

            btnPlayAgain.Text = "Play Again";
            btnMainMenu.Text = "Main Menu";

            lblWinner.SetBounds(((int)ClientSize.Width / 2) - 300, 100, 600, 100);
            btnPlayAgain.SetBounds(this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2, 200, 50);
            btnMainMenu.SetBounds(this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2 + 75, 200, 50);

            btnPlayAgain.Click += new EventHandler(this.btnPlayAgain_Click);
            btnMainMenu.Click += new EventHandler(this.btnRunMenu_Click);

            lblWinner.Font = titleFont;
            lblWinner.TextAlign = ContentAlignment.MiddleCenter;
            btnPlayAgain.Font = menuFont;
            btnMainMenu.Font = menuFont;

            if (scoreP1.Text == Convert.ToString(17))
                lblWinner.Text = gameSettings.getPlayerName() + " (P1) wins!";

            else
            {
                if (gameSettings.getAI())
                    lblWinner.SetBounds(((int)ClientSize.Width / 2) - 195, 100, 390, 100);

                lblWinner.Text = gameSettings.getPlayerName() + " (P2) wins!";
            }

            if (!gameSettings.getAI())
                checkLeaderboard();

            gameSettings.resetScores();
            gameSettings.setPlayer1Turn(true);
            gameSettings.setAI(false);

            Controls.Add(lblWinner);
            Controls.Add(btnPlayAgain);
            Controls.Add(btnMainMenu);
            pnlBack.SendToBack();
        }

        // enters name after game ends for submission to leaderboard
        private void enterName()
        {
            if (scoreP2.Text == Convert.ToString(17) && gameSettings.getAI())
            {
                endgame();
                return;
            }

            clearForm();
            
            Label lbl = new Label();
            Button btnSubmit = new Button();
            pName.MaxLength = 10;

            lbl.Font = menuFont;
            if (scoreP1.Text == Convert.ToString(17))
                lbl.Text = "Enter Player 1's name (10 char max): ";
            else if (scoreP2.Text == Convert.ToString(17) && !gameSettings.getAI())
                lbl.Text = "Enter Player 2's name (10 char max): ";
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.SetBounds(this.ClientSize.Width / 2 - 225, this.ClientSize.Height / 2 - 75, 450, 50);
            pName.Font = menuFont;
            pName.SetBounds(this.ClientSize.Width / 2 - 75, this.ClientSize.Height / 2, 150, 50);
            btnSubmit.Font = menuFont;
            btnSubmit.Text = "Submit";
            btnSubmit.SetBounds(this.ClientSize.Width / 2 - 75, this.ClientSize.Height / 2 + 75, 150, 50);
            btnSubmit.Click += new EventHandler(btnEnterNameEvent_Click);

            Controls.Add(lbl);
            Controls.Add(pName);
            Controls.Add(btnSubmit);
        }

        void btnEnterNameEvent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pName.Text))
            {
                MessageBox.Show("Please enter a name.");
                enterName();
            }
            else
            {
                gameSettings.setPlayerName(pName.Text);
                endgame();
            }
        }

        //function to run the menu function cause that function isnt an button event function so cant be called directly
        void btnRunMenu_Click(object sender, EventArgs e)
        {
            clearForm();
            gameSettings.clearBoards();
            initMenu();
        }

        //function to run the difficulty menu function cause that function isnt an button event function so cant be called directly
        void btnPlayAgain_Click(object sender, EventArgs e)
        {
            clearForm();
            gameSettings.clearBoards();
            difficultyMenu();
        }

        //plays explosion sound when ship is hit
        void playSound()
        {
            if (sound == true)
                explosion.Play();
        }

        //plays background music
        void playMusic(bool play)
        {
            if (sound && play)
                music.Play();
        }

        //timer to reduce weird sound glitch with explosion
        void playMusicEvent_Timer(object sender, EventArgs e)
        {
            ((Timer)sender).Stop();
            playMusic(true);
        }

        private void btnHighscoresEvent_Click(object sender, EventArgs e)
        {
            clearForm();

            Label lblTitle = new Label();
            Label[] lblScores = new Label[3];
            Button btnHome = new Button();
            Panel pnlBack = new Panel();

            lblTitle.Text = "Games with Least Amount of Turns To Win";
            lblTitle.Font = menuFont;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.SetBounds(this.ClientSize.Width / 2 - 225, 75, 450, 50);

            pnlBack.BackColor = Color.LightBlue;
            pnlBack.SetBounds(this.ClientSize.Width / 2 - 225, 125, 450, 360);

            Controls.Add(pnlBack);
            string[] text = readFile();
            string[] names = new string[3];
            string[] turns = new string[3];

            // parse leaderboard text file
            if (text.Length >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    string[] split = text[i].Split(',');
                    names[i] = split[0];
                    turns[i] = split[1];
                }
            }
            else
            {
                for (int i = 0; i < text.Length - 1; i++)
                {
                    string[] split = text[i].Split(',');
                    names[i] = split[0];
                    turns[i] = split[1];
                }
            }

            // configuring labels to match leaderboard information
            for (int i = 0; i < lblScores.Length; i++)
            {
                lblScores[i] = new Label();
                lblScores[i].Font = menuFont;
                lblScores[i].Text = (i + 1) + ". " + names[i] + " : " + turns[i];
                lblScores[i].TextAlign = ContentAlignment.MiddleCenter;
                lblScores[i].SetBounds(this.ClientSize.Width / 2 - 100, (this.ClientSize.Height / 2 - 85) + (50 * i), 200, 50);
                lblScores[i].BackColor = Color.LightBlue;
                Controls.Add(lblScores[i]);
            }

            btnHome.Font = menuFont;
            btnHome.Text = "Main Menu";
            btnHome.SetBounds(this.ClientSize.Width / 2 - 75, this.ClientSize.Height / 2 + 125, 150, 50);
            btnHome.Click += new EventHandler(btnRunMenu_Click);
            Controls.Add(lblTitle);
            Controls.Add(btnHome);
            pnlBack.SendToBack();
        }

        // check if new score is valid to be written to the leaderboard file
        // scores are added, then sorted then last score is removed from leaderboard
        private void checkLeaderboard()
        {
            string[] text = readFile();
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            list.Add(Tuple.Create(gameSettings.getPlayerName(), gameSettings.getTurns()));

            // parse and add names and scores to string array
            for (int i = 0; i < text.Length; i++)
            {
                string[] split = text[i].Split(',');
                list.Add(Tuple.Create(split[0], int.Parse(split[1])));
            }

            // sort the scores in ascending order, then clear fourth element
            list.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            if (list.Count > 3)
                list.RemoveAt(list.Count - 1);

            // write sorted leaderboard to file
            using (StreamWriter sw = new StreamWriter(@"../../leaderboard.txt"))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    sw.WriteLine(list[i].Item1 + "," + Convert.ToString(list[i].Item2));
                }
            }
            pName.Text = null;
            gameSettings.resetTurns();
            gameSettings.setPlayerName(null);
        }

        private string[] readFile()
        {
            string path = @"../../leaderboard.txt";
            if (System.IO.File.Exists(path))
            {
                {
                    string[] text = System.IO.File.ReadAllLines(path);
                    return text;
                }
            }
            else
                using (StreamWriter sw = System.IO.File.CreateText(path)) ;

            return null;
        }
    }
}

