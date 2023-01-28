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
            InitMenu();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Display the main menu
        private void InitMenu()
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

            foreach (Button btn in btns)
            {
                Controls.Add(btn);
            }
        }

        // Clears the form and re-initialises with the menu strip intact
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
            MenuItem rules = strip.MenuItems.Add("&Rules");
            rules.MenuItems.Add(new MenuItem("&Rules"));
            this.Menu = strip;
            strip.GetForm().BackColor = Color.LightCyan;
        }

        // Start button click event, leads directly to difficulty menu
        void btnStartEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            difficultyMenu();
        }

        // Strip "Main Menu" event click
        void stripMainMenuEvent_Click(object sender, EventArgs e)
        {
            clearForm();
            InitMenu();
        }
    }
}
