Public Class CustomControl
    Inherits Control

    Shared Sub New()
        'This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
        'This style is defined in Themes\Generic.xaml
        DefaultStyleKeyProperty.OverrideMetadata(GetType(CustomControl), New FrameworkPropertyMetadata(GetType(CustomControl)))
    End Sub

End Class