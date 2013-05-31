using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace Unique.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle",StyleTargetType=typeof(PowerListBoxItem))]
    public class PowerListBox : ListBox
    {
        #region 构造函数
        public PowerListBox()
        {
            DefaultStyleKey = typeof(PowerListBox);
            Loaded += PowerListBox_Loaded;
            Unloaded += PowerListBox_Unloaded;
        }
        #endregion

        #region event
        public delegate void ItemSelectedEventHandler(Object sender, ItemSelectedEventArgs e);
        public event ItemSelectedEventHandler ItemSelected;
        internal void InvokeItemSelectedEvent(PowerListBoxItem selItem)
        {
            if (ItemSelected == null)
                return;
            if (selItem == null)
                ItemSelected(this, new ItemSelectedEventArgs());
            else if (selItem.DataContext != null)
                ItemSelected(this, new ItemSelectedEventArgs { OldSelectedItem = SelectedItem, NewSelectedItem = selItem.DataContext });
            else
                ItemSelected(this, new ItemSelectedEventArgs { OldSelectedItem = SelectedItem, NewSelectedItem = selItem });
        }
        #endregion

        #region private event handlers

        void PowerListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            ApplyTemplate();
            var child = ItemsPresenter.ChildrenEx().FirstOrDefault();
            if (child is StackPanel)
                _orientation = (child as StackPanel).Orientation;
            else if (child is VirtualizingStackPanel)
                _orientation = (child as VirtualizingStackPanel).Orientation;
            else
                return;
            if (_orientation == Orientation.Vertical)
                _scrollView.ScrollToVerticalOffset(_scrollView.VerticalOffset);
            else
                _scrollView.ScrollToHorizontalOffset(_scrollView.HorizontalOffset);

            var border = _scrollView.Descendants<Border>().FirstOrDefault();
            Debug.Assert(border != null);
            var vsGroup = VisualStateManager.GetVisualStateGroups(border).OfType<VisualStateGroup>().FirstOrDefault(s => s.Name == "ScrollStates");
            Debug.Assert(vsGroup != null);
            vsGroup.CurrentStateChanging += vsGroup_CurrentStateChanging;
        }

        void PowerListBox_Unloaded(object sender, RoutedEventArgs e)
        {
            var border = _scrollView.Descendants<Border>().FirstOrDefault();
            Debug.Assert(border != null);
            var vsGroup = VisualStateManager.GetVisualStateGroups(border).OfType<VisualStateGroup>().FirstOrDefault(s => s.Name == "ScrollStates");
            Debug.Assert(vsGroup != null);
            vsGroup.CurrentStateChanging -= vsGroup_CurrentStateChanging;
            RefreshDisplayArea();
        }

        void PowerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedIndex < 0)
                IsMultipleSelect = false;
        }

        void phonePage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (IsMultipleSelect)
            {
                IsMultipleSelect = false;
                e.Cancel = true;
            }
        }

        void vsGroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name == "NotScrolling")
            {
                _isScrolling = false;
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
            else
            {
                _isScrolling = true;
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RefreshDisplayArea();
        }

        #endregion

        #region private fields

        private ScrollViewer _scrollView;
        private Orientation _orientation;
        private Panel _itemsRootPanel;
        private Panel ItemsRootPanel
        {
            get
            {
                if (_itemsRootPanel == null && ItemsPresenter != null)
                {
                    _itemsRootPanel = ItemsPresenter.ChildrenEx().FirstOrDefault() as Panel;
                }
                return _itemsRootPanel;
            }
        }

        private ItemsPresenter _itemsPresenter;
        private ItemsPresenter ItemsPresenter
        {
            get { return _itemsPresenter ?? (_itemsPresenter = _scrollView.Descendants<ItemsPresenter>().FirstOrDefault()); }
        }

        private bool _isScrolling;
        #endregion

        #region dependency properties
        #region IsMultipleSelect
        public bool IsMultipleSelect
        {
            get { return (bool)GetValue(IsMultipleSelectProperty); }
            internal set { SetValue(IsMultipleSelectProperty, value); }
        }

        public static readonly DependencyProperty IsMultipleSelectProperty =
            DependencyProperty.Register("IsMultipleSelect", typeof(bool), typeof(PowerListBox), new PropertyMetadata(false, OnIsMultipleSelectPropertyChanged));

        private static void OnIsMultipleSelectPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var listBox = sender as PowerListBox;
            Debug.Assert(listBox != null);
            listBox.OnIsMultipleSelectChanged();
        }

        private void OnIsMultipleSelectChanged()
        {
            var items = this.Descendants<PowerListBoxItem>();
            if (IsMultipleSelect)
            {
                SelectionMode = SelectionMode.Multiple;
                items.ForEachEx(t => t.CanCheck = true);
            }
            else
            {
                SelectionMode = SelectionMode.Single;
                SelectedItem = null;
                items.ForEachEx(t => t.CanCheck = false);
            }
        }
        #endregion

        #region IsMultipleSelectEnable
        public static readonly DependencyProperty IsMultipleSelectEnableProperty =
            DependencyProperty.Register("IsMultipleSelectEnable", typeof(bool), typeof(PowerListBox), new PropertyMetadata(true, OnIsMultipleSelectEnablePropertyChanged));

        public bool IsMultipleSelectEnable
        {
            get { return (bool)GetValue(IsMultipleSelectEnableProperty); }
            set { SetValue(IsMultipleSelectEnableProperty, value); }
        }

        private static void OnIsMultipleSelectEnablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = sender as PowerListBox;
            if (target == null)
                return;
            target.OnIsMultipleSelectEnableChanged();
        }

        private void OnIsMultipleSelectEnableChanged()
        {
            if (IsMultipleSelectEnable)
            {
                SelectionChanged += PowerListBox_SelectionChanged;
            }
            else
            {
                IsMultipleSelect = false;
                SelectionChanged -= PowerListBox_SelectionChanged;
            }
        }
        #endregion

        #region FirstVisibleIndex
        public static readonly DependencyProperty FirstVisibleIndexProperty =
            DependencyProperty.Register("FirstVisibleIndex", typeof(int), typeof(PowerListBox), new PropertyMetadata(0));

        public int FirstVisibleIndex
        {
            get { return (int)GetValue(FirstVisibleIndexProperty); }
            private set { SetValue(FirstVisibleIndexProperty, value); }
        }
        #endregion

        #region LastVisibleIndex
        public static readonly DependencyProperty LastVisibleIndexProperty =
            DependencyProperty.Register("LastVisibleIndex", typeof(int), typeof(PowerListBox), new PropertyMetadata(0));

        public int LastVisibleIndex
        {
            get { return (int)GetValue(LastVisibleIndexProperty); }
            private set { SetValue(LastVisibleIndexProperty, value); }
        }
        #endregion

        #region EnableScrollAnimation
        public static readonly DependencyProperty EnableScrollAnimationProperty =
            DependencyProperty.Register("EnableScrollAnimation", typeof(bool), typeof(PowerListBox), new PropertyMetadata(true));

        public bool EnableScrollAnimation
        {
            get { return (bool)GetValue(EnableScrollAnimationProperty); }
            set { SetValue(EnableScrollAnimationProperty, value); }
        }
        #endregion

        #region ContextMenuTemplate
        public static readonly DependencyProperty ContextMenuTemplateProperty =
            DependencyProperty.Register("ContextMenuTemplate", typeof(DataTemplate), typeof(PowerListBox), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ContextMenuTemplate
        {
            get { return (DataTemplate)GetValue(ContextMenuTemplateProperty); }
            set { SetValue(ContextMenuTemplateProperty, value); }
        }
        #endregion

        #endregion

        #region override
        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new PowerListBoxItem();
            if (ItemContainerStyle != null)
                item.Style = ItemContainerStyle;
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PowerListBoxItem;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollView = GetTemplateChild("ScrollViewer") as ScrollViewer;
            Debug.Assert(_scrollView != null);
            if (IsMultipleSelectEnable)
                SelectionChanged += PowerListBox_SelectionChanged;
            IsMultipleSelect = SelectionMode != SelectionMode.Single;

            var phonePage = this.Ancestors<PhoneApplicationPage>().FirstOrDefault();
            if (phonePage != null)
                phonePage.BackKeyPress += phonePage_BackKeyPress;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RefreshDisplayArea();
            return base.ArrangeOverride(finalSize);
        }
        #endregion

        #region private methods

        void RefreshDisplayArea()
        {
            var oldFirst = FirstVisibleIndex;
            var oldLast = LastVisibleIndex;
            if (ItemsSource != null)
            {
                var isVirtualizing = VirtualizingStackPanel.GetIsVirtualizing(this);
                if (!isVirtualizing)
                    CalculateVisibleIndex();
                else if (_orientation == Orientation.Vertical)
                {
                    FirstVisibleIndex = (int)_scrollView.VerticalOffset;
                    var lastIndex = Math.Min(Items.Count - 1, _scrollView.VerticalOffset + _scrollView.ViewportHeight);
                    LastVisibleIndex = (int)lastIndex;
                }
                else
                {
                    FirstVisibleIndex = (int)_scrollView.HorizontalOffset;
                    var lastIndex = Math.Min(Items.Count - 1, _scrollView.HorizontalOffset + _scrollView.ViewportWidth);
                    LastVisibleIndex = (int)lastIndex;
                }
            }
            else
            {
                CalculateVisibleIndex();
            }
            RefreshItemsVisualState(oldFirst, FirstVisibleIndex, oldLast, LastVisibleIndex);
        }

        private void CalculateVisibleIndex()
        {
            Debug.Assert(ItemsRootPanel != null);
            var firstCal = false;
            if (_orientation == Orientation.Vertical)
            {
                var height = 0.0;
                for (var i = 0; i < ItemsRootPanel.Children.Count; i++)
                {
                    var child = ItemsRootPanel.Children[i] as FrameworkElement;
                    Debug.Assert(child != null);
                    height += (child.ActualHeight + child.Margin.Top + child.Margin.Bottom);
                    if (!firstCal && height >= _scrollView.VerticalOffset)
                    {
                        FirstVisibleIndex = i;
                        firstCal = true;
                    }
                    if (height >= (_scrollView.VerticalOffset + _scrollView.ViewportHeight))
                    {
                        LastVisibleIndex = i;
                        break;
                    }
                    if (i == ItemsRootPanel.Children.Count - 1)
                    {
                        LastVisibleIndex = ItemsRootPanel.Children.Count - 1;
                    }
                }
            }
            else
            {
                var width = 0.0;
                for (var i = 0; i < ItemsRootPanel.Children.Count; i++)
                {
                    var child = ItemsRootPanel.Children[i] as FrameworkElement;
                    Debug.Assert(child != null);
                    width += (child.ActualWidth + child.Margin.Left + child.Margin.Right);
                    if (!firstCal && width >= _scrollView.HorizontalOffset)
                    {
                        FirstVisibleIndex = i;
                        firstCal = true;
                    }
                    else if (width >= (_scrollView.HorizontalOffset + _scrollView.ViewportWidth))
                    {
                        LastVisibleIndex = i;
                        break;
                    }
                    else if (i == ItemsRootPanel.Children.Count - 1)
                    {
                        LastVisibleIndex = ItemsRootPanel.Children.Count - 1;
                    }
                }
            }
        }

        private PowerListBoxItem GetItem(int index)
        {
            if (index < 0 || index >= Items.Count)
                return null;
            if (ItemsSource != null)
            {
                var isVirtualizing = VirtualizingStackPanel.GetIsVirtualizing(this);
                if (!isVirtualizing)
                {
                    Debug.Assert(ItemsRootPanel != null);
                    return ItemsRootPanel.Children[index] as PowerListBoxItem;
                }
                Debug.Assert(ItemContainerGenerator != null);
                return ItemContainerGenerator.ContainerFromIndex(index) as PowerListBoxItem;
            }
            return Items[index] as PowerListBoxItem;
        }

        private void RefreshItemsVisualState(int oldFirst, int newFirst, int oldLast, int newLast)
        {
            if (oldFirst == newFirst && oldLast == newLast || oldFirst == 0 && oldLast == 0)
            {
                if (!_isScrolling)
                {
                    for (var i = newFirst; i <= newLast; i++)
                    {
                        var item = GetItem(i);
                        if (item != null)
                            item.SetVisualFieldInternal(PowerListBoxItem.VisualFieldState.DownIn);
                    }
                }
                return;
            }
            //向上滑动
            if (newFirst >= oldFirst)
            {
                for (var i = oldFirst; i < newFirst; i++)
                {
                    var item = GetItem(i);
                    if (item != null)
                        item.VisualField = PowerListBoxItem.VisualFieldState.Out;
                }
                var first = Math.Max(newFirst, oldLast + 1);
                for (var i = first; i <= newLast; i++)
                {
                    var item = GetItem(i);
                    if (item != null)
                        item.VisualField = PowerListBoxItem.VisualFieldState.DownIn;
                }
            }
            //向下滑动
            else
            {
                for (var i = oldLast; i > newLast; i--)
                {
                    var item = GetItem(i);
                    if (item != null)
                        item.VisualField = PowerListBoxItem.VisualFieldState.Out;
                }
                var last = Math.Min(oldFirst - 1, newLast);
                for (var i = last; i >= newFirst; i--)
                {
                    var item = GetItem(i);
                    if (item != null)
                        item.VisualField = PowerListBoxItem.VisualFieldState.UpIn;
                }
            }
        }
        #endregion
    }
}
