﻿using GeneralConfigSetter.Enums;
using GeneralConfigSetter.Models;
using Microsoft.Win32;
using WpfFramework.Core;

namespace GeneralConfigSetter.ViewModels
{
    public class DeleterConfigViewModel : ViewModelBase
    {
        private string _queryTag = "";
        private string _linkInput = "";
        private IContext _context;

        public string QueryTag
        {
            get { return _queryTag; }
            set { SetField(ref _queryTag, value, nameof(QueryTag)); }
        }
        public string LinkInput
        {
            get { return _linkInput; }
            set { SetField(ref _linkInput, value, nameof(LinkInput)); }
        }
        public IContext Context
        {
            get { return _context; }
            set
            {
                SetField(ref _context, value, nameof(Context));
            }
        }

        private void UpdateUiProperties()
        {
            QueryText = Context.QueryText;
            TargetServerName = Context.TargetServerName.Equals("defaultKey") ? "< Invalid link!!! >" : Context.TargetServerName;
            TargetCollectionUrl = Context.TargetCollectionUrl;
            TargetProjectName = Context.TargetProjectName;
        }

        private string _queryText = "";
        private string _targetServerName = "";
        private string _targetCollectionUrl = "";
        private string _targetProjectName = "";

        public string QueryText
        {
            get { return _queryText; }
            set { SetField(ref _queryText, value, nameof(QueryText)); }
        }        
        public string TargetServerName
        {
            get { return _targetServerName; }
            set { SetField(ref _targetServerName, value, nameof(TargetServerName)); }
        }
        public string TargetCollectionUrl
        {
            get { return _targetCollectionUrl; }
            set { SetField(ref _targetCollectionUrl, value, nameof(TargetCollectionUrl)); }
        }
        public string TargetProjectName
        {
            get { return _targetProjectName; }
            set { SetField(ref _targetProjectName, value, nameof(TargetProjectName)); }
        }


        public RelayCommand ExtractLinkDataCommand { get; set; }
        public RelayCommand UpdateDeleterConfigCommand { get; set; }
        public RelayCommandGeneric<NotificationModel, bool> ShowMessageCommand { get; internal set; }

        public DeleterConfigViewModel(IContext context)
        {
            ExtractLinkDataCommand = new RelayCommand(ExtractLinkData, IsExtractLinkDataEnabled);
            UpdateDeleterConfigCommand = new RelayCommand(UpdateDeleterConfig, IsUpdateDeleterConfigEnabled);
            Context = context;
        }

        private void ExtractLinkData()
        {
            Context.QueryText = $"AND [System.Tags] contains '{QueryTag}' AND [System.WorkItemType] NOT IN ('Test Suite', 'Test Plan', 'Test Case')";
            Services.LinkService.GetTargetData(Context, LinkInput);
            UpdateUiProperties();
        }

        private bool IsExtractLinkDataEnabled()
        {
            if (QueryTag != "" && LinkInput != "")
            {
                return true;
            }
            return false;
        }

        private void UpdateDeleterConfig()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            var result = openFileDialog.ShowDialog();

            if (result != null && result == true)
            {
                //Get the path of specified file
                string filePath = openFileDialog.FileName;

                Services.ConfigUpdateService.UpdateDeleterConfig(Context, filePath);
                ShowMessageCommand.Execute(new NotificationModel("SUCCESS!!!!", NotificationType.Information));
            }
            else
            {
                ShowMessageCommand.Execute(new NotificationModel("But why? :(", NotificationType.Error));
            }
        }

        private bool IsUpdateDeleterConfigEnabled()
        {
            if (TargetCollectionUrl != "" && TargetProjectName != "" && TargetServerName != "")
            {
                return true;
            }
            return false;
        }
    }
}