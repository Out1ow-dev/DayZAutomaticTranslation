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

namespace AutomaticTranslationDayZ.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private ObservableCollection<Person> _myCollection;
        private Person _selectedPerson;

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
                    MessageBox.Show("Данная переменная уже сущесвует.");
                    return;
                }
            }
            MyCollection.Add(new Person { Variable = VariableName, OriginalWord = OriginalWord });
        }
    }

}
