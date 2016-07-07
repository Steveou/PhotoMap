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
    public sealed partial class MainPage : Page
    {
        private MainViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();

            DataContext = viewModel = new MainViewModel();
            
            // Always center and zoom to the location of the selected photo
            viewModel.WhenAnyValue(vm => vm.SelectedPhoto)
                .Where(photo => photo != null)
                .Subscribe(async photo => await mapControl.TrySetViewAsync(photo.Location, 16));
        }
    }
}
