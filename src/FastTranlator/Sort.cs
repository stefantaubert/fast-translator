using System;
using System.Collections.Generic;
using System.Text;

namespace Sortieren
{
    internal static class Sortiere
    {
        private static List<int> _tmpIndizes = new List<int>();

        public static void SetArray(List<string> bezugsListe)
        {
            _tmpIndizes.Clear();
            List<string> ausgStr = new List<string>();
            List<int> ausg = new List<int>();
            for (int i = 0; i < bezugsListe.Count; i++)
            {
                int ind = ausgStr.BinarySearch(bezugsListe[i]);
                if (ind < 0) ind = ~ind;
                else while (ind < ausgStr.Count
                          && bezugsListe[ind] == bezugsListe[i]
                    )
                        ind++;
                ausgStr.Insert(ind, bezugsListe[i]);
                _tmpIndizes.Insert(ind, i);
            }
        }

        public static void Sort<T>(ref List<T> liste)
        {
            List<T> tmp = new List<T>();
            if (_tmpIndizes.Count != liste.Count) return;
            for (int i = 0; i < _tmpIndizes.Count; i++) tmp.Add(liste[_tmpIndizes[i]]);
            liste.Clear();
            liste = tmp;
        }

        public static void Sort(ref List<string> liste)
        {
            Sort<string>(ref liste);
        }
    }
}