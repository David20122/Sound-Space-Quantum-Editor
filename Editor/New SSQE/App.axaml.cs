using Avalonia;
using Avalonia.Markup.Xaml;

namespace New_SSQE
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}