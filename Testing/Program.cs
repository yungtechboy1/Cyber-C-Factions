using System;
using System.Collections.Generic;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Dictionary<String,Dictionary<String,String>> a = new Dictionary<String,Dictionary<String,String>>(){
                {"TEST",new Dictionary<String,String>(){
                    {"TEST-1","1"},
                    {"TEST-2","2"},
                    {"TEST-3","3"},
                    {"TEST-4","4"}
                }}
            };
            var aa = a["TEST"];
            aa["TEST-6"] = "B4";
            var aaa = aa["TEST-6"];
            aaa = "asdasdasd";
            a["TEST"]["TEST-5"] = "UMM";
            System.Console.WriteLine(a["TEST"]["TEST-5"]);
            System.Console.WriteLine(a["TEST"]["TEST-6"]);
        }
    }
}