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
        private List<object> values = new List<object>(); 

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

        public List<string> Attributes
        {
            get
            {
                return attributes;
            }

            set
            {
                attributes = value;
            }
        }

        public List<object> Values
        {
            get
            {
                return values;
            }

            set
            {
                values = value;
            }
        }

        public int MaxSize
        {
            get
            {
                int max = 1;
                foreach(Object o in Values)
                {
                    if (o.GetType() == typeof(List<double>))
                        if (((List<double>)o).Count > max)
                            max = ((List<double>)o).Count;
                }
                return max;
            }
        }

        public double getDoubleValue(string attribute)
        {
            int index = Attributes.IndexOf(attribute);
            return getDoubleValueByIndex(index);
        }

        public double getDoubleValueByIndex(int index)
        {
            return (double)Values[index];
        }

        public List<double> getListDoubleValue(string attribute)
        {
            int index = Attributes.IndexOf(attribute);
            return getListDoubleValueByIndex(index);
        }

        public List<double> getListDoubleValueByIndex(int index)
        {
            return (List<double>)Values[index];
        }


        public void setValue(string attribute, double value)
        {
            if(Attributes.Contains(attribute))
            {
                int index = Attributes.IndexOf(attribute);
                Values[index] = value;
            }
        }

        public void addAttribute(string attribute, object value)
        {
            if (!Attributes.Contains(attribute))
            {
                Attributes.Add(attribute);
                Values.Add(value);
            }
            else
            {
                Console.WriteLine("Attributes already exist!");
                //MessageBox.Show("Attributes already exist!");
            }
        }

        public List<List<double>> getAllValues()
        {
            // !!!!!!!!!!!!!!!!!!!!!   WARNING   !!!!!!!!!!!!!!!!!!!!!!!!!
            // if size of list attributes different => PROBLEM
            //

            Console.WriteLine(Name);
            List<List<double>> allInstances = new List<List<double>>();
            for(int i=0;i < MaxSize; i++)
            {
                allInstances.Add(new List<double>());
                for (int j = 0; j < Values.Count; j++)
                {
                    if (Values[j].GetType() == typeof(List<double>))
                    {
                        allInstances[i].Add(getListDoubleValueByIndex(j)[i]);
                    }
                    else {
                        allInstances[i].Add(getDoubleValueByIndex(j));
                    }
                }
            }
            
                    return allInstances;
        }
    }
}
