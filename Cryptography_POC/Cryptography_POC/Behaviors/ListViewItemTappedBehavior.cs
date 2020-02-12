using System;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms;

namespace Cryptography_POC.Behaviors
{
    public class ListViewItemTappedBehavior : Behavior<ListView>
  {
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof (Command), typeof (ICommand), typeof (ListViewItemTappedBehavior), (object) null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    public static readonly BindableProperty InputConverterProperty = BindableProperty.Create(nameof (Converter), typeof (IValueConverter), typeof (ListViewItemTappedBehavior), (object) null, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);

    public ICommand Command
    {
      get
      {
        return (ICommand) this.GetValue(ListViewItemTappedBehavior.CommandProperty);
      }
      set
      {
        this.SetValue(ListViewItemTappedBehavior.CommandProperty, (object) value);
      }
    }

    public IValueConverter Converter
    {
      get
      {
        return (IValueConverter) this.GetValue(ListViewItemTappedBehavior.InputConverterProperty);
      }
      set
      {
        this.SetValue(ListViewItemTappedBehavior.InputConverterProperty, (object) value);
      }
    }

    public ListView AssociatedObject { get; private set; }

    protected override void OnAttachedTo(ListView bindable)
    {
      base.OnAttachedTo(bindable);
      this.AssociatedObject = bindable;
      bindable.BindingContextChanged += new EventHandler(this.OnBindingContextChanged);
      bindable.ItemTapped += new EventHandler<ItemTappedEventArgs>(this.OnListViewItemTapped);
    }

    protected override void OnDetachingFrom(ListView bindable)
    {
      base.OnDetachingFrom(bindable);
      bindable.BindingContextChanged -= new EventHandler(this.OnBindingContextChanged);
      bindable.ItemTapped -= new EventHandler<ItemTappedEventArgs>(this.OnListViewItemTapped);
      this.AssociatedObject = (ListView) null;
    }

    private void OnBindingContextChanged(object sender, EventArgs e)
    {
      this.OnBindingContextChanged();
    }

    private void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
    {
      if (this.Command == null)
        return;
      object parameter = this.Converter.Convert((object) e, typeof (object), (object) null, (CultureInfo) null);
      if (!this.Command.CanExecute(parameter))
        return;
      this.Command.Execute(parameter);
    }

    protected override void OnBindingContextChanged()
    {
      base.OnBindingContextChanged();
      this.BindingContext = this.AssociatedObject.BindingContext;
    }
  }
}