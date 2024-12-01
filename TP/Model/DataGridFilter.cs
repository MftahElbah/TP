using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP
{
    public class VisualElementBehavior : Behavior<VisualElement>
    {
        public SfDataGrid? DataGrid { get; set; }
        public SfComboBox? OptionListPicker { get; set; }
        public DataGridViewModel? viewModels { get; set; }
        public SearchBar? Searchbar { get; set; }
        public SfComboBox? columnListPicker { get; set; }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            var bindablePicker = bindable as SfComboBox;
            if (viewModels != null)
            {
                viewModels.filtertextchanged = OnFilterChanged;
            }
            if (bindable as SearchBar != null)
            {
                Searchbar = (bindable as SearchBar);
                if (Searchbar != null)
                {
                    Searchbar.TextChanged += OnFilterTextChanged;
                }
            }
            else if (bindablePicker != null)
            {

                if (bindablePicker.Text == "Options")
                {
                    OptionListPicker = (bindable as SfComboBox);
                    if (OptionListPicker != null)
                    {
                        OptionListPicker.SelectionChanged += OnFilterOptionsChanged;
                    }
                }
                else
                {
                    columnListPicker = (bindable as SfComboBox);
                    if (columnListPicker != null)
                    {
                        columnListPicker.SelectionChanged += OnColumnsSelectionChanged;
                    }
                }
            }
            base.OnAttachedTo(bindable);

        }
        void OnFilterTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == null)
            {
                if (viewModels != null)
                {
                    viewModels.FilterText = "";
                }
            }
            else
            {
                if (viewModels != null)
                {
                    viewModels.FilterText = e.NewTextValue;
                }
            }
        }
        void OnFilterOptionsChanged(object? sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            SfComboBox? newPicker = (SfComboBox?)sender;
            if (newPicker?.SelectedIndex >= 0)
            {
                if (viewModels != null)
                {
                    var selectedValue = (newPicker!.ItemsSource as List<string>)?[newPicker.SelectedIndex];
                    if (selectedValue != null)
                    {
                        viewModels.SelectedCondition = selectedValue;
                    }
                }
                if (Searchbar != null && Searchbar.Text != null)
                    OnFilterChanged();
            }
        }
        void OnFilterChanged()
        {
            if (DataGrid?.View != null)
            {
                if (viewModels != null)
                {
                    this.DataGrid.View.Filter = viewModels.FilerRecords;
                }
                this.DataGrid.View.RefreshFilter();
            }
        }

        void OnColumnsSelectionChanged(object? sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            SfComboBox? newPicker = (SfComboBox?)sender;
            if (viewModels != null && newPicker != null)
            {
                this.viewModels!.SelectedColumn = GetColumnMappingName((string)newPicker.SelectedItem!);
            }
            if (viewModels?.SelectedColumn == "All Columns")
            {
                viewModels.SelectedCondition = "Contains";
                if (OptionListPicker != null)
                    OptionListPicker.IsVisible = false;
                this.OnFilterChanged();
            }
            else
            {
                if (OptionListPicker != null)
                    OptionListPicker.IsVisible = true;
                foreach (var prop in typeof(OrderInfo).GetProperties())
                {
                    if (prop.Name == viewModels?.SelectedColumn)
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            if (OptionListPicker != null)
                            {
                                var list = new List<string>();

                                list.Add("Equals");
                                list.Add("NotEquals");
                                list.Add("Contains");

                                OptionListPicker.ItemsSource = list;
                                if (this.viewModels.SelectedCondition == "Equals")
                                    OptionListPicker.SelectedIndex = 1;
                                else if (this.viewModels.SelectedCondition == "NotEquals")
                                    OptionListPicker.SelectedIndex = 2;
                                else
                                    OptionListPicker.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (OptionListPicker != null)
                            {
                                var list = new List<string>();

                                list.Add("Equals");
                                list.Add("NotEquals");

                                OptionListPicker.ItemsSource = list;
                                if (this.viewModels.SelectedCondition == "Equals")
                                    OptionListPicker.SelectedIndex = 0;
                                else
                                    OptionListPicker.SelectedIndex = 1;
                            }
                        }
                    }
                }
            }
        }
        private string GetColumnMappingName(string ColumnName)
        {
            return this.DataGrid!.Columns.FirstOrDefault(column => column.HeaderText == ColumnName)?.MappingName ?? ColumnName;
        }
        protected override void OnDetachingFrom(VisualElement bindable)
        {
            if (Searchbar != null)
                Searchbar.TextChanged -= OnFilterTextChanged;
            if (OptionListPicker != null)
                OptionListPicker.SelectionChanged -= OnFilterOptionsChanged;
            if (columnListPicker != null)
                columnListPicker.SelectionChanged -= OnColumnsSelectionChanged;
            Searchbar = null;
            OptionListPicker = null;
            columnListPicker = null;
            DataGrid = null;
            viewModels = null;
            base.OnDetachingFrom(bindable);
        }
    }
}

