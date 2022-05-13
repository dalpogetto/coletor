using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using System.Threading;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;

namespace CollectorQi.Views
{

    public static class  handlePop
    {
        public static ProgressBarPopUp handlePopUp { get; set; }
    }

    public partial class ProgressBarPopUp : PopupPage
    {
        public ProgressBarPopUp(string pStrProgressBar)
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = false;

            lblProgressBar.Text = pStrProgressBar;

            /* pb_ProgressBar.Progress = 0;
             pb_ProgressBar.Animate("SetProgress", (arg) => { pb_ProgressBar.Progress = arg; }, 100, 10000, Easing.Linear, null, null);
             */
            /*Task.Run(() => {
                for (int i = 0; i < 1000; i++)
                {
                    if (i == 999)
                        i = 0;
                    Thread.Sleep(100);
                    pb_ProgressBar.Progress = Convert.ToDouble("00." + i.ToString());
                }
            });*/

            handlePop.handlePopUp = this;

        }

        public void OnAcompanhar (string pStrAcompanhar)
        {
            lblProgressBar.Text = pStrAcompanhar;
        }

        public async Task<bool> OnClose()
        {
            await PopupNavigation.Instance.RemovePageAsync(this);

            return true;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        /*
        protected override void OnDisappearing()
        {
            try
            {

                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        } */
    }
}
