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
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoMap
{
    public class MainViewModel : ReactiveObject
    {
        public ReactiveCommand<List<Photo>> LoadPhotos { get; protected set; }
        public ReactiveCommand<BitmapImage> ShowPhoto { get; protected set; }

        private ObservableAsPropertyHelper<List<Photo>> photos;
        public List<Photo> Photos => photos.Value;

        private ObservableAsPropertyHelper<bool> isLoading;
        public bool IsLoading => isLoading.Value;

        private ObservableAsPropertyHelper<BitmapImage> image;
        public BitmapImage Image => image.Value;

        private ObservableAsPropertyHelper<bool> isPhotoLoading;
        public bool IsPhotoLoading => isPhotoLoading.Value;

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
            ShowPhoto = ReactiveCommand.CreateAsyncTask(_ => LoadImage());

            this.WhenAnyValue(vm => vm.SelectedPhoto)
                .InvokeCommand(ShowPhoto);

            this.WhenAnyValue(vm => vm.SelectedPhoto)
                .Buffer(2, 1)
                .Subscribe(p =>
                {
                    if(p[0] != null)
                        p[0].IsSelected = false;

                    p[1].IsSelected = true;
                });

            photos = LoadPhotos.ToProperty(this, vm => vm.Photos, new List<Photo>());
            isLoading = LoadPhotos.IsExecuting.ToProperty(this, vm => vm.IsLoading, false);
            image = ShowPhoto.ToProperty(this, vm => vm.Image, null);
            isPhotoLoading = ShowPhoto.IsExecuting.ToProperty(this, vm => vm.IsPhotoLoading, false);
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
                        results.Add(new Photo(
                            file.Name,
                            file,
                            new Geopoint(new BasicGeoposition()
                            {
                                Latitude = properties.Latitude.Value,
                                Longitude = properties.Longitude.Value
                            }),
                            properties.DateTaken)
                        );
                    }
                }
            }

            return results;
        }

        private async Task<BitmapImage> LoadImage()
        {
            if (SelectedPhoto == null)
                return null;

            BitmapImage image = new BitmapImage();

            using (var fileStream = await SelectedPhoto.File.OpenAsync(FileAccessMode.Read))
            {
                image.SetSource(fileStream);
            }

            return image;
        }
    }
}
