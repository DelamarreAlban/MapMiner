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

        public int Size
        {
            get { return Nodes.Count; }
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
                ListAttrib.Add(n.Name,n.getDoubleValue(attribute));
            }

            return ListAttrib;
        }

        #region NodestoDataset DatasetToNodes

        #endregion

        private void addAttribute(string attribName, Dictionary<string, List<double>> values)
        {
            if (!Attributes.Contains(attribName))
            {
                Attributes.Add(attribName);
                foreach (KeyValuePair<string, List<double>> kvp in values)
                {
                    Node n = nodes.Find(x => x.Name == kvp.Key);
                    if (n != null)
                    {
                        if(kvp.Value.Count == 1)
                            n.addAttribute(attribName, kvp.Value[0]);
                        else
                            n.addAttribute(attribName, kvp.Value);
                    }
                }
            }
            else
                Console.WriteLine("Attribute " + attribName +" already exists!");
        }

        #region DatasetCreation

        public void fillFromCSV(string filepath, bool header, char separator)
        {
            int counter = 1;
            double d;
            string line;

            List<string> newAttributes = new List<string>();
            List<Dictionary<string, List<double>>> valuesPerState = new List<Dictionary<string, List<double>>>();

            //Scan
            List<List<string>> columns = new List<List<string>>();
            List<bool> attributeListIndex = new List<bool>();
            string[] lastLine = null;
            System.IO.StreamReader fileScan =
            new System.IO.StreamReader(filepath);
            while ((line = fileScan.ReadLine()) != null)
            {
                if(lastLine == null)
                {
                    string[] newLine = line.Split(separator);
                    for (int i = 0; i < newLine.Length; i++)
                    {
                        columns.Add(new List<string>());
                        columns[i].Add(newLine[i]);
                        attributeListIndex.Add(false);
                    }
                    lastLine = newLine;
                }
                else
                {
                    string[] newLine = line.Split(separator);
                    for (int i = 0; i < newLine.Length; i++)
                    {
                        columns[i].Add(newLine[i]);
                        if (newLine[0] == lastLine[0])
                        {
                            if (newLine[i] != lastLine[i])
                                attributeListIndex[i] = true;
                        }
                    }
                    lastLine = newLine;
                }
            }


            bool noListAttribute = false;
            //if noduplicatelist and the first column are the same => only one instance per state
            List<string> noDuplicateList = columns[0].Distinct().ToList();
            Console.WriteLine("no duplicate list : " + noDuplicateList.Count + "   columns 0 : " + columns[0].Count);
            if (noDuplicateList.Count == columns[0].Count)
                noListAttribute = true;

            Console.WriteLine("nolist attribue " + noListAttribute);
            //First column needs to be the state list
            //Extract value
            string pastNode = null;
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
                        valuesPerState.Add(new Dictionary<string, List<double>>());
                    }
                    Console.WriteLine("{0} Attributes found ", newAttributes.Count);
                }
                else
                {
                    
                    string[] newLine = line.Split(separator);
                    Node n = nodes.Find(x => x.Name == newLine[0]);
                    if (n != null)
                    {
                        for (int i = 0; i < newLine.Length - 1; i++)
                        {

                            //First instance of state
                            if (pastNode == null || !pastNode.Equals(n.Name))
                            {
                                Console.WriteLine(n.Name);
                                for (int j = 0; j < newLine.Length - 1; j++)
                                    valuesPerState[j].Add(n.Name, new List<double>());


                                if (double.TryParse(newLine[i + 1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                                    valuesPerState[i][n.Name].Add(d);
                                pastNode = n.Name;
                            }
                            else {
                                //Other instances
                                if (double.TryParse(newLine[i + 1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                                {
                                    valuesPerState[i][n.Name].Add(d);
                                }
                            }
                            
                        }
                        pastNode = n.Name;
                        counter++;
                    }
                    else
                        Console.WriteLine(newLine[0] + " ignored");
                }
            }
            file.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            

            for (int i=0;i < newAttributes.Count;i++)
            {
                addAttribute(newAttributes[i], valuesPerState[i]);
            }

            Console.WriteLine("Attributes : ");
            foreach (String s in attributes)
                Console.Write(s + " , ");
            Console.WriteLine(" ");
            /*Console.WriteLine(nodes[0].Name+" : ");
            foreach (double s in nodes[0].Values)
                Console.Write(s + " , ");
            Console.WriteLine(" ");
            */
        }
        #endregion
    }
}
