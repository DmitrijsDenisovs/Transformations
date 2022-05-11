using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Transformations
{
    public partial class Form1 : Form
    {
        private Color[] colors = { Color.Red, Color.Black, Color.Green, Color.Blue, Color.Yellow};
        private Graphics graphics;
        private List<Point> polygon = new List<Point>();
        private int currentColorIndex = 0;
        public Form1()
        {
            InitializeComponent();
            graphics = this.CreateGraphics();
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            if (polygon.Count > 1)
            {
                graphics.DrawLine(new Pen(new SolidBrush(colors[currentColorIndex])), polygon[0], polygon.ElementAt(polygon.Count - 1));
                ++currentColorIndex;
                if (currentColorIndex >= colors.Length)
                {
                    currentColorIndex = 0;
                }
            } 
            else
            {
                MessageBox.Show("Enter more points");
            }
            
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            int x = this.PointToClient(Cursor.Position).X;
            int y = this.PointToClient(Cursor.Position).Y;
            graphics.DrawRectangle(new Pen(Color.Black, 4), new Rectangle(x, y, 1, 1));

            polygon.Add(this.PointToClient(Cursor.Position));
            if (polygon.Count > 1)
            {
                graphics.DrawLine(new Pen(new SolidBrush(colors[currentColorIndex])), polygon.ElementAt(polygon.Count - 1), polygon.ElementAt(polygon.Count - 2));
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            polygon = new List<Point>();
            graphics.Clear(Color.LightGray);
        }

        private void applyMatrixButton_Click(object sender, EventArgs e)
        {
            double a, b, c, d, p, q, m, n, s;
            bool valid = true;
            if (!Double.TryParse(aTextBox.Text, out a))
            {
                if (String.IsNullOrWhiteSpace(aTextBox.Text))
                {
                    a = 0.0;
                }
                else
                {
                    valid = false;
                }    
            }
            if (!Double.TryParse(bTextBox.Text, out b))
            {
                if (String.IsNullOrWhiteSpace(bTextBox.Text))
                {
                    b = 0.0;
                }
                else
                {
                    valid = false;
                }
            }
            if (!Double.TryParse(cTextBox.Text, out c))
            {
                if (String.IsNullOrWhiteSpace(cTextBox.Text))
                {
                    c = 0.0;
                }
                else
                {
                    valid = false;
                }
            }
            if (!Double.TryParse(dTextBox.Text, out d))
            {
                if (String.IsNullOrWhiteSpace(dTextBox.Text))
                {
                    d = 0.0;
                }
                else
                {
                    valid = false;
                }
            }
            if (!Double.TryParse(pTextBox.Text, out p))
            {
                if (String.IsNullOrWhiteSpace(pTextBox.Text))
                {
                    p = 0.0;
                }
                else
                {
                    valid = false;
                }
            }
            if (!Double.TryParse(qTextBox.Text, out q))
            {
                if (String.IsNullOrWhiteSpace(pTextBox.Text))
                {
                    q = 0.0;
                }
                else
                {
                    valid = false;
                }
            }
            if (!Double.TryParse(mTextBox.Text, out m))
            {
                if (String.IsNullOrWhiteSpace(pTextBox.Text))
                {
                    m = 0.0;
                }
                else
                {
                    valid = false;
                }
            }
            if (!Double.TryParse(nTextBox.Text, out n))
            {
                if (String.IsNullOrWhiteSpace(pTextBox.Text))
                {
                    n = 0.0;
                }
                else
                {
                    valid = false;
                }
            }
            if (!Double.TryParse(sTextBox.Text, out s))
            {
                if (String.IsNullOrWhiteSpace(pTextBox.Text))
                {
                    s = 0.0;
                }
                else
                {
                    valid = false;
                }
            }

            if (valid)
            {
                double[,] transformationMatrix = new double[3,3]{ { a, b, p}, { c, d, q }, { m, n, s } };
                //double[,] transformationMatrix = new double[3, 3] { { a, c, m }, { b, d, n }, { q, p, s } };
                ApplyTransformationMatrix(polygon, transformationMatrix);
            }
            else
            {
                MessageBox.Show("Values must be numbers");
            }
        }

        private void ApplyTransformationMatrix(List<Point> polygon, double[,] transformationMatrix)
        {
            ++currentColorIndex;
            if (currentColorIndex >= colors.Length)
            {
                currentColorIndex = 0;
            }
            bool outOfBounds = false;
            List<Point> newPolygon = new List<Point>();
            foreach (Point point in polygon)
            {
                double[] pointMatrix = new double[] { point.X, point.Y, 1.0 };

                double[] newPointMatrix = MultiplyMatrixes(pointMatrix, transformationMatrix);

                Point newPoint = new Point((int)newPointMatrix[0], (int)newPointMatrix[1]);

                if (newPoint.X < 0 || newPoint.Y < 0 || newPoint.Y > this.Height || newPoint.X > this.Width)
                {
                    outOfBounds = true;
                }
                newPolygon.Add(newPoint);
                if (newPolygon.Count > 1)
                {
                    graphics.DrawLine(new Pen(new SolidBrush(colors[currentColorIndex])), newPolygon.ElementAt(newPolygon.Count - 1), newPolygon.ElementAt(newPolygon.Count - 2));
                }
            }

            graphics.DrawLine(new Pen(new SolidBrush(colors[currentColorIndex])), newPolygon.ElementAt(0), newPolygon.ElementAt(newPolygon.Count - 1));
            
            if (outOfBounds)
            {
                MessageBox.Show("Polygons parts are out of bounds");
            }
        }

        private double[] MultiplyMatrixes(double[] pointMatrix, double[,] transformationMatrix)
        {
            double[] newPointMatrix = new double[3];
            for (int j = 0; j < 3; j++)
            {
                newPointMatrix[j] = 0;
                for (int k = 0; k < 3; k++)
                {
                    newPointMatrix[j] += pointMatrix[k] * transformationMatrix[k, j];
                }
            }
            return newPointMatrix;
        }

        private void angleButton_Click(object sender, EventArgs e)
        {
            double angle;
            int x, y;

            if (!Double.TryParse(angleTextBox.Text, out angle))
            {
                MessageBox.Show("Angle must be a number");
                return;
            }
            if (!String.IsNullOrWhiteSpace(xTextBox.Text) && !String.IsNullOrWhiteSpace(yTextBox.Text))
            {
                if (!Int32.TryParse(xTextBox.Text, out x) || !Int32.TryParse(yTextBox.Text, out y))
                {
                    MessageBox.Show("Coordinate must be a positive integer");
                    return;
                }
            } else
            {
                x = polygon.ElementAt(polygon.Count - 1).X;
                y = polygon.ElementAt(polygon.Count - 1).Y;
            }

            double dx = x, dy = y;
            double angleRadians = angle * Math.PI / 180.0;

            aTextBox.Text = Math.Cos(angleRadians).ToString();
            bTextBox.Text = Math.Sin(angleRadians).ToString();
            cTextBox.Text = (-Math.Sin(angleRadians)).ToString();
            dTextBox.Text = Math.Cos(angleRadians).ToString();
            mTextBox.Text = (-dx * Math.Cos(angleRadians) + dy * Math.Sin(angleRadians) + dx).ToString();
            nTextBox.Text = (-dx * Math.Sin(angleRadians) - dy * Math.Cos(angleRadians) + dy).ToString();
        }
    }
}
