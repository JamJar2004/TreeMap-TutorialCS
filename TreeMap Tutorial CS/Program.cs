using System;
using System.Collections.Generic;
using System.Text;

namespace TreeMap_Tutorial_CS
{
    class Program
    {
        class IntComparator : IComparator<int>
        {
            public int Compare(int left, int right)
            {
                return left - right;
            }
        }

        static void Main(string[] args)
        {
            TreeMap<int, string> map = new TreeMap<int, string>(new IntComparator());
	        map.Place(1, "A");
	        map.Place(2, "B");
	        map.Place(3, "C");
	        map.Place(4, "D");
	        map.Place(5, "E");
	        map.Place(6, "F");

	        map[7 ] = "G";
	        map[8 ] = "H";
	        map[9 ] = "I";
	        map[10] = "J";
	        map[11] = "K";
	        map[12] = "L";
	        map[13] = "M";
	        map[14] = "N";
	        map[15] = "O";
	        map[16] = "P";
	        map[17] = "Q";
	        map[18] = "R";
	        map[19] = "S";

	        foreach(int key in map.GetKeys())
		        Console.WriteLine(map[key]);

	        map.Remove(1);
	        map.Remove(2);
	        map.Remove(3);
	        map.Remove(4);
	        map.Remove(5);
	        map.Remove(6);

	        foreach(int key in map.GetKeys())
		        Console.WriteLine(map[key]);

	        map.Clear();

	        foreach(int key in map.GetKeys())
		        Console.WriteLine(map[key]);
        }
    }
}
