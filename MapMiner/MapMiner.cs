using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapMiner
{
    public partial class MapMiner : Form
    {

        public List<Node> nodes = new List<Node>();
        public List<Edge> edges = new List<Edge>();

        public Image colorMap;
        public Dictionary<string, Color> colorMapping = new Dictionary<string, Color>();

        public MapMiner()
        {
            InitializeComponent();

            string path = Directory.GetCurrentDirectory();
            loadMap(path + @"\example\us");
            //loadConnection(path + @"\example\connection");
        }

        public void loadMap(string path)
        {
            map.Image = Image.FromFile(path + ".png");
            colorMap = Image.FromFile(path + "_colormap.png");
            
            string[] lines = System.IO.File.ReadAllLines(path + ".txt");
            foreach (string line in lines)
            {
                
                string[] word = line.Split(',');
                Console.WriteLine(word[0]);
                if (word[0] != "")
                {
                    colorMapping.Add(word[0], Color.FromArgb(Int32.Parse(word[1]), Int32.Parse(word[2]), Int32.Parse(word[3])));
                    Node newNode = new Node();
                    newNode.Name = word[0];
                    newNode.MapColor = Color.FromArgb(Int32.Parse(word[1]), Int32.Parse(word[2]), Int32.Parse(word[3]));

                    nodes.Add(newNode);
                }
            }
        }

        public bool loadConnection(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path + ".txt");
            foreach (string line in lines)
            {
                string[] word = line.Split(',');

                Edge newEdge = new Edge();
                foreach(Node n in nodes)
                {
                    if (n.Name == word[0])
                        newEdge.Source = n;
                    if (n.Name == word[1])
                        newEdge.Target = n;
                }
                if (newEdge.Source == null || newEdge.Target == null)
                {
                    MessageBox.Show("File not in the good format... Aborting...");
                    return false;
                }
                double weight = 1.0;
                if (word[2] != "")
                {
                    if (double.TryParse(word[2].Replace('.', ','), out weight))
                    {
                        newEdge.Weight = weight;
                    }
                }

                edges.Add(newEdge);
                
            }
            return true;
        }

        private void map_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point clickLocation = me.Location;

            Console.WriteLine((Convert.ToDouble(colorMap.Width) / Convert.ToDouble(map.Width)));
            double ratioWidth = (Convert.ToDouble(colorMap.Width) / Convert.ToDouble(map.Width));
            double ratioHeight = (Convert.ToDouble(colorMap.Height) / Convert.ToDouble(map.Height));
            ratioWidth *= Convert.ToDouble(clickLocation.X);
            ratioHeight *= Convert.ToDouble(clickLocation.Y);

            Point imagePosition = new Point((int)ratioWidth, (int)ratioHeight);
            Color pixelColor = ((Bitmap)colorMap).GetPixel(imagePosition.X, imagePosition.Y);
            
            string stateName = colorMapping.FirstOrDefault(x => x.Value == pixelColor).Key;
            stateNameLabel.Text = colorMapping.FirstOrDefault(x => x.Value == pixelColor).Key;
            if (stateName != null)
            {
                Console.WriteLine(colorMapping.FirstOrDefault(x => x.Value == pixelColor).Key);
                paintState(colorMapping.FirstOrDefault(x => x.Value == pixelColor).Key, pixelColor);
            }

            Invalidate();
            map.Refresh();
        }

        private void paintState(string stateName, Color newColor)
        {
            Bitmap bmp = (Bitmap)map.Image;
            Bitmap bmp_map = (Bitmap)colorMap;
            Color colorRef = colorMapping[stateName];
            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppRgb;
            

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData_map = bmp_map.LockBits(rect, ImageLockMode.ReadOnly, pxf);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite,pxf);

            int bitsPerPixel = Image.GetPixelFormatSize(bmpData.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr_map = bmpData_map.Scan0;
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // int numBytes = bmp.Width * bmp.Height * 3;
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[numBytes];
            byte[] rgbValues_map = new byte[numBytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr_map, rgbValues_map, 0, numBytes);
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // blue value for every other pixel in the the bitmap.
            for (int i = 0; i < rgbValues.Length-2; i += bitsPerPixel / 8)
            {
                if (rgbValues_map[i+2] == colorRef.R && rgbValues_map[i+1] == colorRef.G && rgbValues_map[i] == colorRef.B)
                {
                    rgbValues[i+2] = newColor.R;
                    rgbValues[i+1] = newColor.G;
                    rgbValues[i] = newColor.B;
                }
                
            }
                

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            bmp_map.UnlockBits(bmpData_map);

            map.Image = bmp;
        }


    }
}
