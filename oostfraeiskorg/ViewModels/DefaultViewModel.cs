using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;

namespace oostfraeiskorg.ViewModels
{
    public class DefaultViewModel : MasterPageViewModel
    {

		public string Title { get; set;}

		public DefaultViewModel()
		{
			Title = "Hello from DotVVM!";
		}

    }
}
