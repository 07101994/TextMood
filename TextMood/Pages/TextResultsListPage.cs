﻿using System;

using Xamarin.Forms;

namespace TextMood
{
	public class TextResultsListPage : BaseContentPage<TextResultsListViewModel>
	{
		#region Constant Fields
		readonly ListView _textModelList;
		readonly ToolbarItem _setupPageToolbarItem;
		#endregion

		#region Constructors
		public TextResultsListPage()
		{
			_setupPageToolbarItem = new ToolbarItem { Text = "Setup" };
			ToolbarItems.Add(_setupPageToolbarItem);

			_textModelList = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				ItemTemplate = new DataTemplate(typeof(TextMoodViewCell)),
				IsPullToRefreshEnabled = true,
				HasUnevenRows = true,
			};
			_textModelList.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsRefreshing));
			_textModelList.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.TextList));
			_textModelList.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.PullToRefreshCommand));
			_textModelList.SetBinding(ListView.BackgroundColorProperty, nameof(ViewModel.BackgroundColor));

			Title = PageTitles.TextResultsPage;

			Content = _textModelList;
		}
		#endregion

		#region Methods
		protected override async void OnAppearing()
		{
			base.OnAppearing();

			Device.BeginInvokeOnMainThread(_textModelList.BeginRefresh);

			await SignalRService.Subscribe().ConfigureAwait(false);
		}

		protected override void SubscribeEventHandlers()
		{
			_textModelList.ItemTapped += HandleItemTapped;
			ViewModel.ErrorTriggered += HandleErrorTriggered;
			_setupPageToolbarItem.Clicked += HandleSetupPageToolbarItemClicked;
			BaseSignalRService.InitializationFailed += HandleInitializationFailed;
		}

		protected override void UnsubscribeEventHandlers()
		{
			_textModelList.ItemTapped -= HandleItemTapped;
			ViewModel.ErrorTriggered -= HandleErrorTriggered;
			_setupPageToolbarItem.Clicked -= HandleSetupPageToolbarItemClicked;
			BaseSignalRService.InitializationFailed -= HandleInitializationFailed;
		}

		void HandleSetupPageToolbarItemClicked(object sender, EventArgs e) =>
			Device.BeginInvokeOnMainThread(async () => await Navigation.PushModalAsync(new BaseNavigationPage(new HueBridgeSetupPage())));

		void HandleErrorTriggered(object sender, string message) => DisplayErrorMessage(message);

		void HandleInitializationFailed(object sender, string message) => DisplayErrorMessage(message);

		void DisplayErrorMessage(string message) => Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", message, "OK "));

		void HandleItemTapped(object sender, ItemTappedEventArgs e)
		{
			var listView = sender as ListView;
			listView.SelectedItem = null;
		}
		#endregion
	}
}
