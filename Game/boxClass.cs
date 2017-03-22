using System;
using System.Drawing;

namespace Game
{
	internal class boxClass
	{
		private int x;

		private int y;

		private int id;

		private int speed;

		private int col;

		private int size;

		private int direction;

		private int steps;

		private Color color;

		private Color oldColor;

		public void boxSet(int xx, int yy, int speedd, Color colorr, int sizee, int diro, int idd)
		{
			this.x = xx;
			this.y = yy;
			this.id = idd;
			this.steps = 0;
			this.speed = speedd;
			this.direction = diro;
			this.color = colorr;
			this.oldColor = Color.FromArgb((int)this.color.A, (int)this.color.B, (int)this.color.G, (int)this.color.R);
			this.col = 0;
			this.size = sizee;
		}

		public int getX()
		{
			return this.x;
		}

		public int getY()
		{
			return this.y;
		}

		public int getCol()
		{
			return this.col;
		}

		public int getId()
		{
			return this.id;
		}

		public void setCol(int coll)
		{
			this.col = coll;
		}

		public Color getColor()
		{
			return this.color;
		}

		public void setColor(Color colorr)
		{
			this.oldColor = Color.FromArgb((int)this.color.A, (int)this.color.B, (int)this.color.G, (int)this.color.R);
			this.color = colorr;
		}

		public void setOldColor()
		{
			this.color = Color.FromArgb((int)this.oldColor.A, (int)this.oldColor.B, (int)this.oldColor.G, (int)this.oldColor.R);
		}

		public int getSpeed()
		{
			return this.speed;
		}

		public void setSpeed(int speedd)
		{
			this.speed = speedd;
		}

		public int getSteps()
		{
			return this.steps;
		}

		public void setSteps(int ste)
		{
			this.steps = ste;
		}

		public int getDir()
		{
			return this.direction;
		}

		public void setDir(int diro)
		{
			this.direction = diro;
		}

		public int getSize()
		{
			return this.size;
		}

		public void setSize(int sizee)
		{
			this.size = sizee;
		}

		public void setX(int xx)
		{
			this.x = xx;
		}

		public void setY(int yy)
		{
			this.y = yy;
		}
	}
}
