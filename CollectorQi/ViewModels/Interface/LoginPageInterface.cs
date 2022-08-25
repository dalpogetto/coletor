using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CollectorQi.Views;

namespace CollectorQi.ViewModels.Interface
{
    public static class LoginPageInterface
    {

        static LoginPage _myLoginPage;

        public static async void ShowModalLogin(ContentPage modal)
        {
            // When you want to show the modal page, just call this method
            // add the event handler for to listen for the modal popping event:
            App.Current.ModalPopping += HandleModalPopping;

            _myLoginPage = new LoginPage();
            await modal.Navigation.PushModalAsync(_myLoginPage);
        }

        private static void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (e.Modal == _myLoginPage)
            {
                // now we can retrieve that phone number:
                //AtualizaListView();
                _myLoginPage = null;

                // remember to remove the event handler:
                App.Current.ModalPopping -= HandleModalPopping;
            }
        }

    }
}
