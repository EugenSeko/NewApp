﻿using MapNotepad.Helpers;
using MapNotepad.Model;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace MapNotepad.Controls
{
    public class CustomMap : Map
    {
        private Geocoder geoCoder;
        public CustomMap()
        {
            UiSettings.MyLocationButtonEnabled = false;
            MyLocationEnabled = true;
            UiSettings.ZoomControlsEnabled = false;
            geoCoder = new Geocoder();
            PinClicked += CustomMap_PinClicked;
            MapClicked += CustomMapClicked;
            MapLongClicked += CustomMap_LongClickedAsync;
        }

        #region --- Public Properties ---

        private ICommand _goToMyLocationCommand;
        public ICommand GoToMyLocationCommand => _goToMyLocationCommand ??= SingleExecutionCommand.FromFunc(OnGoToMyLocationCommand);


        public static readonly BindableProperty PinProperty = BindableProperty.Create(
            propertyName: nameof(Pin),
            returnType: typeof(PinModel),
            declaringType: typeof(CustomMap),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);
        public PinModel Pin
        {
            get { return (PinModel)GetValue(PinProperty); }
            set { SetValue(PinProperty, value); }
        }

        public static readonly BindableProperty IsInputVMPositionFocusProperty = BindableProperty.Create(
            propertyName: nameof(IsInputVMPositionFocus),
            returnType: typeof(bool),
            declaringType: typeof(CustomMap),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);
        public bool IsInputVMPositionFocus
        {
            get { return (bool)GetValue(IsInputVMPositionFocusProperty); }
            set { SetValue(IsInputVMPositionFocusProperty, value);}
        }

        public static readonly BindableProperty MapClickedCommandProperty =
          BindableProperty.Create(nameof(MapClickedCommand), typeof(ICommand), typeof(CustomMap), null, BindingMode.TwoWay);
        public ICommand MapClickedCommand
        {
            get { return (ICommand)GetValue(MapClickedCommandProperty); }
            set { SetValue(MapClickedCommandProperty, value); }
        }

        public static readonly BindableProperty MapLongClickedCommandProperty =
          BindableProperty.Create(nameof(MapLongClickedCommand), typeof(ICommand), typeof(CustomMap), null, BindingMode.TwoWay);
        public ICommand MapLongClickedCommand
        {
            get { return (ICommand)GetValue(MapLongClickedCommandProperty); }
            set { SetValue(MapLongClickedCommandProperty, value); }
        }

        public static readonly BindableProperty PinClickedCommandProperty =
          BindableProperty.Create(nameof(PinClickedCommand), typeof(ICommand), typeof(CustomMap), null, BindingMode.TwoWay);
        public ICommand PinClickedCommand
        {
            get { return (ICommand)GetValue(PinClickedCommandProperty); }
            set { SetValue(PinClickedCommandProperty, value); }
        }

        public static readonly BindableProperty PinClickedCommandParameterProperty =
         BindableProperty.Create(nameof(PinClickedCommandParameter), typeof(PinModel), typeof(CustomMap), null, BindingMode.TwoWay);

        public PinModel PinClickedCommandParameter
        {
            get { return (PinModel)GetValue(PinClickedCommandParameterProperty); }
            set { SetValue(PinClickedCommandParameterProperty, value); }
        }


        public static readonly BindableProperty PinSourceProperty =
            BindableProperty.Create(nameof(PinSource), typeof(List<PinModel>), typeof(CustomMap), null);

        public List<PinModel> PinSource
        {
            get { return (List<PinModel>)GetValue(PinSourceProperty); }
            set { SetValue(PinSourceProperty, value);  }
        }
        #endregion
        #region --- Overrides ---
        protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(PinSource))
            {
                if (PinSource != null)
                {
                    Pins.Clear();

                    foreach (PinModel item in PinSource)
                    {

                        var pin = new Pin()
                        {
                            Type = PinType.Place,
                            Label = item.Label,
                            Position = new Position(item.Latitude, item.Longitude),
                            Address = item.Address
                        };

                        this.Pins.Add(pin);
                    }
                    MoveToRegion(MapSpan.FromCenterAndRadius(new Position(PinSource[0].Latitude, PinSource[0].Longitude), Distance.FromMeters(5000)));

                }
            }
            if(propertyName == nameof(IsInputVMPositionFocus))
            {
                if (PinSource == null && Pin !=null)
                {
                    IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(new Position(Pin.Latitude, Pin.Longitude));
                    string address = possibleAddresses.FirstOrDefault();

                    Pin.Address = address;
                    Pins.Clear();
                    Pins.Add(new Pin()
                    {
                        Type = PinType.Place,
                        Label = "",
                        Position = new Position(Pin.Latitude, Pin.Longitude),
                        Address = address
                    });
                    MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Pin.Latitude, Pin.Longitude), Distance.FromMeters(5000)));
                }
            }

        }
        #endregion
        #region --- Private Helpers ---
        private async Task OnGoToMyLocationCommand()
        {
            var locator = CrossGeolocator.Current;
            try
            {
                var position = await locator.GetPositionAsync();
                MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
                                                         Distance.FromMiles(1)));
            }
            catch
            {
                Console.WriteLine("Geolocation is not available"); // TODO: тут будет юзер диалог
            }
        }
        private async void CustomMap_LongClickedAsync(object sender, MapLongClickedEventArgs e)
        {
            IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(e.Point);
            string address = possibleAddresses.FirstOrDefault();
            if (PinSource == null || PinSource.Count<2)
            {
                Pin = new PinModel()
                {
                    Label = "",
                    Latitude = e.Point.Latitude,
                    Longitude = e.Point.Longitude,
                    Address = address
                };
                Pins.Clear();
                Pins.Add(new Pin()
                {
                    Type = PinType.Place,
                    Label = "",
                    Position = e.Point,
                    Address = address
                });
                MapLongClickedCommand?.Execute(Pin);
            }
            
        }
        private void CustomMapClicked(object sender, MapClickedEventArgs e)
        {
            MapClickedCommand?.Execute(null);
        }
        private void CustomMap_PinClicked(object sender, PinClickedEventArgs e)
        {
            var model = PinSource?.FirstOrDefault(item => item.Label == e.Pin.Label);
            if (model != null)
                PinClickedCommand?.Execute(model);
        }
        #endregion
    }
}
