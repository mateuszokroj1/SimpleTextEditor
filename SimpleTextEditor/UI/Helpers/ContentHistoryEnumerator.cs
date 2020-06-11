using System;
using System.Collections;
using System.Collections.Generic;

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
                if (index < 0)
                    throw new InvalidOperationException();

                if (index >= array.Length)
                    throw new IndexOutOfRangeException();

                return this.array[index];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose() { this.array = null; }
        
        public bool MoveNext()
        {
            if (this.index + 1 >= this.array.Length || this.array[this.index + 1] == null)
                return false;

            this.index++;
            return true;
        }

        public void Reset() => this.index = -1;
    }
}
