﻿using MAUIMiniApp.Views;
using YAP.Libs.Alerts;

namespace MAUIMiniApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
            //count++;

            //if (count == 1)
            //{
            //    CounterBtn.Text = $"Clicked {count} time";
            //}
            //else
            //{
            //    CounterBtn.Text = $"Clicked {count} times";
            //}

            //SemanticScreenReader.Announce(CounterBtn.Text);

            //Toasts.Show("Hello");
            //Snackbars.Show("Hello");
        }
    }
}