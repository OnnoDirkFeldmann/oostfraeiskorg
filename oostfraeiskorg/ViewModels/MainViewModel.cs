namespace oostfraeiskorg.ViewModels
{
    public class MainViewModel : MasterPageViewModel
    {

		public string Title { get; set;}

		public MainViewModel()
		{
			Title = "Hello from DotVVM!";
		}

    }
}
