using HealthSync.Data;

namespace HealthSync.Views;

public partial class LifestylePage : ContentPage
{
	private DatabaseOperaties Database;

	public LifestylePage(DatabaseOperaties database)
	{
		InitializeComponent();
		Database = database;
    }
}