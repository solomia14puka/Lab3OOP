using System.Windows.Input;
using Lab3OOP.Utils;
using Contact = Lab3OOP.Model.Contact;

namespace Lab3OOP.ViewModels
{
    public class EditContactViewModel : BaseViewModel
    {
        public Contact Contact { get; private set; }

        public string UI_FullName { get; set; } = string.Empty;
        public string UI_Faculty { get; set; } = string.Empty;
        public string UI_Department { get; set; } = string.Empty;
        public string UI_Specialty { get; set; } = string.Empty;
        public string UI_Collaboration { get; set; } = string.Empty;
        public string UI_TimeFrame { get; set; } = string.Empty;

        public string Title { get; }

        private readonly Action<Contact>? _onSaved;

        public ICommand CancelCommand { get; }

        public EditContactViewModel(Contact contact, string title, Action<Contact>? onSaved = null)
        {
            Contact = contact;
            Title = title;
            _onSaved = onSaved;

            if (contact != null)
            {
                UI_FullName = contact.FullName;
                UI_Faculty = contact.Faculty;
                UI_Department = contact.Department;
                UI_Specialty = contact.Specialty;
                UI_Collaboration = contact.Collaboration;
                UI_TimeFrame = contact.TimeFrame;
            }

            CancelCommand = new DelegateCommand(async _ => await Cancel());
        }

        public async Task Save()
        {
            Contact.FullName = UI_FullName;
            Contact.Faculty = UI_Faculty;
            Contact.Department = UI_Department;
            Contact.Specialty = UI_Specialty;
            Contact.Collaboration = UI_Collaboration;
            Contact.TimeFrame = UI_TimeFrame;

            _onSaved?.Invoke(Contact);

            await Shell.Current.Navigation.PopModalAsync();
        }

        private async Task Cancel()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}