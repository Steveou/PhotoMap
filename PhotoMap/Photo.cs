using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace PhotoMap
{
    /// <summary>
    /// Photo
    /// </summary>
    public class Photo : ReactiveObject
    {
        public string Name { get; set; }
        public StorageFile File { get; set; }
        public Geopoint Location { get; set; }
        public DateTimeOffset DateTaken { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                this.RaiseAndSetIfChanged(ref isSelected, value);
            }
        }

        public Photo()
        {
            isSelected = false;
        }
    }
}
