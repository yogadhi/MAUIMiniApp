﻿namespace YAP.Libs
{
    public partial class PINView
    {
        /// <summary>
        /// Gets or Sets the Border Thickness of the PIN Box. 
        /// </summary>
        public double BoxStrokeThickness
        {
            get => (double)GetValue(BoxStrokeThicknessProperty);
            set => SetValue(BoxStrokeThicknessProperty, value);
        }

        public static readonly BindableProperty BoxStrokeThicknessProperty =
          BindableProperty.Create(
              nameof(BoxStrokeThickness),
              typeof(double),
              typeof(PINView),
              defaultValue: 1.0,
              defaultBindingMode: BindingMode.OneWay,
              propertyChanged: BoxStrokeThicknessPropertyChanged);

        private static void BoxStrokeThicknessPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (PINView)bindable;

            // Apply the BoxStrokeThickness only if it is different then the value in "Color" Property
            control.PINBoxContainer.Children.ToList().ForEach(x =>
            {
                var boxTemplate = (BoxTemplate)x;
                boxTemplate.BoxBorder.StrokeThickness = (double)newValue;
            });
        }
    }
}
