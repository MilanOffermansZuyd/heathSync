using HealthSync.Data;
using HealthSync.Models;

namespace HealthSync.Views;

public partial class EmergencyHelpPage : ContentPage
{
    private readonly DatabaseOperaties Database;
    private readonly User IngelogdeUser;

    private EmergencyContact _selectedContact;

    public EmergencyHelpPage(DatabaseOperaties database, User ingelogdeUser)
    {
        InitializeComponent();
        Database = database;
        IngelogdeUser = ingelogdeUser;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadContactsAsync();
        ResetForm();
    }

    private async Task LoadContactsAsync()
    {
        ContactsView.ItemsSource =
            await Database.GetEmergencyContactsByUserIdAsync(IngelogdeUser.Id);
    }

    private void ResetForm()
    {
        _selectedContact = null;
        NameEntry.Text = string.Empty;
        PhoneEntry.Text = string.Empty;
        DeleteButton.IsVisible = false;
    }

    private void OnContactSelected(object sender, SelectionChangedEventArgs e)
    {
        _selectedContact = e.CurrentSelection.FirstOrDefault() as EmergencyContact;

        if (_selectedContact == null)
            return;

        NameEntry.Text = _selectedContact.Name;
        PhoneEntry.Text = _selectedContact.PhoneNumber;
        DeleteButton.IsVisible = true;
        ContactsView.SelectedItem = null;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(PhoneEntry.Text))
        {
            await DisplayAlert("Fout", "Vul alle velden in", "OK");
            return;
        }

        if (_selectedContact == null)
        {
            _selectedContact = new EmergencyContact
            {
                Name = NameEntry.Text.Trim(),
                PhoneNumber = PhoneEntry.Text.Trim(),
                UserId = IngelogdeUser.Id
            };

            await Database.AddEmergencyContactAsync(_selectedContact);
        }

        _selectedContact.Name = NameEntry.Text.Trim();
        _selectedContact.PhoneNumber = PhoneEntry.Text.Trim();

        await LoadContactsAsync();
        ResetForm();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_selectedContact == null)
            return;

        bool confirm = await DisplayAlert(
            "Verwijderen",
            "Weet je zeker dat je dit contact wilt verwijderen?",
            "Ja",
            "Nee");

        if (!confirm)
            return;

        await Database.DeleteEmergencyContactAsync(_selectedContact);
        await LoadContactsAsync();
        ResetForm();
    }

    private void OnNewClicked(object sender, EventArgs e)
    {
        ResetForm();
    }
}