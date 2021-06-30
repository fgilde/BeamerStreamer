using System;
using System.Windows.Media.Effects;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;

namespace BeamerStreamer
{

    public static class DesignTimeHelper
    {

        public static bool IsInDesignMode
        {
            get
            {
                return
                    ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue));
            }
        }
    }

    public class DesignTimeData : DependencyObject
    {

        static EffectViewModel _designTimeViewModel; 
        static DesignTimeData()
        {
            if ( DesignTimeHelper.IsInDesignMode) 
            {
				_designTimeViewModel = new EffectViewModel();
 
                
            } 
        }

		public static EffectViewModel GetViewModel(DependencyObject obj)
        {
			return (EffectViewModel)obj.GetValue(ViewModelProperty);
        }

		public static void SetViewModel(DependencyObject obj, EffectViewModel value)
        {
            obj.SetValue(ViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for EffectList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.RegisterAttached("ViewModel", typeof(EffectViewModel), typeof(DesignTimeData), new UIPropertyMetadata(_designTimeViewModel));



        public static string GetDesignTimeModel(DependencyObject obj)
        {
            return (string )obj.GetValue(DesignTimeModelProperty);
        }

        public static void SetDesignTimeModel(DependencyObject obj, string value)
        {
            obj.SetValue(DesignTimeModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for DesignTimeModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DesignTimeModelProperty =
            DependencyProperty.RegisterAttached("DesignTimeModel", typeof(string), typeof(DesignTimeData), new UIPropertyMetadata("", 
                    new PropertyChangedCallback (   DesignTimeModelCallback ) 
                ));

        static void DesignTimeModelCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignTimeHelper.IsInDesignMode)
            {
                System.Diagnostics.Debug.WriteLine("designtime view model is being set");
                FrameworkElement fe = d as FrameworkElement;
                if (fe != null)
                {
                    fe.DataContext = _designTimeViewModel;
                }
            } 
        }      
    } 


   

	[Serializable]
    public class EffectViewItem : INotifyPropertyChanged 
    {
        public EffectViewItem(Effect e)
        {
            _effect = e;  
        }

		private EffectViewItem()
		{}

		private Effect _effect;
        private bool _checked;
        private string _name ; 
        public string DisplayName
        {
            get
            {
                if (_name == null || _name == string.Empty)
                {

                    
                    string name = _effect.ToString(); 
                    int index = name.LastIndexOf('.') ; 
                    if ( index != -1 ) 
                        name = name.Substring(index+1); 
                    return name;
                } 
                return _name; 
            }
            set
            {
                _name = value;
                OnPropertyChanged("DisplayName"); 
            }
        }

		[XmlIgnore]
        public Effect DisplayedEffect
        {
            get
            {
                if (IsChecked)
                    return _effect;
                else
                    return null; 
            } 
        }

		[XmlIgnore]
        public Effect Effect
        {
            get
            {
                return _effect; 
            }
            set
            {
                _effect = value;
            } 
        }

        public bool IsChecked
        {
            get
            {
                return _checked;
            }
            set
            {
                bool oldvalue = _checked; 
                _checked = value;
                if (oldvalue != _checked)
                {
                    OnPropertyChanged("IsChecked");
                    OnPropertyChanged("DisplayedEffect"); 
                } 
            } 
        }

        private bool UseMousePosition
        {
            get
            {
                return false;
            }
        } 
        private double _mouseX;
        private double _mouseY; 

        public double MouseX
        {
            get
            { 
                return _mouseX ;  
            }
            set
            {
                _mouseX = value;
                if (IsChecked && UseMousePosition)
                    OnPropertyChanged("MouseX"); 
            } 
        }

        public double MouseY
        {
            get
            {
                return _mouseY;
            }
            set
            {
                _mouseY = value;
                if (IsChecked && UseMousePosition)
                    OnPropertyChanged("MouseY");
            }
        }
 

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler ph = PropertyChanged;
            if (ph != null)
            {
                ph(this, new PropertyChangedEventArgs(name)); 
            } 
        } 
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    
}
