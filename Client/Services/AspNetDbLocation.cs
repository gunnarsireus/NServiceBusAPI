namespace Client.Services
{
	using System.Threading.Tasks;

	// This class is used by the application to get the location of the AspNet.db used for authntication of users.
	// can be replaced with LocalDB
	public class dBlocations
    {
	    public Task GetDbLocationAsync()
	    {
		    return Task.CompletedTask;
		}
	}
}
