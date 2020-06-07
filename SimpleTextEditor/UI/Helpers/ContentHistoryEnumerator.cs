using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleTextEditor.UI.Helpers
{
    public sealed class ContentHistoryEnumerator : IEnumerator<string>
    {
        public ContentHistoryEnumerator(ref string[] collection)
        {
            this.array = collection;
        }

        private string[] array;
        private int index = -1;
        public string Current
        {
            get
            {

            }
        }

        object IEnumerator.Current => Current;

        public void Dispose() { }
        public bool MoveNext()
        {
            if (this.index == this.maxIndex) return false;

            this.index++;
            return true;
        }

        public void Reset() => this.index = -1;
    }
}
