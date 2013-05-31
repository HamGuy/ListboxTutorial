using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Unique.Controls
{
    [TemplateVisualState(Name = "EnableSelection", GroupName = "MultiSelectionStates")]
    [TemplateVisualState(Name = "DisableSelection", GroupName = "MultiSelectionStates")]
    [TemplateVisualState(Name = "UpInVisualField", GroupName = "VisualFieldStates")]
    [TemplateVisualState(Name = "DownInVisualField", GroupName = "VisualFieldStates")]
    [TemplateVisualState(Name = "OutVisualField", GroupName = "VisualFieldStates")]
    [TemplatePart(Name = "SelecterCheckBox", Type = typeof(CheckBox))]
    [TemplatePart(Name = "SelecterRt",Type=typeof(Rectangle))]
    [TemplatePart(Name = "LayoutRoot",Type=typeof(Border))]
    public class PowerListBoxItem : ListBoxItem
    {
        #region dependencyproperties
        #region IsFloatingElement
        public static bool GetIsFloatingElement(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFloatingElementProperty);
        }

        public static void SetIsFloatingElement(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFloatingElementProperty, value);
        }

        public static readonly DependencyProperty IsFloatingElementProperty =
            DependencyProperty.RegisterAttached("IsFloatingElement", typeof(bool), typeof(PowerListBoxItem), new PropertyMetadata(false));

        #endregion

        #region CanCheck
        /// <summary>
        /// 能否选择前面的复选框
        /// </summary>
        internal bool CanCheck
        {
            get { return (bool)GetValue(CanCheckProperty); }
            set { SetValue(CanCheckProperty, value); }
        }

        internal static readonly DependencyProperty CanCheckProperty =
            DependencyProperty.Register("CanCheck", typeof(bool), typeof(PowerListBoxItem), new PropertyMetadata(false, OnCanCheckPropertyChanged));

        private static void OnCanCheckPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as PowerListBoxItem;
            Debug.Assert(item != null);
            item.OnCanCheckChanged();
        }

        private void OnCanCheckChanged()
        {
            RefreshState();
        }
        #endregion 
        #endregion

        #region private fields
        internal enum VisualFieldState
        {
            UpIn,
            DownIn,
            Out
        }

        private PowerListBox _listBox;
        private PowerListBox ListBox
        {
            get { return _listBox ?? (_listBox = this.Ancestors<PowerListBox>().FirstOrDefault()); }
        }

        private CheckBox _checkBox;
        private Rectangle _selecterRt;
        private Border _layoutRoot;
        private Storyboard _showSelecterRtSb;
        private readonly List<FrameworkElement> _floatingElements = new List<FrameworkElement>();

        private VisualFieldState _visualField;
        internal VisualFieldState VisualField
        {
            get { return _visualField; }
            set
            {
                if (_visualField == value)
                {
                    return;
                }
                _visualField = value;
                Floating(_visualField != VisualFieldState.Out && IsSelected && CanCheck);
                //触发状态的改变，将会执行预设好的动画
                GoToVisualFieldState(_visualField);
            }
        } 
        #endregion

        #region internal methods
        /// <summary>
        /// 不会触发状态的改变
        /// </summary>
        /// <param name="value"></param>
        internal void SetVisualFieldInternal(VisualFieldState value)
        {
            if (ListBox.EnableScrollAnimation)
                Opacity = value != VisualFieldState.Out ? 1 : 0;
            else
                Opacity = 1;
            _visualField = value;
            Floating(_visualField != VisualFieldState.Out && IsSelected && CanCheck);
        }

        /// <summary>
        /// 跳转到指定的状态
        /// </summary>
        /// <param name="visualField"></param>
        internal void GoToVisualFieldState(VisualFieldState visualField)
        {
            Opacity = 1;
            if (!ListBox.EnableScrollAnimation || DesignerProperties.GetIsInDesignMode(this))
                return;
            if (_visualField == VisualFieldState.Out)
            {
                //不在可视区域的列表项Opacity设为0，这样当它进入可视区域时，动画效果会更好
                Opacity = 0;
                VisualStateManager.GoToState(this, "OutVisualField", true);
            }
            else if (_visualField == VisualFieldState.UpIn)
            {
                VisualStateManager.GoToState(this, "UpInVisualField", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DownInVisualField", true);
            }
        } 
        #endregion

        #region 构造函数
        public PowerListBoxItem()
        {
            DefaultStyleKey = typeof(PowerListBoxItem);
            Tap += PowerListBoxItem_Tap;
            Loaded += PowerListBoxItem_Loaded;
            Unloaded += PowerListBoxItem_Unloaded;
        } 
        #endregion

        #region private methods
        void PowerListBoxItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            //初始化列表项的多选状态
            CanCheck = ListBox.IsMultipleSelectEnable && ListBox.IsMultipleSelect;
            //初始化该列表项的可视状态
            if (ListBox.EnableScrollAnimation)
            {
                var index = GetIndexInListBox();
                if (index < ListBox.FirstVisibleIndex || index > ListBox.LastVisibleIndex)
                    VisualField = VisualFieldState.Out;
                else
                    VisualField = VisualFieldState.UpIn;
            }
            //初始化列表项的漂浮状态
            _floatingElements.Clear();
            var eles = this.Descendants<FrameworkElement>().Where(GetIsFloatingElement);
            eles.ForEachEx(t =>
            {
                _floatingElements.Add(t);
                t.Tag = GetDefaultFloatingEffect(t);
            });
            _floatingElements.ForEach(t => Floating(IsSelected));
        }

        void PowerListBoxItem_Unloaded(object sender, RoutedEventArgs e)
        {
            _floatingElements.ForEach(t => Floating(false));
            _floatingElements.Clear();
        }

        void PowerListBoxItem_Tap(object sender, GestureEventArgs e)
        {
            if (_selecterRt == null || !IsSelected)
                return;
            var pt = e.GetPosition(_selecterRt);
            if (pt.X > _selecterRt.ActualWidth || pt.Y > _selecterRt.ActualHeight)
            {
                ListBox.InvokeItemSelectedEvent(this);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _checkBox = GetTemplateChild("SelecterCheckBox") as CheckBox;
            _selecterRt = GetTemplateChild("SelecterRt") as Rectangle;
            _layoutRoot = GetTemplateChild("LayoutRoot") as Border;
            Debug.Assert(_checkBox != null);
            Debug.Assert(_selecterRt != null);
            Debug.Assert(_layoutRoot != null);
            //添加必要的事件
            _showSelecterRtSb = GetDefaultTapStoryboard();
            if (_showSelecterRtSb != null)
            {
                _selecterRt.Tap += SelecterRt_Tap;
                _showSelecterRtSb.Completed += ShowSelecterRtSb_Completed;
            }
            _selecterRt.Hold += SelecterRt_Hold;
            _checkBox.Unchecked += _checkBox_Unchecked;
            _checkBox.Checked += _checkBox_Checked;
            RefreshState();
        }

        void _checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_isSelectAll != null && !_isSelectAll.Value)
                _checkBox.IsChecked = false;
            else if (VisualField != VisualFieldState.Out)
                Floating(true);
            _isSelectAll = null;
        }

        void _checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isSelectAll != null && _isSelectAll.Value)
                _checkBox.IsChecked = true;
            else if (VisualField != VisualFieldState.Out)
                Floating(false);
            _isSelectAll = null;
        }

        private bool? _isSelectAll;
        void SelecterRt_Hold(object sender, GestureEventArgs e)
        {
            if (CanCheck)
            {
                if (ListBox.SelectedItems.Count == ListBox.Items.Count)
                {
                    ListBox.SelectedIndex = -1;
                    _isSelectAll = false;
                }
                else
                {
                    ListBox.SelectAll();
                    _isSelectAll = true;
                }
            }
            else
            {
                ListBox.IsMultipleSelect = true;
                IsSelected = true;
                ListBox.SelectAll();
                _isSelectAll = true;
            }
        }

        void SelecterRt_Tap(object sender, GestureEventArgs e)
        {
            if (!CanCheck && ListBox.IsMultipleSelectEnable && ListBox.SelectedIndex >= 0)
            {
                _showSelecterRtSb.Begin();
            }
            e.Handled = true;
        }

        void ShowSelecterRtSb_Completed(object sender, EventArgs e)
        {
            ListBox.IsMultipleSelect = true;
            IsSelected = true;
        } 
        #endregion

        #region private methods
        private void RefreshState()
        {
            VisualStateManager.GoToState(this, CanCheck ? "EnableSelection" : "DisableSelection", true);
        }

        internal void Floating(bool isActive)
        {
            _floatingElements.ForEach(t =>
            {
                var floatingEffect = t.Tag as FloatingEffect;
                if (floatingEffect != null)
                    floatingEffect.IsActive = isActive;
            });
        }

        /// <summary>
        /// 获取默认的漂浮效果
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static FloatingEffect GetDefaultFloatingEffect(FrameworkElement element)
        {
            var floating = new FloatingEffect(element)
            {
                Is3DEnable = true,
                FloatingDuration = TimeSpan.FromMilliseconds(1000),
                IsActive = false,
                ThreeDFloatingRangle = 20
            };
            return floating;
        }

        /// <summary>
        /// 获取默认点击列表左侧时，矩形的动画
        /// </summary>
        /// <returns></returns>
        private Storyboard GetDefaultTapStoryboard()
        {
            if (_selecterRt == null)
            {
                const string msg = "在PowerListBoxItem.cs中执行GetDefaultTapStoryboard时出错！";
                throw new ArgumentNullException(msg);
            }
            var da = new DoubleAnimationUsingKeyFrames();
            var edk1 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = 0 };
            var edk2 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)), Value = 1 };
            var edk3 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)), Value = 0, EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } };
            da.KeyFrames.Add(edk1);
            da.KeyFrames.Add(edk2);
            da.KeyFrames.Add(edk3);
            Storyboard.SetTarget(da, _selecterRt);
            Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));
            var sb = new Storyboard();
            sb.Children.Add(da);
            return sb;
        }

        /// <summary>
        /// 获取该项在列表中的索引
        /// </summary>
        /// <returns></returns>
        private int GetIndexInListBox()
        {
            if (ListBox == null)
                return -1;
            if (ListBox.ItemContainerGenerator == null)
                return ListBox.Items.IndexOf(this);
            return ListBox.ItemContainerGenerator.IndexFromContainer(this);
        } 
        #endregion
    }
}
