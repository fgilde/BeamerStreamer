using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.IO;
using System.Windows.Media;

namespace BeamerStreamer
{
 

	[Serializable]
    public class EffectViewModel 
    {
	    public EffectViewModel()
	    {
			Effects = EffectsList.All;
	    }


	    public List<EffectViewItem> Effects { get; set; } 

    }



  
    public static class EffectsList
    {
        static List<EffectViewItem> _effects;
        public static List<EffectViewItem> All
        {
            get
            {
                if (_effects == null)
                {
                    _effects = new List<EffectViewItem>();
                    
                    //_effects.Add(new EffectViewItem(new ShaderEffectLibrary.DeeperColorEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.BandedSwirlEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.BloomEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.BrightExtractEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.ColorKeyAlphaEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.ColorToneEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.ContrastAdjustEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.DirectionalBlurEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.EmbossedEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.GloomEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.GrowablePoissonDiskEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.InvertColorEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.LightStreakEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.MagnifyEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.MonochromeEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.PinchEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.PixelateEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.RippleEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.SharpenEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.SmoothMagnifyEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.SwirlEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.ToneMappingEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.ToonShaderEffect()));
                    _effects.Add(new EffectViewItem(new ShaderEffectLibrary.ZoomBlurEffect()));

                }
                return _effects;
            }

        }
    } 
      
}
