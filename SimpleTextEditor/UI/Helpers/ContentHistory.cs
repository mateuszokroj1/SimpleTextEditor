using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleTextEditor.UI.Helpers
{
    public class ContentHistory : ModelBase, IEnumerable<string>, IReadOnlyList<string>, IObservable<string>
    {
        #region Constructors

        public ContentHistory([Range(2, ushort.MaxValue)]ushort historySize = 100)
        {
            this.collection = new string[historySize];
            for (int i = 0; i < historySize; i++)
                this.collection[i] = null;

            HistorySize = historySize;

            // Notify about changed properties
            OnPropertyChanged(nameof(CurrentValue));
            OnPropertyChanged(nameof(HistorySize));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(CanRedo));
            OnPropertyChanged(nameof(CanUndo));
        }

        #endregion

        #region Fields

        protected string[] collection;
        protected int currentPosition = -1;
        protected List<IObserver<string>> observers = new List<IObserver<string>>();

        #endregion

        #region Properties

        public int Count => this.currentPosition + 1;

        public ushort HistorySize { get; protected set; }

        /// <summary>
        /// Gets the current setted value 
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public string CurrentValue
        {
            get
            {
                if (currentPosition == -1)
                    throw new InvalidOperationException();

                return this.collection[currentPosition];
            }
        }

        public bool CanRedo
            => currentPosition < this.collection.Length - 1 &&
               this.collection[this.currentPosition + 1] != null;
        
        public bool CanUndo => currentPosition > 0;

        public string this[int index]
        {
            get
            {
                if (index > this.currentPosition)
                    throw new IndexOutOfRangeException();

                return this.collection[index];
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set new version in history
        /// </summary>
        /// <returns><see cref="ContentHistoryChangeValueMode"/> that determins the last value was removed from history</returns>
        /// <exception cref="ArgumentException"/>
        public ContentHistoryChangeValueMode ChangeValue(string value)
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

                // Notify about changed properties
                OnPropertyChanged(nameof(CurrentValue));

                return ContentHistoryChangeValueMode.AddedWithRemovingOldest;
            }
            else
            {
                this.collection[++this.currentPosition] = val;

                List<string> removedValues = new List<string>();
                for (int i = this.currentPosition; i < this.collection.Length; i++)
                    if (this.collection[i] != null)
                    {
                        removedValues.Add(this.collection[i]);
                        this.collection[i] = null;
                    }

                // Notify about change
                foreach(var subscriber in this.observers)
                {
                    subscriber.OnNext(val);

                    foreach (var removedValue in removedValues)
                        subscriber.OnNext(removedValue);

                    subscriber.OnCompleted();
                }

                // Notify about changed properties
                OnPropertyChanged(nameof(CurrentValue));
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(CanRedo));
                OnPropertyChanged(nameof(CanUndo));

                return ContentHistoryChangeValueMode.AddedWithoutRemovingOldest;
            }
        }

        protected void MoveDown()
        {
            for (int i = 1; i < this.collection.Length; i++)
                this.collection[i - 1] = this.collection[i];
        }

        /// <summary>
        /// Undo history if possible.
        /// </summary>
        /// <returns><see langword="true"/> if undo was possible, else <see langword="false"/></returns>
        public bool Undo()
        {
            if(CanUndo)
            {
                currentPosition--;

                // Notify about changed properties
                OnPropertyChanged(nameof(CurrentValue));
                OnPropertyChanged(nameof(CanRedo));
                OnPropertyChanged(nameof(CanUndo));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Redo history if possible.
        /// </summary>
        /// <returns><see langword="true"/> if undo was possible, else <see langword="false"/></returns>
        public bool Redo()
        {
            if(CanRedo)
            {
                currentPosition++;

                // Notify about changed properties
                OnPropertyChanged(nameof(CurrentValue));
                OnPropertyChanged(nameof(CanRedo));
                OnPropertyChanged(nameof(CanUndo));

                return true;
            }

            return false;
        }

        public IEnumerator<string> GetEnumerator() => new ContentHistoryEnumerator(ref this.collection);
        IEnumerator IEnumerable.GetEnumerator() => new ContentHistoryEnumerator(ref this.collection);

        /// <summary>
        /// Search for item in argument
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns index from 0 to max available index or -1 if not found</returns>
        public int IndexOf(string item)
        {
            if (item == null)
                return -1;

            for (int i = 0; i <= this.collection.Length; i++)
                if (this.collection[i]?.Equals(item) ?? false)
                    return i;

            return -1;
        }

        /// <summary>
        /// Removes all items
        /// </summary>
        public void Clear()
        {
            this.currentPosition = -1;

            List<string> removedValues = new List<string>(this.collection.Length);

            for (int i = 0; i < this.collection.Length; i++)
                if(this.collection[i] != null)
                {
                    removedValues.Add(this.collection[i]);
                    this.collection[i] = null; 
                }

            // Notify subscribers
            foreach(var subscriber in this.observers)
            {
                foreach (var removedValue in removedValues)
                    subscriber.OnNext(removedValue);

                subscriber.OnCompleted();
            }

            // Notify about changed properties
            OnPropertyChanged(nameof(CurrentValue));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public bool Contains(string item) => IndexOf(item) > -1;

        /// <summary>
        /// Copy all values to new <see cref="string"/>[] array
        /// </summary>
        /// <param name="array">Destination array</param>
        /// <param name="arrayIndex">Start index</param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            for (int i = arrayIndex; i < array.Length && i < this.collection.Length; i++)
                array[i] = new string(this.collection[i]);
        }

        /// <summary>
        /// Register new <see cref="IObserver{T}"/> object to current <see cref="IObservable{T}"/> collection.
        /// </summary>
        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (observer == null)
                throw new ArgumentNullException();

            if (!observers.Contains(observer))
                observers.Add(observer);
            else return new Unsubscriber(this.observers, observer);

            for (int i = 0; i <= this.currentPosition; i++)
                observer.OnNext(this.collection[i]);

            observer.OnCompleted();

            return new Unsubscriber(observers, observer);
        }

        #endregion
    }
}
