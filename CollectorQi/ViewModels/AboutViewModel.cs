using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace CollectorQi.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "QualiIT";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("http://wwww.qualiit.com.br")));
        }

        public ICommand OpenWebCommand { get; }
    }
}