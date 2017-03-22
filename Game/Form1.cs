using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
	public class Form1 : Form
	{
		private bool startGame = false;

		private gameActions game = new gameActions();

		private int win = 0;

		private IContainer components = null;

		private Label labelTitle;

		private Button buttonStart;

		protected override bool IsInputKey(Keys keyData)
		{
			return true;
		}

		public Form1()
		{
			this.InitializeComponent();
			this.game.showForm(this, base.Size);
			MessageBox.Show("EDIT: I don't know why this is half my code for my game, but mostly the code for the visualization? maybe I built the visualization over the game and only have some of the game code left and lost that exe? idk. \n\n\nGet all of the boxes to your color\n arrow keys = move\nHit boxes to change color\nLarger boxes change the color of smaller boxes on collision");
		}

		public void GameLoop()
		{
			this.game.createBox();
			this.game.InitializeGraphics(this);
			while (base.Created)
			{
				this.GameLogic();
				this.game.Render();
				Application.DoEvents();
				Application.AddMessageFilter(this.game);
			}
		}

		private void GameLogic()
		{
			if (this.startGame)
			{
				this.game.moveBox();
				int num = this.game.calculateScore();
				this.labelTitle.Text = "Score: " + num + "%";
				if (num > 99 && this.win == 0)
				{
					MessageBox.Show("you win!");
					this.win = 1;
				}
			}
		}

		private void Form1_reSize(object sender, EventArgs e)
		{
			this.game.showForm(this, base.Size);
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			this.startGame = true;
			this.buttonStart.Visible = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.WindowState = FormWindowState.Maximized;
            this.labelTitle = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(-2, -1);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(74, 13);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "GameWindow";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(-1, 11);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(39, 21);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(731, 688);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.labelTitle);
            this.Name = "Form1";
            this.Text = "Game";
            this.Resize += new System.EventHandler(this.Form1_reSize);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
