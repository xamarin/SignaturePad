using System;
using Xamarin.Forms;

using SignaturePad.Forms;

namespace Samples
{
	public class CaptureSignatureBehaviorBase : Behavior<SignaturePadView>
	{
		private bool updating;
		private SignaturePadView associated;

		protected override void OnAttachedTo (SignaturePadView bindable)
		{
			base.OnAttachedTo (bindable);

			associated = bindable;
			UpdateBindingContext (bindable, EventArgs.Empty);

			bindable.Cleared += OnSignatureChanged;
			bindable.StrokeCompleted += OnSignatureChanged;
			bindable.BindingContextChanged += UpdateBindingContext;
		}

		protected override void OnDetachingFrom (SignaturePadView bindable)
		{
			bindable.Cleared -= OnSignatureChanged;
			bindable.StrokeCompleted -= OnSignatureChanged;
			bindable.BindingContextChanged -= UpdateBindingContext;

			BindingContext = null;
			associated = null;

			base.OnDetachingFrom (bindable);
		}

		protected virtual void UpdateSignaturePad (SignaturePadView bindable, BindableProperty property, object oldValue, object newValue)
		{
		}

		protected virtual void UpdateBehavior (SignaturePadView signaturePad)
		{
		}

		protected void OnPropertyChanged (BindableObject bindable, BindableProperty property, object oldValue, object newValue)
		{
			var behavior = bindable as CaptureSignatureBehaviorBase;

			if (!behavior.updating)
			{
				behavior.updating = true;
				behavior.UpdateSignaturePad (behavior.associated, property, oldValue, newValue);
				behavior.updating = false;
			}
		}

		private void UpdateBindingContext (object sender, EventArgs e)
		{
			var signaturePad = sender as SignaturePadView;

			BindingContext = signaturePad.BindingContext;
		}

		private void OnSignatureChanged (object sender, EventArgs e)
		{
			var signaturePad = sender as SignaturePadView;

			if (!updating)
			{
				updating = true;
				UpdateBehavior (signaturePad);
				updating = false;
			}
		}

		public static BindableProperty.BindingPropertyChangedDelegate CreatePropertyChanged (BindableProperty property)
		{
			return OnPropertyChanged;

			void OnPropertyChanged (BindableObject bindable, object oldValue, object newValue)
			{
				var behavior = bindable as CaptureSignatureBehaviorBase;
				behavior.OnPropertyChanged (bindable, property, oldValue, newValue);
			}
		}
	}
}
