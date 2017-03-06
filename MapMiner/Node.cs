using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapMiner
{
    public class Node
    {
        private string name;
        private Color mapColor;
        private Color color;
        private List<Edge> incoming = new List<Edge>();
        private List<Edge> outgoing = new List<Edge>();
        private string description;

        private List<string> attributes = new List<string>();
        private List<double> values = new List<double>(); 

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Edge> Incoming
        {
            get { return incoming; }
            set { incoming = value; }
        }

        public List<Edge> Outgoing
        {
            get { return outgoing; }
            set { outgoing = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Color MapColor
        {
            get
            {
                return mapColor;
            }

            set
            {
                mapColor = value;
            }
        }

        public double getValue(string attribute)
        {
            int index = attributes.IndexOf(attribute);
            return getValueByIndex(index);
        }

        public double getValueByIndex(int index)
        {
            return values[index];
        }

        public void setValue(string attribute, double value)
        {
            if(attributes.Contains(attribute))
            {
                int index = attributes.IndexOf(attribute);
                values[index] = value;
            }
        }

        public void addAttribute(string attribute, double value)
        {
            if (!attributes.Contains(attribute))
            {
                attributes.Add(attribute);
                values.Add(value);
            }
            else
            {
                MessageBox.Show("Attributes already exist!");
            }
        }
    }
}
