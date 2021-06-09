using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Template10.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Words.Models;
using Words.Services;
using Words.Views;

namespace Words.ViewModels
{
    // A kezdõoldalhoz tartozó ViewModel.
    // Feladata, hogy kommunikáljon a fordító service-el (YandexDictionaryService)
    // és a kapott adatokat a View-nak szolgáltassa.
    // Emellett lehetõséget biztosít a szinoníma keresõ nézetre való navigáláshoz.
    public class MainPageViewModel : ViewModelBase
    {
        // Konstruktor, amely inicializálja az eseménykezelõket és a DelegateCommandot
        public MainPageViewModel()
        {
            LangFromChanged = new SelectionChangedEventHandler(UpdateLangsTo);
            LangToChanged = new SelectionChangedEventHandler(LanguageToChanged);
            TranslateCommand = new DelegateCommand(Translate, CanExecute);
        }

        private List<string> Langs { get; set; } = new List<string>();

        // A forrásnyelvek listája.
        // Csak az oldalra navigáláskor változnak az elemei.
        public ObservableCollection<string> LangsFrom { get; set; } =
            new ObservableCollection<string>();

        // A célnyelvek listája.
        // Amikor kiválasztjuk a forrásnyelvet,
        // a célnyelvek elemeit frissítjük aszerint,
        // hogy elérhetõek-e az adott forrásnyelvhez.
        public ObservableCollection<string> LangsTo { get; set; } =
            new ObservableCollection<string>();

        // A szervertõl kapott lefordított elemek.
        // Minden Translation objektumhoz tartozik:
        // Lefordított szó, típus és az eredeti nyelven jelentése a szónak a forrásnyelven
        public ObservableCollection<Translation> Translations { get; set; } =
            new ObservableCollection<Translation>();

        private string _word = "";
        // A felhasználó által begépelt szóhoz tatozó property
        public string Word
        {
            get { return _word; }
            set
            {
                _word = value;
                TranslateCommand.RaiseCanExecuteChanged();    
            }
        }

        private string from;
        // A felhasználó által kiválasztott forrásnyelvhez tatozó property
        public string From
        {
            get { return from; }
            set { from = value; }
        }

        private string to;
        // A felhasználó által kiválasztott célnyelvhez tatozó property
        public string To
        {
            get { return to; }
            set { to = value; }
        }

        // A kezdõlapra navigáláskor végrehajtódó függvény
        // Elkéri és elmenti a szervertõl a kiválasztható nyelvpárok listáját és
        // ez alapján feltölti a forrásnyelvek listáját.
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            try
            {
                var service = new YandexDictionaryService();
                var list = await service.GetLangsAsync();
                List<string> from = new List<string>();
                foreach (var lang in list)
                {
                    Langs.Add(lang);
                    string[] pair = lang.Split('-');
                    from.Add(pair[0]);
                }
                LangsFrom.AddRange(from.Distinct());
            }
            catch (HttpRequestException)
            {
                await DisplayDialog("Something went worng", "Please check your internet connection!");
                Application.Current.Exit();
            }
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        // A forrásnyelv megváltozásához tartozó eseménykezelõ property
        public SelectionChangedEventHandler LangFromChanged { get; }
        // A célnyelv megváltozásához tartozó eseménykezelõ property
        public SelectionChangedEventHandler LangToChanged { get; }
        // A fordításhoz tartozó Command property
        public DelegateCommand TranslateCommand { get; }

        private void UpdateLangsTo(object sender, SelectionChangedEventArgs e)
        {
            string fromItem = (sender as ComboBox).SelectedItem.ToString();
            LangsTo.Clear();
            foreach (var item in Langs)
            {
                string[] langPair = item.Split('-');
                if (fromItem == langPair[0] && langPair[0] != langPair[1] && !LangsTo.Contains(langPair[1]))
                    LangsTo.Add(langPair[1]);
            }
            TranslateCommand.RaiseCanExecuteChanged();
        }

        private async void Translate()
        {
            Translations.Clear();
            try
            {
                var service = new YandexDictionaryService();
                var translation = await service.GetTranslationAsync(Word.Trim(), $"{From}-{To}");
                if (translation.Def.Length == 0)
                    await DisplayDialog("Couldn't find translation", "This word was not found in our database.\nPlease try a different word or language.");
                else
                    foreach (var def in translation.Def)
                        foreach (var tr in def.Tr)
                        {
                            string meanings = "";
                            if (tr.Mean != null)
                            {
                                for (int i = 0; i < tr.Mean.Length - 1; i++)
                                    meanings += $"{tr.Mean[i].Text}, ";
                                meanings += tr.Mean[tr.Mean.Length - 1].Text;
                            }
                            Translations.Add(new Translation() { TranslatedWord = tr.Text, WordType = tr.Pos, Meaning = meanings });
                        }
            }
            catch (HttpRequestException)
            {
                await DisplayDialog("Something went worng", "Please check your internet connection!");
            }
        }

        private bool CanExecute() => !string.IsNullOrWhiteSpace(From) && !string.IsNullOrWhiteSpace(To) && !string.IsNullOrWhiteSpace(Word);

        private void LanguageToChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex != -1) {
                TranslateCommand.RaiseCanExecuteChanged();
            }
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

        // Az szinonimák oldalára navigáló függvény
        // Paraméterben átküldi a felhasználó által begépelt szót
        public void NavigateToSynonyms(object sender, Windows.UI.Xaml.RoutedEventArgs e) => NavigationService.Navigate(typeof(SynonimPage), _word.Trim());
    }
}

