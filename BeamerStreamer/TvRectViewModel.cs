using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using BeamerStreamer.Properties;
using Application = System.Windows.Application;

namespace BeamerStreamer
{
	public class TvRectViewModel : NotificationObject
	{

        #region Backingfields
   
        private Thickness margin;
	    private double tvLeft;
	    private double tvTop;
	    private double tvWidth;
	    private double tvHeight;
	    private double overlayOpacity;
	    private Brush tvBackground;
	    private Brush overlayBrush;
	    private EffectViewModel effectViewModel;

	    #endregion

	    internal const string SettingsExtension = "set";
        internal SerializableSettings SavedSettings;
	    private Package settingsPackage;
        private readonly Uri enabledEffectsUri = new Uri("/effect.config", UriKind.Relative);

	    internal string SettingsDirectory
	    {
	        get
	        {	            
	            var res = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData,Environment.SpecialFolderOption.Create),Assembly.GetExecutingAssembly().GetName().Name);

                if (!Directory.Exists(res))
	                Directory.CreateDirectory(res);
	            return res;
	        }
	    }

	    internal string DefaultPreset => Path.Combine(SettingsDirectory, $"DefaultSet.{SettingsExtension}");

        internal List<string> Presets => Directory.GetFiles(SettingsDirectory, $"*.{SettingsExtension}").ToList();

	    public string CurrentSettingsName => Path.GetFileNameWithoutExtension(CurrentSettingsFile);

        public TvRectViewModel()
        {                        
            LoadSettings(!File.Exists(Settings.Default.LastSettingsFile) ? DefaultPreset : Settings.Default.LastSettingsFile);            
		}

	    internal void LoadSettings(string fileName)
	    {
	        if (string.IsNullOrEmpty(fileName))
	            fileName = DefaultPreset;
	        CurrentSettingsFile = fileName;
            settingsPackage?.Close();
	        try
	        {
	            settingsPackage = ZipPackage.Open(fileName, FileMode.OpenOrCreate);
	        }
	        catch (FileFormatException)
	        {
	            if (File.Exists(fileName))
	            {
	                File.Delete(fileName);
                    LoadSettings(fileName);
	            }
	        }            
            EffectViewModel = new EffectViewModel();
	        SavedSettings = SerializableSettings.LoadFromPackage(settingsPackage);
            FillModelProperties();
            LoadEffects();
            WriteMessage(CurrentSettingsName);
        }

	    internal void WriteMessage(string message)
	    {
	        if (MainWindow.Instance != null)
	        {
	            MainWindow.Instance.Message = message;	            
	            if (!string.IsNullOrEmpty(message))
	                Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(task =>
                    {
                        MainWindow.Instance.Message = MainWindow.Instance.Message.Replace(message, "");
                        if (string.IsNullOrWhiteSpace(MainWindow.Instance.Message))
                            WriteMessage(null);
                    });
	        }
	    }

	    internal void SaveSettingsTo(string fileName)
        {
            WriteMessage(string.Format(Resources.PresetSaved, Path.GetFileNameWithoutExtension(fileName)));            
            settingsPackage?.Flush();
            settingsPackage?.Close();
	        if (File.Exists(fileName))	                   
	            File.Delete(fileName);	        
	        if (File.Exists(CurrentSettingsFile))
                File.Copy(CurrentSettingsFile, fileName);
            LoadSettings(fileName);	        
        }

        public string CurrentSettingsFile
	    {
	        get { return Settings.Default.LastSettingsFile; }
	        private set
            {
                Settings.Default.LastSettingsFile = value;
                Settings.Default.Save();
            }
	    }

		/// <summary>
		/// EffectViewModel
		/// </summary>
		public EffectViewModel EffectViewModel
		{
			get { return effectViewModel; }
			set { SetProperty(ref effectViewModel, value); }
		}

        /// <summary>
	    /// Margin
	    /// </summary>
	    public Thickness Margin
        {
            get { return margin; }
            set { SetAndSave(ref margin, value); }
        }

        /// <summary>
        /// TvBackground
        /// </summary>
        public Brush TvBackground
		{
			get { return tvBackground; }
            set { SetAndSave(ref tvBackground, value); }
        }

        /// <summary>
        /// TvBackground
        /// </summary>
        public Brush OverlayBrush
        {
            get { return overlayBrush; }
            set { SetAndSave(ref overlayBrush, value); }
        }

        /// <summary>
        /// OverlayOpacity
        /// </summary>
        public double OverlayOpacity
        {
            get { return overlayOpacity; }
            set { SetAndSave(ref overlayOpacity, value); }
        }

        /// <summary>
        /// TvLeft
        /// </summary>
        public double TvLeft
		{
			get { return tvLeft; }
            set { SetAndSave(ref tvLeft, value); }
        }

		/// <summary>
		/// TvTop
		/// </summary>
		public double TvTop
		{
			get { return tvTop; }
            set { SetAndSave(ref tvTop, value); }
        }

		/// <summary>
		/// TvHeight
		/// </summary>
		public double TvHeight
		{
			get { return tvHeight; }
            set { SetAndSave(ref tvHeight, value); }
        }

		/// <summary>
		/// TvWidth
		/// </summary>
		public double TvWidth
		{
			get { return tvWidth; }
			set { SetAndSave(ref tvWidth, value); }
		}


        private void SetAndSave<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
	    {
            if (SetProperty(ref storage, value, propertyName))
                Save();
        }

		private double CalcTop()
		{
			var window = Application.Current.MainWindow;
			return (window.Height / 2) - (TvHeight / 2);
		}

		private double CalcLeft()
		{
			var window = Application.Current.MainWindow;
			return (window.Width / 2) - (TvWidth / 2);
		}


		private void Save()
		{
            SavedSettings.TvBackgroundColor = ViewUtility.GetColor(TvBackground);		    
		    SavedSettings.OverlayColor = ViewUtility.GetColor(OverlayBrush);
            SavedSettings.OverlayOpacity = OverlayOpacity;            
            SavedSettings.TvLeft = TvLeft;
            SavedSettings.TvTop = TvTop;
            SavedSettings.Margin = Margin;
            SavedSettings.TvHeight = TvHeight;
            SavedSettings.TvWidth = TvWidth;
            SavedSettings.Save(settingsPackage);
        }

        private void FillModelProperties()
        {
            overlayBrush = new SolidColorBrush(SavedSettings.OverlayColor.ToMediaColor());
            overlayOpacity = SavedSettings.OverlayOpacity;
            tvBackground = new SolidColorBrush(SavedSettings.TvBackgroundColor.ToMediaColor());
            margin = SavedSettings.Margin;                     
            tvHeight = SavedSettings.TvHeight;                      
            tvWidth = SavedSettings.TvWidth;
            tvLeft = SavedSettings.TvLeft != -1 ? SavedSettings.TvLeft : CalcLeft();
            tvTop = SavedSettings.TvTop != -1 ? SavedSettings.TvTop : CalcTop();
            RaiseAllPropertiesChanged();
        }


        private void LoadEffects()
        {
            if (settingsPackage.PartExists(enabledEffectsUri))
            {
                var effectSettingsPart = settingsPackage.GetPart(enabledEffectsUri);
                var effects = SerializationHelper.XmlDeserialize<List<EffectViewItem>>(effectSettingsPart.GetStream());
                for (int index = 0; index < EffectViewModel.Effects.Count; index++)
                {
                    var effectViewItem = EffectViewModel.Effects[index];
                    var loadedEffect = effects[index];
                    effectViewItem.IsChecked = loadedEffect.IsChecked;
                    effectViewItem.MouseX = loadedEffect.MouseX;
                    effectViewItem.MouseY = loadedEffect.MouseY;

                    var partUri = new Uri($"/{loadedEffect.DisplayName}.eff.config", UriKind.Relative);
                    if (settingsPackage.PartExists(partUri))
                    {
                        var part = settingsPackage.GetPart(partUri);
                        var dict = SerializationHelper.XmlDeserialize<List<PropertyValue>>(part.GetStream());
                        foreach (var propertyValue in dict)
                        {
                            PropertyInfo propertyInfo = effectViewItem.Effect.GetType().GetProperties().Where(info => info.CanWrite).FirstOrDefault(info => info.Name == propertyValue.Name);
                            if (propertyInfo != null)
                            {
                                try
                                {
                                    if (propertyInfo.PropertyType == typeof(double))
                                        propertyInfo.SetValue(effectViewItem.Effect, Convert.ToDouble(propertyValue.Value), null);
                                    else if (propertyInfo.PropertyType == typeof(bool))
                                        propertyInfo.SetValue(effectViewItem.Effect, Convert.ToBoolean(propertyValue.Value), null);
                                    else if (propertyInfo.PropertyType == typeof(int))
                                        propertyInfo.SetValue(effectViewItem.Effect, Convert.ToInt32(propertyValue.Value), null);
                                    else if (propertyInfo.PropertyType == typeof(Point))
                                    {
                                        string[] strings = propertyValue.Value.Split(';');
                                        var point = new Point(Convert.ToDouble(strings[0]), Convert.ToDouble(strings[1]));
                                        propertyInfo.SetValue(effectViewItem.Effect, point, null);
                                    }
                                    else if (propertyInfo.PropertyType == typeof(Brush))
                                    {
                                        //
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(effectViewItem.Effect, propertyValue.Value, null);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                        }

                    }
                }
            }
        }

        private void SaveEffects()
		{
		    if(settingsPackage.PartExists(enabledEffectsUri))
                settingsPackage.DeletePart(enabledEffectsUri);

		    var part = settingsPackage.CreatePart(enabledEffectsUri, "text/xml");            
		    EffectViewModel.Effects.XmlSerialize(part.GetStream(FileMode.OpenOrCreate));            
            foreach (var effect in EffectViewModel.Effects)
            {
                var partUri = new Uri($"/{effect.DisplayName}.eff.config", UriKind.Relative);
                if (settingsPackage.PartExists(partUri))
                    settingsPackage.DeletePart(partUri);
                
                var effectDetailPart = settingsPackage.CreatePart(partUri, "text/xml");

                var dict = new List<PropertyValue>();
                PropertyInfo[] propertyInfos = effect.Effect.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    try
                    {
                        object value = propertyInfo.GetValue(effect.Effect, null);
                        dict.Add(new PropertyValue(propertyInfo, value));
                    }
                    catch (Exception) { }
                }
                dict.XmlSerialize(effectDetailPart.GetStream());
            }
            settingsPackage.Flush();
        }


	    internal void EditEffect()
		{
			var window = new EffectSetupWindow(EffectViewModel) {Topmost = true};
			window.Closed += (o, args) => SaveEffects();
			window.ShowDialog();
		}

	    public void DeleteAllSettings()
	    {
            settingsPackage?.Close();
	        foreach (var effectViewItem in EffectViewModel.Effects)
	        {
	            effectViewItem.IsChecked = false;
	        }
            foreach (var file in Presets.Where(File.Exists).ToArray())	        
	            File.Delete(file);
	        LoadSettings(DefaultPreset);
	    }

	    public void DeleteCurrentSettings()
	    {
            settingsPackage?.Close();
            foreach (var effectViewItem in EffectViewModel.Effects)
            {
                effectViewItem.IsChecked = false;
            }
            if (File.Exists(CurrentSettingsFile))
                File.Delete(CurrentSettingsFile);
	        LoadSettings(Presets.FirstOrDefault());
	    }
	}
}
