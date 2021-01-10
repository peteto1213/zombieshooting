using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZombieShooter
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up"; //by default, the player will face up
        int playerHealth = 100; 
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 3;
        int score;
        Random randNum = new Random();

        List<PictureBox> zombiesList = new List<PictureBox>();


        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if(playerHealth > 1)
            {
                healthBar.Value = playerHealth; //can't have negative value
            }
            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead; //access the image imported
                GameTimer.Stop();
            }

            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;
            label1.Text = "HP: " + playerHealth + "\n" + "Press enter to restart after dead";

            if (goLeft == true && player.Left > 0) { //player moving left, up, down, right
                player.Left -= speed;
            }
            if(goRight == true && player.Left + player.Width < this.ClientSize.Width) //player can't go outside the game window
            {
                player.Left += speed;
            }
            if(goUp == true && player.Top > 45) //boundary of the text
            {
                player.Top -= speed;
            }
            if(goDown ==true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach(Control x in this.Controls)
            {
                if(x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "firstAid")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        playerHealth += 10;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "zombie")
                {

                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                    }
                    if (x.Left > player.Left) //if the zombie is on the left of the player, move towards the player
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }
                    if (x.Left < player.Left) 
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }
                    if (x.Top < player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }
                    if (x.Top > player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }
                }
                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            zombiesList.Remove((PictureBox)x);
                            MakeZombies();
                        }
                    }
                }
            }
        }

        private void keyIsDown(object sender, KeyEventArgs e)
        {

            if(gameOver == true) 
            {
                return;
            }

            if (e.KeyCode == Keys.Left) //actions: when the left button is pressed
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;

            }
        }
        private void keyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) //actions: when the left button is released
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if(e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)//conditions that can shoot bullet
            {
                ammo--;
                ShootBullet(facing);//string direction

                if(ammo < 2)
                {
                    DropAmmo();
                }

                if(score % 5 == 0 && score >= 10 && playerHealth < 40)
                {
                    DropFirstaid();
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true) //press enter to restart game
            {
                RestartGame();
            }

        }

        private void player_Click(object sender, EventArgs e)
        {

        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            //Random spawn of zombies
            zombie.Left = randNum.Next(0, 900);
            zombie.Top = randNum.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize; //size fit with the pictures
            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront(); //prevent overlap

        }

        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = randNum.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo"; //give ammo a tag, e.g. if( (string)x.Tag == "ammo")
            this.Controls.Add(ammo); //add ammo to control collection to be manipulated
            ammo.BringToFront();
            player.BringToFront();

        }

        private void DropFirstaid()
        {
            PictureBox firstAid = new PictureBox();
            firstAid.Image = Properties.Resources.firstaid_Image;
            firstAid.SizeMode = PictureBoxSizeMode.StretchImage;
            firstAid.Left = randNum.Next(10, this.ClientSize.Width - firstAid.Width);
            firstAid.Top = randNum.Next(60, this.ClientSize.Height - firstAid.Height);
            firstAid.Tag = "firstAid"; 
            this.Controls.Add(firstAid); 
            firstAid.BringToFront();
            player.BringToFront();

        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.up;

            foreach(PictureBox i in zombiesList)//remove zombies after game over
            {
                this.Controls.Remove(i);
            }


            foreach (Control x in this.Controls) //remove all firstAid
            {
                if (x is PictureBox && (string)x.Tag == "firstAid")
                {
                    this.Controls.Remove(x);
                    ((PictureBox)x).Dispose();
                }
            }
                

            zombiesList.Clear();

            for(int i = 0; i < 4; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;

            GameTimer.Start();
        }
    }
}
