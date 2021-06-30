using System;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Xml.Serialization;

namespace BeamerStreamer
{
    [Serializable]
    public class SerializableSettings
    {
        internal static Uri ZipPartUri = new Uri("/rect.config", UriKind.Relative);

        [XmlElement("TvBackgroundColor")]
        public int TvBackgroundColorAsArgb
        {
            get { return TvBackgroundColor.ToArgb(); }
            set { TvBackgroundColor = Color.FromArgb(value); }
        }

        [XmlElement("OverlayColor")]
        public int OverlayColorAsArgb
        {
            get { return OverlayColor.ToArgb(); }
            set { OverlayColor = Color.FromArgb(value); }
        }


        [XmlIgnore]
        public Color TvBackgroundColor { get; set; }

        [XmlIgnore]
        public Color OverlayColor { get; set; }
        public double OverlayOpacity { get; set; }
        public double TvLeft { get; set; }
        public double TvTop { get; set; }
        public double TvHeight { get; set; }
        public double TvWidth { get; set; }
        public Thickness Margin { get; set; }


        internal static SerializableSettings LoadFromPackage(Package package)
        {
            if (package.PartExists(ZipPartUri))
            {
                var part = package.GetPart(ZipPartUri);
                return SerializationHelper.XmlDeserialize<SerializableSettings>(part.GetStream());
            }
            return new SerializableSettings
            {
                OverlayColor = Color.Black, TvBackgroundColor = Color.Black,
                OverlayOpacity = 0,
                TvLeft = -1,
                TvTop = -1,
                TvWidth = 160,
                TvHeight = 90,
            };
        }

        internal void Save(Package package)
        {
            if (package.PartExists(ZipPartUri))
                package.DeletePart(ZipPartUri);

            var part = package.CreatePart(ZipPartUri, "text/xml");
            this.XmlSerialize(part.GetStream(FileMode.OpenOrCreate));
            package.Flush();
        }
       
    }
}