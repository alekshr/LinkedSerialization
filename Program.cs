using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace LinkedSerialization
{

    class ListNode
    {
        public ListNode Previous { get; set; }
        public ListNode Next { get; set; }
        public ListNode Random { get; set; }
        public string Data { get; set; }
    }

    class ListRandom
    {
        private ListNode Head;
        private ListNode Tail;
        private int count;
        public int Count => count;

        public ListRandom()
        {
            count = 0;
            Head = null;
            Tail = null;
        }

        public void AddNode(string data)
        {
            count++;
            var indexRandomNode = new Random().Next(count);
            var newNode = new ListNode();
            if (Head == null)
            {
                newNode.Previous = null;
                Head = newNode;
            }
            else
            {
                newNode.Previous = Tail;
                Tail.Next = newNode;
            }

            newNode.Next = null;
            newNode.Data = data;
            Tail = newNode;
            InitRandomNode(newNode, indexRandomNode);
        }


        private void InitRandomNode(ListNode node, int indexRandomNode)
        {
            node.Random = GetNode(indexRandomNode);
        }

        public void DeleteNode(string data)
        {
            for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
            {
                if (currentNode.Data == data)
                {
                    if (currentNode == Head)
                    {
                        Head = currentNode.Next;
                    }
                    else if (currentNode == Tail)
                    {
                        Tail = currentNode.Previous;
                    }
                
                    currentNode.Next.Previous = currentNode.Previous;
                    currentNode.Previous.Next = currentNode.Next;
                    count--;

                    ReInitRandomNode(currentNode);

                    break;
                }
            }
        }

        private void ReInitRandomNode(ListNode randomNode)
        {
            for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
            {
                if (HashCodeNode(currentNode.Random) == HashCodeNode(randomNode))
                {
                    var indexRandomNode = new Random().Next(count);
                    InitRandomNode(currentNode, indexRandomNode);
                }
            }
        }

        public ListNode GetNode(int index)
        {
            var currentNode = Head;
            if (index > count || index < 0) return null;
            else
            {
                for (int i = 0; i < index; i++)
                {
                    currentNode = currentNode.Next;
                }
            }
            return currentNode;
        }

        private int GetNodeIndex(ListNode node)
        {
            int index = 0;
            for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
            {
                if (HashCodeNode(currentNode) == HashCodeNode(node))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        private int HashCodeNode(ListNode node)
        {
            if (node == Head)
            {
                return node.Data.GetHashCode() ^
                       node.Random.GetHashCode() ^ 1455 ^ 1544;
            }
            else if (node == Tail)
            {
                return node.Data.GetHashCode() ^
                       node.Random.GetHashCode() ^ node.Previous.Data.GetHashCode() ^ 1455;
            }
            else if (count == 0)
            {
                return 0;
            }
            else
            {
                return node.Data.GetHashCode() ^
                       node.Random.GetHashCode() ^ node.Previous.Data.GetHashCode() ^ node.Next.Data.GetHashCode();
            }
        }

        public void Serialize(Stream s)
        {
            using (BinaryWriter writer = new BinaryWriter(s))
            {

                for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
                {
                    writer.Write(currentNode.Data);
                    writer.Write(GetNodeIndex(currentNode.Random));
                }
            }
        }

        public void Deserialize(Stream s)
        {
            IList<int> indexRandomNodes = new List<int>();
            using (BinaryReader reader = new BinaryReader(s))
            {
                while (reader.PeekChar() != -1)
                {
                    string data = reader.ReadString();
                    indexRandomNodes.Add(reader.ReadInt32());
                    AddNode(data);
                }

                ListNode currentNode = Head;

                for (int index = 0; index < count; index++)
                {
                    InitRandomNode(currentNode, indexRandomNodes[index]);
                    currentNode = currentNode.Next;
                }
            }
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            var currentNode = Head;
            for (int index = 0; index < count; index++)
            {
                builder.AppendLine($"Элемент: [{currentNode.Data}]; RandomNode: [{currentNode.Random.Data}]");
                currentNode = currentNode.Next;
            }
            return builder.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TestOne();
            TestTwo();
            TestThree();
        }


        public static void TestOne()
        {
            Console.WriteLine("Тест первый");
            ListRandom listSerialize = new ListRandom();
            ListRandom deserializeList = new ListRandom();
            listSerialize.AddNode("Дата 1");
            listSerialize.AddNode("Дата 2");
            listSerialize.AddNode("Дата 5");
            listSerialize.AddNode("Дата 4");
            listSerialize.AddNode("Дата 3");
            listSerialize.AddNode("Дата 6");
            listSerialize.Serialize(new FileStream("serialize.txt", FileMode.Create));
            deserializeList.Deserialize(new FileStream("serialize.txt", FileMode.Open));

            Console.WriteLine(listSerialize.ToString());

            Console.WriteLine("--------------------------------------------");

            Console.WriteLine(deserializeList.ToString());

            Console.WriteLine("\n\n");
        }

        public static void TestTwo()
        {
            Console.WriteLine("Тест второй");
            ListRandom listSerialize = new ListRandom();
            ListRandom deserializeList = new ListRandom();
            listSerialize.AddNode("Дата 1");
            listSerialize.AddNode("Дата 2");
            listSerialize.AddNode("Дата 5");
            listSerialize.AddNode("Дата 7");
            listSerialize.AddNode("Дата 4");
            listSerialize.AddNode("Дата 3");
            listSerialize.AddNode("Дата 6");
            listSerialize.DeleteNode("Дата 2");
            listSerialize.DeleteNode("Дата 7");
            listSerialize.Serialize(new FileStream("serialize.txt", FileMode.Create));
            deserializeList.Deserialize(new FileStream("serialize.txt", FileMode.Open));

            Console.WriteLine(listSerialize.ToString());

            Console.WriteLine("--------------------------------------------");

            Console.WriteLine(deserializeList.ToString());

            Console.WriteLine("\n\n");
        }

        public static void TestThree()
        {
            Console.WriteLine("Тест третий");
            ListRandom listSerialize = new ListRandom();
            ListRandom deserializeList = new ListRandom();
            listSerialize.AddNode("Дата 1");
            listSerialize.AddNode("Дата 2");
            listSerialize.AddNode("Дата 6");
            listSerialize.AddNode("Дата 4");
            listSerialize.DeleteNode("Дата 6");
            listSerialize.AddNode("Дата 1");
            listSerialize.AddNode("Дата 6");
            listSerialize.Serialize(new FileStream("serialize.txt", FileMode.Create));
            deserializeList.Deserialize(new FileStream("serialize.txt", FileMode.Open));

            Console.WriteLine(listSerialize.ToString());

            Console.WriteLine("--------------------------------------------");

            Console.WriteLine(deserializeList.ToString());

            Console.WriteLine("\n\n");
        }
    }
}
