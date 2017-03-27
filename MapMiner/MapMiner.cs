using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapMiner
{
    public partial class MapMiner : Form
    {
        public Dataset dataset;

        public Image map_reset;
        public Image colorMap;
        public Dictionary<string, Color> colorMapping = new Dictionary<string, Color>();

        public Node selectedNode;
        public Color selectedColor = Color.SkyBlue;

        string path;

        public MapMiner()
        {
            InitializeComponent();

            path = Directory.GetCurrentDirectory() + @"\example\us";
            loadMap("MAP US",path);
            //loadConnection(path + @"\example\connection");

            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        #region Load

        public void loadMap(string name, string path)
        {
            map_reset = Image.FromFile(path + ".png");
            map.Image = Image.FromFile(path + ".png");
            colorMap = Image.FromFile(path + "_colormap.png");

            List<Node> newNodes = new List<Node>();
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

                    newNodes.Add(newNode);
                }
            }
            foreach (Node n in newNodes)
                nodeComboBox.Items.Add(n.Name);

            this.Text = this.Text + " - " + name;
            dataset = new Dataset(name, newNodes);
        }

        public bool loadConnection(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path + ".txt");
            foreach (string line in lines)
            {
                string[] word = line.Split(',');

                Edge newEdge = new Edge();
                foreach(Node n in dataset.Nodes)
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

                dataset.Edges.Add(newEdge);
                
            }
            return true;
        }


        public void select(string name)
        {
            map.Image = (Image)((Bitmap)map_reset).Clone();
            Console.WriteLine("Selected : " + name);
            paintNode(name, selectedColor);
            selectedNode = dataset.Nodes.Find(x => x.Name == name);
            nodeComboBox.SelectedItem = selectedNode.Name;
            updateSelectedGridView(selectedNode);
            Invalidate();
            map.Refresh();
        }

        #endregion

        #region display/Update

        public void updateSelectedGridView(Node n)
        {
            selectedStateGridView.Columns.Clear();
            selectedStateGridView.ColumnCount = 2;
            selectedStateGridView.RowCount = n.Attributes.Count+1;
            selectedStateGridView.Columns[0].HeaderText = "Attribute";
            selectedStateGridView.Columns[1].HeaderText = "Value";
            for (int i = 0; i < n.Attributes.Count; i++)
            {
                selectedStateGridView[0, i].Value = n.Attributes[i];
                selectedStateGridView[1, i].Value = n.Values[i];
            }
        }

        #endregion

        #region Click
        private void map_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point clickLocation = me.Location;

            //Find corresponding pixel
            double ratioWidth = (Convert.ToDouble(colorMap.Width) / Convert.ToDouble(map.Width));
            double ratioHeight = (Convert.ToDouble(colorMap.Height) / Convert.ToDouble(map.Height));
            ratioWidth *= Convert.ToDouble(clickLocation.X);
            ratioHeight *= Convert.ToDouble(clickLocation.Y);
            Point imagePosition = new Point((int)ratioWidth, (int)ratioHeight);
            Color pixelColor = ((Bitmap)colorMap).GetPixel(imagePosition.X, imagePosition.Y);
            string stateName = colorMapping.FirstOrDefault(x => x.Value == pixelColor).Key;

            if (stateName != null)
            {
                select(stateName);
            }
            else
            {
                map.Image = (Image)((Bitmap)map_reset).Clone();
            }

            
        }

        #endregion

        #region paint

        private void resetColor(Color color)
        {
            foreach(Node n in dataset.Nodes)
            {
                paintNode(n.Name, color);
            }
        }


        private void paintNode(string stateName, Color newColor)
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

        #endregion

        private void stateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            select((string)nodeComboBox.SelectedItem);
        }

        private void addDatasetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Browse csv Files";
            openFileDialog1.DefaultExt = "csv";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string sourceFile = openFileDialog1.FileName;
                string directoryPath = Path.GetDirectoryName(sourceFile);
                Console.WriteLine(sourceFile);
                dataset.fillFromCSV(sourceFile, true, ',');
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (VizualisationFeatureSelector VFS = new VizualisationFeatureSelector(dataset.Attributes))
            {
                if (VFS.ShowDialog() == DialogResult.OK)
                {
                    Dictionary<string, double> POPULATION_2010 = dataset.getAllByAttribute(VFS.selectedFeature);
                    List<string> states = new List<string>();
                    List<double> values = new List<double>();
                    foreach (KeyValuePair<string, double> kvp in POPULATION_2010)
                    {
                        states.Add(kvp.Key);
                        values.Add(kvp.Value);
                    }

                    double max = values.Max();
                    double min = values.Min();

                    List<double> percentage = new List<double>();
                    for (int i = 0; i < values.Count; i++)
                    {
                        double p = (values[i] - min) / (max - min);
                        paintNode(states[i], GetColor((int)(p * 100)));
                        percentage.Add((values[i] - min) / (max - min));
                    }
                }
            }      
            
        }

        Color GetColor(int scale)
        {
            // scale is between 1 and 255
            return Color.FromArgb(scale*2, 150 - scale, 0);
        }

        private void generateDatasetbutton_Click(object sender, EventArgs e)
        {
            write_csvFile(Directory.GetCurrentDirectory(), "US_feature");
        }

        public void write_csvFile(string directoryPath, string name)
        {
            //string[] lines = { "First line", "Second line", "Third line" };
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllLines(@"C:\Users\Public\TestFolder\WriteLines.txt", lines);
            List<string> datasetTocsv = new List<string>();
            string lineLabels = dataset.Attributes[0];
            for (int j = 1; j < dataset.Attributes.Count; j++)
                lineLabels += ',' + dataset.Attributes[j];
            datasetTocsv.Add(lineLabels);
            for (int i = 0; i < dataset.Size; i++)
            {
                Node n = dataset.Nodes[i];
                
                List<List<double>> attributeLists = n.getAllValues();
                for (int j = 0; j < attributeLists.Count; j++)
                {
                    string line = n.Name;
                    for (int k = 0; k < attributeLists[j].Count; k++)
                        line += ',' + attributeLists[j][k].ToString(CultureInfo.GetCultureInfo("en-US"));
                    datasetTocsv.Add(line);
                }
                    
                
            }
            string[] readyToWrite = datasetTocsv.ToArray();

            System.IO.File.WriteAllLines(directoryPath + @"\" + name + ".csv", readyToWrite);
            Console.WriteLine("Writting file at " + directoryPath + @"\" + name + ".csv");
        }
    }
}
