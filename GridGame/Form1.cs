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
            private int lastHitX = -1, lastHitY = -1;
            private string playerName = "";
            private int totalTurns = 0;
            public void incrementTurns()
            {
                totalTurns++;
            }

            public void resetTurns()
            {
                totalTurns = 0;
            }

            public void setPlayerName(string name)
            {
                playerName = name;
            }

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

            public int getTurns()
            {
                return totalTurns;
            }

            public string getPlayerName()
            {
                return playerName;
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
                for (int x = 0; x < board1.GetLength(0); x++)
                {
                    for (int y = 0; y < board1.GetLength(1); y++)
                    {
                        board1[x, y] = false;
                    }
                }
            }
            public void clearBoard2()
            {
                for (int x = 0; x < board2.GetLength(0); x++)
                {
                    for (int y = 0; y < board2.GetLength(1); y++)
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

        Label lblPlayerTurn = new Label();
        TextBox scoreP1 = new TextBox();
        TextBox scoreP2 = new TextBox();
        TextBox pName = new TextBox();

        //sound and music control lines of code
        bool sound = true;
        SoundPlayer explosion = new SoundPlayer(@"../../explosion.wav");
        SoundPlayer music = new SoundPlayer(@"../../waves.wav");
        Font menuFont = new Font("Times New Roman", 18.0f);
        /* Had to make the grids for the selection screen global variables, so that the necessary checks can be performed.
         * i.e. when you confirm your ship placements, isSelectionValid() must be called.
         * you can't really use the button grid as a parameter into the method as they're already placed
         * BtnPlayer2Grid represents the CPU, but in case we do 2 player, i've made it public too.
        */
        Button[,] BtnPlayer1Grid;
        Button[,] BtnPlayer2Grid;
        GameSettings gameSettings = new GameSettings();

        Color[] shipColours = { Color.DimGray, Color.DarkGray, Color.LightSlateGray, Color.DarkSlateGray };

        // timer variables
        int count = 0;
        Label lblTimer = new Label();
        Timer t = new Timer();

        public Form1()
        {
            InitializeComponent();
            menuStrip();
            initMenu();
            soundButton(sound);
            t.Tick += new EventHandler(tickEvent);
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
            LblInstructions.Font = new Font(menuFont, FontStyle.Underline);

            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

            BtnConfirm.SetBounds((int)(this.ClientSize.Width / 2) - 60, (int)(this.ClientSize.Height / 1.25), 100, 50);
            BtnConfirm.Text = "Confirm";
            BtnConfirm.Font = menuFont;
            BtnConfirm.Click += new EventHandler(BtnConfirmEvent_Click);

            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {

                LblGridTop[x] = new Label();
                LblGridTop[x].Text = Convert.ToString(letters[x]);
                LblGridTop[x].TextAlign = ContentAlignment.MiddleLeft;
                LblGridTop[x].Font = new Font(LblGridTop[x].Font, FontStyle.Bold);
                LblGridTop[x].SetBounds((int)((this.ClientSize.Width / 3.35555) + (40 * x)) + 15, ((int)(this.ClientSize.Height / 9)) - 15, 12, 15);
                
                LblGridSide[x] = new Label();
                LblGridSide[x].SetBounds((int)(this.ClientSize.Width / 3.35555) - 15, ((int)(this.ClientSize.Height / 9)) + (40 * x) + 14, 10, 15);
                LblGridSide[x].Text = Convert.ToString(x + 1);
                LblGridSide[x].Font = new Font(LblGridSide[x].Font, FontStyle.Bold);
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
        // debug for AI ship generation
        void displayDebugScreenForAIShips()
        {
            BtnPlayer2Grid = new Button[9, 9];
            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {
                for (int y = 0; y < BtnPlayer1Grid.GetLength(1); y++)
                {
                    // Create all buttons on the grid.
                    BtnPlayer2Grid[x, y] = new Button();
                    BtnPlayer2Grid[x, y].Text = Convert.ToString(x + "," + y);
                    BtnPlayer2Grid[x, y].SetBounds((int)(this.ClientSize.Width / 3.33333) + (40 * x), ((int)(this.ClientSize.Height / 9)) + (40 * y), 40, 40);
                    BtnPlayer2Grid[x, y].BackColor = Color.PowderBlue;

                    BtnPlayer2Grid[x, y].Click += new EventHandler(this.BtnPlayer1GridEvent_Click);

                    Controls.Add(BtnPlayer2Grid[x, y]);
                }
            }
            randomiseSelection();
        }
        void randomiseSelection()
        {
            gameSettings.clearBoard2();
            Random random = new Random();

            int shipSize = 5;
            int cointoss;
            int rdmX;
            int rdmY;
            bool firstThirdAdded = false;

            while (shipSize > 1)
            {
                rdmX = random.Next(0, 9);
                rdmY = random.Next(0, 9);
                cointoss = random.Next(2);
                // Ship will be placed horizontally
                if (cointoss == 0)
                {
                    bool validated = false;
                    while (!validated)
                    {
                        // Ship can be placed towards right side without going out of bounds
                        if (rdmX + (shipSize - 1) < gameSettings.getBoard2().GetLength(0))
                        {
                            bool shipOverlaps = false;
                            // checks if the placement will overlap with an existing ship.
                            for (int x = 0; x < shipSize; x++)
                            {
                                if (gameSettings.getBoard2CellState(rdmX + x, rdmY))
                                {
                                    shipOverlaps = true;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                int y = rdmY + 1;
                                if (y > 8)
                                {
                                    y = 0;
                                }
                                while (!foundValidPlacement && y != rdmY)
                                {
                                    if (y > 8)
                                    {
                                        y = 0;
                                    }
                                    bool shipCanFit = true;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        if (gameSettings.getBoard2CellState(rdmX + x, y))
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
                                        y++;
                                    }
                                }
                                if (foundValidPlacement)
                                {
                                    rdmY = y;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        //BtnPlayer2Grid[rdmX + x, rdmY].BackColor = shipColours[(shipSize - 2)];
                                        gameSettings.setBoard2CellState(rdmX + x, rdmY, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            else
                            {
                                for (int x = 0; x < shipSize; x++)
                                {
                                    //BtnPlayer2Grid[rdmX + x, rdmY].BackColor = shipColours[(shipSize - 2)];
                                    gameSettings.setBoard2CellState(rdmX + x, rdmY, true);
                                }
                                validated = true;
                            }
                        }
                        // Ship can be placed towards left side without going out of bounds
                        else
                        {
                            bool shipOverlaps = false;
                            // checks if the placement will overlap with an existing ship.
                            for (int x = 0; x < shipSize; x++)
                            {
                                if (gameSettings.getBoard2CellState(rdmX - x, rdmY))
                                {
                                    shipOverlaps = true;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                int y = rdmY - 1;
                                if (y < 0)
                                {
                                    y = 8;
                                }
                                while (!foundValidPlacement && y != rdmY)
                                {
                                    if (y < 0)
                                    {
                                        y = 8;
                                    }
                                    bool shipCanFit = true;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        if (gameSettings.getBoard2CellState(rdmX - x, y))
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
                                        y++;
                                    }
                                }
                                if (foundValidPlacement)
                                {
                                    rdmY = y;
                                    for (int x = 0; x < shipSize; x++)
                                    {
                                        //BtnPlayer2Grid[rdmX - x, rdmY].BackColor = shipColours[(shipSize - 2)];
                                        gameSettings.setBoard2CellState(rdmX - x, rdmY, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            else
                            {
                                for (int x = 0; x < shipSize; x++)
                                {
                                    //BtnPlayer2Grid[rdmX - x, rdmY].BackColor = shipColours[(shipSize - 2)];
                                    gameSettings.setBoard2CellState(rdmX - x, rdmY, true);
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
                        if (rdmY + (shipSize - 1) < gameSettings.getBoard2().GetLength(1))
                        {
                            // checks if the placement will overlap with an existing ship.
                            for (int y = 0; y < shipSize; y++)
                            {
                                if (gameSettings.getBoard2CellState(rdmX, rdmY + y))
                                {
                                    shipOverlaps = true;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                int x = rdmX + 1;
                                if (x > 8)
                                {
                                    x = 0;
                                }
                                while (!foundValidPlacement && x != rdmX)
                                {
                                    if (x > 8)
                                    {
                                        x = 0;
                                    }
                                    bool shipCanFit = true;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        if (gameSettings.getBoard2CellState(x, rdmY + y))
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
                                        x++;
                                    }
                                }
                                if (foundValidPlacement)
                                {
                                    rdmX = x;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        //BtnPlayer2Grid[rdmX, rdmY + y].BackColor = shipColours[(shipSize - 2)];
                                        gameSettings.setBoard2CellState(rdmX, rdmY + y, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            else
                            {
                                for (int y = 0; y < shipSize; y++)
                                {
                                    //BtnPlayer2Grid[rdmX, rdmY + y].BackColor = shipColours[(shipSize - 2)];
                                    gameSettings.setBoard2CellState(rdmX, rdmY + y, true);
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
                                if (gameSettings.getBoard2CellState(rdmX, rdmY - y))
                                {
                                    shipOverlaps = true;
                                }
                            }
                            if (shipOverlaps == true)
                            {
                                bool foundValidPlacement = false;
                                int x = rdmX - 1;
                                if (x < 0)
                                {
                                    x = 8;
                                }
                                while (!foundValidPlacement && x != rdmX)
                                {
                                    if (x < 0)
                                    {
                                        x = 8;
                                    }
                                    bool shipCanFit = true;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        if (gameSettings.getBoard2CellState(x, rdmY - y))
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
                                        x--;
                                    }
                                }
                                if (foundValidPlacement)
                                {
                                    rdmX = x;
                                    for (int y = 0; y < shipSize; y++)
                                    {
                                        //BtnPlayer2Grid[rdmX, rdmY - y].BackColor = shipColours[(shipSize - 2)];
                                        gameSettings.setBoard2CellState(rdmX, rdmY - y, true);
                                    }
                                    validated = true;
                                }
                                else
                                {
                                    rdmX = random.Next(0, 9);
                                    rdmY = random.Next(0, 9);
                                }
                            }
                            else
                            {
                                for (int y = 0; y < shipSize; y++)
                                {
                                    //BtnPlayer2Grid[rdmX, rdmY - y].BackColor = shipColours[(shipSize - 2)];
                                    gameSettings.setBoard2CellState(rdmX, rdmY - y, true);
                                }
                                validated = true;
                            }
                        }
                    }
                }
                if (shipSize == 3 && !firstThirdAdded)
                {
                   firstThirdAdded = true;
                }
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
                    if (gameSettings.getBoard2CellState(x, y) == true) // if chosen button is a hit then turn the button red otherwise turn it white
                    {
                        ((Button)sender).BackColor = Color.Red;
                        gameSettings.setScore1(gameSettings.getScore1() + 1);
                        scoreP1.Text = Convert.ToString(gameSettings.getScore1());
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
                        if (gameSettings.getBoard1CellState(x, y) == true) //same as earlier
                        {
                            ((Button)sender).BackColor = Color.Red;
                            gameSettings.setScore2(gameSettings.getScore2() + 1);
                            scoreP2.Text = Convert.ToString(gameSettings.getScore2());
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
                else if(gameSettings.getDifficulty() == 2) //Always makes an educated guess as its meant to be hard
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
                        if(rnd.Next(2) == 0)
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
                    if(gameSettings.getBoard1CellState(tempX, tempY) == true) //if that educated guess was a hit then turn the button red
                    {
                        BtnPlayer1Grid[tempX, tempY].BackColor = Color.Red;
                        gameSettings.setScore2(gameSettings.getScore2() + 1); // add one to score label
                        scoreP2.Text = Convert.ToString(gameSettings.getScore2());
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
                while(BtnPlayer1Grid[xval, yval].BackColor == Color.Red || BtnPlayer1Grid[xval, yval].BackColor == Color.White) // keeps randomising until a button has been chosen that hasnt already been chosen before so no repeats happen
                {
                    xval = rnd.Next(9);
                    yval = rnd.Next(9);
                }

                if (gameSettings.getBoard1CellState(xval, yval) == true) // do same as before, put guess at co-ordinates and if hit then button red, if miss then button white
                {
                    BtnPlayer1Grid[xval, yval].BackColor = Color.Red;
                    gameSettings.setScore2(gameSettings.getScore2() + 1);
                    scoreP2.Text = Convert.ToString(gameSettings.getScore2());
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
                clearForm();
                playGame();
                randomiseSelection();
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
                    Debug.WriteLine(Convert.ToString(x) + " " + Convert.ToString(y) + ": " +  BtnPlayer1Grid[x, y].BackColor);
                    if (BtnPlayer1Grid[x, y].BackColor == Color.Gray)
                    {
                        numberOfShipGrids++;
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
            for(int i = 5; i > 1; i--)
            {
                if(!findShipInSelectionHFirst(i))
                {
                    validPlacements = false;
                    break;
                }
                if (i == 3 && !foundFirstThree)
                {
                    i++;
                    foundFirstThree = true;
                }
            }
            if (!validPlacements)
            {
                foundFirstThree = false;
                validPlacements = true;
                for (int i = 5; i > 1; i--)
                {
                    if (!findShipInSelectionVFirst(i))
                    {
                        validPlacements = false;
                        break;
                    }
                    if (i == 3 && !foundFirstThree)
                    {
                        i++;
                        foundFirstThree = true;
                    }
                }
                if(!validPlacements)
                {
                    MessageBox.Show("Your ship placements are not valid, please place your ships horizontally or vertically. The valid ship sizes are: \n\nCarrier (5 boxes), \nBattleship (4 boxes) \nCruiser (3 boxes) \nSubmarine (3 boxes) \nDestroyer (2 boxes)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        // Find the ship on the selection screen and see if the placement is valid.
        bool findShipInSelectionHFirst(int amountToFind)
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
                    while (val != amountToFind)
                    {
                        if (BtnPlayer1Grid[x + val, y].BackColor == Color.Gray)
                        {
                            val++;
                        }
                        else
                        {
                            // forcibly break out of loop.
                            break;
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
                                gameSettings.setBoard1CellState(x + i, y, true);
                            }
                            return true;
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
                            break;
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
                                gameSettings.setBoard1CellState(x, y + i, true);
                            }
                            return true;
                        }
                    }
                }
            }
            gameSettings.clearBoard1();
            return false;
        }

        bool findShipInSelectionVFirst(int amountToFind)
        {
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
                            break;
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
            // HORIZONTAL CHECKING
            // Defines the search space to look for the ships.
            for (int x = 0; x < gameSettings.getBoard1().GetLength(0) - (amountToFind - 1); x++)
            {
                for (int y = 0; y < gameSettings.getBoard1().GetLength(1); y++)
                {
                    // Set this to 0 for indexing.
                    int val = 0;
                    // Check to see if there exists a ship pattern.
                    while (val != amountToFind)
                    {
                        if (BtnPlayer1Grid[x + val, y].BackColor == Color.Gray)
                        {
                            val++;
                        }
                        else
                        {
                            // forcibly break out of loop.
                            break;
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
            gameSettings.clearBoard1();
            return false;
        }

        // Display the main menu
        private void initMenu()
        {
            Button btnStart = new Button();
            Button btnRules = new Button();
            Button btnScores = new Button();

            btnStart.SetBounds(this.ClientSize.Width / 2 - 65, this.ClientSize.Height / 2 - 25, 150, 50);
            btnRules.SetBounds(this.ClientSize.Width / 2 - 65, this.ClientSize.Height / 2 + 50, 150, 50);
            btnScores.SetBounds(this.ClientSize.Width / 2 - 65, this.ClientSize.Height / 2 + 125, 150, 50);

            btnStart.Font = menuFont;
            btnRules.Font = menuFont;
            btnScores.Font = menuFont;
            btnStart.Text = "Start";
            btnRules.Text = "Rules";
            btnScores.Text = "Highscores";

            PictureBox boat = new PictureBox();
            boat.ImageLocation = "../../Battleship.png";
            boat.Size = new Size(200, 200);
            boat.Location = new Point(this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2 - 225);

            btnStart.Click += new EventHandler(this.btnStartEvent_Click);
            btnRules.Click += new EventHandler(this.btnRulesEvent_Click);
            btnScores.Click += new EventHandler(this.btnHighscoresEvent_Click);

            Controls.Add(btnStart);
            Controls.Add(btnRules);
            Controls.Add(btnScores);
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

            Button btnPlayerVsPlayer = new Button();
            btnPlayerVsPlayer.Font = menuFont;
            btnPlayerVsPlayer.SetBounds(this.ClientSize.Width / 2 - 112, this.ClientSize.Height / 2 + 25, 200, 50);
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
            //this.Controls.Clear();
            foreach (var btn in Controls.OfType<Button>().ToList())
            {
                if (btn.Name == "btnSound")
                {
                    continue;
                }
                
                Controls.Remove(btn);
            }
            foreach (var lbl in Controls.OfType<Label>().ToList())
            {
                Controls.Remove(lbl);
            }
            foreach (var tBox in Controls.OfType<TextBox>().ToList())
            {
                Controls.Remove(tBox);
            }
            foreach (var image in Controls.OfType<PictureBox>().ToList())
            {
                Controls.Remove(image);
            }

            t.Enabled = false;

            //this.InitializeComponent();
            //menuStrip();
            //soundButton(sound);
        }

        // sound button, responsible for toggling of in-game music/sounds
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

        // sound toggle click event
        void btnSoundEvent_Click(object sender, EventArgs e)
        {
            if (!sound)
            {
                ((Button)sender).FlatAppearance.BorderColor = Color.Blue;
                sound = true;
                music.Play();
                // toggle sound on
            }
            else if (sound)
            {
                ((Button)sender).FlatAppearance.BorderColor = Color.Red;
                sound = false;
                music.Stop();
                // toggle sound off
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

            // difficulty code handling, where 0 is easy, 1 is normal, 2 is hard, 3 is player vs player
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

            Label[] LblTop1 = new Label[9];
            Label[] LblTop2 = new Label[9];
            Label[] LblSide1 = new Label[9];
            Label[] LblSide2 = new Label[9];

            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

            for (int x = 0; x < BtnPlayer1Grid.GetLength(0); x++)
            {
                LblTop1[x] = new Label();
                LblTop1[x].Font = new Font(LblTop1[x].Font, FontStyle.Bold);
                LblTop1[x].TextAlign = ContentAlignment.MiddleLeft;
                LblTop1[x].SetBounds((this.ClientSize.Width / 2) - 25, 101 + (31 * x) + 6, 15, 15);
                LblTop1[x].Text = Convert.ToString(letters[x]);

                LblTop2[x] = new Label();
                LblTop2[x].Font = new Font(LblTop2[x].Font, FontStyle.Bold);
                LblTop2[x].TextAlign = ContentAlignment.MiddleRight;
                LblTop2[x].SetBounds((this.ClientSize.Width / 2) + 35, 101 + (31 * x) + 6, 15, 15);
                LblTop2[x].Text = Convert.ToString(letters[x]);


                Controls.Add(LblTop1[x]);
                Controls.Add(LblTop2[x]);
                for (int y = 0; y < BtnPlayer1Grid.GetLength(1); y++)
                {
                    // Create all buttons on the grid.
                    BtnPlayer1Grid[x, y].SetBounds(((this.ClientSize.Width / 2) - 50) - (31 * y), 101 + (31 * x), 25, 25);
                    BtnPlayer2Grid[x, y].SetBounds(((this.ClientSize.Width / 2) + 50) + (31 * y), 349 - (31 * x), 25, 25);

                    BtnPlayer1Grid[x, y].Click -= BtnPlayer1GridEvent_Click;
                    if(gameSettings.getAI() == false)
                        BtnPlayer1Grid[x, y].Click += new EventHandler(this.BtnTargetSelectionEvent_Click);
                    BtnPlayer2Grid[x, y].Click += new EventHandler(this.BtnTargetSelectionEvent_Click);

                    BtnPlayer1Grid[x, y].Name = "P1" + Convert.ToString(x) + Convert.ToString(y);
                    BtnPlayer2Grid[x, y].Name = "P2" + Convert.ToString(x) + Convert.ToString(y);

                    // labels for scoring and turn identification
                    scoreP1.SetBounds(((this.ClientSize.Width / 2) - 49), 50, 27, 25);
                    scoreP2.SetBounds(((this.ClientSize.Width / 2) + 50), 50, 27, 25);
                    scoreP1.TextAlign = HorizontalAlignment.Center;
                    scoreP2.TextAlign = HorizontalAlignment.Center;
                    scoreP1.ForeColor = Color.Blue;
                    scoreP2.ForeColor = Color.Red;
                    scoreP1.Font = menuFont;
                    scoreP2.Font = menuFont;
                    scoreP1.Text = Convert.ToString(gameSettings.getScore1());
                    scoreP2.Text = Convert.ToString(gameSettings.getScore2());
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
                    lblP2.SetBounds(((this.ClientSize.Width / 2) + 92), (this.ClientSize.Height / 2) - 223, 50, 25);
                    lblP1.Font = menuFont;
                    lblP2.Font = menuFont;
                    lblP1.Text = "P1";
                    lblP2.Text = "P2";
                    lblP1.ForeColor = Color.Blue;
                    lblP2.ForeColor = Color.Red;

                    Controls.Add(BtnPlayer1Grid[x, y]);
                    Controls.Add(BtnPlayer2Grid[x, y]);
                    Controls.Add(scoreP1);
                    Controls.Add(scoreP2);
                    Controls.Add(lblPlayerTurn);
                    Controls.Add(lblP1);
                    Controls.Add(lblP2);

                    // timer code
                    lblTimer.Font = menuFont;
                    counterSet();
                    //lblTimer.Text = Convert.ToString(count);
                    lblTimer.SetBounds(((this.ClientSize.Width / 2) - 2), (this.ClientSize.Height / 2) - 223, 500, 25);
                    Controls.Add(lblTimer);

                    startTick();
                }
            }
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
                if(gameSettings.getAI() == true)
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
            gameSettings.clearBoard1();
            gameSettings.clearBoard2();
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
            if(scoreP1.Text == Convert.ToString(17) || scoreP2.Text == Convert.ToString(17))
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

            Label lblWinner = new Label();
            Button btnPlayAgain = new Button();
            Button btnMainMenu = new Button();
            Font titleFont = new Font("Times New Roman", 40.0f);

            btnPlayAgain.Text = "Play Again";
            btnMainMenu.Text = "Main Menu";

            lblWinner.SetBounds(((int)ClientSize.Width / 2) - 195, 100, 400, 100);
            btnPlayAgain.SetBounds(this.ClientSize.Width / 2 - 112, this.ClientSize.Height / 2, 200, 50);
            btnMainMenu.SetBounds(this.ClientSize.Width / 2 - 112, this.ClientSize.Height / 2 + 75, 200, 50);

            btnPlayAgain.Click += new EventHandler(this.btnPlayAgain_Click);
            btnMainMenu.Click += new EventHandler(this.btnRunMenu_Click);

            lblWinner.Font = titleFont;
            lblWinner.TextAlign = ContentAlignment.MiddleCenter;
            btnPlayAgain.Font = menuFont;
            btnMainMenu.Font = menuFont;

            if (scoreP1.Text == Convert.ToString(17))
                lblWinner.Text = gameSettings.getPlayerName() + " (P1) wins!";

            else
                lblWinner.Text = gameSettings.getPlayerName() + " (P2) wins!";

            checkLeaderboard();
            gameSettings.setScore1(0);
            gameSettings.setScore2(0);

            Controls.Add(lblWinner);
            Controls.Add(btnPlayAgain);
            Controls.Add(btnMainMenu);
        }

        // enters name after game ends for submission to leaderboard
        private void enterName()
        {
            clearForm();

            Label lbl = new Label();
            Button btnSubmit = new Button();
            pName.MaxLength = 10;

            lbl.Font = menuFont;
            if (scoreP1.Text == Convert.ToString(17))
                lbl.Text = "Enter Player 1's name (10 char max): ";

            else
                lbl.Text = "Enter Player 2's name (10 char max): ";
            lbl.SetBounds(this.ClientSize.Width / 2 - 200, this.ClientSize.Height / 2 - 75, 450, 50);
            pName.Font = menuFont;
            pName.SetBounds(this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2, 150, 50);
            btnSubmit.Font = menuFont;
            btnSubmit.Text = "Submit";
            btnSubmit.SetBounds(this.ClientSize.Width / 2 - 100, this.ClientSize.Height / 2 + 75, 150, 50);
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
        void btnRunMenu_Click(object sender , EventArgs e)
        {
            clearForm();
            initMenu();
        }

        //function to run the difficulty menu function cause that function isnt an button event function so cant be called directly
        void btnPlayAgain_Click(object sender, EventArgs e)
        {
            clearForm();
            gameSettings.clearBoard1();
            gameSettings.clearBoard2();
            difficultyMenu();
        }

        //plays explosion sound when ship is hit
        void playSound()
        {
            if(sound == true)
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

            Label title = new Label();
            Label[] scores = new Label[3];
            Button btnHome = new Button();

            title.Text = "Games with Least Amount of Turns To Win";
            title.Font = menuFont;
            title.SetBounds(this.ClientSize.Width / 2 - 193, 75, 450, 50);

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
            for (int i = 0; i < scores.Length; i++)
            {
                scores[i] = new Label();
                scores[i].Font = menuFont;
                scores[i].Text = (i + 1) + ". " + names[i] + " : " + turns[i];
                scores[i].SetBounds(this.ClientSize.Width / 2 - 50, (this.ClientSize.Height / 2 - 75) + (50 * i), 200, 50);
                Controls.Add(scores[i]);
            }

            btnHome.Font = menuFont;
            btnHome.Text = "Main Menu";
            btnHome.SetBounds(this.ClientSize.Width / 2 - 65, this.ClientSize.Height / 2 + 125, 150, 50);
            btnHome.Click += new EventHandler(btnRunMenu_Click);
            Controls.Add(title);
            Controls.Add(btnHome);
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

