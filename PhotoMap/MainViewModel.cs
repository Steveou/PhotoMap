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
        public ReactiveCommand<Unit> CenterToLocation { get; protected set; }

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

        private Geopoint mapCenter;
        public Geopoint MapCenter
        {
            get { return mapCenter; }
            set
            {
                this.RaiseAndSetIfChanged(ref mapCenter, value);
            }
        }

        private double mapZoomLevel;
        public double MapZoomLevel
        {
            get { return mapZoomLevel; }
            set
            {
                this.RaiseAndSetIfChanged(ref mapZoomLevel, value);
            }
        }

        public MainViewModel()
        {
            LoadPhotos = ReactiveCommand.CreateAsyncTask(p => GetPhotos());
            LoadPhotos.ThrownExceptions.Subscribe(ex => System.Diagnostics.Debug.WriteLine(ex));

            CenterToLocation = ReactiveCommand.CreateAsyncTask(p => CenterMapToLocation());

            photos = LoadPhotos.ToProperty(this, x => x.Photos, new List<Photo>());
            isLoading = LoadPhotos.IsExecuting.ToProperty(this, x => x.IsLoading, false);

            this.WhenAnyValue(x => x.SelectedPhoto).InvokeCommand(CenterToLocation);
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
                            Location = new Geopoint(new BasicGeoposition() {
                                Latitude = properties.Latitude.Value,
                                Longitude = properties.Longitude.Value
                            })
                        };

                        photosList.Add(photo);
                    }
                }
            }

            return photosList;
        }

        /// <summary>
        /// Center the map to the position of the selected photo.
        /// </summary>
        /// <returns></returns>
        private Task<Unit> CenterMapToLocation()
        {
            if (SelectedPhoto != null)
            {
                MapCenter = SelectedPhoto.Location;
                MapZoomLevel = 16;
            }

            return Task.FromResult(Unit.Default);
        }
    }
}
