using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridGame
{
    public partial class Form1 : Form
    {
        Font menuFont = new Font("Times New Roman", 18.0f);
        public Form1()
        {
            InitializeComponent();
            menuStrip();
            initMenu();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
        }

        // Creates the main menu strip in the program
        private void menuStrip()
        {
            MainMenu strip = new MainMenu();
            MenuItem options = strip.MenuItems.Add("&Options");
            options.MenuItems.Add(new MenuItem("&Main Menu", stripMainMenuEvent_Click));
            options.MenuItems.Add(new MenuItem("&Toggle Music On/Off"));
            options.MenuItems.Add(new MenuItem("&Exit"));
            this.Menu = strip;
            MenuItem rules = strip.MenuItems.Add("&Rules", btnRulesEvent_Click);
            this.Menu = strip;
            strip.GetForm().BackColor = Color.LightCyan;
        }
        
        void btnGameStartEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            
            // insert game code
            // currently all difficulties direct to this event
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
    }
}
