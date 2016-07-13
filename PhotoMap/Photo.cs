using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace PhotoMap
{
    /// <summary>
    /// Photo
    /// </summary>
    public class Photo : INotifyPropertyChanged
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
                isSelected = value;

                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Photo(string name, StorageFile file, Geopoint location, DateTimeOffset dateTaken)
        {
            Name = name;
            File = file;
            Location = location;
            DateTaken = dateTaken;

            isSelected = false;

        }
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
