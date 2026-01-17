using HealthSync.Data;

namespace HealthSync.Views;

public partial class DashboardPage : ContentPage
{
	private DatabaseOperaties Database;

	public DashboardPage(DatabaseOperaties database)
	{
		InitializeComponent();
		Database = database;
	}
}