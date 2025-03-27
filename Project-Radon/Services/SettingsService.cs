using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Project_Radon.Contracts.Services;
using Newtonsoft.Json;
using Project_Radon.Models;
using Project_Radon.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;


namespace Project_Radon.Services
{
    interface IWebDivSettings
    {

        Uri HomeUrl { get; set; }
        bool SideKick { get; set; }
        int DefaultSearchProvider { get; set; }
        bool IsLocationOnOff { get; set; }
        string UserAgentWebCore { get; set; }
        ElementTheme ThemeDefault { get; set; }
        Visibility VisibilitySideToolBar { get; set; }
        ObservableCollection<HistoryModel> HistoryModels { get; set; }
        string HomeUrlString { get; set; }
        bool TitleBarPinned { get; set; }

    }

    public partial class WebDiveSettings : ObservableObject, IWebDivSettings 
    {
        private object settings;

        public WebDiveSettings()
        {
        }

        public WebDiveSettings(object settings)
        {
            this.settings = settings;
        }

        public Uri HomeUrl { get; set; }
        public bool SideKick { get; set; }
        public int DefaultSearchProvider { get; set; }
        public bool IsLocationOnOff { get; set; }
        public string UserAgentWebCore { get; set; }
        public ElementTheme ThemeDefault { get; set; }
        public Visibility VisibilitySideToolBar { get; set; }
        public bool TitleBarPinned { get; set; }
        public string HomeUrlString { get; set; }
        public ObservableCollection<HistoryModel> HistoryModels { get; set; }

    }

    class SettingsService : ISettingsService
    {
        public static SettingsService Instance { get; set; }
        public WebDiveSettings AppSettings { get; set; }
        public event EventHandler<NotifyCollectionChangedEventArgs> FavoritesCollectionChanged;
        public event EventHandler<NotifyCollectionChangedEventArgs> HistoryCollectionChanged;
        internal string CorePathFavorites { get; set; } = System.Security.Principal.WindowsIdentity.GetCurrent().Name! + @"\Favorites\";
        internal string CorePathSettings { get; set; } =  System.Security.Principal.WindowsIdentity.GetCurrent().Name! + @"\Settings\";
        public SettingsService()
        {
            InitializeAsync();
            
            var path = Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER");

            if (File.Exists(Path.Combine(path, "Settings", "settings.json")))
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(path, "settings", "settings.json"));
                if (fileInfo.Length <= 0)
                {
                    AppSettings = new() { DefaultSearchProvider = 0, HomeUrl = new Uri("https://google.com"), IsLocationOnOff = false, SideKick = false };
                    File.WriteAllText(Path.Combine(path, "settings", "settings.json"), JsonConvert.SerializeObject(AppSettings));
                }
            }
            else
            {

                 using (var fs = File.Create(Path.Combine(path, "Settings", "settings.json"))) ;

            }

            Instance = this;
            FavoritesStore.CollectionChanged += FavoritesStore_CollectionChanged;
            historyModels.CollectionChanged += HistoryStore_CollectionChanged;

        }

        public async void InitializeAsync()
        {
            var settings = await ReadSettings<WebDiveSettings>();
            if (settings is not null)
            {
                AppSettings = settings;
            }
            else {
                AppSettings = new WebDiveSettings();
            }

        }

        private void HistoryStore_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // this is bubbling up to historyviewmodel and mainpageviewmodel for myhistory collection. need to add to other pages sortly 
            HistoryCollectionChanged?.Invoke(this, e);
        }

        private void FavoritesStore_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FavoritesCollectionChanged?.Invoke(this, e);
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> FileExistsAsync(string fileName)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder; 
                var file = await storageFolder.GetFileAsync(fileName);
                return file != null;
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
        }

        async Task<T> ReadFavorites<T>()
        {
            string json = "{}";
            string path = Path.Combine(Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER"), CorePathFavorites);
            string fileFavs = Path.Combine(path, "favorites.json");

            try
            {

                if (File.Exists(fileFavs))
                {
                    json = File.ReadAllText(fileFavs);
                    var settings = JsonConvert.DeserializeObject<T>(json);
                    return await Task.FromResult<T>(settings);

                }
                else
                {
                    var dir = Directory.CreateDirectory(path);
                    using (var fs = File.Create(fileFavs)) ;
                    return await Json.ToObjectAsync<T>(json);

                }

            }
            catch (Exception)
            {

                throw;
            }



        }

        async Task<T> ReadSettings<T>()
        {
            string json = "{}";
            string path = Path.Combine(Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER"), CorePathSettings);
            string fileFavs = Path.Combine(path, "settings.json");

            try
            {

                if (File.Exists(fileFavs))
                {
                    json = File.ReadAllText(fileFavs);
                    var settings = JsonConvert.DeserializeObject<T>(json);
                    return await Task.FromResult<T>(settings);

                }
                else
                {
                    var dir = Directory.CreateDirectory(path);
                    using (var fs = File.Create(fileFavs));
                    return await Json.ToObjectAsync<T>("{}");

                }

            }
            catch (Exception)
            {

                throw;
            }



        }

        async Task WriteSettings<T>(T settings)
        {


            try
            {

                var Infile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(string.Format(@"{0}settings.json", CorePathSettings));
                if (Infile is not null)
                {
                    var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(CorePathSettings);
                    Windows.Storage.StorageFile file = await folder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);
                    await Windows.Storage.FileIO.WriteTextAsync(file, await Json.StringifyAsync(settings));

                }
                else
                {
                    var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(CorePathSettings);
                    if (folder != null)
                    {
                        Windows.Storage.StorageFile file = await folder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);
                        await Windows.Storage.FileIO.WriteTextAsync(file, await Json.StringifyAsync(settings));
                    }
                }
                return;
            }
            catch (Exception)
            {

                throw;
            }


        }
        async void WriteFavorite<T>(T bookmarks)
        {


            try
            {

                var Infile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(string.Format(@"{0}favorites.json", CorePathFavorites));
                if (Infile is not null)
                {
                    var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(CorePathFavorites);
                    Windows.Storage.StorageFile file = await folder.CreateFileAsync("favorites.json", CreationCollisionOption.OpenIfExists);
                    await Windows.Storage.FileIO.WriteTextAsync(file, await Json.StringifyAsync(bookmarks));
                }
                else
                {
                    var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(CorePathFavorites);
                    if (folder != null)
                    {
                        Windows.Storage.StorageFile file = await folder.CreateFileAsync("favorites.json", CreationCollisionOption.OpenIfExists);
                        await Windows.Storage.FileIO.WriteTextAsync(file, await Json.StringifyAsync(bookmarks));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public string HomeUrlString
        {
            get
            {
                return AppSettings.HomeUrlString;

            }
            set
            {
                AppSettings.HomeUrlString = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }
        }
        public Visibility VisibilitySideToolBar
        {

            get
            {
                return AppSettings.VisibilitySideToolBar;

            }
            set
            {
                AppSettings.VisibilitySideToolBar = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }

        }
        public ElementTheme ElementTheme
        {

            get
            {
                return AppSettings.ThemeDefault;

            }
            set
            {
                AppSettings.ThemeDefault = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }


        }

        public bool TitleBarPinned
        {
            get
            {
                return AppSettings.TitleBarPinned;
            }
            set
            {
                AppSettings.TitleBarPinned = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);

            }
        }
        public int DefaultSearchProvider
        {
            get
            {
                return AppSettings.DefaultSearchProvider;

            }
            set
            {
                AppSettings.DefaultSearchProvider = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }
        }

        public bool SideKick
        {
            get
            {
                return AppSettings.SideKick;
            }
            set
            {
                AppSettings.SideKick = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }
        }
        public string UserAgentWebCore
        {
            get
            {
                return AppSettings.UserAgentWebCore;
            }
            set
            {
                AppSettings.UserAgentWebCore = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }
        }
        public bool IsLocationOnOff
        {
            get
            {
                return AppSettings.IsLocationOnOff;
            }
            set
            {
                AppSettings.IsLocationOnOff = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }
        }
        public Uri HomeUrl
        {

            get
            {
                return AppSettings.HomeUrl;
            }
            set
            {
                AppSettings.HomeUrl = value;
                WriteSettings<WebDiveSettings>(AppSettings).ConfigureAwait(false);
            }
        }

        private ObservableCollection<HistoryModel> historyModels = new ObservableCollection<HistoryModel>();
        public ObservableCollection<HistoryModel> HistoryStore
        {
            get
            {
                return historyModels.Reverse().ToObservableCollection();
            }
            set
            {
                object obj = new object();
                lock (obj)
                {
                    historyModels = value;
                    HistoryCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

            }
        }

        public ObservableCollection<Bookmarks> FavoritesStore
        {
            get
            {
                var result = Task.Run(async () => await ReadFavorites<ObservableCollection<Bookmarks>>());// Task.Run(async () => await ReadAsync(nameof(FavoritesStore), new ObservableCollection<Bookmarks>()));
                return result.Result ?? new ObservableCollection<Bookmarks>();
            }
            set
            {
                var obj = new object();
                lock (obj)
                {
                    WriteFavorite(value);

                    //Write(nameof(FavoritesStore), value);
                    FavoritesCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

            }
        }
    }
}
