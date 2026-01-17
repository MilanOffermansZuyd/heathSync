using HealthSync.Data;
using HealthSync.Models;

namespace HealthSync.Views;

public partial class LifestylePage : ContentPage
{
	private DatabaseOperaties Database;
    public User IngelogdeUser { get; set; }

    public LifestylePage(DatabaseOperaties database, User ingelogdeUser)
	{
		InitializeComponent();
		Database = database;
        IngelogdeUser = ingelogdeUser;
        BindingContext = this;
    }
}