using AutomaticTranslationDayZ.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private ObservableCollection<Languages> _Languages;

        private string _variablename;
        private string _originalword;

        private Visibility _ringVisible = Visibility.Hidden;
        private bool _tranlateEnabled = true;

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

        public ObservableCollection<Languages> Languages
        {
            get => _Languages;
            set => SetProperty(ref _Languages, value);
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

        public Visibility RringVisible
        {
            get
            {
                return _ringVisible;
            }
            set
            {
                SetProperty(ref _ringVisible, value);
            }
        }

        public bool TransplateEnabled
        {
            get
            {
                return _tranlateEnabled;
            }
            set
            {
                SetProperty(ref _tranlateEnabled, value);
            }
        }

        public MainViewModel()
        {
            MyCollection = new ObservableCollection<Person>();

            Languages = new ObservableCollection<Languages>
            {
                //new Languages { Name = "Русский", ShortName = "ru", DayZName = "russian", IsChecked = true }, 
                new Languages { Name = "Английский", ShortName = "en", DayZName = "english", IsChecked = true }, 
                new Languages { Name = "Польский", ShortName = "pl", DayZName ="polish", IsChecked = true }, 
                new Languages { Name = "Французский", ShortName = "fr", DayZName = "french" , IsChecked = true }, 
                new Languages { Name = "Китайский", ShortName = "zh", DayZName = "chinese", IsChecked = true }, 
                new Languages { Name = "Испанский", ShortName = "es",DayZName = "spanish", IsChecked = true }, 
                new Languages { Name = "Чешский", ShortName = "cs",DayZName = "czech", IsChecked = true }, 
                new Languages { Name = "Итальянский", ShortName = "it", DayZName = "italian" ,IsChecked = true }, 
                new Languages { Name = "Немецкий",ShortName = "de", DayZName = "german", IsChecked = true } 
            };
        }

        public List<string> GetSelectedLanguages()
        {
            return Languages.Where(lang => lang.IsChecked).Select(lang => lang.DayZName).ToList();
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

        public ICommand TranslateButton => new RelayCommand(async () =>
        {
            RringVisible = Visibility.Visible;
            TransplateEnabled = false;
            var selectedLanguages = GetSelectedLanguages();
            var csvContent = new StringBuilder();

            csvContent.AppendLine($"\"Language\",\"original\",\"russian\",\"{string.Join("\",\"", selectedLanguages)}\"");

            foreach (var person in MyCollection)
            {
                var translations = await TranslateWordAsync(person.OriginalWord);
                var row = new List<string> { $"\"{person.Variable}\"", $"\"{translations["english"]}\"", $"\"{person.OriginalWord}\"", $"\"{translations["english"]}\"" };

                foreach (var language in selectedLanguages.Where(lang => lang != "english"))
                {
                    row.Add($"\"{translations[language]}\"");
                }

                csvContent.AppendLine(string.Join(",", row));
            }

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV файлы (*.csv)|*.csv";
            saveFileDialog.FileName = "translations.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, csvContent.ToString());    
            }

            RringVisible = Visibility.Hidden;
            TransplateEnabled = true;
        });

        public async Task<Dictionary<string, string>> TranslateWordAsync(string originalWord)
        {
            var selectedLanguages = GetSelectedLanguages();
            var translations = new Dictionary<string, string>();

            foreach (var language in Languages.Where(lang => lang.IsChecked))
            {
                try
                {
                    var url = $"https://api.mymemory.translated.net/get?q={originalWord}&langpair=ru|{language.ShortName}";
                    var response = await SendRequestAsync(url);
                    var json = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<TranslationResponse>(json);

                    if (data.responseData != null && data.responseData.translatedText != null)
                    {
                        translations.Add(language.DayZName, data.responseData.translatedText);
                    }
                    else
                    {
                        translations.Add(language.DayZName, "Перевод не найден");
                    }
                }
                catch (Exception ex)
                {
                    translations.Add(language.DayZName, $"Ошибка: {ex.Message}");
                }
            }

            return translations;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            var client = httpClient;
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                throw new Exception($"Ошибка API: {response.StatusCode}");
            }
        }

        private void ShowTranslations(Dictionary<string, string> translations)
        {
            var message = string.Join(Environment.NewLine, translations.Select(t => $"{t.Key}: {t.Value}"));

            Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();
            messageBox.Width = 450;
            messageBox.Height = 300;
            messageBox.Title = "Переводы";
            messageBox.Content = message;
            messageBox.CloseButtonText = "Закрыть";
            messageBox.ShowDialogAsync();
        }
    }

    


}
