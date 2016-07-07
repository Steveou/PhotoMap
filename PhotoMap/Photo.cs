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
        public string Name { get; private set; }
        public StorageFile File { get; private set; }
        public Geopoint Location { get; private set; }
        public DateTimeOffset DateTaken { get; private set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                this.RaiseAndSetIfChanged(ref isSelected, value);
            }
        }

        public Photo(string name, StorageFile file, Geopoint location, DateTimeOffset dateTaken)
        {
            Name = name;
            File = file;
            Location = location;
            DateTaken = dateTaken;

            isSelected = false;
        }
    }
}
