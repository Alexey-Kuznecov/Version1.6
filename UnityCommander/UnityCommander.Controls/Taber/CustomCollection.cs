using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnityCommander.Controls.Taber
{
    public class CustomCollection : ICollection
    {
        private int[] intArr = { 1, 5, 9 };
        private int Ct;

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        public void CopyTo(Array array, int index)
        {
            foreach (int i in intArr)
            {
                //myArr.SetValue(i, index);
                index = index + 1;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(intArr);
        }
    }

    public class Enumerator : IEnumerator
    {
        private int[] intArr;
        private int Cursor;

        public Enumerator(int[] intarr)
        {
            this.intArr = intarr;
            Cursor = -1;
        }

        void IEnumerator.Reset()
        {
            Cursor = -1;
        }
        bool IEnumerator.MoveNext()
        {
            if (Cursor < intArr.Length)
                Cursor++;

            return (!(Cursor == intArr.Length));
        }
        object IEnumerator.Current
{
    get
    {
        if((Cursor < 0) || (Cursor == intArr.Length))
            throw new InvalidOperationException();
        return intArr[Cursor];
    }
}
    }
}
