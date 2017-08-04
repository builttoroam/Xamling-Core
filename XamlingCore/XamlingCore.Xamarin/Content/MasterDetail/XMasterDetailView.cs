﻿using System;
using System.Diagnostics;
using Xamarin.Forms;
using XamlingCore.Portable.Contract.Device;
using XamlingCore.Portable.Messages.View;
using XamlingCore.Portable.Messages.XamlingMessenger;

namespace XamlingCore.XamarinThings.Content.MasterDetail
{
    public class XMasterDetailView : MasterDetailPage
    {
        private readonly IOrientationSensor _orientationSensor;
        private XMasterDetailViewModel _viewModel;

        public XMasterDetailView(IOrientationSensor orientationSensor)
        {
            _orientationSensor = orientationSensor;

            this.Register<CollapseMasterDetailMessage>(_onCollapse);
            this.Register<ShowMasterDetailMessage>(_onShow);

            this.Register<EnableMenuSwipeGesture>(_onEnableSwipe);
            this.Register<DisableMenuSwipeGesture>(_onDisableSwipe);
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            try
            {
                _orientationSensor.OnRotated();
                base.OnSizeAllocated(width, height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        void _onEnableSwipe()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsGestureEnabled = true;
            });
        }

        void _onDisableSwipe()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsGestureEnabled = false;
            });
        }

        protected override void OnBindingContextChanged()
        {
            _viewModel = BindingContext as XMasterDetailViewModel;

            if (_viewModel == null)
            {
                throw new ArgumentException("BindingContext must be XMasterDetailViewModel");
            }

            _viewModel.PropertyChanged += _viewModel_PropertyChanged;

            _setContent();
            base.OnBindingContextChanged();
        }

        void _onShow(object obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsPresented = true;
            });
        }

        void _onCollapse(object obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsPresented = false;
            });
        }

        void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MasterContent" || e.PropertyName == "DetailContent")
            {
                _setContent();
            }
        }

        void _setContent()
        {
            if (_viewModel.MasterContent != null && _viewModel.MasterContent != Master && Master == null)
            {
                Master = _viewModel.MasterContent;
            }

            if (_viewModel.DetailContent != null && _viewModel.DetailContent != Detail)
            {
                Detail = _viewModel.DetailContent;
            }

            IsPresented = false;
        }

        protected override void OnDisappearing()
        {
            //don't clean up this stuff - this will run when a modal pops too, so when it unpops stuff no workies anymore
            //_viewModel.PropertyChanged -= _viewModel_PropertyChanged;
            base.OnDisappearing();
        }
    }
}
