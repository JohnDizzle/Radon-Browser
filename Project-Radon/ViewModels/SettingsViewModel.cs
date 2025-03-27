using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Yttrium_browser.Contracts.Services;

namespace Yttrium_browser.ViewModels
{
    public class SettingsViewModel : ObservableRecipient
    {
        private readonly ISettingsService settingsService;
        private ElementTheme _elementTheme;
        private string _UserAgentWebCore = default;

        public string UserAgentWebCore
        {
            get { return _UserAgentWebCore; }
            set { SetProperty(ref _UserAgentWebCore, value); }
        }
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        RelayCommand<object> _switchUserAgent = null;
        public RelayCommand<object> SwitchUserAgent
        {
            get
            {
                if (_switchUserAgent != null)
                    return _switchUserAgent;
                _switchUserAgent = new RelayCommand<object>
                (
                       async o =>
                       {
                           settingsService.UserAgentWebCore = UserAgentWebCore;
                           await new ErrorsToUI(new System.Exception("You need to restart your application for changes to take affect")).SendMessage();

                       },
                        o => true
                )
                {

                };
                this.PropertyChanged += (s, e) => _switchUserAgent.NotifyCanExecuteChanged();
                return _switchUserAgent;
            }

        }
        RelayCommand<ElementTheme> _switchThemeCommand = null;
        public RelayCommand<ElementTheme> SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand != null)
                    return _switchThemeCommand;
                _switchThemeCommand = new RelayCommand<ElementTheme>
                (
                       async o =>
                        {
                            if (ElementTheme != o)
                            {
                                ElementTheme = o;
                                await _themeSelectorService.SetThemeAsync(o);
                            }
                        },
                        o => true
                );
                this.PropertyChanged += (s, e) => _switchThemeCommand.NotifyCanExecuteChanged();
                return _switchThemeCommand;
            }

        }
        //private ICommand _switchThemeCommand;


        public SettingsViewModel( ISettingsService _settingsService)
        {
           
            settingsService = _settingsService;
            VersionDescription = GetVersionDescription();

        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
