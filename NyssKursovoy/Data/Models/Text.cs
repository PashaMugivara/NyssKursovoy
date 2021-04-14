using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NyssKursovoy.Data.Models
{
    public static class Text
    {
        private static Dictionary<char, int> letters = new Dictionary<char, int>() {
            {'а',1 }, {'б',2 },  {'в',3 }, {'г',4 }, {'д',5 }, {'е',6 }, {'ё',7 }, {'ж',8 }, {'з',9 }, {'и',10 },
            {'й',11 }, {'к',12 },  {'л',13 }, {'м',14 }, {'н',15 }, {'о',16 }, {'п',17 }, {'р',18 }, {'с',19 }, {'т',20 },
            {'у',21 }, {'ф',22 },  {'х',23 }, {'ц',24 }, {'ч',25 }, {'ш',26 }, {'щ',27 }, {'ъ',28 }, {'ы',29 }, {'ь',30 },
            {'э',31 }, {'ю',32 },  {'я',33 }//зато не надо мучиться с "ё"
        };
        public static string OutFile { get; set; } = "";
        public static string InFile { get; set; } = "";
        public static string Key { get; set; } = "Скорпион";
        private static Queue<char> GetRepeatKey(string s, int n)//очередь из символов ключа
        {

            Queue<char> k = new Queue<char>() { };
            var p = s;
            while (p.Length < n)
            {
                p += p;
            }
            p= p.Substring(0, n);
            foreach (var ir in p) { k.Enqueue(char.ToLower(ir)); }
            return k;
        }
        public static bool CheakKey()
        {
            bool a = true;
            foreach (var ir in Key)
            {
                if (!letters.ContainsKey(char.ToLower(ir))) { a = false; break; }
            }
            return a;
        }
        public static string Decrypt()//дешифрование
        {
            bool isUpper = false;
            string c = InFile;
            Queue<char> k = GetRepeatKey(Key, c.Length);
            string p="";
            for (int i = 0; i < c.Length; i++)
            {
                if (char.IsUpper(c[i])) isUpper = true;
                if (letters.ContainsKey(char.ToLower(c[i]))) 
                {
                    int a = (letters[char.ToLower(c[i])] + 33 - letters[k.Dequeue()]) % 33 + 1;
                    foreach (var ir in letters) 
                        if (ir.Value == a) 
                        {
                            if (isUpper) p += char.ToUpper(ir.Key).ToString();
                            else p += ir.Key.ToString();
                            break;
                        }
                }
                else p += c[i].ToString();
                isUpper = false;
            }
            return p;
        }
        public static string Encrypt()//шифрование
        {
            bool isUpper = false;
            string p = InFile;
            Queue<char> k = GetRepeatKey(Key, p.Length);
            string c = "";
            for (int i = 0; i < p.Length; i++)
            {
                if (char.IsUpper(p[i])) isUpper = true;
                if (letters.ContainsKey(char.ToLower(p[i])))
                {
                    int a = (letters[char.ToLower(p[i])] + letters[k.Dequeue()] - 2) % 33 + 1;
                    foreach (var ir in letters)
                        if (ir.Value == a)
                        {
                            if (isUpper) c += char.ToUpper(ir.Key).ToString();
                            else c += ir.Key.ToString();
                            break;
                        }
                }
                else c += p[i].ToString();
                isUpper = false;
            }
            return c;
        }
        public static string TextFromWord(string file)//чтение файла формата docx
        {
            const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            StringBuilder textBuilder = new StringBuilder();
            using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(file, false))
            { 
                NameTable nt = new NameTable();
                XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                nsManager.AddNamespace("w", wordmlNamespace);
                XmlDocument xdoc = new XmlDocument(nt);
                xdoc.Load(wdDoc.MainDocumentPart.GetStream());
                XmlNodeList paragraphNodes = xdoc.SelectNodes("//w:p", nsManager);
                foreach (XmlNode paragraphNode in paragraphNodes)
                {
                    XmlNodeList textNodes = paragraphNode.SelectNodes(".//w:t", nsManager);
                    foreach (System.Xml.XmlNode textNode in textNodes)
                    {
                        textBuilder.Append(textNode.InnerText);
                    }
                    textBuilder.Append(Environment.NewLine);
                }
            }
            return textBuilder.ToString();
        }
    }
}
