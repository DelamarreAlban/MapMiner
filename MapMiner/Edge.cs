using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMiner
{
    public class Edge
    {
        private Node source;
        private Node target;

        private double weight;

        public Edge() { }

        public Edge(Node _source, Node _target)
        {
            target = _target;
            source = _source;
        }

        public Edge(Edge e)
        {
            source = e.Source;
            target = e.Target;
        }

        public Node Source
        {
            get { return source; }
            set { source = value; }
        }

        public Node Target
        {
            get { return target; }
            set { target = value; }
        }

        public double Weight
        {
            get{return weight;}
            set{weight = value;}
        }
    }
}
