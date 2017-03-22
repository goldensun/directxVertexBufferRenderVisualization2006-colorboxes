using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
	internal class gameActions : IMessageFilter
	{
		private const int amount = 120;

		private const int trails = 150;

		private const int WM_KEYDOWN = 256;

		private Random objRandom = new Random(DateTime.Now.Millisecond % 2147483647);

		private int minSteps = 10;

		private int maxSteps = 400;

		private bool stepsON = true;

		private bool speedON = true;

		private bool repairStuck = true;

		private int playerHitGrow = 1;

		private int minSize = 1;

		private int maxSize = 1;

		private int border = 40;

		private int speedMod = 7;

		private int init = 0;

		private int iiz = 0;

		private CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[90000];

		private Device device = null;

		private VertexBuffer vertexBuffer = null;

		private boxClass[] boxObject = new boxClass[120];

		private int[] steps = new int[120];

		private int[] direction = new int[120];

		private int[] stepsRandom = new int[120];

		private Form1 form;

		private Size size;

		private Point cursors = Cursor.Position;

		private int cursorX;

		private int cursorY;

		public bool PreFilterMessage(ref Message m)
		{
			bool result = false;
			if (m.Msg == 256)
			{
				Keys keys = (Keys)((int)m.WParam & 65535);
				if (keys == Keys.Up)
				{
					this.movePlayer(0);
					result = true;
				}
				else if (keys == Keys.Down)
				{
					this.movePlayer(1);
					result = true;
				}
				else if (keys == Keys.Left)
				{
					this.movePlayer(2);
					result = true;
				}
				else if (keys == Keys.Right)
				{
					this.movePlayer(3);
					result = true;
				}
			}
			return result;
		}

		public Color Colorhue(Color color, int huer, int alpha)
		{
			int num = (int)Math.Max(color.R, Math.Max(color.G, color.B));
			int num2 = (int)Math.Min(color.R, Math.Min(color.G, color.B));
			double hue = (double)(color.GetHue() + (float)huer);
			double arg_76_0 = (num == 0) ? 0.0 : (1.0 - 1.0 * (double)num2 / (double)num);
			double num3 = (double)num / 255.0;
			return gameActions.ColorFromHSV(hue, 1.0, 1.0, alpha);
		}

		public static Color ColorFromHSV(double hue, double saturation, double value, int alpha)
		{
			int num = Convert.ToInt32(Math.Floor(hue / 60.0)) % 6;
			double num2 = hue / 60.0 - Math.Floor(hue / 60.0);
			value *= (double)alpha;
			int num3 = Convert.ToInt32(value);
			int num4 = Convert.ToInt32(value * (1.0 - saturation));
			int num5 = Convert.ToInt32(value * (1.0 - num2 * saturation));
			int num6 = Convert.ToInt32(value * (1.0 - (1.0 - num2) * saturation));
			Color result;
			if (num == 0)
			{
				result = Color.FromArgb(alpha, num3, num6, num4);
			}
			else if (num == 1)
			{
				result = Color.FromArgb(alpha, num5, num3, num4);
			}
			else if (num == 2)
			{
				result = Color.FromArgb(alpha, num4, num3, num6);
			}
			else if (num == 3)
			{
				result = Color.FromArgb(alpha, num4, num5, num3);
			}
			else if (num == 4)
			{
				result = Color.FromArgb(alpha, num6, num4, num3);
			}
			else
			{
				result = Color.FromArgb(alpha, num3, num4, num5);
			}
			return result;
		}

		public int calculateScore()
		{
			float num = 0f;
			for (int i = 1; i < 120; i++)
			{
				if (this.boxObject[0].getColor() == this.boxObject[i].getColor())
				{
					num += 1f;
				}
			}
			return (int)(num / 119f * 100f);
		}

		public int randomDirection()
		{
			return this.objRandom.Next(0, 360);
		}

		public int random(int a, int b)
		{
			return this.objRandom.Next(a, b);
		}

		public int moveBoxObj(boxClass obj)
		{
			int speed = obj.getSpeed();
			int dir = obj.getDir();
			int result = -1;
			if (obj.getCol() > 0)
			{
				obj.setCol(obj.getCol() - 1);
			}
			int num = obj.getX();
			int num2 = obj.getY();
			obj.setX(num += (int)((double)obj.getSpeed() * Math.Cos((double)obj.getDir())));
			obj.setY(num2 += (int)((double)obj.getSpeed() * Math.Sin((double)obj.getDir())));
			int x = obj.getX();
			int y = obj.getY();
			for (int i = 0; i < 120; i++)
			{
				int num3 = this.boxObject[i].getSize();
				int num4 = obj.getSize();
				num3 /= 2;
				num4 /= 2;
				if (this.boxObject[i] != obj)
				{
					if (this.boxObject[i].getX() - num3 < x + num4 && this.boxObject[i].getX() + num3 > x - num4 && this.boxObject[i].getY() - num3 < y + num4 && this.boxObject[i].getY() + num3 > y - num4)
					{
						result = dir;
						obj.setX(num);
						obj.setY(num2);
						Color color = obj.getColor();
						Color color2 = this.boxObject[i].getColor();
						if (i == 0)
						{
							obj.setColor(this.boxObject[i].getColor());
							obj.setSteps(0);
							if (obj.getSize() < this.maxSize)
							{
								obj.setSize(obj.getSize() + this.playerHitGrow);
							}
						}
						else if (obj.getId() == 0)
						{
							this.boxObject[i].setSteps(0);
							this.boxObject[i].setColor(obj.getColor());
							if (this.boxObject[i].getSize() < this.maxSize)
							{
								this.boxObject[i].setSize(this.boxObject[i].getSize() + this.playerHitGrow);
							}
						}
						else if (this.boxObject[i].getSize() < obj.getSize())
						{
							this.boxObject[i].setSteps(0);
							this.boxObject[i].setColor(color);
							if (obj.getSize() > this.minSize && this.boxObject[i].getSize() < this.maxSize)
							{
								this.boxObject[i].setSize(this.boxObject[i].getSize() + 1);
								obj.setSize(obj.getSize() - 1);
							}
						}
						else if (this.boxObject[i].getSize() > obj.getSize())
						{
							obj.setSteps(0);
							obj.setColor(color2);
							if (this.boxObject[i].getSize() > this.minSize && obj.getSize() < this.maxSize)
							{
								this.boxObject[i].setSize(this.boxObject[i].getSize() - 1);
								obj.setSize(obj.getSize() + 1);
							}
						}
						if (this.repairStuck)
						{
							obj.setCol(obj.getCol() + 2);
							if (obj.getId() != 0)
							{
								if (obj.getCol() > 2)
								{
									obj.setX(num + this.random(0, 10) - 5);
									obj.setY(num2 + this.random(0, 10) - 5);
									obj.setCol(0);
								}
							}
						}
					}
				}
			}
			return result;
		}

		public void createBox()
		{
			this.boxObject[0] = new boxClass();
			this.boxObject[0].boxSet(this.size.Width / 2, this.size.Height / 2, 1, Color.FromArgb(this.random(0, 255), this.random(0, 255), this.random(0, 255)), 1, 0, 0);
			for (int i = 1; i < 120; i++)
			{
				this.boxObject[i] = new boxClass();
				this.boxObject[i].boxSet(this.random(0, this.size.Width - this.border), this.random(0, this.size.Height - this.border), this.random(1, 3), Color.FromArgb(this.random(0, 255), this.random(0, 255), this.random(0, 255)), 1, this.randomDirection(), i);
			}
		}

		public void showForm(Form1 formm, Size sizee)
		{
			this.form = formm;
			this.size = sizee;
		}

		public void movePlayer(int dir)
		{
			this.boxObject[0].setDir(dir);
			this.moveBoxObj(this.boxObject[0]);
		}

		public void moveBox()
		{
			Point position = Cursor.Position;
			double num = (double)position.X;
			double num2 = (double)position.Y;
			double num3 = (double)this.boxObject[0].getX();
			double num4 = (double)this.boxObject[0].getY();
			double num5 = num - num3;
			double num6 = num2 - num4;
			double num7 = Math.Sqrt(num5 * num5 + num6 * num6);
			if (num7 == 0.0)
			{
				num7 = 1.0;
			}
			num5 /= num7;
			num6 /= num7;
			double num8 = num7 * 0.4;
			this.boxObject[0].setX((int)(num3 + num5 * num8));
			this.boxObject[0].setY((int)(num4 + num6 * num8));
			for (int i = 1; i < 120; i++)
			{
				if (this.boxObject[i].getSteps() >= this.maxSteps)
				{
					this.boxObject[i].setSteps(0);
					this.boxObject[i].setSize(this.random(this.minSize, this.maxSize));
					this.boxObject[i].setDir(this.randomDirection());
					this.boxObject[i].setColor(this.Colorhue(this.boxObject[i].getColor(), 200, 255));
					this.boxObject[i].setSpeed(this.random(2, 3));
				}
				else
				{
					this.boxObject[i].setSteps(this.boxObject[i].getSteps() + 1);
					this.boxObject[i].setColor(this.Colorhue(this.boxObject[i].getColor(), 1, 255));
					double num9 = (double)this.boxObject[i - 1].getX();
					double num10 = (double)this.boxObject[i - 1].getY();
					double num11 = (double)this.boxObject[i].getX();
					double num12 = (double)this.boxObject[i].getY();
					double num13 = num9 - num11;
					double num14 = num10 - num12;
					double num15 = Math.Sqrt(num13 * num13 + num14 * num14);
					if (num15 == 0.0)
					{
						num15 = 1.0;
					}
					num13 /= num15;
					num14 /= num15;
					double num16 = num15 * 0.3;
					this.boxObject[i].setX((int)(num11 + num13 * num16));
					this.boxObject[i].setY((int)(num12 + num14 * num16));
				}
			}
			this.drawBox(this.device, null);
		}

		public void Render()
		{
			if (!(this.device == null))
			{
				this.device.Clear(ClearFlags.Target, Color.Black, 1f, 0);
				this.device.RenderState.AlphaBlendEnable = true;
				this.device.RenderState.SourceBlend = Blend.SourceAlpha;
				this.device.RenderState.DestinationBlend = Blend.SourceAlpha;
				this.device.BeginScene();
				this.device.SetStreamSource(0, this.vertexBuffer, 0);
				this.device.VertexFormat = (VertexFormats.Diffuse | VertexFormats.Transformed);
				for (int i = 0; i < 18000; i++)
				{
					this.device.DrawPrimitives(PrimitiveType.LineStrip, i * 5, 4);
				}
				this.vertexBuffer.Dispose();
				this.device.EndScene();
				this.device.Present();
			}
		}

		public void InitializeGraphics(Form1 objForm)
		{
			try
			{
				PresentParameters presentParameters = new PresentParameters();
				presentParameters.Windowed = true;
				presentParameters.SwapEffect = SwapEffect.Discard;
				presentParameters.MultiSample = MultiSampleType.EightSamples;
				Caps deviceCaps = Manager.GetDeviceCaps(0, DeviceType.Hardware);
				if (deviceCaps.VertexShaderVersion >= new Version(2, 0) && deviceCaps.PixelShaderVersion >= new Version(2, 0))
				{
					CreateFlags createFlags = CreateFlags.SoftwareVertexProcessing;
					if (deviceCaps.DeviceCaps.SupportsHardwareTransformAndLight)
					{
						createFlags = CreateFlags.HardwareVertexProcessing;
					}
					if (deviceCaps.DeviceCaps.SupportsPureDevice)
					{
						createFlags |= CreateFlags.PureDevice;
					}
					this.device = new Device(0, DeviceType.Hardware, objForm, createFlags, new PresentParameters[]
					{
						presentParameters
					});
				}
				this.device.DeviceReset += new EventHandler(this.drawBox);
				this.drawBox(this.device, null);
			}
			catch (DirectXException ex)
			{
				MessageBox.Show(null, "Error intializing graphics: " + ex.Message, "Error");
				objForm.Close();
			}
		}

		public void drawBox(object sender, EventArgs e)
		{
			this.boxObject[0].setColor(this.Colorhue(this.boxObject[0].getColor(), 1, 255));
			Device device = (Device)sender;
			this.vertexBuffer = new VertexBuffer(typeof(CustomVertex.TransformedColored), 90000, device, Usage.None, VertexFormats.Diffuse | VertexFormats.Transformed, Pool.Default);
			for (int i = 0; i < 120; i++)
			{
				if (this.init == 0)
				{
					int color = Color.FromArgb(255, (int)this.boxObject[i].getColor().R, (int)this.boxObject[i].getColor().G, (int)this.boxObject[i].getColor().B).ToArgb();
					this.verts[this.iiz * 5 * 120 + i * 5].X = (float)(this.boxObject[i].getX() - this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + i * 5].Y = (float)(this.boxObject[i].getY() - this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + i * 5].Z = 0.5f;
					this.verts[this.iiz * 5 * 120 + i * 5].Rhw = (float)i;
					this.verts[this.iiz * 5 * 120 + i * 5].Color = color;
					this.verts[this.iiz * 5 * 120 + 1 + i * 5].X = (float)(this.boxObject[i].getX() + this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + 1 + i * 5].Y = (float)(this.boxObject[i].getY() - this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + 1 + i * 5].Z = 0.5f;
					this.verts[this.iiz * 5 * 120 + 1 + i * 5].Rhw = (float)i;
					this.verts[this.iiz * 5 * 120 + 1 + i * 5].Color = color;
					this.verts[this.iiz * 5 * 120 + 2 + i * 5].X = (float)(this.boxObject[i].getX() + this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + 2 + i * 5].Y = (float)(this.boxObject[i].getY() + this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + 2 + i * 5].Z = 0.5f;
					this.verts[this.iiz * 5 * 120 + 2 + i * 5].Rhw = (float)i;
					this.verts[this.iiz * 5 * 120 + 2 + i * 5].Color = color;
					this.verts[this.iiz * 5 * 120 + 3 + i * 5].X = (float)(this.boxObject[i].getX() - this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + 3 + i * 5].Y = (float)(this.boxObject[i].getY() + this.boxObject[i].getSize() / 2);
					this.verts[this.iiz * 5 * 120 + 3 + i * 5].Z = 0.5f;
					this.verts[this.iiz * 5 * 120 + 3 + i * 5].Rhw = (float)i;
					this.verts[this.iiz * 5 * 120 + 3 + i * 5].Color = color;
					this.verts[this.iiz * 5 * 120 + 4 + i * 5].X = this.verts[this.iiz * 5 * 120 + i * 5].X;
					this.verts[this.iiz * 5 * 120 + 4 + i * 5].Y = this.verts[this.iiz * 5 * 120 + i * 5].Y;
					this.verts[this.iiz * 5 * 120 + 4 + i * 5].Z = this.verts[this.iiz * 5 * 120 + i * 5].Z;
					this.verts[this.iiz * 5 * 120 + 4 + i * 5].Rhw = this.verts[this.iiz * 5 * 120 + i * 5].Rhw;
					this.verts[this.iiz * 5 * 120 + 4 + i * 5].Color = this.verts[this.iiz * 5 * 120 + i * 5].Color;
				}
				else
				{
					for (int j = 1; j <= 149; j++)
					{
						int num = j / 150 * 100;
						Color color2 = Color.FromArgb(255 - j / 149, (int)((byte)(this.verts[j * 5 * 120 + i * 5].Color >> 16 & 255)), (int)((byte)(this.verts[j * 5 * 120 + i * 5].Color >> 8 & 255)), (int)((byte)(this.verts[j * 5 * 120 + i * 5].Color & 255)));
						int color3 = this.Colorhue(color2, -1, 255 - j / 149).ToArgb();
						this.verts[(j - 1) * 5 * 120 + i * 5] = this.verts[j * 5 * 120 + i * 5];
						this.verts[(j - 1) * 5 * 120 + 1 + i * 5] = this.verts[j * 5 * 120 + 1 + i * 5];
						this.verts[(j - 1) * 5 * 120 + 2 + i * 5] = this.verts[j * 5 * 120 + 2 + i * 5];
						this.verts[(j - 1) * 5 * 120 + 3 + i * 5] = this.verts[j * 5 * 120 + 3 + i * 5];
						this.verts[(j - 1) * 5 * 120 + 4 + i * 5] = this.verts[j * 5 * 120 + 4 + i * 5];
						int num2 = 2;
						double num3 = Math.Sin((double)(j / 8)) * 0.1;
						int num4 = 2;
						double num5 = -Math.Cos((double)(this.iiz / 8)) * 0.8;
						this.verts[(j - 1) * 5 * 120 + i * 5].X = this.verts[j * 5 * 120 + i * 5].X - (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 1 + i * 5].X = this.verts[j * 5 * 120 + 1 + i * 5].X + (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 2 + i * 5].X = this.verts[j * 5 * 120 + 2 + i * 5].X + (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 3 + i * 5].X = this.verts[j * 5 * 120 + 3 + i * 5].X - (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 4 + i * 5].X = this.verts[j * 5 * 120 + 4 + i * 5].X - (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + i * 5].Y = this.verts[j * 5 * 120 + i * 5].Y - (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 1 + i * 5].Y = this.verts[j * 5 * 120 + 1 + i * 5].Y - (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 2 + i * 5].Y = this.verts[j * 5 * 120 + 2 + i * 5].Y + (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 3 + i * 5].Y = this.verts[j * 5 * 120 + 3 + i * 5].Y + (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + 4 + i * 5].Y = this.verts[j * 5 * 120 + 4 + i * 5].Y - (float)num4 * (float)((num5 + (double)num2) * num3);
						this.verts[(j - 1) * 5 * 120 + i * 5].Color = color3;
						this.verts[(j - 1) * 5 * 120 + 1 + i * 5].Color = color3;
						this.verts[(j - 1) * 5 * 120 + 2 + i * 5].Color = color3;
						this.verts[(j - 1) * 5 * 120 + 3 + i * 5].Color = color3;
						this.verts[(j - 1) * 5 * 120 + 4 + i * 5].Color = color3;
					}
					int color = Color.FromArgb(255, (int)this.boxObject[i].getColor().R, (int)this.boxObject[i].getColor().G, (int)this.boxObject[i].getColor().B).ToArgb();
					this.verts[89400 + i * 5].X = (float)(this.boxObject[i].getX() - this.boxObject[i].getSize() / 2);
					this.verts[89400 + i * 5].Y = (float)(this.boxObject[i].getY() - this.boxObject[i].getSize() / 2);
					this.verts[89400 + i * 5].Z = 0.5f;
					this.verts[89400 + i * 5].Rhw = (float)i;
					this.verts[89400 + i * 5].Color = color;
					this.verts[89401 + i * 5].X = (float)(this.boxObject[i].getX() + this.boxObject[i].getSize() / 2);
					this.verts[89401 + i * 5].Y = (float)(this.boxObject[i].getY() - this.boxObject[i].getSize() / 2);
					this.verts[89401 + i * 5].Z = 0.5f;
					this.verts[89401 + i * 5].Rhw = (float)i;
					this.verts[89401 + i * 5].Color = color;
					this.verts[89402 + i * 5].X = (float)(this.boxObject[i].getX() + this.boxObject[i].getSize() / 2);
					this.verts[89402 + i * 5].Y = (float)(this.boxObject[i].getY() + this.boxObject[i].getSize() / 2);
					this.verts[89402 + i * 5].Z = 0.5f;
					this.verts[89402 + i * 5].Rhw = (float)i;
					this.verts[89402 + i * 5].Color = color;
					this.verts[89403 + i * 5].X = (float)(this.boxObject[i].getX() - this.boxObject[i].getSize() / 2);
					this.verts[89403 + i * 5].Y = (float)(this.boxObject[i].getY() + this.boxObject[i].getSize() / 2);
					this.verts[89403 + i * 5].Z = 0.5f;
					this.verts[89403 + i * 5].Rhw = (float)i;
					this.verts[89403 + i * 5].Color = color;
					this.verts[89404 + i * 5].X = this.verts[89400 + i * 5].X;
					this.verts[89404 + i * 5].Y = this.verts[89400 + i * 5].Y;
					this.verts[89404 + i * 5].Z = this.verts[89400 + i * 5].Z;
					this.verts[89404 + i * 5].Rhw = this.verts[89400 + i * 5].Rhw;
					this.verts[89404 + i * 5].Color = this.verts[89400 + i * 5].Color;
				}
			}
			for (int k = this.iiz; k < 150; k++)
			{
				for (int i = 0; i < 120; i++)
				{
					if (k == 0)
					{
						Color color2 = Color.FromArgb((int)((byte)(this.verts[k * 5 * 120 + i * 5].Color >> 24 & 255)), (int)((byte)(this.verts[k * 5 * 120 + i * 5].Color >> 16 & 255)), (int)((byte)(this.verts[k * 5 * 120 + i * 5].Color >> 8 & 255)), (int)((byte)(this.verts[k * 5 * 120 + i * 5].Color & 255)));
						this.verts[k * 5 * 120 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 1 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 2 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 3 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 4 + i * 5].Color = color2.ToArgb();
					}
					else
					{
						Color color2 = Color.FromArgb((int)((byte)(this.verts[(k - 1) * 5 * 120 + i * 5].Color >> 24 & 255)), (int)((byte)(this.verts[k * 5 * 120 + i * 5].Color >> 16 & 255)), (int)((byte)(this.verts[k * 5 * 120 + i * 5].Color >> 8 & 255)), (int)((byte)(this.verts[k * 5 * 120 + i * 5].Color & 255)));
						this.verts[k * 5 * 120 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 1 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 2 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 3 + i * 5].Color = color2.ToArgb();
						this.verts[k * 5 * 120 + 4 + i * 5].Color = color2.ToArgb();
					}
				}
			}
			this.iiz++;
			if (this.iiz > 149)
			{
				this.init = 1;
				this.iiz = 0;
			}
			GraphicsStream graphicsStream = this.vertexBuffer.Lock(0, 0, LockFlags.None);
			graphicsStream.Write(this.verts);
			this.vertexBuffer.Unlock();
		}
	}
}
