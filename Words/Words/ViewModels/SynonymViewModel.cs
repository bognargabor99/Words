using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Utils;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Words.Services;

namespace Words.ViewModels
{
    class SynonymViewModel : ViewModelBase
    {
        private string _word;
        // A felhasználó által begépelt szóhoz tatozó property
        public string Word
        {
            get { return _word; }
            set
            {
                _word = value;
                FindSynonymsCommand.RaiseCanExecuteChanged();
            }
        }

        private string _lang;
        // A felhasználó által begépelt nyelvhez tatozó property
        public string Lang
        {
            get { return _lang; }
            set 
            {
                _lang = value;
                FindSynonymsCommand.RaiseCanExecuteChanged();
            }
        }

        private string _selected;
        // A szervertől kapott szinonímák közül az,
        // amelyiket a felhasználó kijelöléssel kiválasztott
        public string Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                FindSynonymsForSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        // A kiválasztható nyelvek listája
        // A begépelt/kiválasztott szót a kiválasztott nyelven fogja értelmezni a szerver
        // és a szinonímákat is ilyen nyelven küldi vissza
        public ObservableCollection<string> Languages { get; set; } =
            new ObservableCollection<string>();

        // A szervertől visszakapott szinonímák listája
        public ObservableCollection<string> Synonyms { get; set; } =
            new ObservableCollection<string>();

        // A begépelt szó szinonímáinak lekérdezéséhez tartozó Command
        public DelegateCommand FindSynonymsCommand;

        // A kiválasztott szó szinonímáinak lekérdezéséhez tartozó Command
        public DelegateCommand FindSynonymsForSelectedCommand;

        // Konstruktor, amely inicializálja a két DelegateCommandot
        public SynonymViewModel()
        {
            FindSynonymsCommand = new DelegateCommand(FindSynonymsForTyped, CanExecute);
            FindSynonymsForSelectedCommand = new DelegateCommand(FindSynonymsForSelected, CanExecuteForSelected);
        }

        // Az oldalra navigáláskor végrehajtódó függvény
        // Elkéri a szervertől a kiválasztható nyelvek listáját és
        // ez alapján feltölti a nyelvek listáját.
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Word = (string)parameter;

            var service = new SaurusService();
            Languages.AddRange(service.GetLangs());

            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        private async void FindSynonymsAsync(string word)
        {
            try
            {
                Synonyms.Clear();
                var service = new SaurusService();
                var synonyms = await service.GetSynonymAsync(word, Lang);
                if (synonyms.Response == null)
                {
                    await DisplayDialog("Couldn't find synonyms", "This word was not found in our database.\nPlease try a different word or language.");
                    return;
                }
                foreach (var item in synonyms.Response)
                {
                    string synonymGroup = item.List.Synonyms;
                    if (synonymGroup.EndsWith('|'))
                        synonymGroup = synonymGroup.Remove(synonymGroup.Length - 1);
                    var synonymsSplit = synonymGroup.Split('|');
                    foreach (var text in synonymsSplit)
                    {
                        Synonyms.Add((text.Contains("(")) ? text.Substring(0, text.IndexOf("(") - 1) : text);
                    }
                }
                var syns = Synonyms.Distinct().ToList();
                Synonyms.Clear();
                Synonyms.AddRange(syns);
            }
            catch (HttpRequestException)
            {
                await DisplayDialog("Something went worng", "Please check your internet connection!");
            }
        }

        private void FindSynonymsForSelected()
        {
            FindSynonymsAsync(Selected);
        }

        private void FindSynonymsForTyped()
        {
            FindSynonymsAsync(Word);
        }

        private async Task DisplayDialog(string title, string content)
        {
            ContentDialog noResultDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };
            await noResultDialog.ShowAsync();
        }

        private bool CanExecute() => !string.IsNullOrWhiteSpace(Word) && !string.IsNullOrWhiteSpace(Lang);
        
        private bool CanExecuteForSelected() => !string.IsNullOrWhiteSpace(Selected) && !string.IsNullOrWhiteSpace(Lang);
    }
}
