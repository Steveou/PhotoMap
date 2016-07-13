using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PhotoMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IViewFor<MainViewModel>
    {
        public MainViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value;}
        }

        public MainPage()
        {
            InitializeComponent();

            DataContext = ViewModel = new MainViewModel();
            
            // Always center and zoom to the location of the selected photo
            ViewModel.WhenAnyValue(vm => vm.SelectedPhoto)
                .Where(photo => photo != null)
                .Subscribe(async photo => await Map.TrySetViewAsync(photo.Location, 16));
            
            // Scroll to selected image.
            ViewModel.WhenAnyValue(vm => vm.SelectedPhoto)
                .Where(photo => photo != null)
                .Subscribe(photo => PhotosListBox.ScrollIntoView(photo));
        }

        private void Pin_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var pin = (FrameworkElement)sender;

            ViewModel.SelectedPhoto = pin.DataContext as Photo;
        }
    }
}
