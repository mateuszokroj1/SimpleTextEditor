using SimpleTextEditor.UI.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTextEditor.UI.Models
{
    public class OpenedFile : ModelBase
    {
        #region Constructors

        public OpenedFile(FileStream file)
        {
            if (file == null)
                throw new ArgumentNullException();

            if (!file.CanRead)
                throw new IOException("Cannot read from file.");

            this.file = file;
            this.fileHistory = new ContentHistory();
        }

        protected OpenedFile() { }

        #endregion

        #region Fields

        private bool isLoaded = false;
        private FileStream file;
        private string content;
        private ContentHistory fileHistory;
        private bool readOnlyMode;
        private Encoding encoding;

        #endregion

        #region Properties

        public FileStream File
        {
            get => this.file;
            protected set
            {
                if(this.file != value)
                {
                    this.file = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Content
        {
            get => this.content;
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    OnPropertyChanged();
                }
            }
        }

        public Encoding FileEncoding
        {
            get => this.encoding;
            protected set
            {
                if(this.encoding != value)
                {
                    this.encoding = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReadOnlyMode
        {
            get => this.readOnlyMode;
            protected set
            {
                if(this.readOnlyMode != value)
                {
                    this.readOnlyMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public ContentHistory FileHistory
        {
            get => this.fileHistory;
            protected set
            {
                if(this.fileHistory != value)
                {
                    this.fileHistory = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public Task LoadContentAsync(Encoding defaultEncoding)
        {
            if (this.file == null)
                throw new InvalidOperationException("File is not open.");

            if (!this.file.CanRead)
                throw new IOException("Cannot read from file.");

            IsReadOnlyMode = !file.CanWrite;

            file.Lock(0, file.Length);
            var reader = new StreamReader(file, defaultEncoding, true);

            FileEncoding = reader.CurrentEncoding;

            return Task.Run(()=>
            {
                Content = reader.ReadToEnd();
                FileHistory.ChangeValue(Content);

                reader?.Close();
                file.Unlock(0, file.Length);
            });
        }

        public async void LoadContent(Encoding defaultEncoding)
        => await LoadContentAsync(defaultEncoding);

        #endregion
    }
}
