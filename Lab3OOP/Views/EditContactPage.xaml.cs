using Lab3OOP.ViewModels;

namespace Lab3OOP.Views;

public partial class EditContactPage : ContentPage
{
    public EditContactPage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (BindingContext is EditContactViewModel vm)
        {
            if (NameEntry != null) vm.UI_FullName = NameEntry.Text;
            if (FacEntry != null) vm.UI_Faculty = FacEntry.Text;
            if (DepEntry != null) vm.UI_Department = DepEntry.Text;
            if (SpecEntry != null) vm.UI_Specialty = SpecEntry.Text;
            if (CollabEntry != null) vm.UI_Collaboration = CollabEntry.Text;
            if (TimeEntry != null) vm.UI_TimeFrame = TimeEntry.Text;

            await vm.Save();
        }
        else
        {
            await DisplayAlert("Помилка", "BindingContext is null", "OK");
        }
    }
}