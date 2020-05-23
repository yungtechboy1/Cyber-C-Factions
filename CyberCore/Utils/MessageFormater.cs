using System;
using MiNET.Utils;

namespace CyberCore.Utils
{
       public class MessageFormater
        {
            String start;
            String runn;
            bool inmiddileofword = true;
            string line = "";
            int lines = 0;
            int count = 0;
            int wordchar = 0;
            int wordcount = 0;
            int Maxperline = 0;
            private String final = "";
            public MessageFormater(String m, int maxperline = 80,string PRE_Line_String ="",String POST_Line_String="")
            {
                Maxperline = maxperline;
                start = m;
                runn = start;
                inmiddileofword = true;
                line = "";
                lines = 0;
                count = 0;
                wordchar = 0;
                wordcount = 0;
             pre_line_String = $"{ChatColors.Aqua}|||{ChatColors.Green}";
             post_line_String =$"{ChatColors.Aqua}|||";
            }



            public String[] getWords()
            {
                return start.Split(" ");
            }

            private string pre_line_String = $"{ChatColors.Aqua}|||{ChatColors.Green}";
            private string post_line_String =$"{ChatColors.Aqua}|||";
            
            public String run()
            {
                AddWordToLine(pre_line_String);
                String[] Words = getWords();
                foreach (var word in Words)
                {
                    if (CanAddWordToLine(word+" "))
                    {
                        AddWordToLine(word, true);
                    }
                    else
                    {
                        AddNewLine();
                        AddWordToLine(word, true);
                    }
                }
                
                    return final;
                // foreach (char c in start)
                // {
                //     if (inmiddileofword)
                //     {
                //         inmiddileofword = false;
                //         line += c;
                //     }
                //     else
                //     {
                //         if (c == ' ')
                //         {
                //         }
                //
                //         line += c;
                //     }
                //
                //     lines++;
                // }
            }

            private void AddNewLine()
            {
                final += line + "\n";
                line = pre_line_String;
                lines++;
            }

            private void AddWordToLine(string word, bool addSpace = false)
            {
                line += CenterWords(word);
                if (addSpace) line += " ";
            }

            private string CenterWords(string words)
            {
                int max = Maxperline - (pre_line_String.Length + post_line_String.Length);
                var dif = max - words.Length;
                if (dif > 2)
                {
                    int diff = (int) Math.Floor((decimal) (dif / 2));
                    string finalword = "";
                    string prespace = "";
                    string postspace = "";
                    for (int i = 0; i <diff; i++)
                    {
                        if(i > diff)prespace += " ";
                        else postspace += " ";
                        
                    }

                    finalword += prespace;
                    finalword += words;
                    finalword += postspace;
                    return finalword;

                }

                return words;
            }

            private bool CanAddWordToLine(string word)
            {
                int max = Maxperline- post_line_String.Length;
                int length = line.Length;
                int wordlength = word.Length;
                return (length + wordlength <= max);
            }
        }

}