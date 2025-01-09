namespace TP.Methods
{
    public static class HideContentViewMethod
    {
        public static void HideContentView(ContentView contentView, VisualElement excludeElement)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += (s, e) =>
            {
                // Get the touch point location
                var touchPoint = (e as TappedEventArgs)?.GetPosition(contentView);

                if (touchPoint != null && !excludeElement.Bounds.Contains(touchPoint.Value))
                {
                    contentView.IsVisible = false;
                }
            };

            contentView.InputTransparent = false;
            contentView.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }
}
