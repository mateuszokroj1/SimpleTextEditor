using SimpleTextEditor.UI.Helpers;
using SimpleTextEditor.UI.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleTextEditor.UI.ViewModels
{
    public class MainWindowViewModel : ModelBase
    {
        #region Constructors
        
        public MainWindowViewModel(Window window)
        {
            this.window = window;
        }

        public MainWindowViewModel(Window window, params FileStream[] filesToOpen) : this(window)
        {
            foreach (var file in filesToOpen)
#pragma warning disable CS4014
                OpenFileAsync(file);
#pragma warning restore CS4014
        }

        #endregion

        #region Fields

        protected OpenedFile focusedFile;
        protected Window window;

        #endregion

        #region Properties

        public ObservableCollection<OpenedFile> OpenedFiles { get; protected set; } = new ObservableCollection<OpenedFile>();

        public OpenedFile FocusedFile
        {
            get => this.focusedFile;
            set
            {
                if(this.focusedFile != value)
                {
                    this.focusedFile = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public async Task OpenFileAsync(FileStream file)
        => await Task.Run(()=>
        {
            
        });

        public async void OpenFile(FileStream file)
        => await OpenFileAsync(file);

        public async Task CreateNewFileAsync()
        => await Task.Run(()=>
        {
            
        });

        #endregion
    }
}
