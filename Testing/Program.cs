using System;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var a = new Foo();
            var aa = new Fooo();
            Console.WriteLine("Hello World!");
            System.Console.WriteLine(aa.a);
            System.Console.WriteLine(aa.aa);
            aa.a = 30;
            System.Console.WriteLine(aa.a);
            System.Console.WriteLine(aa.aa);
        }
    }
    
    class Foo
    {
        public int a { get; set; }

        void Main()
        {
            a = 10;
        }
    }


    class Fooo : Foo
    {
        public new int aa =999;

        public new int _a { get; set; } = -50;
        public new int a
        {
            get { return _a;}
            set
            {
                aa = value;
                _a = value;
            }
        }
    
        void Main()
        {
            a = 20;
        }
    }
}