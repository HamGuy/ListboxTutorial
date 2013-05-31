using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

//using Microsoft.Expression.Interactivity.Core;

namespace Unique.Controls
{
    public class FloatingBehavior : Behavior<FrameworkElement>
    {
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
            DependencyProperty.Register("FloatingRange", typeof(double), typeof(FloatingBehavior), new PropertyMetadata(10.0));
 
	    #endregion

        #region FloatingDuration
		public static readonly DependencyProperty FloatingDurationProperty = 
            DependencyProperty.Register("FloatingDuration", typeof(TimeSpan), typeof(FloatingBehavior), new PropertyMetadata(TimeSpan.FromMilliseconds(800)));
        /// <summary>
        /// 单次漂浮动画的持续时间
        /// </summary>
        public TimeSpan FloatingDuration
        {
            get { return (TimeSpan)GetValue(FloatingDurationProperty); }
            set { SetValue(FloatingDurationProperty, value); }
        }
	    #endregion

        #region IsActive    
		public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(FloatingBehavior), new PropertyMetadata(true));
        /// <summary>
        /// 是否启用漂浮
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
	    #endregion

        #region Is3DEnable
        public static readonly DependencyProperty Is3DEnableProperty =
            DependencyProperty.Register("Is3DEnable", typeof(bool), typeof(FloatingBehavior), new PropertyMetadata(default(bool)));
        /// <summary>
        /// 漂浮过程中是否启用3d效果
        /// </summary>
        public bool Is3DEnable
        {
            get { return (bool)GetValue(Is3DEnableProperty); }
            set { SetValue(Is3DEnableProperty, value); }
        }
        #endregion

        #region ThreeDFloatingRangle
        public static readonly DependencyProperty ThreeDFloatingRangleProperty =
            DependencyProperty.Register("ThreeDFloatingRangle", typeof(double), typeof(FloatingBehavior), new PropertyMetadata(10.0));
        /// <summary>
        /// 3D效果的x,y方向旋转范围
        /// </summary>
        public double ThreeDFloatingRangle
        {
            get { return (double)GetValue(ThreeDFloatingRangleProperty); }
            set { SetValue(ThreeDFloatingRangleProperty, value); }
        }
        #endregion

        private FloatingEffect _floatingEffect;
        protected override void OnAttached()
        {
            base.OnAttached();
            _floatingEffect = new FloatingEffect(AssociatedObject);
            var b1 = new Binding("FloatingRange") {Source = this};
            BindingOperations.SetBinding(_floatingEffect, FloatingEffect.FloatingRangeProperty, b1);
            var b2 = new Binding("FloatingDuration") { Source = this };
            BindingOperations.SetBinding(_floatingEffect, FloatingEffect.FloatingDurationProperty, b2);
            var b3 = new Binding("IsActive") { Source = this };
            BindingOperations.SetBinding(_floatingEffect, FloatingEffect.IsActiveProperty, b3);
            var b4 = new Binding("Is3DEnable") { Source = this };
            BindingOperations.SetBinding(_floatingEffect, FloatingEffect.Is3DEnableProperty, b4);
            var b5 = new Binding("ThreeDFloatingRangle") { Source = this };
            BindingOperations.SetBinding(_floatingEffect, FloatingEffect.ThreeDFloatingRangleProperty, b5);
            _floatingEffect.Floating();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            IsActive = false;
            _floatingEffect = null;
        }
    }
}