using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Search;

namespace PhotoMap
{
    public class MainViewModel : ReactiveObject
    {
        public ReactiveCommand<List<Photo>> LoadPhotos { get; protected set; }

        private ObservableAsPropertyHelper<List<Photo>> photos;
        public List<Photo> Photos => photos.Value;

        public MainViewModel()
        {
            LoadPhotos = ReactiveCommand.CreateAsyncTask(p => GetPhotos());
            LoadPhotos.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            photos = LoadPhotos.ToProperty(this, x => x.Photos, new List<Photo>());
        }

        /// <summary>
        /// Returns all photos with geo information from the picture library.
        /// </summary>
        /// <returns>List of photos.</returns>
        private async Task<List<Photo>> GetPhotos()
        {
            var photosList = new List<Photo>();
            var pictureLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);

            foreach (var folder in pictureLibrary.Folders)
            {
                var files = await folder.GetFilesAsync(CommonFileQuery.OrderByDate);

                foreach (var file in files)
                {
                    var properties = await file.Properties.GetImagePropertiesAsync();

                    // Only show photos with location info.
                    if (properties.Latitude != null
                        && properties.Longitude != null)
                    {
                        Photo photo = new Photo()
                        {
                            Name = file.Name,
                            Location = new Geopoint(new BasicGeoposition() { Latitude = properties.Latitude.Value, Longitude = properties.Longitude.Value })
                        };

                        photosList.Add(photo);
                    }
                }
            }

            return photosList;
        }

    }
}
