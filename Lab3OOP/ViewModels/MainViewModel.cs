using Lab3OOP.Model;
using Lab3OOP.Services;
using Lab3OOP.Utils;
using Lab3OOP.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Contact = Lab3OOP.Model.Contact;

namespace Lab3OOP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly JsonContactsService _jsonService;
        private string? _currentFilePath;
        private string? _searchText;
        private string? _selectedFaculty;
        private string? _selectedCollaboration;
        private Contact? _selectedContact;

        public MainViewModel(JsonContactsService jsonService)
        {
            _jsonService = jsonService;

            OpenJsonCommand = new DelegateCommand(async _ => await OpenJson());
            SaveJsonCommand = new DelegateCommand(async _ => await SaveJson(), _ => Contacts.Any());

            AddContactCommand = new DelegateCommand(async _ => await AddContact());
            EditContactCommand = new DelegateCommand(async _ => await EditContact(), _ => SelectedContact != null);
            DeleteContactCommand = new DelegateCommand(async _ => await DeleteContact(), _ => SelectedContact != null);

            ClearFiltersCommand = new DelegateCommand(_ => ClearFilters());
        }

        #region Колекції
        public ObservableCollection<Contact> Contacts { get; } = new();
        public ObservableCollection<Contact> FilteredContacts { get; } = new();
        public ObservableCollection<string> Faculties { get; } = new();
        public ObservableCollection<string> Collaborations { get; } = new();
        #endregion

        #region Властивості
        public string? SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilters(); }
        }
        public string? SelectedFaculty
        {
            get => _selectedFaculty;
            set { _selectedFaculty = value; OnPropertyChanged(); ApplyFilters(); }
        }
        public string? SelectedCollaboration
        {
            get => _selectedCollaboration;
            set { _selectedCollaboration = value; OnPropertyChanged(); ApplyFilters(); }
        }
        public Contact? SelectedContact
        {
            get => _selectedContact;
            set { _selectedContact = value; OnPropertyChanged(); UpdateCanExecutes(); }
        }
        #endregion

        #region Команди
        public ICommand OpenJsonCommand { get; }
        public ICommand SaveJsonCommand { get; }
        public ICommand AddContactCommand { get; }
        public ICommand EditContactCommand { get; }
        public ICommand DeleteContactCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        #endregion

        private async Task OpenJson()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Оберіть JSON" });
                if (result == null) return;

                _currentFilePath = result.FullPath;
                var loaded = await _jsonService.LoadAsync(_currentFilePath);

                Contacts.Clear();
                foreach (var c in loaded) Contacts.Add(c);

                RebuildFilters();
                ApplyFilters();
                UpdateCanExecutes();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        private async Task SaveJson()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                _currentFilePath = Path.Combine(docs, "contacts.json");
            }

            try
            {
                await _jsonService.SaveAsync(_currentFilePath, Contacts);
                await Shell.Current.DisplayAlert("Успіх", $"Збережено в:\n{_currentFilePath}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Помилка збереження", ex.Message, "OK");
            }
        }

        private async Task AddContact()
        {
            var newContact = new Contact();

            var vm = new EditContactViewModel(newContact, "Додати контакт", (savedContact) =>
            {
                if (!string.IsNullOrEmpty(savedContact.FullName))
                {
                    Contacts.Add(savedContact);

                    SearchText = string.Empty;
                    SelectedFaculty = null;
                    SelectedCollaboration = null;

                    RebuildFilters();
                    ApplyFilters();
                    UpdateCanExecutes();
                }
            });

            var page = new EditContactPage { BindingContext = vm };
            await Shell.Current.Navigation.PushModalAsync(page);
        }

        private async Task EditContact()
        {
            if (SelectedContact == null) return;

            var vm = new EditContactViewModel(SelectedContact, "Редагувати", (savedContact) =>
            {
                RebuildFilters();
                ApplyFilters();
            });

            var page = new EditContactPage { BindingContext = vm };
            await Shell.Current.Navigation.PushModalAsync(page);
        }

        private async Task DeleteContact()
        {
            if (SelectedContact == null) return;
            bool answer = await Shell.Current.DisplayAlert("Видалення", $"Видалити {SelectedContact.FullName}?", "Так", "Ні");
            if (!answer) return;

            Contacts.Remove(SelectedContact);
            SelectedContact = null;
            RebuildFilters();
            ApplyFilters();
            UpdateCanExecutes();
        }

        private void ApplyFilters()
        {
            var query = Contacts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(c => c.FullName != null && c.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(SelectedFaculty))
                query = query.Where(c => c.Faculty == SelectedFaculty);

            if (!string.IsNullOrWhiteSpace(SelectedCollaboration))
                query = query.Where(c => c.Collaboration == SelectedCollaboration);

            FilteredContacts.Clear();
            foreach (var item in query) FilteredContacts.Add(item);
        }

        private void RebuildFilters()
        {
            var facs = Contacts.Select(c => c.Faculty).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x);
            Faculties.Clear();
            foreach (var f in facs) Faculties.Add(f);

            var cols = Contacts.Select(c => c.Collaboration).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x);
            Collaborations.Clear();
            foreach (var c in cols) Collaborations.Add(c);
        }
        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedFaculty = null;
            SelectedCollaboration = null;
        }

        private void UpdateCanExecutes()
        {
            (SaveJsonCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            (EditContactCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            (DeleteContactCommand as DelegateCommand)?.RaiseCanExecuteChanged();
        }
    }
}