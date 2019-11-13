using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{
    public class Permutation
    {

        public RichTextBox t1;
        public int count = 0;
        public List<List<int>> patternLstInt;
        public List<List<string>> patternLstStr;
        public void AllPermutation(char[] s, int n)
        {
            for (int i = n; i < s.Length; i++)
            {
                if (n != s.Length - 1)
                {
                    int tn = n + 1;
                    if (i != n)
                    {
                        char tempch = s[n];
                        s[n] = s[i];
                        s[i] = tempch;
                        AllPermutation(s, tn);
                        s[i] = s[n];
                        s[n] = tempch;
                    }
                    else
                    {
                        AllPermutation(s, tn);
                    }
                }
                else
                {

                    count++;

                    string str = new string(s);

                    if (t1 != null) t1.AppendText(str+"\r\n");
                    List<int> intL = new List<int>();
                    List<string> strL = new List<string>();
                    foreach ( char c in s)
                    {
                        int id=0;

                        if (int.TryParse(c.ToString(),out id))
                        {
                            if (patternLstInt != null)
                            {
                                intL.Add(id);
                            }
                        }
                        

                        if (patternLstStr!=null)
                        strL.Add(c.ToString());

                    }


                    if (patternLstInt != null)
                    {
                        patternLstInt.Add(intL);
                    }
                    if (patternLstStr != null)
                    {
                        patternLstStr.Add(strL);
                    }


                }
            }
        }

    }

}
