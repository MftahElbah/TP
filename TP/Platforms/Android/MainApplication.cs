using Android.App;
using Android.Content.Res;
using Android.Runtime;

namespace TP
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            // Customization for Entry
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                if (view is Entry)
                {
                    // Remove underline
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

                    // Change placeholder text color
                    handler.PlatformView.SetHintTextColor(ColorStateList.ValueOf(Android.Graphics.Color.Rgb(95, 95, 95)));
                }
            });

            // Customization for Editor
            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) =>
            {
                if (view is Editor)
                {
                    // Remove underline
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

                    // Change placeholder text color
                    handler.PlatformView.SetHintTextColor(ColorStateList.ValueOf(Android.Graphics.Color.Rgb(95, 95, 95)));
                }
            });

            // Customization for RadioButton
            Microsoft.Maui.Handlers.RadioButtonHandler.Mapper.AppendToMapping(nameof(RadioButton), (handler, view) =>
            {
                if (view is RadioButton)
                {
                    if (handler.PlatformView is Android.Widget.RadioButton nativeRadioButton)
                    {
                        // Change the check color for the RadioButton
                        nativeRadioButton.ButtonTintList = ColorStateList.ValueOf(Android.Graphics.Color.Rgb(26, 26, 26)); // #1a1a1a
                    }
                }
            });
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
