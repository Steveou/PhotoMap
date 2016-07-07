using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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

        private ObservableAsPropertyHelper<bool> isLoading;
        public bool IsLoading => isLoading.Value;

        private Photo selectedPhoto;
        public Photo SelectedPhoto
        {
            get { return selectedPhoto; }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedPhoto, value);
            }
        }

        public MainViewModel()
        {
            LoadPhotos = ReactiveCommand.CreateAsyncTask(_ => GetPhotos());
            LoadPhotos.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            photos = LoadPhotos.ToProperty(this, vm => vm.Photos, new List<Photo>());
            isLoading = LoadPhotos.IsExecuting.ToProperty(this, vm => vm.IsLoading, false);
        }

        /// <summary>
        /// Returns all photos with geo information from the picture library.
        /// </summary>
        /// <returns>List of photos.</returns>
        private async Task<List<Photo>> GetPhotos()
        {
            var results = new List<Photo>();
            var pictureLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);

            foreach (var folder in pictureLibrary.Folders)
            {
                var files = await folder.GetFilesAsync(CommonFileQuery.OrderByDate);

                foreach (var file in files)
                {
                    var properties = await file.Properties.GetImagePropertiesAsync();

                    // Only use photos with location info.
                    if (properties.Latitude != null
                        && properties.Longitude != null)
                    {
                        results.Add(new Photo()
                        {
                            Name = file.Name,
                            Location = new Geopoint(new BasicGeoposition()
                            {
                                Latitude = properties.Latitude.Value,
                                Longitude = properties.Longitude.Value
                            })
                        });
                    }
                }
            }

            return results;
        }
    }
}
