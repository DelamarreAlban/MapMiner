using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMiner
{
    public class Dataset
    {
        private string name;
        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();

        private List<string> attributes = new List<string>();

        public Dataset()
        {

        }

        public Dataset(string _name, List<Node> _nodes)
        {
            name = _name;
            nodes = _nodes;
        }

        #region accessors
        public List<Node> Nodes
        {
            get
            {
                return nodes;
            }

            set
            {
                nodes = value;
            }
        }

        public List<Edge> Edges
        {
            get
            {
                return edges;
            }

            set
            {
                edges = value;
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

        #endregion

        public Dictionary<string, double> getAllByAttribute(string attribute)
        {
            Dictionary<string, double> ListAttrib = new Dictionary<string, double>();
            foreach(Node n in nodes)
            {
                ListAttrib.Add(n.Name,n.getValue(attribute));
            }

            return ListAttrib;
        }

        #region NodestoDataset DatasetToNodes

        #endregion

        private void addAttribute(string attribName, Dictionary<string, double> values)
        {
            foreach (KeyValuePair<string, double> kvp in values)
            {
                Node n = nodes.Find(x => x.Name == kvp.Key);
                if (n != null)
                    n.addAttribute(attribName, kvp.Value);
            }
        }

        #region DatasetCreation

        public void fillFromCSV(string filepath, bool header, char separator)
        {
            int counter = 0;
            double d;
            string line;

            List<string> newAttributes = new List<string>();
            List<Dictionary<string, double>> valuesPerState = new List<Dictionary<string, double>>();

            //First column needs to be the state list
            System.IO.StreamReader file =
            new System.IO.StreamReader(filepath);
            while ((line = file.ReadLine()) != null)
            {
                if (header)
                {
                    header = false;
                    string[] firstLine = line.Split(separator);
                    for (int i = 1; i < firstLine.Length; i++)
                    {
                        newAttributes.Add(firstLine[i]);
                        valuesPerState.Add(new Dictionary<string, double>());
                    }
                }
                else {
                    string[] newLine = line.Split(separator);
                    for (int i = 1; i < newLine.Length-1; i++)
                    {
                        Node n = nodes.Find(x => x.Name == newLine[0]);
                        if (n != null)
                        {
                            if (double.TryParse(newLine[i], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                            {
                                valuesPerState[i].Add(newLine[0], d);
                            }
                        }
                        else
                            Console.WriteLine(newLine[0] + " ignored");
                    }
                    counter++;
                }
            }
            file.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            attributes.Clear();
            for(int i=0;i < newAttributes.Count;i++)
            {
                attributes.Add(newAttributes[i]);
                addAttribute(newAttributes[i], valuesPerState[i]);
            }
        }
        #endregion
    }
}
