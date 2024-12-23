using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using Microsoft.Maui.Controls;

namespace TP.Methods
{

    public static class HideContentViewMethod
    {
        public static void HideContentView(ContentView contentView)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => contentView.IsVisible = false;

            contentView.InputTransparent = false;
            contentView.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }

}
