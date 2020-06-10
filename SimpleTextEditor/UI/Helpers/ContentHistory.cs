using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleTextEditor.UI.Helpers
{
    public class ContentHistory : ModelBase, IEnumerable<string>, IList<string>, IObservable<string>
    {
        #region Constructors

        public ContentHistory(ushort historySize = 100)
        {
            this.collection = new string[historySize];
            for (int i = 0; i < historySize; i++)
                this.collection[i] = null;
        }

        #endregion

        #region Fields

        protected string[] collection;
        protected int currentPosition = -1;
        protected List<IObserver<string>> observers = new List<IObserver<string>>();

        #endregion

        #region Properties

        public int Count => this.currentPosition + 1;

        public bool IsReadOnly => false;

        public string this[int index]
        {
            get
            {
                if (index > this.currentPosition)
                    throw new IndexOutOfRangeException();

                return this.collection[index];
            }
        }

        public string CurrentValue
        {
            get
            {
                if (currentPosition == -1) throw new InvalidOperationException();

                return this.collection[currentPosition];
            }
            set => throw new InvalidOperationException();
        }

        public bool CanRedo => currentPosition < this.collection.Length - 1;
        
        public bool CanUndo => currentPosition > 0;

        string IList<string>.this[int index] { get => this[index]; set => throw new InvalidOperationException(); }

        #endregion

        #region Methods

        public bool ChangeValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException();

            string val = new string(value);

            if (this.currentPosition == this.collection.Length - 1)
            {
                MoveDown();
                this.collection[this.currentPosition] = val;

                // Notify about changes
                foreach (var subscriber in this.observers)
                {
                    foreach (var data in this.collection)
                        subscriber.OnNext(data);

                    subscriber.OnCompleted();
                }
                OnPropertyChanged(nameof(CurrentValue));
                return true;
            }
            else
            {
                this.collection[++this.currentPosition] = val;

                // Notify about change
                foreach(var subscriber in this.observers)
                {
                    subscriber.OnNext(val);
                    subscriber.OnCompleted();
                }

                OnPropertyChanged(nameof(CurrentValue));
                return false;
            }
        }

        protected void MoveDown()
        {
            for (int i = 1; i < this.collection.Length; i++)
                this.collection[i - 1] = this.collection[i];
        }

        public bool Undo()
        {
            if(currentPosition > 0)
            {
                currentPosition--;
                OnPropertyChanged(nameof(CurrentValue));
                return true;
            }

            return false;
        }

        public bool Redo()
        {
            if(currentPosition < this.collection.Length - 1)
            {
                currentPosition++;
                OnPropertyChanged(nameof(CurrentValue));
                return true;
            }

            return false;
        }

        public IEnumerator<string> GetEnumerator() => new ContentHistoryEnumerator(ref this.collection);
        IEnumerator IEnumerable.GetEnumerator() => new ContentHistoryEnumerator(ref this.collection);


        public int IndexOf(string item)
        {
            for (int i = 0; i <= this.collection.Length; i++)
                if (this.collection[i] == item)
                    return i;

            return -1;
        }
        /// <summary>See ChangeValue() method.</summary>
        /// <exception cref="InvalidOperationException" />
        [Obsolete]
        public void Insert(int index, string item) => throw new NotImplementedException();
        /// 
        public void RemoveAt(int index) => throw new NotImplementedException();
        /// <summary>See ChangeValue() method.</summary>
        /// <exception cref="InvalidOperationException" />
        [Obsolete]
        public void Add(string item) => throw new InvalidOperationException();

        public void Clear()
        {
            this.currentPosition = -1;

            for (int i = 0; i < this.collection.Length; i++)
                this.collection[i] = null;
        }

        public bool Contains(string item) => IndexOf(item) > -1;

        public void CopyTo(string[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            for (int i = arrayIndex; i < array.Length && i < this.collection.Length; i++)
                array[i] = new string(this.collection[i]);
        }

        /// <exception cref="InvalidOperationException"/>
        [Obsolete]
        public bool Remove(string item) => throw new InvalidOperationException();

        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        #endregion
    }
}
