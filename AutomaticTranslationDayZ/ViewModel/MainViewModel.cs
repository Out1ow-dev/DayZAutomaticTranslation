using AutomaticTranslationDayZ.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace AutomaticTranslationDayZ.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private ObservableCollection<Person> _myCollection;
        private Person _selectedPerson;
        private ObservableCollection<Language> _languages;      

        private string _variablename;
        private string _originalword;

        public ObservableCollection<Person> MyCollection
        {
            get { return _myCollection; }
            set
            {
                _myCollection = value;
                OnPropertyChanged(nameof(MyCollection));
            }
        }

        public Person SelectedPerson 
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }

        public ObservableCollection<Language> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        public string VariableName
        {
            get
            {
                return _variablename;
            }
            set 
            {
                SetProperty(ref _variablename, value);
            }
        }

        public string OriginalWord
        {
            get
            {
                return _originalword;
            }
            set
            {
                SetProperty(ref _originalword, value);
            }
        }

        public MainViewModel()
        {
            MyCollection = new ObservableCollection<Person>();

            Languages = new ObservableCollection<Language>
            {
                new Language { Name = "Русский", IsChecked = true },
                new Language { Name = "Английский", IsChecked = true },
                new Language { Name = "Польский", IsChecked = true },
                new Language { Name = "Французский", IsChecked = true },
                new Language { Name = "Китайский", IsChecked = true },
                new Language { Name = "Испанский", IsChecked = true },
                new Language { Name = "Чешский", IsChecked = true },
                new Language { Name = "Итальянский", IsChecked = true },
                new Language { Name = "Немецкий", IsChecked = true }
            };
        }

        public ICommand DeleteButton
        {
            get
            {
                return new RelayCommand(() => MyCollection.Remove(SelectedPerson));
            }
        }

        public ICommand AddButton
        {
            get
            {
                
                return new RelayCommand(() => AddNewElement());
            }
        }

        private void AddNewElement()
        {
            foreach (var person in MyCollection)
            {
                if (person.Variable == VariableName)
                {
                    Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();
                    messageBox.Width = 350;
                    messageBox.Height = 200;
                    messageBox.Title = "Ошибка";
                    messageBox.Content = "Данная переменная уже существует в списке.";
                    messageBox.CloseButtonText = "Закрыть";
                    messageBox.ShowDialogAsync();
                    return;
                }
            }
            MyCollection.Add(new Person { Variable = VariableName, OriginalWord = OriginalWord });
        }
    }

}
