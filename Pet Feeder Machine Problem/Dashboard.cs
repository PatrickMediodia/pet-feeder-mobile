﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Pet_Feeder_Machine_Problem.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Pet_Feeder_Machine_Problem
{
    [Activity(Label = "Dashboard", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class Dashboard : Activity
    {
        TextView temperatureTxt, humidityTxt, foodTxt, waterTxt, timestampTxt;
        Button dispeseBtn, dispenseLogBtn, accountManagementBtn, viewSlotsBtn;
        HttpClient client;
        Timer RefreshDataTimer;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.dashboard);

            temperatureTxt = FindViewById<TextView>(Resource.Id.temperatureTxt);
            humidityTxt = FindViewById<TextView>(Resource.Id.humidityTxt);
            foodTxt = FindViewById<TextView>(Resource.Id.foodTxt);
            waterTxt = FindViewById<TextView>(Resource.Id.waterTxt);
            timestampTxt = FindViewById<TextView>(Resource.Id.timestampTxt);
            
            dispeseBtn = FindViewById<Button>(Resource.Id.dispenseBtn);
            dispenseLogBtn = FindViewById<Button>(Resource.Id.btnDispenseLog);
            accountManagementBtn = FindViewById<Button>(Resource.Id.accountManagementBtn);
            viewSlotsBtn = FindViewById<Button>(Resource.Id.btnViewSlots);

            RunOnUiThread(async () =>
            {
                await UpdateStatus();
            });

            RefreshDataTimer = new Timer(1000);
            RefreshDataTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            RefreshDataTimer.AutoReset = true;
            RefreshDataTimer.Enabled = true;

            dispenseLogBtn.Click += DispenseLog;
            dispeseBtn.Click += ManualDispense;
            accountManagementBtn.Click += AccountManagement;
            viewSlotsBtn.Click += DispenseSlots;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            RunOnUiThread(async () =>
            {
                await UpdateStatus();
            });
        }

        public async Task UpdateStatus() 
        {
            client = new HttpClient();

            string url = RESTAPI.url() + $"getEnvironmentAndSupplyStatus.php";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<EnvironmentAndSupply>(result);

                temperatureTxt.Text = responseObject.temperature;
                humidityTxt.Text = responseObject.humidity;
                foodTxt.Text = responseObject.foodLevel;
                waterTxt.Text = responseObject.waterLevel;

                string input = responseObject.timestamp;
                var timeFromInput = DateTime.ParseExact(input, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None);
                string timeIn12HourFormatForDisplay = timeFromInput.ToString("MM/dd/yyy hh:mm tt", CultureInfo.InvariantCulture);
                timestampTxt.Text = timeIn12HourFormatForDisplay;
            }
            else
            {
                Toast.MakeText(this, "Error", ToastLength.Long).Show();
            }
        }

        public void ManualDispense(object source, EventArgs e)
        {
            Intent i = new Intent(this, typeof(Dispense));
            StartActivity(i);
        }

        public void DispenseLog(object source, EventArgs e)
        {
            Intent i = new Intent(this, typeof(DispenseLog));
            StartActivity(i);
        }

        public void AccountManagement(object source, EventArgs e)
        {
            Intent i = new Intent(this, typeof(AccountManagement));
            StartActivity(i);
        }

        public void DispenseSlots(object source, EventArgs e)
        {
            Intent i = new Intent(this, typeof(DispenseSlots));
            StartActivity(i);
        }
    }
}