using HealthSync.Data;
using HealthSync.Models;

namespace HealthSync.Views;

public partial class MedicatiePage : ContentPage
{
	private DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }
    public MedicatiePage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
		IngelogdeUser = ingelogdeUser;
		BindingContext = this;	
    }
}