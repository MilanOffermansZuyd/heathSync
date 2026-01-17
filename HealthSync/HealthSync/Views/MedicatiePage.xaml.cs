using HealthSync.Data;

namespace HealthSync.Views;

public partial class MedicatiePage : ContentPage
{
	private DatabaseOperaties Database;
    public MedicatiePage(DatabaseOperaties database)
	{
		InitializeComponent();
		Database = database;
    }
}