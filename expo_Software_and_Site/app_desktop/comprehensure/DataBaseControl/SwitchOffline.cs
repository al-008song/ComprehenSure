using System;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Controls;  

namespace comprehensure.DataBaseControl
{
    internal class SwitchOffline 
    {
       
        private static SwitchOffline _instance; 

        private SwitchOffline()  
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        public static void Checker()  
        {
            Shell.Current.DisplayAlert("Connection Lost", "You are now offline.", "OK");  // im so killing my self
            if (_instance == null)
            {
                _instance = new SwitchOffline();
            }
        }


        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var current = e.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Connected", "You are now online.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Connection Lost", "You are now offline.", "OK");
            }
        }
    }
}