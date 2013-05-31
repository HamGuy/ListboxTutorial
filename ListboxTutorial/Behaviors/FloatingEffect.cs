using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

//using Microsoft.Expression.Interactivity.Core;

namespace Unique.Controls
{
    public class FloatingEffect : DependencyObject
    {
        #region DependencyProperties
        #region FloatingRange
        /// <summary>
        /// x,y方向漂浮的范围
        /// </summary>
        public double FloatingRange
        {
            get { return (double)GetValue(FloatingRangeProperty); }
            set { SetValue(FloatingRangeProperty, value); }
        }

        public static readonly DependencyProperty FloatingRangeProperty =
            DependencyProperty.Register("FloatingRange", typeof(double), typeof(FloatingEffect), new PropertyMetadata(10.0));

        #endregion

        #region FloatingDuration
        public static readonly DependencyProperty FloatingDurationProperty =
            DependencyProperty.Register("FloatingDuration", typeof(TimeSpan), typeof(FloatingEffect), new PropertyMetadata(TimeSpan.FromMilliseconds(800), OnFloatingDurationPropertyChanged));
        /// <summary>
        /// 单次漂浮动画的持续时间
        /// </summary>
        public TimeSpan FloatingDuration
        {
            get { return (TimeSpan)GetValue(FloatingDurationProperty); }
            set { SetValue(FloatingDurationProperty, value); }
        }
        private static void OnFloatingDurationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = sender as FloatingEffect;
            if (target == null)
                return;
            target.OnFloatingDurationChanged();
        }
        private void OnFloatingDurationChanged()
        {
            if (_timer == null || _sb == null)
                return;
            _timer.Interval = FloatingDuration;
            foreach (DoubleAnimation da in _sb.Children)
            {
                da.Duration = new Duration(FloatingDuration);
            }
        }
        #endregion

        #region IsActive
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(FloatingEffect), new PropertyMetadata(true, OnIsActivePropertyChanged));
        /// <summary>
        /// 是否启用漂浮
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        private static void OnIsActivePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = sender as FloatingEffect;
            if (behavior != null)
                behavior.OnIsActiveChanged();
        }

        private void OnIsActiveChanged()
        {
            if (_timer == null)
                return;
            if (IsActive)
                _timer.Start();
            else
            {
                MakeSbOrigin();
                Make3DsbOrigin();
                _timer.Stop();
            }
        }
        #endregion

        #region Is3DEnable
        public static readonly DependencyProperty Is3DEnableProperty =
            DependencyProperty.Register("Is3DEnable", typeof(bool), typeof(FloatingEffect), new PropertyMetadata(default(bool), OnIs3DEnablePropertyChanged));
        /// <summary>
        /// 漂浮过程中是否启用3d效果
        /// </summary>
        public bool Is3DEnable
        {
            get { return (bool)GetValue(Is3DEnableProperty); }
            set { SetValue(Is3DEnableProperty, value); }
        }

        private static void OnIs3DEnablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var target = sender as FloatingEffect;
            if (target == null)
                return;
            target.OnIs3DEnableChanged();
        }

        private void OnIs3DEnableChanged()
        {
            if (Is3DEnable)
            {
                Ensure3DsbCreated();
            }
            else
            {
                Make3DsbOrigin();
            }
        }
        #endregion

        #region ThreeDFloatingRangle
        public static readonly DependencyProperty ThreeDFloatingRangleProperty =
            DependencyProperty.Register("ThreeDFloatingRangle", typeof(double), typeof(FloatingEffect), new PropertyMetadata(10.0));
        /// <summary>
        /// 3D效果的x,y方向旋转范围
        /// </summary>
        public double ThreeDFloatingRangle
        {
            get { return (double)GetValue(ThreeDFloatingRangleProperty); }
            set { SetValue(ThreeDFloatingRangleProperty, value); }
        }
        #endregion 
        #endregion

        #region private fields
        private readonly Random _rand;
        private readonly DispatcherTimer _timer;
        private Storyboard _sb;
        private Storyboard _3dsb;
        private readonly FrameworkElement _floatingObject; 
        #endregion

        #region 默认构造函数
        public FloatingEffect(FrameworkElement floatingObject)
        {
            if (floatingObject == null)
                throw new ArgumentNullException();
            _floatingObject = floatingObject;
            _rand = new Random(DateTime.Now.Millisecond);
            _timer = new DispatcherTimer { Interval = FloatingDuration };
            _timer.Tick += _timer_Tick;
            EnsureSbCreated();
            Ensure3DsbCreated();
        } 
        #endregion

        #region public methods
        /// <summary>
        /// 开始漂浮
        /// </summary>
        public void Floating()
        {
            if (!IsActive)
                return;
            _sb.Begin();
            if (Is3DEnable)
            {
                _3dsb.Begin();
            }
            _timer.Start();
        } 
        #endregion

        #region private methods
        /// <summary>
        /// 保证平动动画已经创建
        /// </summary>
        void EnsureSbCreated()
        {
            if (!(_floatingObject.RenderTransform is CompositeTransform))
                _floatingObject.RenderTransform = new CompositeTransform();
            if (_sb != null)
            {
                return;
            }
            var duration = new Duration(FloatingDuration);
            var dax = new DoubleAnimation { Duration = duration, To = GetNextValue(FloatingRange / 2) };
            var day = new DoubleAnimation { Duration = duration, To = GetNextValue(FloatingRange / 2) };

            Storyboard.SetTarget(dax, _floatingObject);
            Storyboard.SetTargetProperty(dax, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            Storyboard.SetTarget(day, _floatingObject);
            Storyboard.SetTargetProperty(day, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
            _sb = new Storyboard();
            _sb.Children.Add(dax);
            _sb.Children.Add(day);
        }

        /// <summary>
        /// 保证3d动画已经创建
        /// </summary>
        void Ensure3DsbCreated()
        {
            if (!(_floatingObject.Projection is PlaneProjection))
                _floatingObject.Projection = new PlaneProjection();
            if (_3dsb != null || _floatingObject == null)
                return;
            var duration = new Duration(FloatingDuration);
            var dax = new DoubleAnimation { Duration = duration, To = GetNextValue(ThreeDFloatingRangle / 2) };
            var day = new DoubleAnimation { Duration = duration, To = GetNextValue(ThreeDFloatingRangle / 2) };

            Storyboard.SetTarget(dax, _floatingObject);
            Storyboard.SetTargetProperty(dax, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationX)"));
            Storyboard.SetTarget(day, _floatingObject);
            Storyboard.SetTargetProperty(day, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)"));
            _3dsb = new Storyboard();
            _3dsb.Children.Add(dax);
            _3dsb.Children.Add(day);
        }

        /// <summary>
        /// 恢复平动的初始状态
        /// </summary>
        void MakeSbOrigin()
        {
            if (_sb == null)
                return;
            foreach (DoubleAnimation da in _sb.Children)
            {
                da.From = 0;
                da.To = 0;
            }
            var trans = _floatingObject.RenderTransform as CompositeTransform;
            if (trans == null)
                return;
            trans.TranslateX = 0;
            trans.TranslateY = 0;
        }

        /// <summary>
        /// 恢复3d的初始状态
        /// </summary>
        void Make3DsbOrigin()
        {
            if (_3dsb == null)
                return;
            foreach (DoubleAnimation da in _3dsb.Children)
            {
                da.From = 0;
                da.To = 0;
            }
            var trans = _floatingObject.Projection as PlaneProjection;
            if (trans == null)
                return;
            trans.RotationX = 0;
            trans.RotationY = 0;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            EnsureSbCreated();
            _sb.SkipToFill();
            foreach (DoubleAnimation da in _sb.Children)
            {
                da.From = da.To;
                da.To = GetNextValue(FloatingRange / 2);
            }
            _sb.Begin();
            if (!Is3DEnable)
                return;
            Ensure3DsbCreated();
            _3dsb.SkipToFill();
            foreach (DoubleAnimation da in _3dsb.Children)
            {
                da.From = da.To;
                da.To = GetNextValue(ThreeDFloatingRangle / 2);
            }
            _3dsb.Begin();
        }

        /// <summary>
        /// 根据给定的值，获取一个随机的数，该随机数的取值空间为{value,0,-value)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        double GetNextValue(double value)
        {
            var dd = _rand.Next(3);
            if (dd == 0)
                return 0;
            if (dd == 1)
                return value;
            return -value;
        } 
        #endregion
    }
}