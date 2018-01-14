using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Threading;

class DictCC
{
    int _lang = 0;

    string[] _namen = new string[] { "de-en", "de-fr" };
    string[] _sprachen = new string[] { "deen", "defr" };

    public string[] Namen { get { return _namen; } }

    string Path { get { return Application.StartupPath + "\\" + _namen[_lang] + " vocables.txt"; } }
    string SuchUrl { get { return "http://" + _sprachen[_lang] + ".pocket.dict.cc/?s="; } }
    public string Name { get { return _namen[_lang]; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wert">0 = Eng; 1 = Frz</param>
    public DictCC(int wert)
    {
        if (wert > _sprachen.Length || wert < 0)
            wert = 0;
        _lang = wert;
    }
    public string Search(string wort)
    {
        wort = wort.Trim();
        string url = SuchUrl + wort;
        // url = MakePathOk(url);
        Thread thr = new Thread(delegate()
        {
            string trans = GetFirstTrans(url).Trim();
            if (trans != "") WriteOnEnd(Path, wort + " - " + trans);
        });
        thr.Start();
        return url;
    }
    private string MakePathOk(string path)
    {
        string ausg = path;
        string[] fehlerZeichen = new string[] { "\\", "*", "\"", "<", ">", "{", "}" };
        foreach (var item in fehlerZeichen)
            ausg = ausg.Replace(item, String.Empty);
        return ausg;
    }
    private string RemoveTags(string inputString)
    {
        string pattern = "<.*?>";
        return System.Text.RegularExpressions.Regex.Replace(inputString, pattern, string.Empty).Trim();
    }
    private string[] GetAllSubstrings(string source, string anfangsString, string endString)
    {
        List<string> subStringList = new List<string>();
        int wert = 0;
        int biggestWert = 0;
        int anfangswert = 0;
        bool vorbei = false;
        do
        {
            int anfg = source.IndexOf(anfangsString, wert) + anfangsString.Length;
            if (!vorbei)
            {
                anfangswert = anfg;
                vorbei = true;
            }
            int ende = source.IndexOf(endString, anfg);
            if (anfg < anfangswert)
                break;
            string teilString = source.Substring(anfg, ende - anfg);
            wert = anfg + teilString.Length;
            subStringList.Add(teilString);
            // höchstwert zulegen
            if (biggestWert <= wert)
                biggestWert = wert;
        }
        while (biggestWert <= wert);
        return subStringList.ToArray();
    }
    private string[] ReadLines(string pfad)
    {
        if (!File.Exists(pfad)) return new string[] { };
        StreamReader sR = new StreamReader(pfad);
        string text = sR.ReadToEnd();
        sR.Dispose();
        sR.Close();
        string[] lines = text.Split('\n');
        List<string> ausg = new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim() != "")
                ausg.Add(lines[i].Trim());
        }
        return ausg.ToArray();
    }
    private void WriteOnEnd(string path, string zeile)
    {
        List<string> lines = new List<string>(ReadLines(path));
        List<string> wieOft = new List<string>();
        for (int i = 0; i < lines.Count; i++)
        {
            string zeiles = lines[i].Trim();
            if (zeiles == String.Empty)
            {
                lines.RemoveAt(i);
                i--;
                continue;
            }
            wieOft.Add(zeiles.Substring(0, zeiles.IndexOf(" ")).Trim());
            lines[i] = lines[i].Substring(wieOft[i].ToString().Length).Trim();
        }
        Sortieren.Sortiere.SetArray(lines);
        Sortieren.Sortiere.Sort(ref lines);
        Sortieren.Sortiere.Sort(ref wieOft);
        int index = lines.BinarySearch(zeile);

        if (index >= 0)
            wieOft[index] = (Convert.ToInt32(wieOft[index]) + 1).ToString();
        else
        {
            lines.Insert(~index, zeile);
            wieOft.Insert(~index, "1");
        }

        Sortieren.Sortiere.SetArray(wieOft);
        Sortieren.Sortiere.Sort(ref lines);
        Sortieren.Sortiere.Sort(ref wieOft);
        lines.Reverse();
        wieOft.Reverse();

        for (int i = 0; i < lines.Count; i++)
            lines[i] = wieOft[i] + " " + lines[i];
        File.WriteAllLines(path, lines.ToArray(), Encoding.UTF8);
       
    }
    private void Clean(ref string input)
    {
        //  "&nbsp;{f}"
        if (input.StartsWith("pocket.dict.cc")) { input = String.Empty; return; }
        string a = "&", b = ";", c = "{", d = "}";
        if (input.Contains(a) && input.Contains(b))
            foreach (var item in GetAllSubstrings(input, "&", ";"))
                input = input.Replace("&" + item + ";", "");
        if (input.Contains(c) && input.Contains(d))
            foreach (var item in GetAllSubstrings(input, "{", "}"))
                input = input.Replace("{" + item + "}", "");
        input = input.Trim();
    }
    private string GetFirstTrans(string url)
    {
        string a = "<dd><a href=\"", b = "</a>", a2 = "\">";
        WebClient wc = new WebClient();
        string quel = wc.DownloadString(url);
        int in1 = quel.IndexOf(a) + a.Length;
        if (in1 == -1) return "";
        int in2 = quel.IndexOf(b, in1);
        string sub = quel.Substring(in1, in2 - in1);
        sub = sub.Substring(sub.IndexOf(a2) + a2.Length, sub.Length - sub.IndexOf(a2) - a2.Length);
        sub = RemoveTags(sub);
        Clean(ref sub);
        return sub.Trim().Replace("Ã¼", "ü").Replace("Ã¶", "ö").Replace("Ã¤", "ä").Replace("Ãœ", "Ü").Replace("ÃŸ", "ß").Replace("Ã©", "é").Replace("Ã", "à").Replace("Ã€", "À").Replace("Ã¨", "è"); ;
    }
}